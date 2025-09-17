using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Netflix_BackendAPI.Data;
using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Interface.IRepository;
using Netflix_BackendAPI.Models;
using System.Linq;

namespace Netflix_BackendAPI.Repository
{
    public class MusicRepository : IMusicRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MusicRepository> _logger;
        public MusicRepository(ApplicationDbContext context, IUnitOfWork unitOfWork,ILogger<MusicRepository> logger)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _logger = logger;

        }
        public async Task<IEnumerable<Music>> GetAllAsync()
        {
            return await _context.Music.Include(m => m.Album)
                                       .Include(m => m.Genre)
                                       .ToListAsync();
        }
        public async Task<Music?> GetByIdAsync(int id)
        {
            return await _context.Music.Include(m => m.Album)
                                       .Include(m => m.Genre)
                                       .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task<IEnumerable<Music>> GetByAlbumAsync(int albumId)
        {
            return await _context.Music.Where(m => m.AlbumId == albumId)
                                       .Include(m => m.Album)
                                       .Include(m => m.Genre)
                                       .ToListAsync();
        }
        public async Task<IEnumerable<Music>> GetByGenreAsync(string genre)
        {
            return await _context.Music
               .Include(m => m.Artists)
               .Include(m => m.Genre)
               .Where(m => m.Genre.Name.ToLower() == genre.ToLower())
                .ToListAsync();
        }
        public async Task<IEnumerable<Music>> GetByArtistAsync(int artistId)
        {
            return await _context.Music
                .Include(m => m.Artists)
                .Where(m => m.Artists.Any(a => a.Id == artistId))
                .ToListAsync();
        }
        public async Task<Music?> AddAsync(Music music)
        {
            try
            {
                _context.Music.Add(music);
                await _unitOfWork.SaveChangesAsync();
                return music;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }
        public async Task<Music?> AddWithManualIdAsync(Music music)
        {
            var exists = await _context.Music.AnyAsync(m => m.Id == music.Id);
            if (exists)
            {
                return null; // Prevent duplicate key violation
            }

            _context.Music.Add(music);
            await _context.SaveChangesAsync();
            return music;
        }

        public async Task<Music?> UpdateAsync(Music music)
        {
            var existingMusic = await _context.Music.Include(m => m.Artists).FirstOrDefaultAsync(m => m.Id == music.Id);
            if (existingMusic == null)
            {
                return null;
            }

            // Update properties
            existingMusic.Title = music.Title;
            existingMusic.Url = music.Url;
            existingMusic.Description = music.Description;
            existingMusic.AlbumId = music.AlbumId;
            existingMusic.GenreId = music.GenreId;
            existingMusic.Language = music.Language;
            existingMusic.IsPremium = music.IsPremium;
            existingMusic.ReleaseDate = music.ReleaseDate;
            existingMusic.UpdatedAt = DateTime.UtcNow;

            // Update Artists (if needed)
            existingMusic.Artists = music.Artists;

            _context.Music.Update(existingMusic);
            await _unitOfWork.SaveChangesAsync();
            return existingMusic;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var music = await _context.Music.FindAsync(id);
            if (music == null)
            {
                return false;
            }
            _context.Music.Remove(music);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Music.AnyAsync(m => m.Id == id);
        }
        public async Task<List<Artist>> GetArtistsByIdsAsync(List<int> ids)
        {
            return await _context.Artists
                .Where(a => ids.Contains(a.Id))
                .ToListAsync();
        }
        public async Task<IEnumerable<Music>> GetPagedAsync(int skip, int take)
        {
            return await _context.Music
                .Include(m => m.Artists)
                .Include(m => m.Album)
                .Include(m => m.Genre)
                .OrderByDescending(m => m.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
        public async Task<bool> GenreExistsAsync(int genreId)
        {
            return await _context.Genres.AnyAsync(g => g.Id == genreId);
        }
        public async Task<List<Artist>> GetOrCreateArtistsAsync(List<ArtistResponseDto> artistDtos)
        {
            if (artistDtos == null || !artistDtos.Any())
                return new List<Artist>();

            // Normalize names for comparison
            var inputNames = artistDtos
                .Select(a => a.Name.Trim().ToLower())
                .ToHashSet();

            var inputIds = artistDtos
                .Where(a => a.Id > 0)
                .Select(a => a.Id)
                .ToList();

            // Fetch existing artists by Id or Name
            var existingArtists = await _context.Artists
                .Where(a => inputIds.Contains(a.Id) || inputNames.Contains(a.Name.ToLower()))
                .ToListAsync();

            var existingIds = existingArtists.Select(a => a.Id).ToHashSet();
            var existingNames = existingArtists.Select(a => a.Name.Trim().ToLower()).ToHashSet();

            // Identify missing artists by name
            var missingArtists = artistDtos
                .Where(a => !existingIds.Contains(a.Id) && !existingNames.Contains(a.Name.Trim().ToLower()))
                .Select(a => new Artist
                {
                    Name = a.Name
                })
                .ToList();

            if (missingArtists.Any())
            {
                await _context.Artists.AddRangeAsync(missingArtists);
                await _unitOfWork.SaveChangesAsync();
                existingArtists.AddRange(missingArtists);
            }

            return existingArtists;
        }
        public async Task<bool> AlbumExistsAsync(int albumId)
        {
            return await _context.Albums.AnyAsync(a => a.Id == albumId);
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }


    }
}
