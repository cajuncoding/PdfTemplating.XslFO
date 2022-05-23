# PdfTemplating.XslFO
This is a C#.Net solution that provides the capability to easily generate Pdf files using a templated approach that provides great separation between presentation
(the template) and data (the Model); as opposed to extremely complex code based approaches most libraries provide.
It's based on the Xsl-FO standard and currently is dependent on an old but still very functional libray "Fonet" -- which is port of Apache FOP to C#.

In addition, this is a completely open-source and free solution to use (even commercially).  Many of the complex (powerful maybe, but horribly difficult to develop and maintain)
API libraries out there require licenses and are no longer possible solutions when simply due to steep licensing costs.

**Apache FOP support is now in Progress:** It's still pending documentation and readme updates, but I've got a fully functioning serverless implementation of Apache FOP (up-to-date Xsl-FO rendering engine) in it's own repo!  It's a simple Java app enabling a serverless microservice for Apache FOP via an Azure Function! Super light-weight, really slick, and easy to deploy . . . providing the flexibility of the latest (e.g. v2.5) Xsl-FO processing from Apache FOP!

* <a href="https://github.com/cajuncoding/ApacheFOP.Serverless">ApacheFOP.Serverless Repo is here!</a>
* <a href="https://github.com/cajuncoding/PdfTemplating.XslFO/tree/feature/iniial_support_for_apache_fop_serverless_rendering">PdfTemplating.XslFO Branch Compatible with Serverless Apache FOP is here!</a>

**I hope this helps anyone looking to dynamically generate PDF files in C# or .Net with a templating approach that is far more maintainable than other code based generation/manipulation approaches . . .**

### Project Overview:
This project illustrates the capabilities of using templating based approaches to render Xsl-FO for dynamically generating PDF documents. 
This project illustrates the use and support of two of the most common/well-known templating engines for .Net web development -- Razor Templating 
& XSLT Templating. 

The Razor & Xslt template based approaches to rendering PDF files gives you the benefits of separating the presentation from the Data model, allows 
different team members to work at the same time because the Template can be developed offline with sample Model data that can be easily loaded, and 
the code can be made very manageable with the use of Xslt include files, variables, etc. to divide your code into modular "DRY" components for re-use 
across multiple reports, etc.

In addition, the Razor & Xslt engines are extensible and can support virtually unlimited capabilities with C# based extension functions. For Razor 
templates the world of .Net is immediately available (e.g. Linq).  And, for XSLT custom extension functions can be defined in the assembly or inlinein 
the Xslt, and this project has many custom extensions already included to augment the Xslt v1.0 engine that .Net provides.

Finally, this project also provides basic a Windows Client (WinForms) application that provides a UI that can be used for developing when using the 
Xslt templating engine.

