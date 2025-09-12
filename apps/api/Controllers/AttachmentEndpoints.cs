using Microsoft.EntityFrameworkCore;
using ShipLife.Api.Data;
using ShipLife.Api.Domain;
using ShipLife.Api.Services;
using ShipLife.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ShipLife.Api.Controllers;

public static class AttachmentEndpoints
{
    public static RouteGroupBuilder MapAttachmentApi(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/attachments");

        g.MapPost("/upload/{cardId:guid}", async (Guid cardId, AppDb db, IStorageService storage, IHubContext<BoardHub> hub, IFormFile file, CancellationToken ct) =>
        {
            var card = await db.Cards.FirstOrDefaultAsync(c => c.Id == cardId);
            if (card is null) return Results.NotFound();

            if (file.Length == 0) return Results.BadRequest(new { message = "Empty file." });

            var (url, size, mime) = await storage.SaveAsync(file, ct);
            var att = new Attachment { CardId = card.Id, FileName = file.FileName, Url = url, Size = size, MimeType = mime };
            db.Attachments.Add(att);
            await db.SaveChangesAsync();
            await hub.Clients.All.SendAsync("AttachmentAdded", new { cardId = card.Id, attachmentId = att.Id, att.Url });
            return Results.Ok(att);
        });

        g.MapDelete("/{id:guid}", async (Guid id, AppDb db) =>
        {
            var att = await db.Attachments.FirstOrDefaultAsync(a => a.Id == id);
            if (att is null) return Results.NotFound();
            db.Attachments.Remove(att);
            await db.SaveChangesAsync();
            return Results.Ok();
        });

        return g;
    }
}
