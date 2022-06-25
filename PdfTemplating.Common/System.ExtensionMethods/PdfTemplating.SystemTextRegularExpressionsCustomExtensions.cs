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
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using PdfTemplating.SystemCustomExtensions;

namespace PdfTemplating.SystemTextRegularExpressionsCustomExtensions
{
    public static class RegExPatterns
    {
        //General RegEx Patterns
        public const string WebUrl = @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$";
        public const string GUID = @"^[({]?(0x)?[0-9a-fA-F]{8}([-,]?(0x)?[0-9a-fA-F]{4}){2}((-?[0-9a-fA-F]{4}-?[0-9a-fA-F]{12})|(,\{0x[0-9a-fA-F]{2}(,0x[0-9a-fA-F]{2}){7}\}))[)}]?$";
        public const string PhoneNumber = @"\(?(?<AreaCode>[0-9]{3})\)?[-. ]?(?<Exchange>[0-9]{3})[-. ]*?(?<Suffix>[0-9]{4})[-. x]?(?<Extension>[0-9]*)";
        public const string Email = @"^(\w[-._\w]*\w@\w[-._\w]*\w\.\w{2,3})$";
        public const string ZIPCode = @"^[0-9]{5}([- /]?[0-9]{4})?$";
        public const string NumericOnly = @"^([0-9])*$";
        public const string AnyText = @"^.+";
        public const string BeginningWhitespace = @"^\s*";
        public const string EndingWhitespace = @"\s*$";
        public const string CurrencyPositiveOnly = @"^\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$";
        public const string CurrencyPositiveNumericOnly = @"^([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$";
        public const string Date = @"^(((0?[1-9]|1[012])/(0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])/(29|30)|(0?[13578]|1[02])/31)/(19|[2-9]\d)\d{2}|0?2/29/((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00)))$";
    }

    public static class RegularExpressionsCustomExtensions
    {
        public static string ReplaceFromGroup(this Regex rxThis, string input, string groupNameForReplacement)
        {
            int groupNumber = rxThis.GroupNumberFromName(groupNameForReplacement);
            return rxThis.ReplaceFromGroup(input, groupNumber);
        }

        public static string ReplaceFromGroup(this Regex rxThis, string input, int groupNumberForReplacement)
        {
            string output = input;
            MatchCollection colMatches = rxThis.Matches(input);

            var lstReplacements = from Match objMatch in colMatches
                                  select new
                                  {
                                      FindValue = objMatch.Value,
                                      ReplaceValue = objMatch.Groups[groupNumberForReplacement].Value
                                  };

            foreach (var replaceItem in lstReplacements)
            {
                output = output.Replace(replaceItem.FindValue, replaceItem.ReplaceValue);
            }

            return output;
        }

        public static XElement MatchesAsXml(this Regex rxThis, string input)
        {
            int intGroupCounter = 0;
            var groupNumbers = rxThis.GetGroupNumbers();

            var xResults = new XElement("Matches",
                              from Match rxMatch in rxThis.Matches(input)
                              select new XElement("Match", 
                                      new XElement("Value", rxMatch.Value),
                                      new XAttribute("Success", rxMatch.Success),
                                      new XAttribute("Index", rxMatch.Index),
                                      new XAttribute("Length", rxMatch.Length),
                                  //NOTE: Top Level Captures Group ONLY contains data on the Last Capture
                                  //      which is equivalent to the Value property above so there is no need
                                  //      to process again!
                                  //new XElement("Captures",
                                  //      from Capture rxCapture in rxMatch.Captures
                                  //      select new XElement("Capture",
                                  //                  new XElement("Index", rxCapture.Index),
                                  //                  new XElement("Length", rxCapture.Length),
                                  //                  new XElement("Value", rxCapture.Value)
                                  //  )),
                                  new XElement("Groups",
                                        //from Group rxGroup in rxMatch.Groups
                                        from groupNum in groupNumbers
                                        let rxGroup = rxMatch.Groups[groupNum]
                                        where rxGroup != null
                                        select new XElement("Group",
                                                    new XAttribute("Number", groupNum),
                                                    new XAttribute("Name", rxThis.GroupNameFromNumber(groupNum)),
                                                    new XAttribute("Success", rxGroup.Success),
                                                    new XAttribute("Index", rxGroup.Index),
                                                    new XAttribute("Counter", intGroupCounter++),
                                                    new XAttribute("Length", rxGroup.Length),
                                                    new XElement("Value", rxGroup.Value),
                                                    new XElement("Captures",
                                                        from Capture rxCapture in rxGroup.Captures
                                                        select new XElement("Capture",
                                                                    new XAttribute("Index", rxCapture.Index),
                                                                    new XAttribute("Length", rxCapture.Length),
                                                                    new XElement("Value", rxCapture.Value)
                                                    ))
                                    ))
                            ));

            return xResults;
        }
    }