NOTE: Currently the Razor Implementation requires Microsoft.AspNet.MVC and does not yet support .Net Core. I plan to extend this as soon as I have
a real need to render Pdf file from a .Net Core web application; or a console app for that matter 
(useful info. [https://stackoverflow.com/questions/38247080/using-razor-outside-of-mvc-in-net-core](here on StackOverflow).)

#### Example Usages from the Demo MVC Project:

##### Xslt (.Net Standard 2.0):
```csharp
    //Use XSLT Template + FONet PDF Rendering Engine...
    public virtual byte[] RenderPdf(MovieSearchResponse templateModel)
    {
        //***********************************************************
        //Execute the XSLT Transform to generate the XSL-FO output
        //***********************************************************
        //Render the XSL-FO output from the Razor Template and the View Model
        var xslFODoc = this.RenderXslFOXml(templateModel);

        //Create the Pdf Options for the XSL-FO Rendering engine to use
        var pdfOptions = this.CreatePdfOptions();

        //****************************************************************************
        //Execute the Trasnformation of the XSL-FO source to Binary Pdf via Fonet
        //****************************************************************************
        var xslFOPdfRenderer = new FONetXslFOPdfRenderer(xslFODoc, pdfOptions);
        var pdfBytes = xslFOPdfRenderer.RenderPdfBytes();
        return pdfBytes;
    }
```

##### Razor View + FONet (.Net Framework and requires Microsoft.AspNet.Mvc):
```csharp
    //Use Razor Template + FONet PDF Rendering Engine...
    public virtual byte[] RenderPdf(MovieSearchResponse templateModel)
    {
        //***********************************************************
        //Execute the Razor View to generate the XSL-FO output
        //***********************************************************
        var razorViewRenderer = new AspNetMvcRazorViewRenderer(this.ControllerContext);
        var renderResult = razorViewRenderer.RenderView(this.RazorViewVirtualPath, templateModel);

        //Load the XSL-FO output into a fully validated XDocument.
        //NOTE: This template must generate valid Xsl-FO output -- via the well-formed xml we load into the XDocument return value -- to be rendered as a Pdf Binary!
        var xslFODoc = XDocument.Parse(renderResult.RenderOutput);

        //Create the Pdf Options for the XSL-FO Rendering engine to use
        var pdfOptions = this.CreatePdfOptions();

        //****************************************************************************
        //Execute the Transformation of the XSL-FO source to Binary Pdf via Fonet
        //****************************************************************************
        var xslFOPdfRenderer = new FONetXslFOPdfRenderer(xslFODoc, pdfOptions);
        var pdfBytes = xslFOPdfRenderer.RenderPdfBytes();
        return pdfBytes;
    }
```

##### Razor View + ApacheFOP.Serverless (PDF-as-a-service via Azure Functions):
```csharp
    //Use Razor Template + ApacheFOP.Serverless Rendering Engine; PDF-as-a-service via Azure Functions...
    public virtual async Task<byte[]> RenderPdfAsync(MovieSearchResponse templateModel)
    {
        //Ensure that compatibility Mode is Disabled for proper rendering of our Model
        templateModel.FonetCompatibilityEnabled = false;

        //***********************************************************
        //Execute the Razor View to generate the XSL-FO output
        //***********************************************************
        var razorViewRenderer = new AspNetMvcRazorViewRenderer(this.ControllerContext);
        var renderResult = razorViewRenderer.RenderView(this.RazorViewVirtualPath, templateModel);

        //Load the XSL-FO output into a fully validated XDocument.
        //NOTE: This template must generate valid Xsl-FO output -- via the well-formed xml we load into the XDocument return value -- to be rendered as a Pdf Binary!
        var xslFODoc = XDocument.Parse(renderResult.RenderOutput);

        //******************************************************************************************
        //Execute the Transformation of the XSL-FO source to Binary Pdf via Apache FOP Service...
        //******************************************************************************************
        var pdfBytes = await ApacheFOPServiceHelper.RenderXslFOToPdfAsync(xslFODoc);
        return pdfBytes;
    }
```

#### Notes about the Xsl-FO Implementation(s):
This project is now implements support for two PDF rendering engines:
1. **FO.NET (FONet)**:  in-memory C# managed implementation based on a port of an older version of ApacheFOP (some version less than 1.0).
   - The original open source project from CodePlex located here: [https://archive.codeplex.com/?p=fonet](https://archive.codeplex.com/?p=fonet)
   - There also appears to be another version hosted here, but I have not validated the state of this code:
[https://github.com/hahmed/Fo.Net](https://github.com/hahmed/Fo.Net)
2. **ApacheFOP.Serverless**: The latest-and-greatest ApacheFOP via Serverless deployment in AzureFunctions 
   - A Ready to deploy PDF-as-a-service project named [ApacheFOP.Serverless can be found here.](https://github.com/cajuncoding/ApacheFOP.Serverless) 

**NOTES:**
* Because the original FO.Net hasn't been updated in so long, I wanted to make sure that this solution contained a stable version. 
Therefore, I've cloned the working stable version that I've used in several projects and included here in this project.  

* I've also updated the included version to target and compile as a .Net Standard 2.0 project for greater compatibility. 
Though some feedback from the community is that FO.Net may have underlying dependencies on Windows specific Dlls and therefore may
not function as expected in a Linux environment.**

* **This is why I've shifted to using ApacheFOP.Serverless as the recommended solution for is ongoing support by Apache, latest-and-greatest
XSL-FO spec. support, serverless deployment support via Azure Functions, etc... it's just a more robust and supported solution therefore
it is my go-to architecture going forward.**


#### TODO: 
* Once I have a need to generate Pdf files with .Net Core I will create an implementation demo for that also.

## Testing Projects
#### XslFO.ViewerApplication project
Provides a Windows client/desktop application that can be used as a viewer to provide real-time previews while creating/developing your Xslt and/or Xsl-FO.
1. You use any text editor you like (e.g. Notepad++) to edit your Xslt.
2. Then you can point the ViewerApplication to any of the following to view preview the results:
	* Xml data + Xslt transform/report
	* Xsl-FO xml data (pre-rendered)
	* Binary Pdf File (any existing file to test the Viewer with)
	* **NOTE: The Windows Client viewer MUST be run in 32-bit mode for the Acrobat ActiveX controls to work correctly (e.g. it must be built in x86 mode).**
3. This application is completely generic and doesn't come with any pre-bundled Xslt Reports, but you can leverage the sample Xml data and MovieReport.xslt 
that are located in the Web MVC project (see below).
    * Samples of the pre-generated FO Xml -- generated by the MVC project -- are saved in the `Samples` folder of the XslFO.ViewerApplication:
      * Star Wars Movie Report.fo
      * The Matrix Movie Report.fo

#### XslFO.WebMvc.AspNet project
Provides a Web based client application based on ASP.Net MVC that can be used to dynamically render and stream the Pdf to the browser.  It provides an example of how elegant it can be to manage Xslt or Razor based (templated approach) reports in a web application and dynamically render Pdf's to client requests.
1. The sample report uses the OMDB Api to get results based on Movie Title Search
	* [http://www.omdbapi.com](http://www.omdbapi.com)
3. The default route for the application is **/movies/pdf** and will result in the results being rendered by the 
**Razor Templating engine + FONet (in-memory)** with a default Search using **"Star Wars"**
    * **The XSLT Templating engine is also supported (see below for explicitly testing with that engine)** 
	* In addition, you can specify any dynamic search you want by using the "title" parameter: **/pdf?title={movie title here}**, some samples are:
		1. /movies/pdf?title=star%20trek
        2. /movies/pdf?title=the%20matrix
		2. /movies/pdf?title=braveheart
		3. /movies/pdf?title=finding%20nemo
3. The Razor Mediator reports that render the FO (Formatting Objects Xml markup) -- that is converted into Pdf format -- are 
located in the MVC Project at:
	* **XslFO.TestSolution/XslFO.WebMvc.AspNet/Reports.Razor/...**
	* You can always explicity run the Razor Templating engine by using the explicit route:
	    1. /movies/pdf/razor?title=star%20wars
4. The XSLT reports that render the FO  (Formatting Objects Xml markup) -- that is converted into Pdf format -- are 
located in the MVC Project at:
	* **XslFO.TestSolution/XslFO.WebMvc.AspNet/Reports.XslFO/...**
	* You can always explicity run the XSLT Templating engine by using the explicit route:
	    1. /movies/pdf/xslt?title=star%20wars
5. The **ApacheFOP.Serverless Rendering engine** implementation in the Demo project utilizes either Xslt or Razore templates by appending
**/apache-fop** to the above paths as follows:
    * Note: The XSL-FO markup will be generated with the specified template (per the path route), and then the
request to render the PDF will be sent to the configured serverless deployment of [ApacheFOP.Serverless](https://github.com/cajuncoding/ApacheFOP.Serverless) 
to render the binary PDF file.
    * The reports that render the FO (Formatting Objects Xml markup) -- that is converted into Pdf format -- are located as noted above.
located in the MVC Project at:
	* You can explicity run the ApacheFOP Renderings by using the explicit routes:
	  1. **XSLT + ApacheFOP.Serverless:** /movies/pdf/xslt/apache-fop?title=star%20wars
	  2. **Razor + ApacheFOP.Serverless:** /movies/pdf/razor/apache-fop?title=star%20wars

```
/*
Copyright 2012 Brandon Bernard

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

	 http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
```



