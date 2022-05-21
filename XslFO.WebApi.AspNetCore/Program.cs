var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

//Pipe our configuration values into our Environment for simplified access...
foreach (var c in configuration.GetChildren().Where(c => !string.IsNullOrWhiteSpace(c.Key)))
    Environment.SetEnvironmentVariable(c.Key, c.Value ?? string.Empty);

// Add services to the container.

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
