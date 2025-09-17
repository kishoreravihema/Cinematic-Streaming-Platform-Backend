namespace Netflix_BackendAPI.Models
{
    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation
        public ICollection<Music> MusicTracks { get; set; } = new List<Music>();
    }

}
