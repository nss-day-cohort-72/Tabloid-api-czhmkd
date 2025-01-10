using Tabloid.Models;

public class PostTag
{
    public int PostId { get; set; }
    public Posts Post { get; set; }

    public int TagId { get; set; }
    public Tag Tag { get; set; }
}