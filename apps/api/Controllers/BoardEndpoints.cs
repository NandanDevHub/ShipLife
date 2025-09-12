using Microsoft.EntityFrameworkCore;
using ShipLife.Api.Data;
using ShipLife.Api.Domain;
using ShipLife.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ShipLife.Api.Controllers;

public static class BoardEndpoints
{
    public static RouteGroupBuilder MapBoardApi(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/boards");

        g.MapPost("/seed", async (AppDb db) =>
        {
            if (await db.Boards.AnyAsync()) return Results.Ok("Already seeded.");
            var now = DateTime.UtcNow;
            var qNum = ((now.Month - 1) / 3) + 1;
            var q = new Quarter
            {
                Year = now.Year,
                Number = qNum,
                SeasonGoal = "Define your Season Goal"
            };
            var board = new Board { Quarter = q };
            board.Columns.AddRange(new[]
            {
                new Column { Name = "Backlog", Order = 1 },
                new Column { Name = "Ready",   Order = 2 },
                new Column { Name = "Doing",   Order = 3 },
                new Column { Name = "Done",    Order = 4 },
            });
            db.Quarters.Add(q);
            db.Boards.Add(board);
            await db.SaveChangesAsync();
            return Results.Ok(new { QuarterId = q.Id, BoardId = board.Id });
        });

        g.MapGet("/current", async (AppDb db) =>
        {
            var board = await db.Boards
                .Include(b => b.Quarter)
                .Include(b => b.Columns).ThenInclude(c => c.Cards).ThenInclude(c=>c.CardTags).ThenInclude(ct=>ct.Tag)
                .Include(b => b.Columns).ThenInclude(c => c.Cards).ThenInclude(c=>c.Checklists).ThenInclude(cl=>cl.Items)
                .FirstOrDefaultAsync();

            return board is null ? Results.NotFound() : Results.Ok(board);
        });

        return g;
    }
}
