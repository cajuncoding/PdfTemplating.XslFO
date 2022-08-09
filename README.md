# PdfTemplating.XslFO
This is a C#.Net solution that provides the capability to easily generate Pdf files using a templated approach that provides great separation between presentation
(the template) and data (the Model); as opposed to extremely complex code based approaches most libraries provide.
It's based on the Xsl-FO standard and now with ApacheFOP.Serverless provides a PDF-as-a-service architecture for generating PDF reports without unnecessary complexity
and long term technical debt of legacy approaches (e.g. reporting engines).

In addition, this is a completely open-source and free solution to use (even commercially).  Many of the complex (powerful maybe, but horribly difficult to develop and maintain)
API libraries out there require licenses that make them no longer feasible solutions simply due to steep licensing costs for many projects.

`ApacheFOP.Serverless` is a fully functioning serverless implementation of Apache FOP (up-to-date Xsl-FO rendering engine).  
It's a simple Java based Azure Function enabling a serverless microservice for Apache FOP via an Azure Function! Super light-weight, really slick, and easy to deploy . . . providing the flexibility of the latest (e.g. v2.5) Xsl-FO processing from Apache FOP!
* <a href="https://github.com/cajuncoding/ApacheFOP.Serverless">ApacheFOP.Serverless Repo is here!</a>

#### May 2022 Updates:
  * All Example projects have been updated with support for PDF-as-a-service as the strongly recommended approach; vs in-memory processing with the legacy/deprecated Fonet library (which has known issues in non-windows environments).
  * New ASP.NET Core MVC implementattion and Nuget libraries have been added.

**I hope this helps anyone looking to dynamically generate PDF files in C# or .Net with a templating approach that is far more maintainable than other code based generation/manipulation approaches . . .**

**If you like this project and/or use it the please give me a Star (c'mon it's free, and it'll make my day)!**

### [Buy me a Coffee ☕](https://www.buymeacoffee.com/cajuncoding)
*I'm happy to share with the community, but if you find this useful (e.g for professional use), and are so inclinded,
then I do love-me-some-coffee!*

<a href="https://www.buymeacoffee.com/cajuncoding" target="_blank">
<img src="https://cdn.buymeacoffee.com/buttons/default-orange.png" alt="Buy Me A Coffee" height="41" width="174">
</a> 


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

### Example Usages from the Demo MVC Project:

#### ASP.NET Framework:
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
        //NOTE: This template must generate valid Xsl-FO output to be rendered as a Pdf Binary! 
        //      This is optional, but parsing the output into XML will validate that it is well-formed!
        var xslFODoc = XDocument.Parse(renderResult.RenderOutput);

        //******************************************************************************************
        //Execute the Transformation of the XSL-FO source to Binary Pdf via Apache FOP Service...
        //******************************************************************************************
        var pdfBytes = await ApacheFOPServiceHelper.RenderXslFOToPdfAsync(xslFODoc);
        return pdfBytes;
    }
```
#### NEW ASP.NET CORE Support (as of May 2022)
NOTES: 
 1. The AspNetCore implementation nearly identical to the legacy .NET Framework example above, but must now be Async and
          uses the new namespace `using PdfTemplating.XslFO.Razor.AspNetCoreMvc;`
 2. Fonet in-memory implementation is no longer provided due to known issues in non-windows environments, so we now
     strongly encourage the PDF-as-a-service (de-coupled) implementation approach, such as ApacheFOP.Serverless.

##### Xslt (.Net Standard 2.0):
```csharp
    //Use XSLT Template + FONet PDF Rendering Engine...
    public virtual async Task<byte[]> RenderPdfAsync(MovieSearchResponse templateModel)
    {
        //NOTE: This is only needed becasue we share the same Report Template between legacy Fonet and new
        //  ApacheFOP.Serverless rendering; and there are some markup compatibility logic we want to disable.
        //Ensure that compatibility Mode is Disabled for proper rendering of our Model
        templateModel.FonetCompatibilityEnabled = false;

        //***********************************************************
        //Execute the XSLT Transform to generate the XSL-FO output
        //***********************************************************
        //Render the XSL-FO output from the Razor Template and the View Model
        var xslFODoc = this.RenderXslFOContent(templateModel);

        //******************************************************************************************
        //Execute the Transformation of the XSL-FO source to Binary Pdf via Apache FOP Service...
        //******************************************************************************************
        var pdfBytes = await _apacheFopHelperClient.RenderXslFOToPdfAsync(xslFODoc).ConfigureAwait(false);
        return pdfBytes;
    }
