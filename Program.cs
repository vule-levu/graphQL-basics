using GraphQL;
using GraphQL.Models;
using HotChocolate.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Configure the database connection
builder.Services.AddDbContext<ApplicationDbContext>
    (options =>
        options.UseInMemoryDatabase("InMemoryDb"));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services
    .AddOpenApi()
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true);

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // seeding data for testing directly
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Check if the data is already seeded
    if (!context.Users.Any())
    {
        context.Users.RemoveRange(context.Users);
        context.Posts.RemoveRange(context.Posts);
        context.SaveChanges();

        var user1 = new User
        {
            Name = "John Smith",
            Age = 50
        };
        var user2 = new User
        {
            Name = "John Beans",
            Age = 55
        };

        context.Users.Add(user1);
        context.Users.Add(user2);

        context.Posts.Add(
            new Post
            {
                Title = "GraphQL Basics",
                Content = "Let's start learning GraphQL!",
                UserId = user1.Id
            }
            );
        context.Posts.Add(
            new Post
            {
                Title = "Revisiting Mendel's experiments with peas",
                Content = "Life history is fascinating.",
                UserId = user2.Id
            }
            );
        context.Posts.Add(
            new Post
            {
                Title = "NoSQL from the ground up",
                Content = "Replicas all fail and latency increase.",
                UserId = user1.Id
            }
            );
        context.Posts.Add(
           new Post
           {
               Title = "Mung Beans and Sweet potatoes",
               Content = "Let's cook the best Vietnamese food.",
               UserId = user2.Id
           }
           );

        context.SaveChanges();
    }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapGraphQL();
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
