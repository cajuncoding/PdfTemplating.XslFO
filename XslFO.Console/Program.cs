// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using PdfTemplating.XslFO.Render.ApacheFOP.Serverless;

var directoryInfo = new DirectoryInfo(@$"{Directory.GetCurrentDirectory()}\FOSamples");
var fileInfo = directoryInfo.GetFiles("StarWarsMovies.fo").FirstOrDefault();
Console.WriteLine($"{Environment.NewLine}Loading XslFO Markup from File: {fileInfo.FullName}");

var xslfoMarkup = await File.ReadAllTextAsync(fileInfo.FullName);
Console.WriteLine($"{Environment.NewLine}[{xslfoMarkup.Length}] Bytes loaded successfully...");

var pdfServiceClient = new ApacheFOPServerlessClient(
    new Uri("https://apachefop-serverless.azurewebsites.net/api/apache-fop/xslfo"),
    "AOwzwaHl8eiHMywNdIb5DC7oy8BJmfpl2X6oPYDxWUpIM9AiJHsh6g=="
);

Console.WriteLine($"{Environment.NewLine}Rendering into PDF via ApacheFOP.Serverless at [{pdfServiceClient.ApacheFOPServerlessUri}]...");

var pdfBytes = await pdfServiceClient.RenderPdfAsync(xslfoMarkup);
Console.WriteLine($"{Environment.NewLine}Received [{pdfBytes.Length}] rendered PDF bytes...");

var pdfFileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, $"{fileInfo.Name}.pdf"));
Console.WriteLine($"{Environment.NewLine}Saving rendered PDF file [{pdfFileInfo.Name}]...");
await File.WriteAllBytesAsync(pdfFileInfo.FullName, pdfBytes);

Console.WriteLine($"{Environment.NewLine}Launching PDF File: {pdfFileInfo.FullName}");
var process = new Process(){ StartInfo = new ProcessStartInfo(pdfFileInfo.FullName) { UseShellExecute = true } };
process.Start();



