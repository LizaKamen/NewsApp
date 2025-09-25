using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsApp.Data;
using NewsApp.Models;

namespace NewsApp.Controllers;

public class AdminController : Controller
{
    private readonly NewsAppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public AdminController(NewsAppDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }
    
    [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        var news = _context.NewsItems.ToList();
        return View(news);
    }
    
    
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid? id)
    {
        var news = await _context.NewsItems.FirstOrDefaultAsync(n => n.Id.Equals(id));
        var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, news.PathToImage.TrimStart('/'));
        if (System.IO.File.Exists(imagePath))
            System.IO.File.Delete(imagePath);
        _context.NewsItems.Remove(news);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Guid? id)
    {
        var news = await _context.NewsItems.FirstOrDefaultAsync(n => n.Id.Equals(id));
        return View(news);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Edit(NewsItem newsItem)
    {
        var oldNewsItem = await _context.NewsItems.AsNoTracking().FirstOrDefaultAsync(n => n.Id.Equals(newsItem.Id));
        if (!ModelState.IsValid) return View(newsItem);

        if (newsItem.ImageFile != null && newsItem.ImageFile.Length > 0)
        {
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, oldNewsItem.PathToImage.TrimStart('/'));
            if (System.IO.File.Exists(oldImagePath))
                System.IO.File.Delete(oldImagePath);
            var fileName = $"{newsItem.Title}-{DateTime.Now.ToFileTime()}" +
                           Path.GetExtension(newsItem.ImageFile.FileName);
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await newsItem.ImageFile.CopyToAsync(fileStream);
            }

            newsItem.PathToImage = "/images/" + fileName;
        }

        _context.NewsItems.Update(newsItem);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(NewsItem newsItem)
    {
        if (ModelState.IsValid)
        {
            if (newsItem.ImageFile != null && newsItem.ImageFile.Length > 0)
            {
                var fileName = $"{newsItem.Title}-{DateTime.Now.ToFileTime()}" +
                               Path.GetExtension(newsItem.ImageFile.FileName);
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await newsItem.ImageFile.CopyToAsync(fileStream);
                }

                newsItem.PathToImage = "/images/" + fileName;
            }

            newsItem.Created = DateTime.UtcNow;
            newsItem.Author = User.Identity.Name;
            _context.Add(newsItem);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        return View(newsItem);
    }
}