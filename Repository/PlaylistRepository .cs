using Microsoft.EntityFrameworkCore;
using Netflix_BackendAPI.Data;
using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Interface.IRepository;
using Netflix_BackendAPI.Models;

namespace Netflix_BackendAPI.Repository
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly ApplicationDbContext _context;
   

        public PlaylistRepository(ApplicationDbContext context)
        {
            _context = context;
         
        }

        public async Task<bool> MusicExistsInPlaylistAsync(int playlistId, int musicId)
        {
            return await _context.PlaylistMusics
                .AnyAsync(pm => pm.PlaylistId == playlistId && pm.MusicId == musicId);
        }

        public async Task<bool> VideoExistsInPlaylistAsync(int playlistId, int videoId)
        {
            return await _context.PlaylistVideos
                .AnyAsync(pv => pv.PlaylistId == playlistId && pv.VideoId == videoId);
        }
        public async Task AddMusicAsync(PlaylistMusic entry)
        {
            await _context.PlaylistMusics.AddAsync(entry);
        }

        public async Task AddVideoAsync(PlaylistVideo entry)
        {
            await _context.PlaylistVideos.AddAsync(entry);
        }
        public async Task<bool> PlaylistExistsForUserAsync(int userId)

        {
            return await _context.Playlists.AnyAsync(p => p.UserId == userId);
        }
        public async Task<Playlist?> GetByIdAsync(int playlistId)
        {
            return await _context.Playlists
                .Include(p => p.PlaylistMusics)
                    .ThenInclude(pm => pm.Music)
                .Include(p => p.PlaylistVideos)
                    .ThenInclude(pv => pv.Video)
                .FirstOrDefaultAsync(p => p.Id == playlistId);
        }
        public async Task AddAsync(Playlist playlist)
        {
            await _context.Playlists.AddAsync(playlist);
         
        }
        public async Task<List<PlaylistDto>> GetAllPlaylistsWithMediaAsync()
        {
            var playlists = await _context.Playlists
                .Where(p => p.IsPublic)
                .Include(p => p.PlaylistVideos).ThenInclude(pv => pv.Video)
                .Include(p => p.PlaylistMusics).ThenInclude(pm => pm.Music).ThenInclude(m => m.Artists)
                .ToListAsync(); // ✅ Fetch into memory first

            var playlistDtos = playlists.Select(p => new PlaylistDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                IsPublic = p.IsPublic,
                CoverImageUrl = p.CoverImageUrl,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Tags = p.Tags, // ✅ Safe now because it's in memory

                Videos = p.PlaylistVideos.Select(v => new VideoDto
                {
                    Title = v.Video.Title,
                    Url = v.Video.Url,
                    DurationInSeconds = v.Video.DurationInSeconds,
                    Description = v.Video.Description,
                    ThumbnailUrl = v.Video.ThumbnailUrl,
                    Language = v.Video.Language,
                    AgeRating = v.Video.AgeRating,
                    Genre = v.Video.Genre,
                    Director = v.Video.Director,
                    IsPremium = v.Video.IsPremium,
                    ReleaseDate = v.Video.ReleaseDate,
                    CreatedAt = v.Video.CreatedAt,
                    Rating = v.Video.Rating,
                    Actors = v.Video.Actors.Select(a => new ActorResponseDto
                    {
                        Id = a.Id,
                        Name = a.Name
                    }).ToList(),
                  
                }).ToList(),

                Musics = p.PlaylistMusics.Select(m => new MusicDto
                {
                    // Core Info
                    Title = m.Music.Title,
                    Url = m.Music.Url,
                    Description = m.Music.Description,
                    Lyrics = m.Music.Lyrics,
                    ThumbnailUrl = m.Music.ThumbnailUrl,
                    Language = m.Music.Language,

                    // Media Info
                    DurationInSeconds = m.Music.DurationInSeconds,
                    IsExplicit = m.Music.IsExplicit,
                    IsPremium = m.Music.IsPremium,
                    PlayCount = m.Music.PlayCount,

                    // Dates
                    ReleaseDate = m.Music.ReleaseDate,
                    CreatedAt = m.Music.CreatedAt,
                    UpdatedAt = m.Music.UpdatedAt,

                    // Foreign Keys
                    AlbumId = m.Music.AlbumId,
                    GenreId = m.Music.GenreId,
                    UserId = m.Music.UserId,

                    // Navigation
                    Artists = m.Music.Artists.Select(a => new ArtistResponseDto
                    {
                        Id = a.Id,
                        Name = a.Name
                    }).ToList()
                }).ToList()
            }).ToList();

            return playlistDtos;
        }

    }
}
