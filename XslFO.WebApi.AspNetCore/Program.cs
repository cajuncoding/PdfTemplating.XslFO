using System.CustomExtensions;
using PdfTemplating.AspNetCoreMvc.Reports.PdfRenderers;
using PdfTemplating.XslFO;
using PdfTemplating.XslFO.ApacheFOP.Serverless;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

//Pipe our configuration values into our Environment for simplified access...
foreach (var c in configuration.GetChildren().Where(c => !string.IsNullOrWhiteSpace(c.Key)))
    Environment.SetEnvironmentVariable(c.Key, c.Value ?? string.Empty);

// Add services to the container.

builder.Services.AddControllersWithViews();

//************************************************************************************************
//Configure ApacheFOP.Serverless Client via DI
//************************************************************************************************
//NOTE: Configuration values are piped into the Environment at application startup!
var apacheFOPServiceHostString = Environment.GetEnvironmentVariable("XslFO.ApacheFOP.Serverless.Host")
    .AssertArgumentIsNotNullOrBlank("XslFO.ApacheFOP.Serverless.Host", "Configuration value for ApacheFOP Service Host is missing or undefined.");

//Construct the REST request options and append the Security Token (as QuerystringParam):
builder.Services.AddSingleton(s => new ApacheFOPServerlessXslFORenderOptions(new Uri(apacheFOPServiceHostString))
{
    EnableGzipCompressionForRequests = Environment.GetEnvironmentVariable("XslFO.ApacheFOP.Serverless.GzipRequestsEnabled").EqualsIgnoreCase(bool.TrueString),
    EnableGzipCompressionForResponses = Environment.GetEnvironmentVariable("XslFO.ApacheFOP.Serverless.GzipResponsesEnabled").EqualsIgnoreCase(bool.TrueString)
});

builder.Services.AddSingleton<IAsyncXslFOPdfRenderer, ApacheFOPServerlessPdfRenderService>();

//Finally add our own Helper Client that encapsulates working with the ApacheFOP Service for our App (always a good idea to encapsulate):
builder.Services.AddSingleton<ApacheFOPServerlessHelperClient>();

//OPTIONALLY you may Manually configure the base paths that will be searched for Views (when Relative Paths are provided...
//RazorPdfTemplatingConfig
//    .ClearViewSearchPaths() //Clear default search paths.
//    .AddViewSearchPath(builder.Environment.WebRootPath); //Add a known path to the search list after existing entries
//    .AddViewSearchPathAsTopPriority(builder.Environment.ContentRootPath) // Insert known path at the top of the Search List above existing entries

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Run();
