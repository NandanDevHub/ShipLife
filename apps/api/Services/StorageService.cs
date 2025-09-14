using Microsoft.AspNetCore.StaticFiles;

namespace ShipLife.Api.Services;

public interface IStorageService
{
    Task<(string url, long size, string mime)> SaveAsync(IFormFile file, CancellationToken ct);
}

public class LocalStorageService(IConfiguration cfg, IWebHostEnvironment env) : IStorageService
{
    public async Task<(string url, long size, string mime)> SaveAsync(IFormFile file, CancellationToken ct)
    {
        var root = cfg["Storage:LocalUploadRoot"] ?? "wwwroot/uploads";
        Directory.CreateDirectory(root);

        var fname = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var path = Path.Combine(root, fname);

        await using var fs = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(fs, ct);

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(path, out var mime)) mime = "application/octet-stream";

        var rel = $"/uploads/{fname}";
        return (rel, file.Length, mime!);
    }
}
