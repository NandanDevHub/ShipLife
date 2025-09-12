using Microsoft.EntityFrameworkCore;
using ShipLife.Api.Data;
using ShipLife.Api.Domain;

namespace ShipLife.Api.Controllers;

public record QuarterCreateDto(int Year, int Number, string SeasonGoal);
public record SprintCreateDto(Guid QuarterId, int Number, string Goal, DateTime StartDate, DateTime EndDate, string? Reward);

public static class QuarterSprintEndpoints
{
    public static RouteGroupBuilder MapQuarterSprintApi(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/plan");

        g.MapPost("/quarter", async (AppDb db, QuarterCreateDto dto) =>
        {
            var q = new Quarter { Year = dto.Year, Number = dto.Number, SeasonGoal = dto.SeasonGoal };
            var b = new Board { Quarter = q };
            b.Columns.AddRange(new[]
            {
                new Column { Name="Backlog", Order=1 },
                new Column { Name="Ready", Order=2 },
                new Column { Name="Doing", Order=3 },
                new Column { Name="Done", Order=4 }
            });
            db.Quarters.Add(q);
            db.Boards.Add(b);
            await db.SaveChangesAsync();
            return Results.Ok(new { q.Id, boardId = b.Id });
        });

        g.MapPost("/sprint", async (AppDb db, SprintCreateDto dto) =>
        {
            var q = await db.Quarters.FindAsync(dto.QuarterId);
            if (q is null) return Results.NotFound();
            var s = new Sprint { QuarterId = q.Id, Number = dto.Number, Goal = dto.Goal, StartDate = dto.StartDate, EndDate = dto.EndDate, Reward = dto.Reward };
            db.Sprints.Add(s);
            await db.SaveChangesAsync();
            return Results.Ok(s);
        });

        g.MapPatch("/sprint/{id:guid}/retro", async (Guid id, AppDb db, string win, string blocker, string oneChange) =>
        {
            var s = await db.Sprints.FindAsync(id);
            if (s is null) return Results.NotFound();
            // Retro is stored as a DailyNote with special text marker (simple & visible)
            db.DailyNotes.Add(new DailyNote { Date = DateOnly.FromDateTime(DateTime.UtcNow), Text = $"[RETRO Sprint {s.Number}] Win: {win} | Blocker: {blocker} | Change: {oneChange}" });
            await db.SaveChangesAsync();
            return Results.Ok();
        });

        return g;
    }
}
