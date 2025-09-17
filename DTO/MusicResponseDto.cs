namespace Netflix_BackendAPI.DTO
{
    public class MusicResponseDto
    {
        public int Id { get; set; }

        // Core Info
        public string Title { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? Description { get; set; }
        public string? Lyrics { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Language { get; set; }

        // Media Info
        public int DurationInSeconds { get; set; }
        public bool IsExplicit { get; set; }
        public bool IsPremium { get; set; }
        public int PlayCount { get; set; }

        // Dates
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Foreign Keys
        public int AlbumId { get; set; }
        public int GenreId { get; set; }
        public int? UserId { get; set; }

       
      
        public List<ArtistResponseDto> Artists { get; set; } = new();
    }

}
//public class MusicResponseDto
//{
//    public int Id { get; set; }
//    public string Title { get; set; } = string.Empty;
//    public string Url { get; set; } = string.Empty;
//    public string? Description { get; set; }
//    public int AlbumId { get; set; }
//    public int GenreId { get; set; }
//    public string? Language { get; set; }
//    public bool IsPremium { get; set; }
//    public DateTime ReleaseDate { get; set; }
//    public DateTime CreatedAt { get; set; }
//    public List<ArtistResponseDto> Artists { get; set; } = new();
//}