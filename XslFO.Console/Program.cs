// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using PdfTemplating.XslFO.ApacheFOP.Serverless;

var directoryInfo = new DirectoryInfo(@$"{Directory.GetCurrentDirectory()}\FOSamples");
var fileInfo = directoryInfo.GetFiles("StarWarsMovies.fo").FirstOrDefault();
Console.WriteLine($"{Environment.NewLine}Loading XslFO Markup from File: {fileInfo?.FullName}");

var xslfoMarkup = await File.ReadAllTextAsync(fileInfo.FullName);
Console.WriteLine($"{Environment.NewLine}[{xslfoMarkup.Length}] Bytes loaded successfully...");

var pdfServiceClient = new ApacheFOPServerlessPdfRenderService(new ApacheFOPServerlessXslFORenderOptions(
    new Uri("https://apachefop-serverless.azurewebsites.net/api/apache-fop/xslfo"),
    "{{AZ_FUNC_TOKEN_GOES_HERE}}"
));

Console.WriteLine($"{Environment.NewLine}Rendering into PDF via ApacheFOP.Serverless at [{pdfServiceClient.Options.ApacheFOPServiceHost}]...");

var renderResponse = await pdfServiceClient.RenderPdfAsync(xslfoMarkup);
var pdfBytes = renderResponse.PdfBytes;
Console.WriteLine($"{Environment.NewLine}Received [{renderResponse.PdfBytes.Length}] rendered PDF bytes...");

Console.WriteLine($"{Environment.NewLine}RENDER EVENT LOG:");
foreach (var eventLog in renderResponse.EventLogEntries)
    Console.WriteLine($"    - {eventLog}");

var pdfFileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, $"{fileInfo.Name}.pdf"));
Console.WriteLine($"{Environment.NewLine}Saving rendered PDF file [{pdfFileInfo.Name}]...");
await File.WriteAllBytesAsync(pdfFileInfo.FullName, pdfBytes);

Console.WriteLine($"{Environment.NewLine}Launching PDF File: {pdfFileInfo.FullName}");
var process = new Process(){ StartInfo = new ProcessStartInfo(pdfFileInfo.FullName) { UseShellExecute = true } };
process.Start();



