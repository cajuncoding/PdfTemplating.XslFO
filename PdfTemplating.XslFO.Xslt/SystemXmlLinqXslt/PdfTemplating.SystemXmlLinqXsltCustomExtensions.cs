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

using System.Collections.Generic;
using System.IO;
using PdfTemplating.SystemIOCustomExtensions;
using System.Linq;
using PdfTemplating.SystemXmlLinqCustomExtensions;
using System.Xml.Xsl;
using System;
using System.Xml;
using System.Xml.Linq;

namespace PdfTemplating.SystemXmlLinqXsltCustomExtensions
{
    /// <summary>
    /// Provides extension methods for simple conversion, proxy, processing, etc. between System.Xml and System.Xml.Linq classes.
    /// </summary>
    public static class XmlLinqXsltExtensions
    {
        /// <summary>
        /// Process the XDocument for any "include" and "import" tags -- using standard Xsl syntax (i.e. &lt;include href="???" /&gt;) that are 
        /// members of teh specified Namespace.  All elements found will then be processed using the provided
        /// XmlResolver and replaced inline.  To remain in conformance with standard "xsl:include" and "xsl:import" elements this process will
        /// iterate until all nested references all also resolved.  Nesting is not limited however to minimize issues with infinite loops 
        /// all external resource "href" values will only be resolved ONCE, otherwise the reference tag will simply be removed.
        /// </summary>
        /// <param name="xXsltDocument"></param>
        /// <param name="xmlResolver"></param>
        public static void ResolveExternalReferences(this XDocument xXsltDocument, XNamespace xCustomInclusionNamespace, XmlResolver xmlResolver)
        {
            //Process All External Resource Links...
            //Note:  This loop will search for items, process them then remove them but will
            //       then Iterate recursively until NO further items are found for processing
            //       before continuing.
            IEnumerable<XElement> xReferences = null;
            List<XElement> referencesProcessedList = new List<XElement>();
            Dictionary<String, String> iterationProcessedItems = new Dictionary<string, string>();
            do
            {
                //Initialize our Items to Remove so we can iteratively process correctly
                referencesProcessedList.Clear();

                //Now search for Items to Process!
                //Note:  We MUST execute a ToList to ensure that our Enumeration
                //       doesn't grow and get affected when we modify the source Document DOM...
                xReferences = (from xEl in xXsltDocument.Descendants()
                               where (xEl.Name == xCustomInclusionNamespace.GetName("include")
                                  || xEl.Name == xCustomInclusionNamespace.GetName("import"))
                                  && xEl.Attributes("href").Count() == 1
                               select xEl).ToList();

                foreach (var xRefLink in xReferences)
                {
                    string refLinkType = xRefLink.Name.LocalName;
                    string refLinkValue = xRefLink.Attribute("href").Value;

                    //Log the Current Item in our List and ONLY Process if it has NEVER been processed before
                    //so that we are not even thrown into an infinite Loop of includes that include each other or themselves.
                    if (!String.IsNullOrEmpty(refLinkValue) && !iterationProcessedItems.ContainsKey(refLinkValue))
                    {
                        iterationProcessedItems.Add(refLinkValue, "New External Resource to Process");

                        //Resolve the current Reference as a Stream
                        Uri refLinkUri = xmlResolver.ResolveUri(null, refLinkValue);
                        Stream memoryStream = xmlResolver.GetEntity(refLinkUri, null, typeof(Stream)) as Stream;
                        XElement xExternalSource = memoryStream.ToXElement();

                        //Process each valid Referenced Item Correctly!
                        IEnumerable<XElement> xElementsToImport = null;
                        if (refLinkType == "include")
                        {
                            //Note:  For "include" elements we replace the original <include> element with the Children of the included
                            //       stylesheet in a Copy/Paste form with no alteration therefore we include ALL child elements in the import.
                            xElementsToImport = xExternalSource.Elements();
                        }
                        else if (refLinkType == "import")
                        {
                            //Note:  For "import" elements we replace the original <import> element with ONLY the Children of the included
                            //       stylesheet that do NOT already exist in the Parent Stylesheet. This improves Safety of Imports when 
                            //       items may conflict and due to our iterative loop means that Include/Import statements above this particular
                            //       element in teh DOM will have precedence over Lower!  Therefore we perform a filter to get only Valid Elements
                            //       for the import.
                            //Note:  We ONLY need to be concerned with Elements that nave a Name Attribute (i.e. only Named Templates can conflict)
                            //       and that also already exist in the Source Document.  To put another way, we should import ANY element that
                            //       does NOT have a "name" attirbute or does NOT have a matching attribute already existing in the Document.
                            xElementsToImport = from newEl in xExternalSource.Elements()
                                                where newEl.Attributes("name").Count() == 0
                                                    || xXsltDocument.Root.Elements().Any(existingEl =>
                                                    {
                                                        return existingEl.Name == newEl.Name
                                                                && existingEl.Attribute("name") != null
                                                                && newEl.Attribute("name") != null
                                                                && existingEl.Attribute("name").Value == newEl.Attribute("name").Value;

                                                    }) == false
                                                select newEl;
                        }

                        //Finally we Append new list of elements to import after our Referenced Element!
                        //Note:  We can NOT replace it yet because we are Enumerating over the references but we log it for later removal
                        xRefLink.AddAfterSelf(xElementsToImport);
                    }

                    //Alwasy add the item to our Processed List to ensure that it is Removed since it was a Duplicate!
                    referencesProcessedList.Add(xRefLink);
                }

                //After Enumeration is Complete we are able to remove all references
                if (referencesProcessedList.Count > 0) referencesProcessedList.Remove();
            }
            while (referencesProcessedList.Count > 0);
        }

