using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WorldDominion.Models;
using WorldDominion.Services;

var builder = WebApplication.CreateBuilder(args);

//Add session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); //This session will only last 30 minutes
    //We can also set it for hours
    //Session : Their cart information will last for the time mentioned above.
    //If the user closes the browser, the session will die 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Add services to the container.
builder.Services.AddControllersWithViews();

//Once we do this it will be available to all the controllers and models and everything that is available in our application 
//defining the connection

//Add MySQL
var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string not found");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySQL(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Registering the DbInitializer seeder
builder.Services.AddTransient<DbInitializer>();

builder.Services.AddAuthentication().AddGoogle(options => {
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

builder.Services.AddScoped<CartService>();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

//Enable sessions on our requests
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Seed roles
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
await DbInitializer.Initialize(
    scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>(),
    scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>()
);

app.Run();
