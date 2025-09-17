namespace Netflix_BackendAPI.DTO
{
    public class AlbumDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? CoverImagePath { get; set; }
        public string? UserName { get; set; }
        public List<MusicDto> MusicTracks { get; set; } = new();
    }

    public class ArtistDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<MusicDto> MusicTracks { get; set; } = new();
    }

    public class GenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<MusicDto> MusicTracks { get; set; } = new();
    }

    public class PlaylistMusicDto
    {
        public int PlaylistId { get; set; }
        public string PlaylistName { get; set; } = string.Empty;
        public int MusicId { get; set; }
        public string MusicTitle { get; set; } = string.Empty;
        public string? AlbumTitle { get; set; }
        public string? CoverImagePath { get; set; }
        public List<ArtistDto> Artists { get; set; } = new();
        public bool IsPremium { get; internal set; }
    }

    public class PlaylistVideoDto
    {
        public int PlaylistId { get; set; }
        public string PlaylistName { get; set; } = string.Empty;
        public int VideoId { get; set; }
        public string VideoTitle { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public bool IsPremium { get; internal set; }
    }
}
