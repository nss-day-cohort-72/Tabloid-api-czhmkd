namespace Tabloid.Models;

public class Comment
{
    public int Id { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; }
    public int PostId { get; set; }
    public Posts Post { get; set; }
}