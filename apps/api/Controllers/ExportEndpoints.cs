using Microsoft.EntityFrameworkCore;
using ShipLife.Api.Data;

namespace ShipLife.Api.Controllers;

public static class ExportEndpoints
{
    public static RouteGroupBuilder MapExportApi(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/export");

        g.MapGet("/json", async (AppDb db) =>
        {
            var all = new
            {
                Quarters = await db.Quarters.ToListAsync(),
                Sprints = await db.Sprints.ToListAsync(),
                Board = await db.Boards.Include(b=>b.Columns).ThenInclude(c=>c.Cards).FirstOrDefaultAsync(),
                Tags = await db.Tags.ToListAsync(),
                Notes = await db.DailyNotes.ToListAsync()
            };
            return Results.Ok(all);
        });

        return g;
    }
}
