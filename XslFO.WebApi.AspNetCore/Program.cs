using PdfTemplating.XslFO.Razor.AspNetCoreMvc;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

//Pipe our configuration values into our Environment for simplified access...
foreach (var c in configuration.GetChildren().Where(c => !string.IsNullOrWhiteSpace(c.Key)))
    Environment.SetEnvironmentVariable(c.Key, c.Value ?? string.Empty);

// Add services to the container.

builder.Services.AddControllersWithViews();

//OPTIONALLY you may Manually configure the base paths that will be searched for Views (when Relative Paths are provided...
//RazorPdfTemplatingConfig
//    .ClearViewSearchPaths() //Clear default search paths.
//    .AddViewSearchPath(builder.Environment.WebRootPath); //Add a known path to the search list after existing entries
//    .AddViewSearchPathAsTopPriority(builder.Environment.ContentRootPath) // Insert known path at the top of the Search List above existing entries

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