        #region ToXslTransformEngine (Custom Wrapper Implementation) Extension Methods

        /// <summary>
        /// Convert the current XDocument object into a Compiled Xsl Transformer with the specified XmlResovlver to use when Loading/Compiling the Xslt.  
        /// Errors will be thrown if the input Xml is not valid for an Xslt.
        /// This allows increaed performance because the caller can keep a reference to the fully compiled Transformer, after all 
        /// parsing/imports/includes etc., have been processed.
        /// </summary>
        /// <param name="xXsltDocument"></param>
        /// <param name="xsltOptions"></param>
        /// <returns></returns>
        public static XslTransformEngine CreateXslTransformEngine(this XDocument xXsltDocument, XslTransformEngineOptions xsltOptions = null)
        {
            XslTransformEngine transformer = new XslTransformEngine(xXsltDocument, xsltOptions);
            return transformer;
        }

        #endregion

        #region Quick Transform Extension Methods

        /// <summary>
        /// Transforms an XDocument using the speicified Xslt Xml.
        /// </summary>
        /// <param name="xInputDocument">The XDocument to Transform.</param>
        /// <param name="xXsltDocument">The XDocument containing the source of the Xslt.</param>
        /// <returns>The Transform Output in Text format supporting any output from transformation (text, html, xml, etc.).</returns>
        public static String Transform(this XDocument xInputDocument, XDocument xXsltDocument)
        {
            return xInputDocument.Transform(xXsltDocument, new XmlUrlResolver());
        }

        /// <summary>
        /// Transforms an XDocument using the speicified Xslt Xml.
        /// </summary>
        /// <param name="xInputDocument">The XDocument to Transform.</param>
        /// <param name="xXsltDocument">The XDocument containing the source of the Xslt.</param>
        /// <param name="xmlResolver">An XmlResolver object to use during the Transform.</param>
        /// <returns>The Transform Output in Text format supporting any output from transformation (text, html, xml, etc.).</returns>
        public static String Transform(this XDocument xInputDocument, XDocument xXsltDocument, XmlResolver xmlResolver)
        {
            return xInputDocument.Transform(xXsltDocument, null, null, xmlResolver);
        }

