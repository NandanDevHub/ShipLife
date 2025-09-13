using Microsoft.EntityFrameworkCore;
using ShipLife.Api.Data;
using ShipLife.Api.Domain;
using ShipLife.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ShipLife.Api.Controllers;

public static class BoardExtras
{
    public static RouteGroupBuilder MapBoardExtras(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/cards");

        // Comments
        g.MapPost("/{cardId:guid}/comments", async (Guid cardId, AppDb db, IHubContext<ShipLife.Api.Hubs.BoardHub> hub, string body, Guid? parentId) =>
        {
            var card = await db.Cards.FindAsync(cardId);
            if (card is null) return Results.NotFound();

            var c = new Comment { CardId = cardId, Body = body, ParentCommentId = parentId };
            db.Comments.Add(c);
            await db.SaveChangesAsync();
            await hub.Clients.All.SendAsync("CommentAdded", new { cardId, commentId = c.Id });
            return Results.Ok(c);
        });

        g.MapGet("/{cardId:guid}/comments", async (Guid cardId, AppDb db) =>
        {
            var list = await db.Comments.Where(c => c.CardId == cardId && c.ParentCommentId == null)
                .Include(c => c.Replies)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return Results.Ok(list);
        });

        // Checklist
        g.MapPost("/{cardId:guid}/checklists", async (Guid cardId, AppDb db, string title) =>
        {
            var card = await db.Cards.FindAsync(cardId);
            if (card is null) return Results.NotFound();
            var cl = new Checklist { CardId = cardId, Title = string.IsNullOrWhiteSpace(title) ? "Checklist" : title };
            db.Checklists.Add(cl);
            await db.SaveChangesAsync();
            return Results.Ok(cl);
        });

        g.MapPost("/checklists/{id:guid}/items", async (Guid id, AppDb db, string text) =>
        {
            var cl = await db.Checklists.FindAsync(id);
            if (cl is null) return Results.NotFound();
            var item = new ChecklistItem { ChecklistId = id, Text = text };
            db.ChecklistItems.Add(item);
            await db.SaveChangesAsync();
            return Results.Ok(item);
        });

        g.MapPatch("/checklist-items/{id:guid}", async (Guid id, AppDb db, bool done) =>
        {
            var it = await db.ChecklistItems.FindAsync(id);
            if (it is null) return Results.NotFound();
            it.Done = done;
            it.DoneAt = done ? DateTime.UtcNow : null;
            await db.SaveChangesAsync();
            return Results.Ok(it);
        });

        return g;
    }
}
