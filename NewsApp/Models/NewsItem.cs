using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsApp.Models;

public class NewsItem
{
    public Guid Id  { get; set; }
    [Required(ErrorMessageResourceName = "TitleRequired", ErrorMessageResourceType = typeof(Resources.Resources))]
    [MaxLength(25)]
    public string Title { get; set; }
    
    [Required(ErrorMessageResourceName = "SubtitleRequired", ErrorMessageResourceType = typeof(Resources.Resources))]
    [MaxLength(50)]
    public string SubTitle { get; set; }
    
    [Required(ErrorMessageResourceName = "ContentRequired", ErrorMessageResourceType = typeof(Resources.Resources))]
    [MaxLength(2000)]
    public string Content { get; set; }
    public DateTime? Created { get; set; }
    public string? Author { get; set; }
    public string? PathToImage { get; set; }
    
    [Required(ErrorMessageResourceName = "ImageRequired", ErrorMessageResourceType = typeof(Resources.Resources))]
    [NotMapped]
    public IFormFile ImageFile { get; set; }
}