using System.ComponentModel.DataAnnotations.Schema;

namespace NewsApp.Models;

public class NewsItem
{
    public Guid Id  { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string Content { get; set; }
    public string? PathToImage { get; set; }
    
    [NotMapped]
    public IFormFile ImageFile { get; set; }
}