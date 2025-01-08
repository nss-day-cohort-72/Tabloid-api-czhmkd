using System.ComponentModel.DataAnnotations;

namespace Tabloid.Models.DTOs
{
    public class CreatePostDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public int CategoryId { get; set; }


        public string HeaderImage { get; set; }
        public DateTime? PublicationDate { get; set; }
    }
}
