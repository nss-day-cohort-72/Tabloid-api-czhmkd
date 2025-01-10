namespace Tabloid.Models.DTOs
{
    public class EditPostDTO
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }
        public string HeaderImage { get; set; }
        public DateTime? PublicationDate { get; set; }
    }
}
