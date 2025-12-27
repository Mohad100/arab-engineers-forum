using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Fourm.Services;
using Fourm.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();

// Get connection string from environment variable (Render/Fly.io) or appsettings
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

// If connection string is empty, use a default for development
if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString = "Host=localhost;Port=5433;Database=ForumDb;Username=postgres";
}

// Add PostgreSQL DbContext
builder.Services.AddDbContext<ForumDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add session support for simple authentication
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add authentication using cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.AccessDeniedPath = "/Login";
    });

// Add scoped services for database access
builder.Services.AddScoped<IUserService, DbUserService>();
builder.Services.AddScoped<IForumService, DbForumService>();
builder.Services.AddScoped<IPrivateMessageService, DbPrivateMessageService>();

var app = builder.Build();

// Configure forwarded headers for Fly.io proxy
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

// Apply migrations and create database
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ForumDbContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
        // Don't crash the app if migration fails - let it try to connect later
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Only redirect to HTTPS in production when not behind a proxy
if (!app.Environment.IsDevelopment() && !app.Configuration.GetValue<bool>("ASPNETCORE_FORWARDEDHEADERS_ENABLED"))
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
