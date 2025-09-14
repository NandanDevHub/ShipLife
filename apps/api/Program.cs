using Microsoft.EntityFrameworkCore;
using Serilog;
using ShipLife.Api.Controllers;
using ShipLife.Api.Data;
using ShipLife.Api.Hubs;
using ShipLife.Api.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

var allowed = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:3000"];
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(p => p.WithOrigins(allowed).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
});

builder.Services.AddDbContext<AppDb>(opt => opt.UseInMemoryDatabase("shiplife"));

builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    o.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Services
builder.Services.AddSignalR().AddJsonProtocol();
builder.Services.AddSingleton<IStorageService, LocalStorageService>(); // TO switch to S3 later

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.MapBoardApi();
app.MapCardApi();
app.MapBoardExtras();
app.MapAttachmentApi();
app.MapDailyApi();
app.MapQuarterSprintApi();
app.MapRewardPatchApi();
app.MapSearchApi();
app.MapAnalyticsApi();
app.MapExportApi();

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/health", () => Results.Ok(new { ok = true, time = DateTime.UtcNow }));
app.MapHub<BoardHub>("/hubs/board");

app.MapPost("/seed", async (AppDb db) =>
{
    if (await db.Boards.AnyAsync()) return Results.Ok("Already seeded.");
    var now = DateTime.UtcNow;
    var qNum = ((now.Month - 1) / 3) + 1;
    var q = new ShipLife.Api.Domain.Quarter { Year = now.Year, Number = qNum, SeasonGoal = "Define your Season Goal" };
    var b = new ShipLife.Api.Domain.Board { Quarter = q };
    b.Columns.AddRange(new[]
    {
        new ShipLife.Api.Domain.Column { Name="Backlog", Order=1 },
        new ShipLife.Api.Domain.Column { Name="Ready", Order=2 },
        new ShipLife.Api.Domain.Column { Name="Doing", Order=3 },
        new ShipLife.Api.Domain.Column { Name="Done", Order=4 },
    });
    db.Quarters.Add(q); db.Boards.Add(b);
    await db.SaveChangesAsync();
    return Results.Ok(new { QuarterId = q.Id, BoardId = b.Id });
});

app.Run();
