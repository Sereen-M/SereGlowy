using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SereGlowy.Models;

namespace SereGlowy.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext(options)
    {
        public DbSet<SkinProfile> SkinProfiles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<ProductIngredient> ProductIngredients { get; set; }
        public DbSet<MyProduct> MyProducts { get; set; }
        public DbSet<MakeupProduct> MakeupProducts { get; set; }
        public DbSet<RoutineRecommendation> RoutineRecommendations { get; set; }
    }
}