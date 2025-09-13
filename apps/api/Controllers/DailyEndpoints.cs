using Microsoft.EntityFrameworkCore;
using ShipLife.Api.Data;
using ShipLife.Api.Domain;

namespace ShipLife.Api.Controllers;

public record DailyNoteDto(DateOnly Date, string Text);

public static class DailyEndpoints
{
    public static RouteGroupBuilder MapDailyApi(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/daily");

        g.MapGet("/", async (AppDb db) =>
        {
            var last10 = await db.DailyNotes.OrderByDescending(d => d.Date).Take(10).ToListAsync();
            return Results.Ok(last10);
        });

        g.MapPost("/", async (AppDb db, DailyNoteDto dto) =>
        {
            var dn = await db.DailyNotes.FirstOrDefaultAsync(d => d.Date == dto.Date);
            if (dn is null)
            {
                dn = new DailyNote { Date = dto.Date, Text = dto.Text };
                db.DailyNotes.Add(dn);
            }
            else dn.Text = dto.Text;

            await db.SaveChangesAsync();
            return Results.Ok(dn);
        });

        return g;
    }
}
