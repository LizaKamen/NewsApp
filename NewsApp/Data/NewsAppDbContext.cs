using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsApp.Models;

namespace NewsApp.Data;

public class NewsAppDbContext : IdentityDbContext<User>
{
    public NewsAppDbContext(DbContextOptions<NewsAppDbContext> options) : base(options)
    {
    }
    public DbSet<NewsItem> NewsItems { get; set; }
}