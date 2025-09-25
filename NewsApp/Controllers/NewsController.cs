using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsApp.Data;

namespace NewsApp.Controllers;

public class NewsController : Controller
{
    private readonly NewsAppDbContext _context;

    public NewsController(NewsAppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var news = await _context.NewsItems.ToListAsync();
        return View(news);
    }

    public async Task<IActionResult> Details(Guid? id)
    {
        var news = await _context.NewsItems.FirstOrDefaultAsync(n => n.Id.Equals(id));
        return View(news);
    }

}