using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsApp.Data;
using NewsApp.Models;
using NewsApp.ViewModels;

namespace NewsApp.Controllers;

public class NewsController : Controller
{
    private readonly NewsAppDbContext _context;
    private const int PageSize = 5;

    public NewsController(NewsAppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var news = await _context.NewsItems.ToListAsync();
        var items = news.Skip((page - 1) * PageSize).Take(PageSize).ToList();
        var pagingInfo = new PagingInfo()
        {
            CurrentPage = page,
            ItemsPerPage = PageSize,
            TotalItems = news.Count()
        };

        var viewModel = new IndexViewModel<NewsItem>()
        {
            Items = items,
            PagingInfo = pagingInfo
        };
        return View(viewModel);
    }

    public async Task<IActionResult> Details(Guid? id)
    {
        var news = await _context.NewsItems.FirstOrDefaultAsync(n => n.Id.Equals(id));
        return View(news);
    }

}