        /// <summary>
        /// Transforms an XDocument using the speicified Xslt Xml and allows injection of parameters and extension objects.
        /// </summary>
        /// <param name="xInputDocument">The XDocument to Transform.</param>
        /// <param name="xXsltDocument">The XDocument containing the source of the Xslt.</param>
        /// <param name="kvParameters">An list of key/object pairs of Parameter values to inject.</param>
        /// <param name="kvExtensionObjects">A list of Extension objects to Inject keyed by Xml Namespace strings.</param>
        /// <returns>The Transform Output in Text format supporting any output from transformation (text, html, xml, etc.).</returns>
        public static String Transform(this XDocument xInputDocument, XDocument xXsltDocument, IDictionary<String, Object> kvParameters, IDictionary<String, Object> kvExtensionObjects)
        {
            XmlResolver xmlDefaultResolver = new XmlUrlResolver();
            return xInputDocument.Transform(xXsltDocument, kvParameters, kvExtensionObjects, xmlDefaultResolver, xmlDefaultResolver);
        }

         /// <summary>
        /// Transforms an XDocument using the speicified Xslt Xml and allows injection of parameters and extension objects.
        /// </summary>
        /// <param name="xInputDocument">The XDocument to Transform.</param>
        /// <param name="xXsltDocument">The XDocument containing the source of the Xslt.</param>
        /// <param name="kvParameters">An list of key/object pairs of Parameter values to inject.</param>
        /// <param name="kvExtensionObjects">A list of Extension objects to Inject keyed by Xml Namespace strings.</param>
        /// <param name="documentResolver">An XmlResolver object to use during the execution of the Transform.</param>
        /// <param name="loadResolver">An XmlResolver object to use during the Pre-compile loading of external resources for the Transform.</param>
        /// <returns>The Transform Output in Text format supporting any output from transformation (text, html, xml, etc.).</returns>
        public static String Transform(this XDocument xInputDocument, XDocument xXsltDocument, IDictionary<String, Object> kvParameters, IDictionary<String, Object> kvExtensionObjects, XmlResolver documentResolver, XmlResolver loadResolver)
        {
            //Convert the input Dictionaries into XsltArguments with all Shareable values in Text Format
            XsltArgumentList xXsltParams = new XsltArgumentList();

            if (kvParameters != null)
            {
                //Note:  Because Xml parameters are also sensitive to Invalid Character issues in the Param Names
                //       we must make sure we Validate the Name but do not stop the process for continuing when issues arise...
                foreach (var kvParam in kvParameters)
                {
                    try
                    {
                        xXsltParams.AddParam(XmlConvert.EncodeName(kvParam.Key), String.Empty, kvParam.Value);
                    }
                    catch
                    {
                        //Do Nothing and allow Continued Processing
                    }
                }
            }

            if (kvExtensionObjects != null)
            {
                foreach (var kvObject in kvExtensionObjects)
                {
                    xXsltParams.AddExtensionObject(kvObject.Key, kvObject.Value);
                }
            }

            return xInputDocument.Transform(xXsltDocument, xXsltParams, documentResolver, loadResolver);
        }


        /// <summary>
        /// Transforms an XDocument using the speicified Xslt Xml.
        /// </summary>
        /// <param name="xInputDocument">The XDocument to Transform.</param>
        /// <param name="xXsltDocument">The XDocument containing the source of the Xslt.</param>
        /// <param name="xXsltParams">An XsltArgumentList of Parameter values to inject.</param>
        /// <returns>The Transform Output in Text format supporting any output from transformation (text, html, xml, etc.).</returns>
        public static String Transform(this XDocument xInputDocument, XDocument xXsltDocument, XsltArgumentList xXsltParams)
        {
            XmlResolver xmlDefaultResolver = new XmlUrlResolver();
            return xInputDocument.Transform(xXsltDocument, xXsltParams, xmlDefaultResolver, xmlDefaultResolver);
        }

        /// <summary>
        /// Transforms an XDocument using the speicified Xslt Xml.
        /// </summary>
        /// <param name="xInputDocument">The XDocument to Transform.</param>
        /// <param name="xXsltDocument">The XDocument containing the source of the Xslt.</param>
        /// <param name="xXsltParams">An XsltArgumentList of Parameter values to inject.</param>
        /// <param name="documentResolver">An XmlResolver object to use during the Transform.</param>
        /// <returns>The Transform Output in Text format supporting any output from transformation (text, html, xml, etc.).</returns>
        public static String Transform(this XDocument xInputDocument, XDocument xXsltDocument, XsltArgumentList xXsltParams, XmlResolver xmlResolver)
        {
            return xInputDocument.Transform(xXsltDocument, xXsltParams, xmlResolver, xmlResolver);
        }