```

##### Razor View + ApacheFOP.Serverless (PDF-as-a-service via Azure Functions):
```csharp
    //Use ASP.NET Core Razor Template + ApacheFOP.Serverless Rendering Engine; PDF-as-a-service via Azure Functions...
    public virtual async Task<byte[]> RenderPdfAsync(MovieSearchResponse templateModel)
    {
        //Ensure that compatibility Mode is Disabled for proper rendering of our Model
        templateModel.FonetCompatibilityEnabled = false;

        //***********************************************************
        //Execute the Razor View to generate the XSL-FO output
        //***********************************************************
        var razorViewRenderer = new MvcRazorViewRenderer(this.MvcController);
        var renderResult = await razorViewRenderer.RenderViewAsync(this.RazorViewPath, templateModel).ConfigureAwait(false);

        //***********************************************************
        //OPTIONALLY validate the Output by Loading the XSL-FO output into a fully validated XDocument...
        //***********************************************************
        //Load the XSL-FO output into a fully validated XDocument.
        //NOTE: This template must generate valid Xsl-FO output to be rendered as a Pdf Binary! 
        //      This is optional, but parsing the output into XML will validate that it is well-formed!
        var xslFODoc = XDocument.Parse(renderResult.RenderOutput);

        //******************************************************************************************
        //Execute the Transformation of the XSL-FO source to Binary Pdf via Apache FOP Service...
        //******************************************************************************************
        //var pdfBytes = await ApacheFOPServerlessHelper.RenderXslFOToPdfAsync(xslFODoc).ConfigureAwait(false);
        var pdfBytes = await _apacheFopHelperClient.RenderXslFOToPdfAsync(renderResult.RenderOutput).ConfigureAwait(false);
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
** BUT this version is not supported and there will be no enhancements, bug-fixees, etc. going forward.!** *We strongly encourage the use of the 
new PDF-as-a-service (de-coupled) use of ApacheFOP.Serverless implementation approach.*

* I've also updated the included version to target and compile as a .Net Standard 2.0 project for greater compatibility. 
Though some feedback from the community is that FO.Net may have underlying dependencies on Windows specific Dlls and therefore may
not function as expected in a Linux environment.**

* **This is why I've shifted to using ApacheFOP.Serverless as the recommended solution for is ongoing support by Apache, latest-and-greatest
XSL-FO spec. support, serverless deployment support via Azure Functions, etc... it's just a more robust and supported solution therefore
it is my go-to architecture going forward.**


## Example Projects
### XslFO.Console
A very simple console application that illustrates the value of having the rendering of the PDF de-coupled as it's own service (e.g. ApacheFOP.Serverless).
This allows even a simple Console App to read a pre-formatted XslFO document and render to a PDF and open the file locally.

While this app currently uses a pre-formatted report, it is absolutely possible for even a console app to render templates either XSLT or Razor based
to create the reports, but this is out of the scope of this sample project since most use-cases will be web based and therefore the following
.NET Framework and .NET Core web applications are of greater value.  But this Console example may help illustrate how easy it is to bridge the gap
and show that there are no boundaries -- especially when using the PDF-as-a-service approach with ApacheFOP.Serverless.

### XslFO.WebMvc.AspNet & XslFO.WebApi.AspNetCore projects
Provides Web based client applications based on Asp.NET MVC & Asp.NET Core MVC that can be used to dynamically render and stream the Pdf to the 
browser.  It provides an example of how elegant it can be to manage Xslt or Razor based (templating approaches) reports in a web application and 
dynamically render PDF's for client requests.
1. The sample report uses the OMDB Api to get results based on Movie Title Search
	* [http://www.omdbapi.com](http://www.omdbapi.com)

2. The default route for the application is **/movies/pdf** and will result in the results being rendered by the 
**Razor Templating engine + FONet (in-memory)** with a default Search using **"Star Wars"**
    * **The XSLT Templating engine is also supported (see below for explicitly testing with that engine)** 
	* In addition, you can specify any dynamic search you want by using the "title" parameter: **/pdf?title={movie title here}**, some samples are:
		1. `/movies/pdf?title=star%20trek`
        2. `/movies/pdf?title=the%20matrix`
		2. `/movies/pdf?title=braveheart`
		3. `/movies/pdf?title=finding%20nemo`
    
    * For ASP.NET Core project you must use the ApacheFOP.Serverless routes as follows:
    * **The above Fonet default in-memory processing does NOT Apply to the sample ASP.NET CORE project, due to known issues in non-windows environments! 
      So ApacheFOP.Serverless de-coupled rending is strongly encouraged and provided as the only implementation example.***
		1. `/movies/pdf/razor/apache-fop?title=star%20trek`
        2. `/movies/pdf/razor/apache-fop?title=the%20matrix`
		2. `/movies/pdf/razor/apache-fop?title=braveheart`
		3. `/movies/pdf/razor/apache-fop?title=finding%20nemo`
    
    * Any of the can also be renderered by replacing `razor` with `xstl` in the route to engage the Xslt rendering engine instead:
		1. `/movies/pdf/xslt/apache-fop?title=star%20trek`
        2. `/movies/pdf/xslt/apache-fop?title=the%20matrix`
		2. `/movies/pdf/xslt/apache-fop?title=braveheart`
		3. `/movies/pdf/xslt/apache-fop?title=finding%20nemo`

3. The Razor Mediator reports that render the FO (Formatting Objects Xml markup) -- that is converted into Pdf format -- are 
located in the MVC Project at:
	* XslFO.TestSolution/XslFO.WebMvc.AspNet/**Reports.Razor**/... or .../XslFO.WebApi.AspNetCore/**Reports.Razor**/...
	* You can always explicity run the Razor Templating engine by using the explicit route:
	    1. `/movies/pdf/razor?title=star%20wars`
4. The XSLT reports that render the FO  (Formatting Objects Xml markup) -- that is converted into Pdf format -- are 
located in the MVC Project at:
	* **XslFO.TestSolution/XslFO.WebMvc.AspNet/Reports.XslFO/...**
	* You can always explicity run the XSLT Templating engine by using the explicit route:
	    1. `/movies/pdf/xslt?title=star%20wars`
5. The **ApacheFOP.Serverless Rendering engine** implementation in the Demo project utilizes either Xslt or Razor templates by appending
**/apache-fop** to the above paths as follows:
    * Note: The XSL-FO markup will be generated with the specified template (per the path route), and then the
request to render the PDF will be sent to the configured serverless deployment of [ApacheFOP.Serverless](https://github.com/cajuncoding/ApacheFOP.Serverless) 
to render the binary PDF file.
        * The ApacheFOP.Serverless client configuration is in either `web.config` or `appsettings.json` depending on the project*
    * This uses the exact same reports to render the FO (Formatting Objects Xml markup) -- that is converted into Pdf format -- as noted above.
	* You can explicity run the ApacheFOP Renderings by using the explicit routes:
	  1. **XSLT + ApacheFOP.Serverless:** `/movies/pdf/xslt/apache-fop?title=star%20wars`
	  2. **Razor + ApacheFOP.Serverless:** `/movies/pdf/razor/apache-fop?title=star%20wars`


#### XslFO.ViewerApplication project
*NOTE: Not Yet Updated to work with ApacheFOP.Serverless, but works well with legacy Fonet in-memory rendering*

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



