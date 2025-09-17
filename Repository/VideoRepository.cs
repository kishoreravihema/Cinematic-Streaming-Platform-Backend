using Microsoft.EntityFrameworkCore;
using Netflix_BackendAPI.Class;
using Netflix_BackendAPI.Data;
using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Interface.IRepository;
using Netflix_BackendAPI.Models;
using System.Linq;

namespace Netflix_BackendAPI.Repository
{
    public class VideoRepository:IVideoRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _UnitOfWork;
        public VideoRepository(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _UnitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Video>> GetAllVideosAsync()
        {
            return await _context.Videos.ToListAsync();
        }
        public async Task<IEnumerable<Video>> GetPagedVideosAsync(int skip, int take)
        {
            return await _context.Videos
                .Include(v => v.Actors)
                .Include(v => v.Category)
                .Include(v => v.PlaylistVideos)
                    .ThenInclude(pv => pv.Playlist) // 🔥 This includes the Playlists properly
                .OrderByDescending(v => v.CreatedAt)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<Video?> GetVideoByIdAsync(int id)
        {
            //return await _context.Videos.FindAsync(id);
            return await _context.Videos
      .AsNoTracking()
      .FirstOrDefaultAsync(v => v.Id == id);
        }
        public async Task<IEnumerable<Video>> GetVideosByPlaylistIdAsync(int playlistId)
        {
            return await _context.Videos
                .Where(v => v.PlaylistId == playlistId)
                .ToListAsync();
        }
        public async Task<Video?> AddVideoAsync(Video video)
        {
            _context.Videos.Add(video);
            await _UnitOfWork.SaveChangesAsync();
            return video;
        }
        public async Task<List<Actor>> GetOrCreateActorsAsync(List<ActorResponseDto> actorDtos)
        {
            if (actorDtos == null || !actorDtos.Any())
                return new List<Actor>();

            // Normalize names for comparison
            var inputNames = actorDtos
                .Select(a => a.Name.Trim().ToLower())
                .ToHashSet();

            var inputIds = actorDtos
                .Where(a => a.Id > 0)
                .Select(a => a.Id)
                .ToList();

            // Fetch existing actors by Id or Name
            var existingActors = await _context.Actors
                .Where(a => inputIds.Contains(a.Id) || inputNames.Contains(a.Name.ToLower()))
                .ToListAsync();

            var existingIds = existingActors.Select(a => a.Id).ToHashSet();
            var existingNames = existingActors.Select(a => a.Name.Trim().ToLower()).ToHashSet();

            // Identify missing actors by name
            var missingActors = actorDtos
                .Where(a => !existingIds.Contains(a.Id) && !existingNames.Contains(a.Name.Trim().ToLower()))
                .Select(a => new Actor
                {
                    Name = a.Name
                })
                .ToList();

            if (missingActors.Any())
            {
                await _context.Actors.AddRangeAsync(missingActors);
                await _UnitOfWork.SaveChangesAsync();
                existingActors.AddRange(missingActors);
            }

            return existingActors;
        }
        public async Task<Video?> AddVideoWithManualIdAsync(Video video)
        {
            try
            {
                await _UnitOfWork.BeginTransactionAsync();
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Videos ON");
                _context.Videos.Add(video);
                await _UnitOfWork.SaveChangesAsync();
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Videos OFF");
                await _UnitOfWork.CommitAsync();
                return video;
            }
            catch(Exception ex)
            {
                await _UnitOfWork.RollbackAsync();
                LogExceptionHandler.LogException(ex,"Error in AddVideoWithManualIdAsync");
                return null;
            }

        }
      public async Task<Video?> UpdateVideoAsync(Video video)
        {
            // 1. Load the existing video with its actors
            var existingVideo = await _context.Videos
                .Include(v => v.Actors)
                .FirstOrDefaultAsync(v => v.Id == video.Id);

            if(existingVideo == null)
            {
                return null;
            }

            //2.Update the all scalar properties

            _context.Entry(existingVideo).CurrentValues.SetValues(video);

            //3. replace the many-to-many relationship list
            existingVideo.Actors.Clear();
            foreach(var actor in video.Actors)
            {
                if(!_context.Actors.Local.Any(a=>a.Id==actor.Id))
                {
               // If the actor is not already tracked, attach it
                    _context.Actors.Attach(actor);
                }
            }
            //4.Update the timestamp
            existingVideo.UpdatedAt = DateTime.UtcNow;
            //5.Savechanges
            await _UnitOfWork.SaveChangesAsync();
            return existingVideo;
        }
        public async Task<bool> DeleteVideoAsync(int id)
        {
            var video = await _context.Videos.SingleOrDefaultAsync(v => v.Id == id);
            if (video == null)
            {
                return false;
            }
            _context.Videos.Remove(video);
            await _UnitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<bool> VideoExistsAsync(int id)
        {
            return await _context.Videos.AnyAsync(v => v.Id == id);
        }
        public async Task<IEnumerable<Video>> GetVideosByCategoryAsync(string categoryName)
        {
            return await _context.Videos
                .Include(v => v.Category)
                .Include(v => v.Actors)
                .Where(v => v.Category != null && v.Category.Name.ToLower() == categoryName.ToLower())
                .ToListAsync();
        }




    }
}
