# XslFO.TestSolution
This is a C#.Net solution that tests and illustrates the capabilities of Xsl-FO for dynamically generating PDF documents.  It also provides basic UI's that can be used for developing the Xslt and Xsl-FO markup.

The Xslt template based approach to rendering PDF's gives you the benefits of separating the presentation from the Data model, allows different team members to work at the same time because the Template can be developed offline with sample Xml data, and the code can be made very manageable with the use of Xslt include files, variables, etc. to divide your code into modular "DRY" components for re-use across multiple reports, etc.

In addition, the Xslt engine is extensible and can support virtually unlimited capabilities with C# based extension functions; which can be defined in the assembly or inlinein the Xslt.  This project has many custom extensions already included to augment the Xslt v1.0 engine that .Net provides.

#### Xsl-FO Implementation(s):
This project is based on FO.NET, an open source project from CodePlex located here:
[https://archive.codeplex.com/?p=fonet](https://archive.codeplex.com/?p=fonet)

Currently the full source code from FO.NET is contained within this project; because the original hasn't been updated in so long, I wanted to make sure that this solution contained a stable version.

There also appears to be another version hosted here, but I have not validated the state of this code:
[https://github.com/hahmed/Fo.Net](https://github.com/hahmed/Fo.Net)

#### Caveats:
Unfortunately the FONET library that this currently uses doesn't support all features that Xsl-FO can provide, and it's not as up to date as the Apache FOP project that it ported over from.  But, in my experience it's still very capable and has worked very well for my projects for many many years.

## Projects
#### XslFO.ViewerApplication project
Provides a Windows client/desktop application that can be used as a viewer to provide real-time previews while creating/developing your Xslt and/or Xsl-FO.
1. You use any text editor you like (e.g. Notepad++) to edit your Xslt.
2. Then you can point the ViewerApplication to any of the following to view preview the results:
	* Xml data + Xslt transform/report
	* Xsl-FO xml data (pre-rendered)
	* Binary Pdf File (any existing file to test the Viewer with)
3. This application is completely generic and doesn't come with any pre-bundled Xslt Reports, but you can leverage the sample Xml data and MoviewReport that are located in the Web MVC project (see below).

#### XslFO.WebMVC project
Provides a Web based client application based on ASP.Net MVC that can be used to dynamically render and stream the Pdf to the browser.  It provides an example of how elegant it can be to manage Xslt based (templated approach) reports in a web application and dynamically render Pdf's to client requests.
1. The sample Xslt report uses the OMDB Api to get results based on Movie Title Search
	* [http://www.omdbapi.com](http://www.omdbapi.com)
3. The default route for the application is **/pdf** and will result in a default Search using **"Star Wars"**
	* But you can specify any dynamic search you want by using the bytitle route at: **/pdf/bytitle/{movie title here}**, some samples are:
		1. /pdf/bytitle/star%20trek 
		2. /pdf/bytitle/braveheart
		2. /pdf/bytitle/finding%20nemo
4. The Xslt Report that is run is located in the MVC Project at:
	* **XslFO.TestSolution/XslFO.WebMVC/XslFO.Reports/...**

I hope this helps anyone looking to dynamically generate PDF's in C# or .Net with a templating approach that is far more maintainable than other code manipulation approaches . . . 


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