    public static class StringRegexCustomExtensions
    {
        /// <summary>
        /// Quickly escape Regex characters for the selected string with extension method for easier to read and concise code.
        /// </summary>
        /// <param name="objThis"></param>
        /// <returns></returns>
        public static String EscapeRegex(this String objThis)
        {
            //Note:  We should always handle null cases in Extension Methods!
            if (objThis.IsNullOrEmpty()) return String.Empty;

            return Regex.Escape(objThis);
        }

        /// <summary>
        /// Quickly convert the current String to A Regular Expression.
        /// </summary>
        /// <param name="objThis"></param>
        /// <returns></returns>
        public static Regex ToRegex(this String objThis)
        {
            //Note:  We should always handle null cases in Extension Methods!
            if (objThis.IsNullOrEmpty()) return null;

            return new Regex(objThis, RegexOptions.Compiled);
        }

        /// <summary>
        /// Quickly convert the current String to A Regular Expression with specific options.
        /// </summary>
        /// <param name="objThis"></param>
        /// <param name="rxOptions"></param>
        /// <returns></returns>
        public static Regex ToRegex(this String objThis, RegexOptions rxOptions)
        {
            //Note:  We should always handle null cases in Extension Methods!
            if (objThis.IsNullOrEmpty()) return null;

            return new Regex(objThis, rxOptions);
        }

        /// <summary>
        /// Prepare a String with escape sequence processing for enhanced use of custom parsing implementations.
        /// </summary>
        /// <param name="objThis"></param>
        /// <param name="escapeSymbol"></param>
        /// <returns></returns>
        public static String RegexPreparePatternWithEscapeSequence(this String objThis, String escapeSymbol)
        {
            //Note:  We should always handle null cases in Extension Methods!
            if (objThis.IsNullOrEmpty()) return String.Empty;

            String nonEscapedTextRxFormatString = @"(?<!" + escapeSymbol.EscapeRegex() + @"){0}";
            String preparedRxPattern = String.Format(nonEscapedTextRxFormatString, objThis);
            return preparedRxPattern;
        }

        /// <summary>
        /// Process a String with escape sequence processing for enhanced use of custom parsing implementations.

        /// Note:   This process is designed optimally to use a single Cleanup RegEx pattern regardless of the delimiter 
        ///	        and includes a match for the Escape character itself so that "\abc" becomes "abc",  "d\ef" becomes "def",
        ///	        "\" becomes "", and an escaped escape-char "\\" becomes "\", and "x\\yz" becomes "x\yz".
        /// </summary>
        /// <param name="objThis"></param>
        /// <param name="escapeSymbol"></param>
        /// <returns></returns>
        public static String RegexProcessEscapeSequences(this String objThis, String escapeSymbol)
        {
            //Note:  We should always handle null cases in Extension Methods!
            if (objThis.IsNullOrEmpty()) return String.Empty;

            //Note:  This pattern is designed as a single Cleanup pattern regardless of the delimiter and includes a match for the Escape character itself
            //		 so that "\abc" becomes "abc",  "\" becomes "", and an escaped escape-char "\\" becomes "\"
            //Note:  Sample Expression for processing Escape Sequences: (?<EscapeSeq>\\)(?<EscapedVal>.{0,1})
            string strEscapeExpression = @"(?<EscapeSeq>" + escapeSymbol.EscapeRegex() + ")(?<EscapedVal>.{0,1})";
            Regex rxReplaceEscapedChars = strEscapeExpression.ToRegex();
            return rxReplaceEscapedChars.ReplaceFromGroup(objThis, "EscapedVal");
        }
    }
}
