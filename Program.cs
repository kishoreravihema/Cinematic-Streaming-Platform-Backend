using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Netflix_BackendAPI.Data;
using Netflix_BackendAPI.Extensions;
using Netflix_BackendAPI.Helper;

var builder = WebApplication.CreateBuilder(args);

// ✅ CORS configuration for Angular frontend
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ✅ Core services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ Swagger for development
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Music API", Version = "v1" });

    c.MapType<FileStreamResult>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });

    c.OperationFilter<BinaryResponseOperationFilter>();
});

// ✅ EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Custom service registrations
builder.Services.AddCustomServices();

// ✅ Large file upload limits (Form + Kestrel)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50L * 1024 * 1024 * 1024; // 50 GB
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 50L * 1024 * 1024 * 1024; // 50 GB
});

// ✅ Response compression (for faster streaming)
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

// ✅ Swagger in dev environment only
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Ensure required folders exist
if (string.IsNullOrEmpty(app.Environment.WebRootPath))
{
    app.Environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
}
Directory.CreateDirectory(Path.Combine(app.Environment.WebRootPath, "uploads"));
Directory.CreateDirectory(Path.Combine(app.Environment.WebRootPath, "thumbnails"));
Directory.CreateDirectory(Path.Combine(app.Environment.WebRootPath, "hls"));

// ✅ Custom MIME provider
var mimeProvider = MediaHelper.GetCustomMimeProvider();

// ✅ Serve /uploads with CORS headers
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.WebRootPath, "uploads")),
    RequestPath = "/uploads",
    ContentTypeProvider = mimeProvider,
    ServeUnknownFileTypes = true,
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:4200");
    }
});

// ✅ Serve /thumbnails with CORS headers
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.WebRootPath, "thumbnails")),
    RequestPath = "/thumbnails",
    ContentTypeProvider = mimeProvider,
    ServeUnknownFileTypes = true,
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:4200");
    }
});
// ✅ Serve /hls with CORS headers (HLS playlist + segments)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.WebRootPath, "hls")),
    RequestPath = "/hls",
    ServeUnknownFileTypes = true, // allow .m3u8 and .ts
    ContentTypeProvider = mimeProvider,
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:4200");
    }
});
// ✅ Serve /PlaylistThumbnail with CORS headers
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.WebRootPath, "PlaylistThumbnail")),
    RequestPath = "/PlaylistThumbnail",
    ContentTypeProvider = mimeProvider,
    ServeUnknownFileTypes = true,
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:4200");
    }
});


// ✅ Middleware order
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();

// ✅ Route to controllers
app.MapControllers();

app.Run();
