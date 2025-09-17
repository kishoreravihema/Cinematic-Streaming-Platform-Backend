namespace Netflix_BackendAPI.DTO
{
    public class MusicDto
    {
        // Core Info
        public string? Title { get; set; } = string.Empty;
        public string? Url { get; set; } = string.Empty;      
        public string? Description { get; set; }
        public string? Lyrics { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Language { get; set; }

        // Media Info
        public int? DurationInSeconds { get; set; }
        public bool IsExplicit { get; set; } = false;
        public bool IsPremium { get; set; } = false;
        public int PlayCount { get; set; } = 0;

        // Dates
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public int AlbumId { get; set; }
        public int GenreId { get; set; }
        public int? UserId { get; set; }

        // Navigation
        public List<ArtistResponseDto> Artists { get; set; } = new();
    }
    //public class MusicDto
    //{
    //    public string? Title { get; set; } = string.Empty;
    //    public string? AudioUrl { get; set; } = string.Empty;
    //    public string? Description { get; set; }
    //    public int AlbumId { get; set; }
    //    public int GenreId { get; set; }
    //    public string? Language { get; set; }
    //    public bool IsPremium { get; set; }
    //    public int? DurationInSeconds { get; set; }
    //    public DateTime ReleaseDate { get; set; }
    //    public DateTime CreatedAt { get; set; }
    //    public List<ArtistResponseDto> Artists { get; set; } = new();

    //}

}
