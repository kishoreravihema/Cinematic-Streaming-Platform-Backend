namespace Netflix_BackendAPI.DTO
{
    public class VideoDto
    {
        public string? Title { get; set; } = string.Empty;
        public string? Url { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int PlaylistId { get; set; }
        public int CategoryId { get; set; }
        public string? ThumbnailUrl { get; set; }
        public double Rating { get; set; }
        public int DurationInSeconds { get; set; }
        public string? Language { get; set; }
        public string? AgeRating { get; set; }
        public string? Genre { get; set; }

        // Only use full actor DTOs
        public ICollection<ActorResponseDto> Actors { get; set; } = new List<ActorResponseDto>();

        public string? Director { get; set; }
        public bool IsPremium { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
