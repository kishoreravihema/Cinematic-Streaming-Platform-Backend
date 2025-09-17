using Netflix_BackendAPI.Interface.IRepository;
using Netflix_BackendAPI.Interface.IServices;
using Netflix_BackendAPI.Repository;
using Netflix_BackendAPI.Service;
using Netflix_BackendAPI.Services;

namespace Netflix_BackendAPI.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            // Register your services 
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<IMusicService, MusicService>();
            services.AddScoped<IPlaylistService, PlaylistService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IFfmpegService, FfmpegService>();
            services.AddScoped<IMediaService, MediaService>();
            

            // Register your repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IVideoRepository, VideoRepository>();
            services.AddScoped<IMusicRepository, MusicRepository>();
            services.AddScoped<IPlaylistRepository, PlaylistRepository>();
            services.AddScoped<IMediaRepository, MediaRepository>();
            // Configure HTTP features
            services.AddHttpClient();
           
          

            return services;
        }
    }
}

