using NewsApp.Models;

namespace NewsApp.ViewModels;

public class IndexViewModel<T>
{
    public IEnumerable<T> Items { get; set; }
    public PagingInfo PagingInfo { get; set; }
}