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

using System;
using System.CustomExtensions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Xml.Linq.CustomExtensions;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.CustomExtensions;
using System.Security;
using System.Web;
using System.IO;
using System.IO.CustomExtensions;

namespace System.Xml.Linq.CustomExtensions
{
    #region Xslt Transform Utility Classes

    public class XslTransformEngineOptions
    {
        public XslTransformEngineOptions()
        {
        }

        public XmlResolver XsltDocumentResolver { get; set; }
        public XmlResolver XsltLoadResolver { get; set; }
        public XmlWriterSettings XmlWriterSettings { get; set; }
    }

    public class XslTransformEngineException : Exception
    {
        public XslTransformEngineException(String message, Exception innerExc, String xsltSource = "", String xsltOutput = "", bool isXsltSuccessful = false)
            : base(message, innerExc)
        {
            this.XsltSource = xsltSource;
            this.XsltOutput = xsltOutput;
            this.IsXsltSuccessful = isXsltSuccessful;
        }
        public String XsltSource { get; protected set; }
        public String XsltOutput { get; protected set; }
        public bool IsXsltSuccessful { get; protected set; }
    }

    public class XslTransformEngine
    {
        #region Default fallback Xml/Xslt configuration objects

        //Init our default Load Resolver for when null is specified
        private static XmlUrlExtendedResolver _xmlDefaultLoadResolver = new XmlUrlExtendedResolver();
        private static XmlUrlExtendedResolver _xmlDefaultDocumentResolver = new XmlUrlExtendedResolver();
        private static XmlWriterSettings _xmlDefaultWriterSettings = new XmlWriterSettings()
        {
            ConformanceLevel = ConformanceLevel.Fragment,
            Encoding = System.Text.ASCIIEncoding.UTF8
        };
        private static XsltArgumentList _xsltCustomArgumentsList = CreateCustomXsltArgumentList();

        #endregion

        #region Constructors

        public XslTransformEngine(XDocument xXsltDocument, XslTransformEngineOptions options)
        {
            XslCompiledTransform transformer = new XslCompiledTransform();
            String strOutput = String.Empty;

            if (options == null) throw new ArgumentNullException("Xsl Transform Options parameter is null or missing; a valid set of XslTransformOptions must be specified.");

            //To keep the code safe, we alwasy instantiate an Options object, 
            //though it's properties may be null, the .Options references will still work!

            using (XmlReader xXsltReader = xXsltDocument.CreateReader())
            {
                try
                {
                    //NOTE:  We use the "Load Resolver" specified here for Loading the Xslt!  And, we pass on the "Document Resolver" to be used
                    //       during the actual transformation execution!
                    //NOTE:  To ensure that we have at least a resolver attached we coalesce the passed in values with a fallback of XmlUrlResolver() when
                    //       the parameters are null; which can happen by either overloaded call or user call with null value, so we always coalesce.
                    transformer.Load(xXsltReader, XsltSettings.TrustedXslt, options.XsltLoadResolver ?? _xmlDefaultLoadResolver);
                }
                catch (Exception exc)
                {
                    throw new XslTransformEngineException("XslTransformEngine initialization failed; Xslt Source could not be compiled.", exc, xXsltDocument.ToString());
                }
            }

            this.Options = options;
            this.XslSourceDocument = xXsltDocument;
            this.XslCompiledTransform = transformer;
        }

        #endregion

        #region Public Properties

        public XslCompiledTransform XslCompiledTransform { get; protected set; }
        public XDocument XslSourceDocument { get; protected set; }
        public XslTransformEngineOptions Options { get; set; }

        #endregion

        #region Static Methods / Factory Methods

        /// <summary>
        /// Create the Custom XsltArgumentsList with the Xslt CustomExtensionObjects implementation ready for use.
        /// </summary>
        /// <param name="fnXsltEventHandler"></param>
        /// <returns></returns>
        public static XsltArgumentList CreateCustomXsltArgumentList(EventHandler<XsltExtensionEventArgs> fnXsltEventHandler = null)
        {
            //Initialize the CustomExtensionObjects for the XslTransformEngine
            XsltSystemCustomExtensionObject xsltExtensionObject = new XsltSystemCustomExtensionObject(fnXsltEventHandler ?? null);

            var kvExtensionObjects = new Dictionary<String, Object>();
            kvExtensionObjects.Add(XsltSystemCustomExtensionObject.XmlnsUrn, xsltExtensionObject);

            //Finally return the Default Argument list created with the Custom Extension Methods Objects
            return CreateXsltArgumentList(null, kvExtensionObjects);
        }

