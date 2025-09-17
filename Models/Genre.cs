using System.ComponentModel.DataAnnotations.Schema;

namespace Netflix_BackendAPI.Models
{
    [Table("Genres")]
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation
        public ICollection<Music> MusicTracks { get; set; } = new List<Music>();
    }
 
}
