using Microsoft.EntityFrameworkCore;
using ShipLife.Api.Data;
using ShipLife.Api.Domain;

namespace ShipLife.Api.Controllers;

public record RewardClaimDto(bool Claimed);

public record PatchCreateDto(Guid CardId, string Hypothesis, DateTime StartDate, DateTime EndDate);
public record PatchOutcomeDto(string Outcome, string? Notes);

public static class RewardPatchEndpoints
{
    public static RouteGroupBuilder MapRewardPatchApi(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/extras");

        g.MapPatch("/sprint/{id:guid}/reward", async (Guid id, AppDb db, RewardClaimDto dto) =>
        {
            var s = await db.Sprints.FindAsync(id);
            if (s is null) return Results.NotFound();
            s.RewardClaimed = dto.Claimed;
            s.RewardClaimedAt = dto.Claimed ? DateTime.UtcNow : null;
            await db.SaveChangesAsync();
            return Results.Ok(s);
        });

        g.MapPost("/patch", async (AppDb db, PatchCreateDto dto) =>
        {
            var card = await db.Cards.FindAsync(dto.CardId);
            if (card is null) return Results.NotFound();
            var p = new PatchExperiment { CardId = dto.CardId, Hypothesis = dto.Hypothesis, StartDate = dto.StartDate, EndDate = dto.EndDate };
            db.PatchExperiments.Add(p);
            await db.SaveChangesAsync();
            return Results.Ok(p);
        });

        g.MapPatch("/patch/{id:guid}/outcome", async (Guid id, AppDb db, PatchOutcomeDto dto) =>
        {
            var p = await db.PatchExperiments.FindAsync(id);
            if (p is null) return Results.NotFound();
            p.Outcome = dto.Outcome;
            p.Notes = dto.Notes;
            await db.SaveChangesAsync();
            return Results.Ok(p);
        });

        return g;
    }
}
