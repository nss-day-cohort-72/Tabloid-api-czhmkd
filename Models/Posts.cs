using System.ComponentModel.DataAnnotations;

namespace Tabloid.Models;

public class Posts
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Author { get; set; }
    [Required]
    public string Category { get; set; }
    [Required]
    public DateTime PublicationDate { get; set; }
    public bool IsApproved { get; set; }

}