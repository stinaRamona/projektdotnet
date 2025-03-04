using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using projektdotnet.Models;

namespace projektdotnet.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProductModel> Products { get; set; }

    public DbSet<CategoryModel> Categories { get; set; }

}
