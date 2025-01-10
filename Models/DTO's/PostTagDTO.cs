namespace Tabloid.Models.DTOs
{
    public class PostTagDTO
    {
        public int PostId { get; set; }
        public List<int> TagIds { get; set; }
    }
}
