using Microsoft.EntityFrameworkCore;
using ShipLife.Api.Data;

namespace ShipLife.Api.Controllers;

public static class SearchEndpoints
{
    public static RouteGroupBuilder MapSearchApi(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/search");

        g.MapGet("/", async (AppDb db, string? q, string? type, string? tag) =>
        {
            var query = db.Cards.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(c => c.Title.Contains(q) || (c.Description ?? "").Contains(q));

            if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<Domain.CardType>(type, true, out var ct))
                query = query.Where(c => c.Type == ct);

            if (!string.IsNullOrWhiteSpace(tag))
                query = query.Where(c => c.CardTags.Any(t => t.Tag!.Name == tag));

            var results = await query.OrderByDescending(c => c.CreatedAt).Take(50).ToListAsync();
            return Results.Ok(results);
        });

        return g;
    }
}
