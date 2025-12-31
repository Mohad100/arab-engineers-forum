using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Fourm.Services;
using Fourm.Data;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container
    builder.Services.AddRazorPages();

    // Get connection string from environment variable (Render/Fly.io) or appsettings
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    string? connectionString = null;
    
    // Render provides DATABASE_URL in postgres:// format, need to convert
    if (!string.IsNullOrWhiteSpace(databaseUrl))
    {
        try
        {
            // Check if it's in postgres:// format (Render style)
            if (databaseUrl.StartsWith("postgres://") || databaseUrl.StartsWith("postgresql://"))
            {
                // Parse postgres://user:password@host:port/database
                var uri = new Uri(databaseUrl.Replace("postgres://", "postgresql://"));
                
                var userInfo = uri.UserInfo.Split(':');
                var username = userInfo.Length > 0 ? userInfo[0] : "";
                var password = userInfo.Length > 1 ? userInfo[1] : "";
                
                // Use default PostgreSQL port 5432 if not specified
                var port = uri.Port > 0 ? uri.Port : 5432;
                var database = uri.AbsolutePath.TrimStart('/');
                
                connectionString = $"Host={uri.Host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
            }
            else
            {
                // Already in correct format
                connectionString = databaseUrl;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR parsing DATABASE_URL: {ex.Message}");
            connectionString = null;
        }
    }
    
    // Fallback to appsettings or default if connectionString is still null/empty
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=localhost;Port=5433;Database=ForumDb;Username=postgres";
        }
    }

    // Final validation
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("No valid database connection string found!");
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
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Configure forwarded headers for Fly.io proxy
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

// Apply migrations and create database
if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DATABASE_URL")))
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ForumDbContext>();
            dbContext.Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WARNING: Migration failed - {ex.Message}");
            // Continue anyway - database might not be ready yet
        }
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // Don't use HSTS or HTTPS redirect - Render handles SSL termination
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"FATAL ERROR: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
    }
    throw;
}
