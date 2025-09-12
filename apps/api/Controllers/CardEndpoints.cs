// apps/api/Controllers/CardEndpoints.cs
using Microsoft.AspNetCore.Http.HttpResults;           // FIX: add this
using Microsoft.EntityFrameworkCore;
using ShipLife.Api.Data;
using ShipLife.Api.Domain;
using ShipLife.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ShipLife.Api.Controllers;

public record CreateCardDto(Guid ColumnId, string Title, CardType Type, Priority Priority, DateTime? DueAt);
public record MoveCardDto(Guid CardId, Guid ToColumnId, int NewOrder);
public record UpdateCardDto(string? Title, string? Description, CardType? Type, Priority? Priority, DateTime? DueAt, int? Order, CardStatus? Status);

public static class CardEndpoints
{
    public static RouteGroupBuilder MapCardApi(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/cards");

        g.MapPost("/", async Task<Results<Ok<Card>, NotFound>> (AppDb db, IHubContext<BoardHub> hub, CreateCardDto dto) =>
        {
            var col = await db.Columns.Include(c => c.Cards).FirstOrDefaultAsync(c => c.Id == dto.ColumnId);
            if (col is null) return TypedResults.NotFound();
            var order = col.Cards.Count == 0 ? 0 : col.Cards.Max(c => c.Order) + 1;
            var card = new Card
            {
                ColumnId = col.Id,
                Title = dto.Title,
                Type = dto.Type,
                Priority = dto.Priority,
                DueAt = dto.DueAt,
                Order = order,
                Status = col.Name switch
                {
                    "Backlog" => CardStatus.Backlog,
                    "Ready" => CardStatus.Ready,
                    "Doing" => CardStatus.Doing,
                    "Done" => CardStatus.Done,
                    _ => CardStatus.Backlog
                }
            };
            db.Cards.Add(card);
            await db.SaveChangesAsync();
            await hub.Clients.All.SendAsync("CardCreated", new { card.Id, card.Title, card.ColumnId });
            return TypedResults.Ok(card);
        });

        g.MapPatch("/{id:guid}", async Task<Results<Ok<Card>, NotFound>> (Guid id, AppDb db, IHubContext<BoardHub> hub, UpdateCardDto dto) =>
        {
            var card = await db.Cards.FirstOrDefaultAsync(c => c.Id == id);
            if (card is null) return TypedResults.NotFound();
            if (dto.Title is not null) card.Title = dto.Title;
            if (dto.Description is not null) card.Description = dto.Description;
            if (dto.Type.HasValue) card.Type = dto.Type.Value;
            if (dto.Priority.HasValue) card.Priority = dto.Priority.Value;
            if (dto.DueAt.HasValue) card.DueAt = dto.DueAt;
            if (dto.Order.HasValue) card.Order = dto.Order.Value;
            if (dto.Status.HasValue)
            {
                card.Status = dto.Status.Value;
                if (dto.Status.Value == CardStatus.Done && card.CompletedAt is null)
                    card.CompletedAt = DateTime.UtcNow;
            }
            await db.SaveChangesAsync();
            await hub.Clients.All.SendAsync("CardUpdated", new { card.Id });
            return TypedResults.Ok(card);
        });

        // REPLACE ONLY THIS ENDPOINT
        g.MapPost("/move", async (AppDb db, IHubContext<BoardHub> hub, MoveCardDto dto) =>
        {
            var card = await db.Cards.Include(c => c.Column).FirstOrDefaultAsync(c => c.Id == dto.CardId);
            var dest = await db.Columns.Include(c => c.Cards).FirstOrDefaultAsync(c => c.Id == dto.ToColumnId);
            if (card is null || dest is null) return Results.NotFound();

            // Enforce WIP ≤ 2 in Doing
            if (dest.Name == "Doing" && dest.Cards.Count >= 2)
                return Results.BadRequest(new { message = "WIP limit reached in Doing (2)." });

            // Remove from current
            var src = await db.Columns.Include(c => c.Cards).FirstAsync(c => c.Id == card.ColumnId);
            src.Cards.Remove(card);

            // Clamp target order (don't mutate dto)
            var newOrder = Math.Clamp(dto.NewOrder, 0, dest.Cards.Count);

            // Shift orders to make a gap
            foreach (var c in dest.Cards.Where(c => c.Order >= newOrder))
                c.Order++;

            // Move card
            card.Order = newOrder;
            card.ColumnId = dest.Id;
            card.Status = dest.Name switch
            {
                "Backlog" => CardStatus.Backlog,
                "Ready" => CardStatus.Ready,
                "Doing" => CardStatus.Doing,
                "Done" => CardStatus.Done,
                _ => card.Status
            };
            if (card.Status == CardStatus.Done && card.CompletedAt is null)
                card.CompletedAt = DateTime.UtcNow;

            dest.Cards.Add(card);

            await db.SaveChangesAsync();
            await hub.Clients.All.SendAsync("CardMoved", new { card.Id, to = dest.Id, order = card.Order });
            return Results.Ok();
        });


        g.MapDelete("/{id:guid}", async Task<Results<Ok, NotFound>> (Guid id, AppDb db, IHubContext<BoardHub> hub) =>
        {
            var card = await db.Cards.FirstOrDefaultAsync(c => c.Id == id);
            if (card is null) return TypedResults.NotFound();
            db.Cards.Remove(card);
            await db.SaveChangesAsync();
            await hub.Clients.All.SendAsync("CardDeleted", new { id });
            return TypedResults.Ok();
        });

        return g;
    }
}