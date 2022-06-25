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
    /// <summary>
    /// Custom Extension Methods / Utility Methods for using when executing Xslt with .Net to give the Xsl Developer
    /// much greater flexibility and power through the .Net framework.
    /// </summary>
    public class XsltSystemCustomExtensionObject
    {
        #region Constants
        public const String XmlnsUrn = "urn:Xslt.System.Extensions";
        #endregion

        #region Constructors

        public XsltSystemCustomExtensionObject()
        {
        }

        public XsltSystemCustomExtensionObject(EventHandler<XsltExtensionEventArgs> fnXsltExtensionEventHandler)
        {
            if (fnXsltExtensionEventHandler != null)
            {
                //Wire up the Event Handler subscription to our internal Event
                this.XsltExtensionEvent += fnXsltExtensionEventHandler;
            }
        }

        #endregion

        #region Public Events

        public event EventHandler<XsltExtensionEventArgs> XsltExtensionEvent;

        #endregion

        #region Private Helper Methods

        private void LogHelper(string message)
        {
            LogHelper(message, null, null);
        }

        private void LogHelper(string message, params object[] arguments)
        {
            LogHelper(message, null, arguments);
        }

        private void LogHelper(string message, Exception exc, params object[] arguments)
        {
            String strMessage = String.Format(message, arguments);
            XsltErrorEventArgs eventArgs = new XsltErrorEventArgs("{0} -- {1}{2}{2}{3}", strMessage, exc.GetMessagesRecursively(), Environment.NewLine, exc.StackTrace);

            if (exc != null && !XsltExtensionEvent.Raise(this, eventArgs))
            {
                throw new Exception(eventArgs.Message, exc);
            }
        }

        private TimeSpan DateDiffHelper(string startDate, string endDate)
        {
            TimeSpan timeSpanDiff = new TimeSpan(0);
            try
            {
                DateTime parsedStartDate;
                DateTime parsedEndDate;
                if (DateTime.TryParse(startDate, out parsedStartDate) && DateTime.TryParse(endDate, out parsedEndDate))
                {
                    timeSpanDiff = parsedEndDate.Subtract(parsedStartDate);
                }
            }
            catch (Exception exc)
            {
                LogHelper(@"Error while computing Date Difference between [{0}] and [{1}].", exc, startDate, endDate);
            }

            return timeSpanDiff;
        }

        private String FormatDateHelper(DateTime dateInput, string format)
        {
            return FormatDateHelper(dateInput, format, null, null);
        }

        private String FormatDateHelper(DateTime dateInput, string format, string countryCode, string languageCode)
        {
            string formattedOutput = String.Empty;
            try
            {
                if ((!countryCode.IsNullOrEmpty()) && (!languageCode.IsNullOrEmpty()))
                {
                    formattedOutput = dateInput.ToString(format, CultureInfo.CreateSpecificCulture("{0}-{1}".FormatArgs(languageCode.ToLowerInvariant(), countryCode.ToUpperInvariant())));
                }
                else
                {
                    formattedOutput = dateInput.ToString(format);
                }
            }
            catch (Exception exc)
            {
                LogHelper("Error formatting DateTime value [{0}] into specified format [{1}] -- {2}", dateInput.ToString(), format, exc.GetMessagesRecursively());
            }

            return formattedOutput;
        }

        #endregion

        #region Event Methods for Callbacks and Events from the XSLT being processed

        public void Debug(String message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        #endregion

        #region Extension Methods for easier operations inside XSLT

        //Note: Remove/Strip all Namespaces from any NodeSet passed in
        public XPathNavigator RemoveNamespaces(XPathNavigator inputNodeSet)
        {
            return inputNodeSet.ToXElement().RemoveNamespaces().CreateNavigator();
        }

        //Note:  Convert any valid nodeset into the String form of the Xml
        //NOTE:  For more information of Data Type Mapping see:
        //       http://msdn.microsoft.com/en-us/library/dfktf882%28v=vs.90%29.aspx
        public String NodeListToString(XPathNodeIterator inputNodeList)
        {
            var stringBuilder = new StringBuilder();
            //int c = 1;
            //LogHelper("Starting to Convert NodeList of [{0}] Items!!!", inputNodeList.Count);
            foreach (XPathNavigator nodeNavigator in inputNodeList)
        {
                //LogHelper("    - Converting Node [{0} of {1}] to String!!!", c++, inputNodeList.Count);
                stringBuilder.Append(nodeNavigator.OuterXml);
        }
            return stringBuilder.ToString();
        }


        //Note:  Convert any valid nodeset into the String form of the Xml
        public String NodeSetToString(XPathNavigator inputNodeSet)
        {
            var result = inputNodeSet.ToXElement().ToString();
            //_tbbContext.LogDebugFormat("Attempting to convert the following NodeSet to String: \n{0}\n", inputNodeSet);
            return result;
        }

        //Note:  Convert any valid nodeset into the String form of the Xml
        public String NodeSetToString(XPathNavigator inputNodeSet, bool bMoveToRoot)
        {
            var result = inputNodeSet.ToXElement(bMoveToRoot).ToString();
            //_tbbContext.LogDebugFormat("Attempting to convert the following NodeSet to String: \n{0}\n", inputNodeSet);
            return result;
        }

        //Note:  To enable additional XPath execution on the Result of an Extension Method
        //       we must return an XPathNavigator object back to the Xslt!
        public XPathNavigator ToNodeSet(string input)
        {
            return ToNodeSet(input, true);
        }

        public XPathNavigator ToNodeSet(string input, bool bRemoveNamespaces)
        {
            XPathNavigator xPathNav = null;
            XElement xEl = null;

            //Parse into an XElement to enable easy conversion
            try
            {
                LogHelper("Parsing Xml Text to XElement...");
                xEl = XElement.Parse(input);

                if (xEl != null && bRemoveNamespaces)
                {
                    LogHelper("Removing Namespaces from XElement...");
                    xEl = xEl.RemoveNamespaces();
                }

            }
            catch (Exception exc)
            {
                LogHelper(@"Error trapped in Xsl Custom Extension ToNodeSet(""{0}"", ""{1}""): {2}{3}",
                    exc,
                    input.Substring(0, input.Length > 20 ? 20 : input.Length - 1) + "...",
                    bRemoveNamespaces,
                    Environment.NewLine,
                    exc.GetMessagesRecursively());

                LogHelper(exc.StackTrace);
            }

            if (xEl == null)
            {
                LogHelper("Unable to Parse Xml Text to XElement; XElement object is null.");
                LogHelper(input);
                xEl = new XElement("ParseError", "Unable to Parse Xml Text to XElement; XElement object is null");
            }

            LogHelper("Converting to XPathNavigator...");
            xPathNav = xEl.CreateNavigator();

            LogHelper("Completed Conversion of input text to Xml ready for XPath");
            return xPathNav;
        }

        public object StringXPath(string inputXml, string inputXPath)
        {
            XPathNavigator xPathNav = ToNodeSet(inputXml);
            return NodesetXPath(xPathNav, inputXPath);
        }

        public object NodesetXPath(XPathNavigator inputNodeSet, string inputXPath)
        {
            return inputNodeSet.Evaluate(inputXPath);
        }

        public string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public string FormatString(string formatString, string arg0)
        {
            return formatString.FormatArgs(arg0);
        }

        public string FormatString(string formatString, string arg0, string arg1)
        {
            return formatString.FormatArgs(arg0, arg1);
        }

        public string FormatString(string formatString, string arg0, string arg1, string arg2)
        {
            return formatString.FormatArgs(arg0, arg1, arg2);
        }

        public string FormatString(string formatString, string arg0, string arg1, string arg2, string arg3)
        {
            return formatString.FormatArgs(arg0, arg1, arg2, arg3);
        }

        public string FormatString(string formatString, string arg0, string arg1, string arg2, string arg3, string arg4)
        {
            return formatString.FormatArgs(arg0, arg1, arg2, arg3, arg4);
        }

        public string FormatData(string input, string format)
        {
            string formattedOutput = String.Empty;
            try
            {
                formattedOutput = String.Format("{0:" + format + "}", input);
            }
            catch (Exception exc)
            {
                LogHelper("Error formatting String value [{0}] into specified format [{1}].", exc, input.ToString(), format);
            }

            return formattedOutput;
        }
        public string Replace(string inputString, string findString, string replaceString)
        {
            return inputString.Replace(findString, replaceString);
        }

        public string ToUpper(string input)
        {
            return input.ToUpper();
        }

        public string ToLower(string input)
        {
            return input.ToLower();
        }

        public string Trim(string input, string trimCharacters)
        {
            return input.Trim(trimCharacters.ToCharArray());
        }

        public string TrimStart(string input, string trimCharacters)
        {
            return input.TrimStart(trimCharacters.ToCharArray());
        }

        public string TrimEnd(string input, string trimCharacters)
        {
            return input.TrimEnd(trimCharacters.ToCharArray());
        }

        public string PadLeft(string input, int totalWidth, string paddingChar)
        {
            return input.PadLeft(totalWidth, paddingChar[0]);
        }

        public string PadRight(string input, int totalWidth, string paddingChar)
        {
            return input.PadRight(totalWidth, paddingChar[0]);
        }

        public string Substring(string input, int startIndex)
        {
            return input.Substring(startIndex);
        }

        public string Substring(string input, int startIndex, int length)
        {
            return input.Substring(startIndex, length);
        }

        public string Left(string input, int lengthFromLeft)
        {
            return input.Left(lengthFromLeft);
        }

        public string Right(string input, int lengthFromRight)
        {
            return input.Right(lengthFromRight);
        }

        public string Repeat(string input, int count)
        {
            return input.ToNullSafe().Repeat(count);
        }

        public bool RegExTest(string input, string regEx)
        {
            return Regex.IsMatch(input, regEx);
        }

        public string RegExMatch(string input, string regEx)
        {
            Match match = Regex.Match(input, regEx);
            return match.Value;
        }

        public XPathNavigator RegExGetMatches(string input, string regEx)
        {
            //Initialize xMatchesEl as Default with "Matches" Root.
            XElement xMatchesEl = new XElement("Matches");
            String errorNote = String.Empty;
            try
            {
                LogHelper("Compiling Regex: {0}", regEx);
                Regex rx = new Regex(regEx, RegexOptions.Compiled);
                LogHelper("Executing Regex on String Value: {0}", input);
                
                xMatchesEl = rx.MatchesAsXml(input);
            }
            catch (Exception exc)
            {
                LogHelper(@"Error trapped in Xslt Custom Extension RegExGetMatches(""{0}"", ""{1}"")".FormatArgs(input, regEx), exc);
                //LogErrorHelper("Error ocurred while executing Regular Expression [{0}] and converting result set to Xml -- {1}".FormatArgs(regEx), exc);
                
                //Note: Error note begins with a space to ensure final output string has proper spacing.
                errorNote = " WITH EXCEPTIONS";
            }

            return new XElement("Matches").CreateNavigator();
        }

        public string XmlEncode(string input)
        {
            return input.ToXmlEncoded();
        }

        public string XmlDecode(string input)
        {
            return input.ToXmlDecoded();
        }

        public string HtmlEncode(string input)
        {
            return input.ToHtmlEncoded();
        }

        public string HtmlDecode(string input)
        {
            return input.ToHtmlDecoded();
        }

        public string UrlEncode(string input)
        {
            return input.ToUrlEncodedStrict();
        }

        public string UrlDecode(string input)
        {
            return input.ToUrlDecodedStrict();
        }

        public string Base64Encode(string input)
        {
            return input.ToBase64();
        }

        public string Base64Decode(string input)
        {
            return input.FromBase64();
        }

        public bool IsDate(string input)
        {
            DateTime parsedDate;
            return DateTime.TryParse(input, out parsedDate);
        }

        public bool IsNumeric(string input)
        {
            Decimal parsedDecimal;
            return Decimal.TryParse(input, out parsedDecimal);
        }

        //NOTE:  We use Double as the base numeric so that all numbers are supported, and
        //       Xslt maps this back to it's basic number type
        //       http://msdn.microsoft.com/en-us/library/533texsx%28v=vs.71%29.aspx

        public double Max(double value1, double value2)
        {
            return Math.Max(value1, value2);
        }

        public double Min(double value1, double value2)
        {
            return Math.Min(value1, value2);
        }

        public int Floor(double value)
        {
            return Convert.ToInt32(Math.Floor(value));
        }

        public int Ceiling(double value)
        {
            return Convert.ToInt32(Math.Ceiling(value));
        }

        public int Round(double value)
        {
            return Convert.ToInt32(Math.Round(value));
        }

        public int Round(double value, int numberOfDecimals)
        {
            return Convert.ToInt32(Math.Round(value, numberOfDecimals));
        }

        public double Abs(double value)
        {
            return Math.Abs(value);
        }

        public double Remainder(double number, double divisor)
        {
            return Math.IEEERemainder(number, divisor);
        }

        public string Coalesce(string test, string fallback1)
        {
            return Coalesce(test, fallback1, String.Empty);
        }

        public string Coalesce(string test, string fallback1, string fallback2)
        {
            return Coalesce(test, fallback1, fallback2, String.Empty);
        }

        public string Coalesce(string test, string fallback1, string fallback2, string fallback3)
        {
            return Coalesce(test, fallback1, fallback2, fallback3, String.Empty);
        }

        public string Coalesce(string test, string fallback1, string fallback2, string fallback3, string fallback4)
        {
            return Coalesce(test, fallback1, fallback2, fallback3, fallback4, String.Empty);
        }

        public string Coalesce(string test, string fallback1, string fallback2, string fallback3, string fallback4, string fallback5)
        {
            String result = String.Empty;
            if (!String.IsNullOrEmpty(test.Trim())) result = test;
            else if (!String.IsNullOrEmpty(fallback1.Trim())) result = fallback1;
            else if (!String.IsNullOrEmpty(fallback2.Trim())) result = fallback2;
            else if (!String.IsNullOrEmpty(fallback3.Trim())) result = fallback3;
            else if (!String.IsNullOrEmpty(fallback4.Trim())) result = fallback4;
            else if (!String.IsNullOrEmpty(fallback5.Trim())) result = fallback5;
            return result;
        }

        public object Ternary(bool bTest, object trueValue, object falseValue)
        {
            return IIf(bTest, trueValue, falseValue);
        }

        public object IIf(bool bTest, object trueValue, object falseValue)
        {
            return bTest ? trueValue : falseValue;
        }

        public string FormatDate(string inputDate, string format)
        {
            return FormatDate(inputDate, format, null, null);
        }

        public string FormatDate(string inputDate, string format, string countryCode, string languageCode)
        {
            string formattedOutput = String.Empty;
            try
            {
                DateTime parsedDate;
                if (DateTime.TryParse(inputDate, out parsedDate))
                {
                    formattedOutput = FormatDateHelper(parsedDate, format, countryCode, languageCode);
                }
            }
            catch (Exception exc)
            {
                LogHelper(@"Error trapped in Xslt Custom Extension FormatDate(""{0}"", ""{1}"", ""{2}"", ""{3}"")".FormatArgs(inputDate, format, countryCode, languageCode), exc);
                //LogHelper("Error parsing DateTime value [{0}] -- {1}", inputDate, exc.GetMessages());
            }

            return formattedOutput;
        }

        public string FormatNumber(string input, string format)
        {
            string formattedOutput = String.Empty;
            try
            {
                Double parsedInput = Double.Parse(input);
                formattedOutput = parsedInput.ToString(format);
            }
            catch (Exception exc)
            {
                LogHelper(@"Error trapped in Xslt Custom Extension FormatNumber(""{0}"", ""{1}"")".FormatArgs(input, format), exc);
                //LogHelper("Error formatting Number value [{0}] into specified format [{1}].", exc, input.ToString(), format);
            }

            return formattedOutput;
        }

        public double DateDiff(string startDate, string endDate, string diffUnitsMode)
        {
            String diffMode = diffUnitsMode.ToLower();
            TimeSpan diffTimeSpan = DateDiffHelper(startDate, endDate);

            if (diffMode == "milliseconds" || diffMode == "ms")
            {
                return diffTimeSpan.TotalMilliseconds;
            }
            else if (diffMode == "seconds" || diffMode == "s")
            {
                return diffTimeSpan.TotalSeconds;
            }
            else if (diffMode == "minutes" || diffMode == "m")
            {
                return diffTimeSpan.TotalMinutes;
            }
            else if (diffMode == "hours" || diffMode == "h")
            {
                return diffTimeSpan.TotalHours;
            }
            else if (diffMode == "days" || diffMode == "d")
            {
                return diffTimeSpan.TotalDays;
            }

            throw new ArgumentException("The date difference units mode specified was not valid; a valid units mode must be specified (i.e. Days, Hours, Minutes, Seconds, Milliseconds).");
        }

        public double DateDiffDays(string startDate, string endDate)
        {
            return DateDiff(startDate, endDate, "days");
        }

        public double DateDiffSeconds(string startDate, string endDate)
        {
            return DateDiff(startDate, endDate, "seconds");
        }

        public string DateNow()
        {
            return DateTime.Now.ToString();
        }

        public string DateNow(string format)
        {
            return DateNow(format, null, null);
        }

        public string DateNow(string format, string countryCode, string languageCode)
        {
            return FormatDateHelper(DateTime.Now, format, countryCode, languageCode);
        }

        /* Added "RaiseException" on 21st May 2012 */
        /* Brandon & Ryan */
        public void RaiseException(string errorMessage)
        {
            throw new XsltCustomExtensionException(!String.IsNullOrEmpty(errorMessage) ? errorMessage : "XSLT Custom Exception: Detailed Error Undefined.");
        }

        public void RaiseException(string errorMessage, XPathNavigator inputNodeSet)
        {
            //Note:  The custom extension method will convert legacy XPathNavigator to XElement using optimized in-memory conversion without any re-parsing.
            throw new XsltCustomExtensionException(!String.IsNullOrEmpty(errorMessage) ? errorMessage : "XSLT Custom Exception: Detailed Error Undefined.", inputNodeSet.ToXElement());
        }

        #endregion
    }

    public class XsltCustomExtensionException : Exception
    {
        public XsltCustomExtensionException(String errorMessage)
            : this(errorMessage, null)
        {
        }

        public XsltCustomExtensionException(String errorMessage, XElement errorXml)
            : base(errorMessage)
        {
            this.ErrorXml = errorXml;
        }

        public XElement ErrorXml { get; protected set; }

        public override string ToString()
        {
            String errorMessage = String.Empty;
            if(this.ErrorXml != null)
            {
                errorMessage = "XSLT Custom Exception: {0}. \n\nXML: {1}".FormatArgs(this.Message, this.ErrorXml);
            }
            else
            {
                errorMessage = "XSLT Custom Exception: {0}".FormatArgs(this.Message);
            }
            return errorMessage;
        }
    }
}