        /// <summary>
        /// Transforms an XDocument using the speicified Xslt Xml.
        /// </summary>
        /// <param name="xInputDocument">The XDocument to Transform.</param>
        /// <param name="xXsltDocument">The XDocument containing the source of the Xslt.</param>
        /// <param name="xXsltParams">An XsltArgumentList of Parameter values to inject.</param>
        /// <param name="documentResolver">An XmlResolver object to use during the Transform for the document() function.  If Null a default XmlUrlResolver with no credentials will be used.</param>
        /// <param name="loadResolver">An XmlResolver object to use during the Initialization/Load of the XSLT Source.  If Null a default XmlUrlResolver with no credentials will be used.</param>
        /// <returns>The Transform Output in Text format supporting any output from transformation (text, html, xml, etc.).</returns>
        public static String Transform(this XDocument xInputDocument, XDocument xXsltDocument, XsltArgumentList xXsltParams, XmlResolver documentResolver, XmlResolver loadResolver)
        {
            //NOTE:  We use the Overloaded method here with null XmlResolver to ensure that we run the Xslt trusted
            //       with full access to the document() function and script blocks.
            //NOTE:  FROM MSDN:
            //          The XmlResolver to use to load the style sheet and any style sheet(s) referenced in xsl:import and xsl:include elements.
            //          If this is null, a default XmlUrlResolver with no user credentials is used to open the style sheet. 
            //          The default XmlUrlResolver is not used to resolve any external resources in the style sheet, so xsl:import and xsl:include elements are not resolved.
            //NOTE:  We use the "Load Resolver" specified here for Loading the Xslt!  And, we pass on the "Document Resolver" to be used
            //       during the actual transformation execution!
            //NOTE:  To ensure that we have at least a resolver attached we coalesce the passed in values with a fallback of XmlUrlResolver() when
            //       the parameters are null.
            XslCompiledTransform xXslTransformer = xXsltDocument.ToXslCompiledTransform(loadResolver);
            String strOutput = xInputDocument.Transform(xXslTransformer, xXsltParams, documentResolver ?? new XmlUrlResolver());

            return strOutput;
        }

        /// <summary>
        /// Transforms an XDocument using the speicified Xslt.
        /// </summary>
        /// <param name="xInputDocument">The XDocument to Transform.</param>
        /// <param name="xXslTranformer">The Compiled Xsl Transform to convert.</param>
        /// <param name="xXsltParams">The Arguments List to inject into the Xsl Transform to convert.</param>
        /// <returns>The Transform Output in Text format supporting any output from transformation (text, html, xml, etc.).</returns>
        public static String Transform(this XDocument xInputDocument, XslCompiledTransform xXslTranformer, XsltArgumentList xXsltParams)
        {
            return xInputDocument.Transform(xXslTranformer, xXsltParams, new XmlUrlResolver());
        }

