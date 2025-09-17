using Microsoft.EntityFrameworkCore;
using Netflix_BackendAPI.Data;
using Netflix_BackendAPI.Interface.IRepository;
using Netflix_BackendAPI.DTO;

namespace Netflix_BackendAPI.Repository
{
    public class MediaRepository : IMediaRepository
    {
        private readonly ApplicationDbContext _context;

        public MediaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AlbumDto>> GetAllAlbumsAsync()
        {
            return await _context.Albums
                .AsNoTracking()
                .Select(a => new AlbumDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    ReleaseDate = a.ReleaseDate,
                    CoverImagePath = a.CoverImagePath,
                    UserName = a.User != null ? a.User.UserName : null,
                    MusicTracks = a.MusicTracks.Select(m => new MusicDto
                    {
                        Title = m.Title,
                        Url = m.Url,
                        Description = m.Description,
                        Lyrics = m.Lyrics,
                        ThumbnailUrl = m.ThumbnailUrl,
                        Language = m.Language,
                        DurationInSeconds = m.DurationInSeconds,
                        IsExplicit = m.IsExplicit,
                        IsPremium = m.IsPremium,
                        PlayCount = m.PlayCount,
                        ReleaseDate = m.ReleaseDate,
                        CreatedAt = m.CreatedAt,
                        UpdatedAt = m.UpdatedAt,
                        AlbumId = m.AlbumId,
                        GenreId = m.GenreId,
                        UserId = m.UserId,
                        Artists = m.Artists.Select(ar => new ArtistResponseDto
                        {
                            Id = ar.Id,
                            Name = ar.Name
                        }).ToList()
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ArtistDto>> GetAllArtistsAsync()
        {
            return await _context.Artists
                .AsNoTracking()
                .Select(ar => new ArtistDto
                {
                    Id = ar.Id,
                    Name = ar.Name,
                    MusicTracks = ar.MusicTracks.Select(m => new MusicDto
                    {
                        Title = m.Title,
                        Url = m.Url,
                        Description = m.Description,
                        Lyrics = m.Lyrics,
                        ThumbnailUrl = m.ThumbnailUrl,
                        Language = m.Language,
                        DurationInSeconds = m.DurationInSeconds,
                        IsExplicit = m.IsExplicit,
                        IsPremium = m.IsPremium,
                        PlayCount = m.PlayCount,
                        ReleaseDate = m.ReleaseDate,
                        CreatedAt = m.CreatedAt,
                        UpdatedAt = m.UpdatedAt,
                        AlbumId = m.AlbumId,
                        GenreId = m.GenreId,
                        UserId = m.UserId,
                        Artists = m.Artists.Select(a2 => new ArtistResponseDto
                        {
                            Id = a2.Id,
                            Name = a2.Name
                        }).ToList()
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<GenreDto>> GetAllGenresAsync()
        {
            return await _context.Genres
                .AsNoTracking()
                .Select(g => new GenreDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    MusicTracks = g.MusicTracks.Select(m => new MusicDto
                    {
                        Title = m.Title,
                        Url = m.Url,
                        Description = m.Description,
                        Lyrics = m.Lyrics,
                        ThumbnailUrl = m.ThumbnailUrl,
                        Language = m.Language,
                        DurationInSeconds = m.DurationInSeconds,
                        IsExplicit = m.IsExplicit,
                        IsPremium = m.IsPremium,
                        PlayCount = m.PlayCount,
                        ReleaseDate = m.ReleaseDate,
                        CreatedAt = m.CreatedAt,
                        UpdatedAt = m.UpdatedAt,
                        AlbumId = m.AlbumId,
                        GenreId = m.GenreId,
                        UserId = m.UserId,
                        Artists = m.Artists.Select(a2 => new ArtistResponseDto
                        {
                            Id = a2.Id,
                            Name = a2.Name
                        }).ToList()
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PlaylistMusicDto>> GetAllPlaylistMusicAsync()
        {
            return await _context.PlaylistMusics
                .AsNoTracking()
                .Select(pm => new PlaylistMusicDto
                {
                    PlaylistId = pm.PlaylistId,
                    PlaylistName = pm.Playlist.Name,
                    MusicId = pm.MusicId,
                    MusicTitle = pm.Music.Title,
                    AlbumTitle = pm.Music.Album != null ? pm.Music.Album.Title : null,
                    CoverImagePath = pm.coverImagePath,
                    IsPremium = pm.IsPremium,
                    Artists = pm.Music.Artists.Select(ar => new ArtistDto
                    {
                        Id = ar.Id,
                        Name = ar.Name,
                        MusicTracks = ar.MusicTracks.Select(m => new MusicDto
                        {
                            Title = m.Title,
                            Url = m.Url,
                            Description = m.Description,
                            Lyrics = m.Lyrics,
                            ThumbnailUrl = m.ThumbnailUrl,
                            Language = m.Language,
                            DurationInSeconds = m.DurationInSeconds,
                            IsExplicit = m.IsExplicit,
                            IsPremium = m.IsPremium,
                            PlayCount = m.PlayCount,
                            ReleaseDate = m.ReleaseDate,
                            CreatedAt = m.CreatedAt,
                            UpdatedAt = m.UpdatedAt,
                            AlbumId = m.AlbumId,
                            GenreId = m.GenreId,
                            UserId = m.UserId,
                            Artists = m.Artists.Select(a2 => new ArtistResponseDto
                            {
                                Id = a2.Id,
                                Name = a2.Name
                            }).ToList()
                        }).ToList()
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PlaylistVideoDto>> GetAllPlaylistVideosAsync()
        {
            return await _context.PlaylistVideos
                .AsNoTracking()
                .Select(pv => new PlaylistVideoDto
                {
                    PlaylistId = pv.PlaylistId,
                    PlaylistName = pv.Playlist.Name,
                    VideoId = pv.VideoId,
                    VideoTitle = pv.Video.Title,
                    ThumbnailUrl = pv.Video.ThumbnailUrl,
                    IsPremium = pv.IsPremium
                })
                .ToListAsync();
        }
    }
}
