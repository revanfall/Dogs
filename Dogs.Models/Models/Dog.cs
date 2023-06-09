using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dogs.Models.Models
{
    public class Dog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Color { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        [JsonPropertyName("tail_length")]
        public int TailLength { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Weight { get; set; }
    }
}