        /// <summary>
        /// Transforms an XDocument using the speicified Xslt.  
        /// BBernard - 05/06/2013 fixed several issues related to the <xsl:output> attribute processing and using
        /// the correct Encoding, OmitXmlDeclaration, and CDATA attribute values in the processing of the Xslt!
        /// </summary>
        /// <param name="xInputDocument">The XDocument to Transform.</param>
        /// <param name="xXslTranformer">The Compiled Xsl Transform to convert.</param>
        /// <param name="xXsltParams">The Arguments List to inject into the Xsl Transform to convert.</param>
        /// <returns>The Transform Output in Text format supporting any output from transformation (text, html, xml, etc.).</returns>
        public static String Transform(this XDocument xInputDocument, XslCompiledTransform xXslTranformer, XsltArgumentList xXsltParams, XmlResolver documentResolver)
        {
            //BBernard - 05/06/2013
            //NOTE: IDENTIFIED BUG here where the low level .Net XmlWriter behaves NON-Intuitively and inconsistently
            //       for processing Xslt results.  The issue is that the document is processed using the Xslt OutputSettings
            //       BUT in the final rendering of the results to a String object the XmlDeclartion marks it as UTF-16 because the underlying
            //       String value from .Net is in fact using UTF-16 in memory.  So we must be very careful here to process correctly --
            //       We cannot make any assumptions as .Net will enforce defaults that we do not expect.
            //       The result is confusing and can break processes -- For Example: 
            //       the Xslt document may process perfectly and the output generated using the OutputSettings with UTF-8 
            //       encoding but then the final result is marked as UTF-16 in the XmlDecleartion header by this behavior.
            //NOTE: The root of this behavior is in the fact that ALL System.String objects in .NET as represented in UTF-16 encoding. 
            //       Every last one. And System.String object's encoding is fixed; it cannot be changed. This behaviour is 
            //       inherited and enforced by the default TextWriter encoding and Therefore, the final output is mis-marked.
            //NOTE: FOR a Much more detailed explanation of this see:
            //          http://www.undermyhat.org/blog/2009/08/tip-force-utf8-or-other-encoding-for-xmlwriter-with-stringbuilder/
            //          http://geekswithblogs.net/pakistan/archive/2005/08/23/50884.aspx
            //          http://msdn.microsoft.com/en-us/library/system.xml.xmlwriter%28VS.80%29.aspx?PHPSESSID=6c7e47uec18v8n3121a6p001h4
            //NOTE: This affects the XmlTextWriter implementation based on StringWriter & StringBuilder.  Therefore I
            //       have re-factored this to process the Xslt via a MemoryStream instead, and then use the
            //       additional custom extensions for handling streams to efficiently read this back into a String,
            //       therefore there is NO expected performance degradation from the previous implementation using StringBuilder.
            //NOTE: With the MemoryStream we make no assumptions for .Net to process into a String.  Instead we process
            //       the raw Xslt outputs ourselves with the correct encoding and THEN FINALLY output the full result to
            //       a string ourselves; whereby the STring is treated as UTF-16 in memory, but the content is our final data
            //       with all encodings already handled and marked correctly inside the String object's value!  
            //NOTE: By using the existing CustomExtension methods for Stream handling we can easily specify the Encoding by
            //       which we want to process the data from the MemoryStreram.  This allows us to enforce the Encoding specified
            //       inside the Xslt just as we actually expect it to behave.
            ////StringBuilder stringBuilder = new StringBuilder();
            using(Stream memoryStream = new MemoryStream())
            using (XmlReader xmlInputReader = xInputDocument.CreateReader())
            {
                //NOTE: IDENTIFIED BUG here where CDATA[] elements are NOT output as expected.
                //       This because in order for the Xslt Output element to have any effect on the output
                //       we MUST pass the "loaded" settings -- as processed from the Xslt -- into the XmlTextWriter; as opposed to a 
                //       new set of settings instantiated here.  Then the XmlTextWriter will behave as we expect, handling elements correctly
                //       such as OmitXmlDeclaration, Encoding, AND CDATA elements!
                //NOTE: The XslCompiledTransform will create the settings based on the <xsl:output> element and now we will
                //       allow those settings to control the XmlTextWriter object that is ACTUALLY controlling the output
                //       here in .Net.  This fixes the bugs where <xsl:output> flags were not taking effect (ie. cdata-elements not being implemented)!
                ////using (XmlWriter xmlWriter = XmlTextWriter.Create(stringBuilder, xmlWriterSettings))
                using (XmlWriter xmlWriter = XmlTextWriter.Create(memoryStream, xXslTranformer.OutputSettings))
                {
                    //Perform the Transform and retrieve results from the Writer Stream
                    xXslTranformer.Transform(xmlInputReader, xXsltParams, xmlWriter, documentResolver);
                }

                return memoryStream.ReadString(xXslTranformer.OutputSettings.Encoding);
            }
        }

        /// <summary>
        /// Compiles the XDocument as an XslTransform ready for Execution and processing with.
        /// </summary>
        /// <param name="xXsltDocument">The XDocument containing the source of the Xslt.</param>
        /// <returns>The Transform Output in Text format supporting any output from transformation (text, html, xml, etc.).</returns>
        public static XslCompiledTransform ToXslCompiledTransform(this XDocument xXsltDocument)
        {
            return xXsltDocument.ToXslCompiledTransform(null);
        }

