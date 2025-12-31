using Microsoft.EntityFrameworkCore;
using Fourm.Data;

var connectionString = "Host=localhost;Port=5433;Database=ForumDb;Username=postgres;Include Error Detail=true";

var optionsBuilder = new DbContextOptionsBuilder<ForumDbContext>();
optionsBuilder.UseNpgsql(connectionString);

using var context = new ForumDbContext(optionsBuilder.Options);

Console.WriteLine("Clearing database...");

// Delete all records
var repliesDeleted = await context.Database.ExecuteSqlRawAsync("DELETE FROM \"Replies\"");
Console.WriteLine($"Deleted {repliesDeleted} replies");

var messagesDeleted = await context.Database.ExecuteSqlRawAsync("DELETE FROM \"PrivateMessages\"");
Console.WriteLine($"Deleted {messagesDeleted} private messages");

var threadsDeleted = await context.Database.ExecuteSqlRawAsync("DELETE FROM \"Threads\"");
Console.WriteLine($"Deleted {threadsDeleted} threads");

var usersDeleted = await context.Database.ExecuteSqlRawAsync("DELETE FROM \"Users\"");
Console.WriteLine($"Deleted {usersDeleted} users");

Console.WriteLine("\nDatabase cleared successfully!");
