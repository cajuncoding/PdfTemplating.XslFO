namespace Fonet.Cli
{
    using System;
    using System.IO;
    using Fonet.Cli.Parser;
    using Fonet.Render;
    using Fonet.Render.Pdf;

    /// <summary>
    /// Command line interface for FO.NET.
    /// </summary>
    public class FonetCli
    {
        /// <summary>
        /// Option to specify the input FO file.
        /// </summary>
        private Option fofOption;

        /// <summary>
        /// Option to specify the output PDF option.
        /// </summary>
        private Option pdfOption;

        /// <summary>
        /// Option to display command line help.
        /// </summary>
        private Option helpOption;

        /// <summary>
        /// Option to specify the font kerning for PDF output.
        /// </summary>
        private Option kerningOption;

        /// <summary>
        /// Option to specify how fonts should be handled for PDF output.
        /// </summary>
        private Option fontTypeOption;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">System provided command line arguments.</param>
        /// <returns>0 if successful, otherwise -1.</returns>
        [STAThread]
        public static int Main(string[] args)
        {
            return new FonetCli().Run(args);
        }

        /// <summary>
        /// Entry point into the command line interface.
        /// </summary>
        /// <param name="args">System provided command line arguments.</param>
        /// <returns>0 if successful, otherwise -1.</returns>
        internal int Run(string[] args)
        {
            try
            {
                long start = DateTime.Now.Ticks;

                CommandLineParser cmd = new CommandLineParser();
                this.fofOption = cmd.AddParameterOption("fo");
                this.pdfOption = cmd.AddOption("pdf");
                this.helpOption = cmd.AddOption("h");
                this.kerningOption = cmd.AddOption("kerning");
                this.fontTypeOption = cmd.AddParameterOption("fonttype");

                FonetDriver driver = FonetDriver.Make();
                driver.OnError += new FonetDriver.FonetEventHandler(this.FonetError);

                // May throw a CommandLineException if arguments are unparseable
                cmd.Parse(args);

                Stream inputStream = null;
                Stream outputStream = null;

                if (this.helpOption.IsProvided)
                {
                    this.PrintUsage();
                    return 0;
                }

                if (this.fofOption.IsProvided)
                {
                    // The filename argument given to the -fo option
                    string fofFilename = this.fofOption.Argument;

                    try
                    {
                        // Attmept to open the source file
                        inputStream = new FileStream(
                            fofFilename, FileMode.Open, FileAccess.Read);
                    }
                    catch (Exception e)
                    {
                        throw new FonetException(
                            String.Format("Unable to open file {0}", fofFilename), e);
                    }
                }
                else
                {
                    throw new FonetException("No input file specified");
                }

                driver.Options = this.GetPdfRendererOptions();

                string[] remainder = cmd.GetRemainder();
                if (remainder.Length > 0)
                {
                    string outputFile = remainder[0];
                    try
                    {
                        outputStream = new FileStream(outputFile, FileMode.Create);
                    }
                    catch (Exception e)
                    {
                        throw new FonetException(
                            String.Format("Unable to open file {0}", outputFile), e);
                    }
                }
                else
                {
                    // Default to standard output
                    outputStream = Console.OpenStandardOutput();
                }

                driver.Render(inputStream, outputStream);

                long end = DateTime.Now.Ticks;

                Console.WriteLine("Took {0} ms.", (end - start) / 10000);
            }
            catch (CommandLineException cle)
            {
                Console.WriteLine("Incorrect commandline arguments: " + cle.Message);
                this.PrintUsage();
                return -1;
            }
            catch (FonetException e)
            {
                Console.WriteLine("[ERROR] FO.NET failed to render your document: {0}", e.Message);
                return -1;
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Returns the PDF renderer options configured according to command line options.
        /// </summary>
        /// <returns>PDF render options configured according to command line options.</returns>
        private PdfRendererOptions GetPdfRendererOptions()
        {
            PdfRendererOptions options = new PdfRendererOptions();

            // Enable/disable evil kerning
            options.Kerning = this.kerningOption.IsProvided;

            // Enum.Parse() will throw an exception if the fontTypeOption argument 
            // does not represent one of the pre-defined FontType members
            if (this.fontTypeOption.IsProvided)
            {
                try
                {
                    options.FontType = (FontType)Enum.Parse(
                        typeof(FontType),
                        this.fontTypeOption.Argument,
                        true);
                }
                catch (Exception)
                {
                    Console.WriteLine(
                        "[WARN] Unrecognised -fonttype argument: '{0}' - defaulting to 'Link'",
                        this.fontTypeOption.Argument);
                }
            }

            return options;
        }

        /// <summary>
        ///     Receives any errors thrown by FO.NET.
        /// </summary>
        /// <param name="driver">A reference to a driver.</param>
        /// <param name="e">The event arguments.</param>
        private void FonetError(object driver, FonetEventArgs e)
        {
            Console.WriteLine("[ERROR] {0}", e.GetMessage());
        }

        /// <summary>
        /// Outputs command line interface help information.
        /// </summary>
        private void PrintUsage()
        {
            Console.WriteLine("Usage: fonet [-options]");
            Console.WriteLine(String.Empty);
            Console.WriteLine("Where -options are:");
            Console.WriteLine("    -kerning  Enable kerning");
            Console.WriteLine("    -fonttype <TrueType|Embed|Subset>");
            Console.WriteLine("              Specifies how to handle TrueType fonts:");
            Console.WriteLine("              Link   - fonts are linked");
            Console.WriteLine("              Embed  - fonts are embedded in PDF");
            Console.WriteLine("              Subset - fonts are subsetted and embedded in PDF");
            Console.WriteLine("    -fo       <filename>");
            Console.WriteLine("              Path to an XSL-FO file");
            Console.WriteLine("    -pdf      <filename>");
            Console.WriteLine(String.Empty);
            Console.WriteLine("Example:");
            Console.WriteLine(String.Empty);
            Console.WriteLine("fonet -fo manual.fo -pdf manual.pdf");
            Console.WriteLine(String.Empty);
            Console.WriteLine("    Transforms the XSL-FO document 'manual.fo' into the PDF");
            Console.WriteLine("    document 'manual.pdf'");
        }
    }
}