        /// <summary>
        /// Compiles the XDocument as an XslTransform ready for Execution and processing with.
        /// </summary>
        /// <param name="xXsltDocument">The XDocument containing the source of the Xslt.</param>
        /// <param name="loadResolver">An XmlResolver object to use during the Initialization/Load of the XSLT Source.  If Null a default XmlUrlResolver with no credentials will be used.</param>
        /// <returns>The Transform Output in Text format supporting any output from transformation (text, html, xml, etc.).</returns>
        public static XslCompiledTransform ToXslCompiledTransform(this XDocument xXsltDocument, XmlResolver loadResolver)
        {
            XslCompiledTransform xXslTranformer = new XslCompiledTransform();
            String strOutput = String.Empty;

            using (XmlReader xXsltReader = xXsltDocument.CreateReader())
            {
                //NOTE:  We use the Overloaded method here with null XmlResolver to ensure that we run the Xslt trusted
                //       with full access to the document() function and script blocks.
                //NOTE:  FROM MSDN:
                //          The XmlResolver to use to load the style sheet and any style sheet(s) referenced in xsl:import and xsl:include elements.
                //          If this is null, a default XmlUrlResolver with no user credentials is used to open the style sheet. 
                //          The default XmlUrlResolver is not used to resolve any external resources in the style sheet, so xsl:import and xsl:include elements are not resolved.
                //NOTE:  We use the "Load Resolver" specified here for Loading the Xslt!
                //NOTE:  To ensure that we have at least a resolver attached we coalesce the passed in values with a fallback of XmlUrlResolver() when
                //       the parameters are null.
                xXslTranformer.Load(xXsltReader, XsltSettings.TrustedXslt, loadResolver ?? new XmlUrlResolver());
            }

            return xXslTranformer;
        }

        /// <summary>
        /// Transforms an XDocument using the specified Compiled Xslt and returns a generic string containing the text results.
        /// </summary>
        /// <param name="xInputDocument">The XDocument to Transform.</param>
        /// <param name="xslTransformer">The Compiled Xsl Transform to convert.</param>
        /// <param name="xXsltParams">The Arguments List to inject into the Xsl Transform to convert.</param>
        /// <returns>The Transform Output in Text format supporting any output from transformation (text, html, xml, etc.).</returns>
        public static String TransformToString(this XDocument xInputDocument, XslTransformEngine xslTransformer, XsltArgumentList xXsltParams = null)
        {
            return xslTransformer.TransformToString(xInputDocument, xXsltParams);
        }

        /// <summary>
        /// Transforms an XDocument using the specified Compiled Xslt and returns a Stream object contains the results.
        /// </summary>
        /// <param name="xInputDocument">The XDocument to Transform.</param>
        /// <param name="xslTransformer">The Compiled Xsl Transform to convert.</param>
        /// <param name="xXsltParams">The Arguments List to inject into the Xsl Transform to convert.</param>
        /// <returns>The Transform Output in Text format supporting any output from transformation (text, html, xml, etc.).</returns>
        public static MemoryStream TransformToStream(this XDocument xInputDocument, XslTransformEngine xslTransformer, XsltArgumentList xXsltParams = null)
        {
            return xslTransformer.TransformToStream(xInputDocument, xXsltParams);
        }

        /// <summary>
        /// Transforms an XDocument using the specified Compiled Xslt and returns an XDocument with the results parsed.  
        /// Note:  This assumes that the Results are Valid Xml or an exception will occur.
        /// </summary>
        /// <param name="xInputDocument">The XDocument to Transform.</param>
        /// <param name="xslTransformer">The Compiled Xsl Transform to convert.</param>
        /// <param name="xXsltParams">The Arguments List to inject into the Xsl Transform to convert.</param>
        /// <returns>The Transform Output in Text format supporting any output from transformation (text, html, xml, etc.).</returns>
        public static XDocument TransformToXDocument(this XDocument xInputDocument, XslTransformEngine xslTransformer, XsltArgumentList xXsltParams = null)
        {
            return xslTransformer.TransformToXDocument(xInputDocument, xXsltParams);
        }
        #endregion

    }

}
