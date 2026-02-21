using Discounts.Application.Extensions;
using Discounts.Application.Validators.Offers;
using Discounts.Infra.Extensions;
using Discounts.Infra.Persistence;
using Discounts.Infra.Persistence.Seeding;
using Discounts.Infra.Security;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register Application & Infrastructure layers (shared with API)
builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddInfrastructure();
builder.Services.AddApplication();

// JwtSettings binding (required by JwtService which AuthService depends on)
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateOfferCommandValidator>();

// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Area route must be registered before default route
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Run database migration & seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
    var transaction = await context.Database.BeginTransactionAsync();
    try
    {
        await SeedDataGenerator.SeedAsync(context: context);
        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}

app.Run();