        /// <summary>
        /// Create an XsltArgumentList from a pair of Dictionary objects for both Parameters, and ExtensionObjects
        /// </summary>
        /// <param name="kvParameters"></param>
        /// <param name="kvExtensionObjects"></param>
        /// <returns></returns>
        public static XsltArgumentList CreateXsltArgumentList(IDictionary<String, Object> kvParameters, IDictionary<String, Object> kvExtensionObjects)
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

            return xXsltParams;
        }

        #endregion

        #region Transform Methods

        /// <summary>
        /// Transform method that provides easier wrapper interface to the transformation by simpy returning the result as a MemoryStream.
        /// </summary>
        /// <param name="xslCompiledTransform"></param>
        /// <param name="xInputDocument"></param>
        /// <param name="xXsltParams"></param>
        /// <param name="documentResolver"></param>
        /// <param name="xmlWriterSettings"></param>
        /// <returns></returns>
        public MemoryStream TransformToStream(XDocument xInputDocument, XsltArgumentList xXsltParams = null, EventHandler<XsltExtensionEventArgs> fnXsltEventHandler = null)
        {
            try
            {
                //Construct a valid Stream to write the output into
                MemoryStream stream = new MemoryStream();
                using (XmlReader xmlInputReader = xInputDocument.CreateReader())
                using (XmlWriter xmlWriter = XmlTextWriter.Create(stream, this.Options.XmlWriterSettings ?? _xmlDefaultWriterSettings))
                {
                    //Perform the Transform and retrieve results from the Writer Stream
                    this.XslCompiledTransform.Transform(xmlInputReader, xXsltParams ?? _xsltCustomArgumentsList, xmlWriter, this.Options.XsltDocumentResolver ?? _xmlDefaultDocumentResolver);
                }

                //Reset the stream for our consumers to use
                stream.Reset();

                //Return the stream
                return stream;
            }
            catch (Exception exc)
            {
                throw new XslTransformEngineException(
                    "Exception occurred within the XslTransformEngine while processing the Xslt into a stream output.",
                    exc,
                    xInputDocument == null ? String.Empty : xInputDocument.ToString()
                );
            }
        }

        /// <summary>
        /// Transform method that provides easier wrapper interface to the transformation by simpy returning the result as a string.
        /// </summary>
        /// <param name="xslTransformer"></param>
        /// <param name="xInputDocument"></param>
        /// <param name="xXsltParams"></param>
        /// <param name="documentResolver"></param>
        /// <param name="xmlWriterSettings"></param>
        /// <returns></returns>
        public String TransformToString(XDocument xInputDocument, XsltArgumentList xXsltParams = null) 
        {
            //NOTE:  We MUST wrap the stream in a Using block to ensure it is properly disposed of since 
            //       we both instantiate it and convert it, but do NOT pass the disposable stream back up the line.
            //NOTE:  TransformToStream will handle exception handling for actual Transformation so we only have
            //       to handle additional logic inside the using statement.
            using (MemoryStream stream = TransformToStream(xInputDocument, xXsltParams))
            {
                try
                {
                    return stream.ReadString(true);
                }
                catch (Exception exc)
                {
                    throw new XslTransformEngineException(
                        "Exception occurred within the XslTransformEngine while processing the Xslt into a string output.",
                        exc,
                        xInputDocument == null ? String.Empty : xInputDocument.ToString()
                    );
                }
            }
        }

        /// <summary>
        /// Transform method that provides easier wrapper interface to the transformation by simpy returning the result as an XDocument.
        /// 
        /// Note:  Returning an XDocument will result in an Exception being thrown if the results of the Xslt are not valid Xml.
        /// </summary>
        /// <param name="xInputDocument"></param>
        /// <param name="xXsltParams"></param>
        /// <returns></returns>
        public XDocument TransformToXDocument(XDocument xInputDocument, XsltArgumentList xXsltParams = null)
        {
            //NOTE:  We MUST wrap the stream in a Using block to ensure it is properly disposed of since 
            //       we both instantiate it and convert it, but do NOT pass the disposable stream back up the line.
            //NOTE:  TransformToStream will handle exception handling for actual Transformation so we only have
            //       to handle additional logic inside the using statement.
            using (MemoryStream stream = TransformToStream(xInputDocument, xXsltParams))
            {
                try
                {
                    return stream.ToXDocument();
                }
                catch (Exception exc)
                {
                    String xsltOutput = String.Empty;
                    try
                    {
                        stream.Reset();
                        xsltOutput = stream.ReadString();
                    }
                    catch {}

                    throw new XslTransformEngineException(
                        "Exception occurred within the XslTransformEngine while processing the Xslt into an XDocument output.",
                        exc,
                        xInputDocument == null ? String.Empty : xInputDocument.ToString(),
                        xsltOutput.ToNullSafe(),
                        !xsltOutput.IsNullOrEmpty() //Is Successful if the output is not null/empty
                    );
                }
            }
        }

        #endregion
    }

    #endregion
}
