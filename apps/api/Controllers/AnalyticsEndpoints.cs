using Microsoft.EntityFrameworkCore;
using ShipLife.Api.Data;
using ShipLife.Api.Domain;

namespace ShipLife.Api.Controllers;

public static class AnalyticsEndpoints
{
    public static RouteGroupBuilder MapAnalyticsApi(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/analytics");

        g.MapGet("/summary", async (AppDb db) =>
        {
            var total = await db.Cards.CountAsync();
            var done = await db.Cards.CountAsync(c => c.Status == CardStatus.Done);
            var doing = await db.Cards.CountAsync(c => c.Status == CardStatus.Doing);
            var bugs = await db.Cards.CountAsync(c => c.Type == CardType.Bug);
            var streak = await db.DailyNotes.CountAsync(d => d.Date >= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)));

            return Results.Ok(new { total, done, doing, bugs, last7DailyNotes = streak });
        });

        return g;
    }
}
