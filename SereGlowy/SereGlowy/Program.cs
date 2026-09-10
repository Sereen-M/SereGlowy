using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SereGlowy.Data;
using SereGlowy.Models;

var builder = WebApplication.CreateBuilder(args);

// =========================
// Database Connection
// =========================

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


// =========================
// Identity + Roles
// =========================

builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddControllersWithViews();

var app = builder.Build();


// =========================
// Create Roles + Admin Account
// + Fix Existing User Roles
// + Seed Categories
// =========================

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<IdentityUser>>();

    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();


    // =========================
    // Create Admin Role
    // =========================

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(
            new IdentityRole("Admin"));
    }


    // =========================
    // Create User Role
    // =========================

    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(
            new IdentityRole("User"));
    }


    // =========================
    // Create Admin Account
    // =========================

    string adminEmail = "admin@sereglowy.com";
    string adminPassword = "Admin@123";

    var adminUser =
        await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result =
            await userManager.CreateAsync(
                adminUser,
                adminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(
                adminUser,
                "Admin");
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(
            adminUser,
            "Admin"))
        {
            await userManager.AddToRoleAsync(
                adminUser,
                "Admin");
        }
    }


    // =========================
    // Give Existing Users
    // the User Role
    // =========================

    var existingUsers =
        await userManager.Users.ToListAsync();

    foreach (var user in existingUsers)
    {
        var roles =
            await userManager.GetRolesAsync(user);

        if (!roles.Any() &&
            user.Email != adminEmail)
        {
            await userManager.AddToRoleAsync(
                user,
                "User");
        }
    }


    // =========================
    // Seed Categories
    // =========================

    if (!await dbContext.Categories.AnyAsync())
    {
        var categories = new List<Category>
        {
            new Category
            {
                CategoryName = "Cleanser",
                Description =
                    "Products used to cleanse the skin and remove dirt and impurities."
            },

            new Category
            {
                CategoryName = "Moisturizer",
                Description =
                    "Products used to hydrate the skin and maintain moisture."
            },

            new Category
            {
                CategoryName = "Serum",
                Description =
                    "Concentrated skincare products used for specific skincare needs."
            },

            new Category
            {
                CategoryName = "Sunscreen",
                Description =
                    "Products used to protect the skin from sun exposure."
            },

            new Category
            {
                CategoryName = "Toner",
                Description =
                    "Products used after cleansing to prepare the skin for the next routine steps."
            }
        };

        dbContext.Categories.AddRange(categories);

        await dbContext.SaveChangesAsync();
    }
}


// =========================
// HTTP Request Pipeline
// =========================

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();


// =========================
// Authentication
// =========================

app.UseAuthentication();
app.UseAuthorization();


// =========================
// Routes
// =========================

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();