namespace Netflix_BackendAPI.DTO
{
    public class VideoResponseDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? VideoUrl { get; set; }
        public string? Description { get; set; }
        public int? PlaylistId { get; set; }
        public int? CategoryId { get; set; }
        public string? ThumbnailUrl { get; set; }
        public double Rating { get; set; }
        public int DurationInSeconds { get; set; }
        public string? Language { get; set; }
        public string? AgeRating { get; set; }
        public string? Genre { get; set; }
        public string? Director { get; set; }
        public bool IsPremium { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Instead of the full Actor entity, we use the Actor DTO
        public ICollection<ActorResponseDto> Actors { get; set; } = new List<ActorResponseDto>();
    }
}
