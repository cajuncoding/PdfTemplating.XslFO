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
using System.Collections.Generic;
using System.Linq;
using System.Linq.CustomExtensions;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.CustomExtensions;
using System.IO;
using System.IO.CustomExtensions;
using System.Security;
using System.Xml;
using System.Xml.Linq;
using System.Web;
//using System.Collections.CustomExtensions;
using System.CustomExtensions;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq.CustomExtensions;

namespace System.CustomExtensions
{

	public static class SystemObjectCustomExtensions
	{
		/// <summary>
		/// Chainable 'as' conversion for use within Linq Chains.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		/// <returns></returns>
		public static T As<T>(this object item) where T : class
		{
			return item as T;
		}

		/// <summary>
		/// Chainable tenerary (if null return default) conversion for use within Linq Chains and for enhanced code readability.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		/// <param name="defaultItem"></param>
		/// <returns></returns>
		public static T OrDefault<T>(this object item, T defaultItem) where T : class
		{
			return (item ?? defaultItem).As<T>();
		}

		/// <summary>
		/// Chainable terenary null case handler conversion for use within Linq Chains and for enhanced code readability (ie. if null return default).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		/// <param name="defaultItem"></param>
		/// <returns></returns>
		public static T OrDefault<T>(this object item, Func<T> fnDefaultItemGetter) where T : class
		{
			//MUST make sure we do NOT execute the Getter Func unless necessary!
			return (item == null ? fnDefaultItemGetter() : item).As<T>();
		}

		/// <summary>
		/// Execute the action on the object; useful for mass property updates of objects (i.e. VB With Command).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="act"></param>
		public static T With<T>(this T obj, Action<T> act)
		{
			act(obj);
			return obj;
		}

		/// <summary>
		/// Project/Map the current object into a new form using Lambda converter Function.  Benefitial when standard
		/// anonymous type instantiation isn't supported, such as within Linq Chains and Queries.
		/// 
		/// See Also ProjectTo()
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="R"></typeparam>
		/// <param name="input"></param>
		/// <param name="fn"></param>
		/// <returns>Result of Function Converter supporting anonymous types.</returns>
		public static R Map<T, R>(this T input, Func<T, R> fn)
		{
			return fn(input);
		}

		/// <summary>
		/// Project/Map the current object into a new form using Lambda converter Function.  Benefitial when standard
		/// anonymous type instantiation isn't supported, such as within Linq Chains and Queries.
		/// 
		/// See Also Map()
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="R"></typeparam>
		/// <param name="input"></param>
		/// <param name="fn"></param>
		/// <returns>Result of Function Converter supporting anonymous types.</returns>
		public static R ProjectTo<T, R>(this T input, Func<T, R> fn)
		{
			return input.Map(fn);
		}

		/// <summary>
		/// Safely disposes of Any IDisposable, eliminating unnecessary null checks.
		/// </summary>
		/// <param name="input"></param>
		public static void DisposeSafely(this IDisposable input)
		{
			if (input != null) input.Dispose();
		}


		///// <summary>
		///// Copy/Merges the properties of an Object into this one where the Property Name and Type are a match.
		///// NOTE:  This uses reflection and therefore will have a performance penalty if heavily utilized.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="target"></param>
		///// <param name="source"></param>
		//public static TTarget MergeProperties<TTarget, TSource>(this TTarget target, TSource source) where TTarget : class where TSource: class
		//{
		//    Type targetType = typeof (TTarget);
		//    Type sourceType = typeof (TSource);

		//    var targetProps = targetType.GetProperties().Where(p => p.CanRead && p.CanWrite);
		//    var targetPropDictionary = targetProps.ToDictionary(p => p.PropertyType);
		//    var sourceProps = sourceType.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

		//    foreach (var prop in sourceProps)
		//    {
		//        var value = prop.GetValue(source, null);
		//        if (value != null)
		//            prop.SetValue(target, value, null);
		//    }
		//}

	}

	public static class SystemTypeExtensions
	{
		public static object GetDefault(this Type type)
		{
			object output = null;

			if (type.IsValueType)
			{
				output = Activator.CreateInstance(type);
			}

			return output;
		}
	}

	public static class SystemExceptionCustomExtensions
	{
		/// <summary>
		/// Retrieve a string containing ALL descendent Exception Messages using the default format of "{message}; [InnerException] {inner_message}; [InnerException] {innerException]...
		/// It traversing all nested exceptions as necessary.
		/// </summary>
		/// <param name="objThis"></param>
		/// <returns></returns>
		public static String GetMessages(this Exception objThis)
		{
			return objThis.GetMessages("[{0}] {1}", ";");
		}

		/// <summary>
		/// Retrieve a string containing ALL descendent Exception Messages using the specified format string; traversing all nested exceptions as necessary.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="exceptionFormatString"></param>
		/// <returns></returns>
		public static String GetMessages(this Exception objThis, String exceptionFormatString, String delimiter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Exception excInner = null;
			if (objThis != null)
			{
				stringBuilder.AppendFormat(exceptionFormatString, objThis.GetType().Name, objThis.Message);
				
				//Traverse all InnerExceptions
				exceptionFormatString = String.Concat(delimiter, " ", exceptionFormatString);
				excInner = objThis.InnerException;
				while (excInner != null)
				{
					stringBuilder.AppendFormat(exceptionFormatString, excInner.GetType().Name, excInner.Message);
					excInner = excInner.InnerException;
				}

				//Handle New .Net 4.0 Aggregate Exception Type as a special Case because
				//AggregateExceptions contain a list of Exceptions thrown by background threads.
				if (objThis is AggregateException)
				{
					foreach (var exc in ((AggregateException)objThis).InnerExceptions)
					{
						stringBuilder.AppendFormat(exceptionFormatString, exc.GetType().Name, exc.Message);
					}
				}
			}

			return stringBuilder.ToString();
		}
	}

	public static class SystemStringCustomExtensions
	{
		/// <summary>
		/// Strip out all new line/carriage returns from the specified string.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static String StripNewLines(this String value)
		{
			return value.Replace(Environment.NewLine, String.Empty);
		}

		/// <summary>
		/// Strips all NON-alphabetic/word characters from the string.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static String StripNonAlphabeticCharacters(this String value) {
			return Regex.Replace(value, @"[^\w]", "");
		}

		/// <summary>
		/// Strips all NON-numeric/digit characters from the string.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static String StripNonNumericCharacters(this String value)
		{
			//NOTE:  For rational numbers we need to also keep decimal separators.
			return Regex.Replace(value, @"[^.\d]", "");
		}

		/// <summary>
		/// Strips all NON-alphanumeric, word or digit, characters from the string.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static String StripNonAlphaNumericCharacters(this String value)
		{
			//NOTE:  periods for digits are also matched by \w
			return Regex.Replace(value, @"[^\w\d]", "");
		}

		/// <summary>
		/// Easily repeat any string any number of times.  This extension method enables chaining and code that is much more concise.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static String Repeat(this char charValue, int count)
		{
			return new String(charValue, count);
		}

		/// <summary>
		/// Easily repeat any string any number of times.  This extension method enables chaining and code that is much more concise.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static String Repeat(this String value, int count)
		{
			return new StringBuilder().Insert(0, value, count).ToString();
		}

		/// <summary>
		/// Trim a string using the specified string of characters as the chars to trim from the beginning and end.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="chars"></param>
		/// <returns></returns>
		public static String Trim(this String objThis, String chars)
		{
			return objThis.Trim(chars.ToCharArray());
		}

		/// <summary>
		/// Trim a string using the specified string of characters as the chars to trim from the beginning.  Each character
		/// in the specified string will be treated as an individual character to remove.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="chars"></param>
		/// <returns></returns>
		public static String TrimStart(this String objThis, String chars)
		{
			return objThis.TrimStart(chars.ToCharArray());
		}

		/// <summary>
		/// Trim a string using the specified string of characters as the chars to trim from the end.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="chars"></param>
		/// <returns></returns>
		public static String TrimEnd(this String objThis, String chars)
		{
			return objThis.TrimEnd(chars.ToCharArray());
		}

		/// <summary>
		/// Trim a string using the specified string of characters as the chars to trim from the beginning and end.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="chars"></param>
		/// <returns></returns>
		public static String TrimWord(this String objThis, String word, bool bCaseSensitive)
		{
			return objThis.TrimWordStart(word, bCaseSensitive).TrimWordEnd(word, bCaseSensitive);
		}

		/// <summary>
		/// Trim a string using the specified word to trim from the beginning.  All instances of the word
		/// are removed from the beginning of the string.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="chars"></param>
		/// <returns></returns>
		public static String TrimWordStart(this String objThis, String word, bool bCaseSensitive)
		{
			var find = word.EscapeRegex();
			var trimStartExpression = "^({0})*".FormatArgs(find);
			var rxTrimStart = trimStartExpression.ToRegex(bCaseSensitive ? (RegexOptions.Compiled | RegexOptions.IgnoreCase) : RegexOptions.Compiled);
			return rxTrimStart.Replace(objThis, String.Empty);
		}

		/// <summary>
		/// Trim a string using the specified string of characters as the chars to trim from the end.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="word"></param>
		/// <returns></returns>
		public static String TrimWordEnd(this String objThis, String word, bool bCaseSensitive)
		{
			var find = word.EscapeRegex();
			var trimEndExpression = "({0})*$".FormatArgs(find);
			var rxTrimEnd = trimEndExpression.ToRegex(bCaseSensitive ? (RegexOptions.Compiled | RegexOptions.IgnoreCase) : RegexOptions.Compiled);
			return rxTrimEnd.Replace(objThis, String.Empty);
		}

		/// <summary>
		/// Provides case insensitive replacement functionality for String.Replace(); delegates to .Net Regex for actual replacement.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="find"></param>
		/// <param name="replace"></param>
		/// <returns></returns>
		public static String ReplaceCaseInsenstive(this String objThis, String find, String replace) {
			return Regex.Replace(objThis, find.EscapeRegex(), replace, RegexOptions.IgnoreCase);
		}

		/// <summary>
		/// Append the specified Text to the string; improves chained code readability.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public static String Append(this String objThis, String text)
		{
			return objThis + text;
		}

		/// <summary>
		/// Safely appends the specified text to the string by Trimming it first to make sure that it is not appended in duplication;
		/// perfect for building name with delimiters / special conventions.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public static String AppendSafely(this String objThis, String text)
		{
			var trimmed = objThis.TrimWordEnd(text, true);
			return objThis.Append(trimmed);
		}

		/// <summary>
		/// Prepend the specified Text to the string; improves chained code readability.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public static String Prepend(this String objThis, String text)
		{
			return text + objThis;
		}

		/// <summary>
		/// Safely appends the specified text to the string by Trimming it first to make sure that it is not appended in duplication;
		/// perfect for building name with delimiters / special conventions.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public static String PrependSafely(this String objThis, String text)
		{
			objThis.TrimWordStart(text, true);
			return objThis.Prepend(text);
		}

		/// <summary>
		/// Determine if a string is numeric or not
		/// </summary>
		/// <param name="objThis"></param>
		/// <returns></returns>
		public static bool IsNumeric(this String objThis)
		{
			decimal isNumber = 0;
			return decimal.TryParse(objThis, out isNumber);
		}

		/// <summary>
		/// Quickly format the current string as a format string with the specified aruments param array.  
		/// This extension method provides for chainability as well as code that is easier to read and more concise.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public static String FormatArgs(this String objThis, params object[] arguments)
		{
			//Note:  We should always handle null cases in Extension Methods!
			if (objThis.IsNullOrEmpty()) return String.Empty;

			return String.Format(objThis, arguments);
		}

		/// <summary>
		/// Returns true if the referenced string is either a null or an empty string (i.e. "").
		/// Exactly the same as String.IsNullOrEmpty(string1).
		/// </summary>
		/// <param name="objThis"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty(this String objThis)
		{
			return String.IsNullOrEmpty(objThis);
		}

		public static String ToNullSafe(this String objThis)
		{
			return objThis.ToNullSafe(String.Empty);
		}

		public static String ToNullSafe(this String objThis, String defaultIfNullOrEmpty)
		{
			if (String.IsNullOrEmpty(objThis))
			{
				return defaultIfNullOrEmpty;
			}
			return objThis;
		}

		public static T ToNullSafe<T>(this T objThis, T defaultIfNullOrEmpty)
		{
			if (objThis == null)
			{
				return default(T);
			}
			return objThis;
		}

		public static String Left(this String objThis, int lengthFromLeft)
		{
			//Check if the value is valid
			if (objThis.IsNullOrEmpty())
			{
				return String.Empty;
			}
			else if (objThis.Length > lengthFromLeft)
			{
				//Make the string no longer than the max length from the Right.
				return objThis.Substring(0, lengthFromLeft);
			}

			//Return the string in all other cases
			return objThis;
		}

		public static String Right(this String objThis, int lengthFromRight)
		{
			//Check if the value is valid
			if (objThis.IsNullOrEmpty())
			{
				return String.Empty;
			}
			else if (objThis.Length > lengthFromRight)
			{
				//Make the string no longer than the max length from the Right.
				return objThis.Substring(objThis.Length - lengthFromRight, lengthFromRight);
			}

			//Return the string in all other cases
			return objThis;
		}

		public static String RemoveFromLeft(this String objThis, int lengthFromLeftToRemove)
		{
			//If the remove length is greater than the string length then return an Empty string
			//becaue ALL characters shoudl be removed. Otherwise process the string.
			if (lengthFromLeftToRemove > objThis.Length)
			{
				return String.Empty;
			}
			else
			{
				return objThis.Right(objThis.Length - lengthFromLeftToRemove);
			}
		}

		public static String RemoveFromRight(this String objThis, int lengthFromRightToRemove)
		{
			//If the remove length is greater than the string length then return an Empty string
			//becaue ALL characters should be removed. Otherwise process the string.
			if (lengthFromRightToRemove > objThis.Length)
			{
				return String.Empty;
			}
			else
			{
				return objThis.Left(objThis.Length - lengthFromRightToRemove);
			} 
		}

		/// <summary>
		/// Compares two strings, using current culture sort rules, while ignoring case in the comparison.
		/// Exactly the same as string1.Equals(string2, StringComparison.CurrentCultureIgnoreCase).
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="stringToCompareWith"></param>
		/// <returns></returns>
		public static bool EqualsIgnoreCase(this String objThis, String stringToCompareWith)
		{
			return objThis.Equals(stringToCompareWith, StringComparison.CurrentCultureIgnoreCase);
		}

		/// <summary>
		/// Compares two strings, using invariant culture sort rules, while ignoring case in the comparison.
		/// Exactly the same as string1.Equals(string2, StringComparison.InvariantCultureIgnoreCase).
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="stringToCompareWith"></param>
		/// <returns></returns>
		public static bool EqualsIgnoreCaseInvariant(this String objThis, String stringToCompareWith)
		{
			return objThis.Equals(stringToCompareWith, StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>
		/// Single String Item reverse contains lookup to find if an String item is within a list with case sensitivity (case sensitive).  ONLY Supports String Comparisons and will use ToString() methods
		/// to render the object into it's String form for comparison.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static bool IsIn(this String item, params String[] list)
		{
			if (item == null) throw new ArgumentNullException("String is null; Extension method String.IsIn() cannot search for a null string.");
			return item.IsIn(StringComparer.InvariantCulture, list);
		}

		/// <summary>
		/// Single String Item reverse contains lookup to find if an String item is within a list ignoring case sensitivity (case insensitive).  ONLY Supports String Comparisons and will use ToString() methods
		/// to render the object into it's String form for comparison.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static bool IsInIgnoreCase(this String item, params String[] list)
		{
			if (item == null) throw new ArgumentNullException("String is null; Extension method String.IsInIgnoreCase() cannot search for a null string.");
			return item.IsIn(StringComparer.InvariantCultureIgnoreCase, list);
		}

		/// <summary>
		/// Convert the string into a byte array with the default UTF8 encoding.
		/// </summary>
		/// <param name="objThis"></param>
		/// <returns></returns>
		public static byte[] GetBytes(this String objThis)
		{
			return objThis.GetBytes(Encoding.UTF8);
		}

		/// <summary>
		/// Convert the string into a byte array using the specified Encoding.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="objEncoder"></param>
		/// <returns></returns>
		public static byte[] GetBytes(this String objThis, Encoding objEncoder)
		{
			return objEncoder.GetBytes(objThis);
		}

		/// <summary>
		/// Convert the string to a MemoryStream using default UTF8 encoding.
		/// </summary>
		/// <param name="objThis"></param>
		/// <returns></returns>
		public static MemoryStream ToStream(this String objThis)
		{
			return objThis.ToStream(Encoding.UTF8);
		}

		/// <summary>
		/// Convert the string to a writeable MemoryStream using the specified Encoder.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="objEncoder"></param>
		/// <returns></returns>
		public static MemoryStream ToStream(this String objThis, Encoding objEncoder)
		{
			return objThis.ToStream(objEncoder, true);
		}

		/// <summary>
		/// Convert the string to a MemoryStream using the specified Encoding and Writeable flag.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="objEncoder"></param>
		/// <param name="bWritable"></param>
		/// <returns></returns>
		public static MemoryStream ToStream(this String objThis, Encoding objEncoder, Boolean bWritable)
		{
			return new MemoryStream(objThis.GetBytes(objEncoder), bWritable);
		}

		/// <summary>
		/// Encode the string into Base64 with default UTF8 Encoding.
		/// </summary>
		/// <param name="objThis"></param>
		/// <returns></returns>
		public static String ToBase64(this String objThis)
		{
			byte[] bytes = objThis.GetBytes(Encoding.UTF8);
			return Convert.ToBase64String(bytes);
		}


		/// <summary>
		/// Encode the string into Base64 with the specified Encoding.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static String ToBase64(this String objThis, Encoding encoding)
		{
			byte[] byteArray = objThis.GetBytes(encoding);
			return Convert.ToBase64String(byteArray);
		}

		/// <summary>
		/// Decode the string from Base64 with default UTF8 Encoding.
		/// </summary>
		/// <param name="objThis"></param>
		/// <returns></returns>
		public static String FromBase64(this String objThis)
		{
			return FromBase64(objThis, Encoding.UTF8);
		}

		/// <summary>
		/// Decode the string from Base64 with the specified encoding.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static String FromBase64(this String objThis, Encoding encoding)
		{
			byte[] byteArray = Convert.FromBase64String(objThis);
			return encoding.GetString(byteArray);
		}

		/// <summary>
		/// Encode the string into a valid Xml escaped value.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static String ToXmlEncoded(this String objThis)
		{
			//StringBuilder stringBuilder = new StringBuilder();
			//using (XmlWriter xmlWriter = XmlTextWriter.Create(stringBuilder))
			//{
			//    xmlWriter.WriteRaw(objThis);
			//    xmlWriter.Close();
			//}
			//String temp = stringBuilder.ToString();

			//Note:  MUCH more  simple manner without the overhead of IDisposable XmlWriter objects:
			return SecurityElement.Escape(objThis);
		}

		/// <summary>
		/// Decode the string from a valid Xml escaped value.  NOTE:  This uses true valid Xml processing via .Net Xml Reader API, NOT
		/// a hack workaround with invalid cases such as HttpUtility.HtmlDecode().
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static String ToXmlDecoded(this String objThis)
		{
			var decodedResult = String.Empty;
			using (var stream = objThis.ToStream())
			using (var xmlReader = stream.ToXmlReader(new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment }))
			{
				xmlReader.MoveToContent();
				decodedResult = xmlReader.ReadString();
			}
			return decodedResult;
		}

		/// <summary>
		/// Encode the string into a valid Html encoded value.
		/// Also handles known issues that exist in .NET 3.5 and earlier for Apostrophe and characters outside normal ASCII code range.
		/// 
		/// NOTE:  THIS IS UPDATED IN .NET 4.0 SO SOME CODE MAY BECOME UNNECESSARY.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static String ToHtmlEncoded(this String objThis)
		{
			//First, Use core .Net API for what is can process
			String dotNetEncodedValue = HttpUtility.HtmlEncode(objThis);
			
			//NOTE:  Due to known issues with the .Net HttpUtility.EncodeHtml() method not encoding
			//       some characters in .Net 3.5 and below we must manually process all remaining
			//       characters below after using the out of the box .NET API.
			//NOTE:  THIS IS FIXED IN .NET 4.0 AND MAY BE REMOVED IN THE FUTURE
			StringBuilder sb = new StringBuilder(dotNetEncodedValue.Length);

			//NOTE:  For MAXIMUM iteration performance (other than using "unsafe CLI direct pointers") we use String
			//       indexers for access to characters instead of "foreach (c in string)" iteration, 
			//       which yields roughly 60%+ speed boost.
			//foreach(char c in chars)
			char c;
			int intChar;
			for (int i = 0, len = dotNetEncodedValue.Length; i < len; i++)
			{
				c = dotNetEncodedValue[i];
				intChar = (int)c;
				switch(c)
				{
					//Process &apos; (ie. Apostrophe) workaround
					case '\'':
						//Process Apostrophe's via HTML Workaround documented by W3C: http://www.w3.org/TR/xhtml1/#C_16
						sb.Append("&#").Append(intChar).Append(";");
						break;

					//Process all other characters
					default:
						//IF ANY character remaining is Above Normal ASCII code then Encode it manually into Decimal format.
						if (intChar > 127)
						{
							sb.Append("&#").Append(intChar).Append(";");
						}
						else
						{
							sb.Append(c);
						}
						break;
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Decode the string from a valid Html encoded value.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static String ToHtmlDecoded(this String objThis)
		{
			return HttpUtility.HtmlDecode(objThis);
		}

		/// <summary>
		/// Encode the string as a valid CSV encoded value accounting for double quotes 
		/// and doubling them up to correctly encode the value so that it can be wrapped by double quotes.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static String ToCsvEncoded(this String objThis)
		{
			return objThis.Replace(@"""", @"""""");
		}

		/// <summary>
		/// Encode the string into a valid Url escaped value.
		/// 
		/// NOTE:  This is not strictly RFC3986 compliant or Internationally compliant as it will
		/// also encode spaces as '+' characters (and perhaps other characters).  Therefore double decoding with these will
		/// not result in an invariant base decodings (i.e. ONLY Hex encodings parsed out).  
		/// 
		/// To Encode into ONLY valid Hex digits (i.e. ONLY Percent encoding) and support decoding into base invariant see the ToUrlEncodedStrict() method;
		/// which is more strictly compliant with RFC3986 guidelines -- http://tools.ietf.org/html/rfc3986.
		/// 
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static String ToUrlEncoded(this String objThis)
		{
			return HttpUtility.UrlEncode(objThis);
		}

		/// <summary>
		/// Determine if the specified string is Url Encoded using Strict Percent Encoding detection so
		/// that ONLY valid Hex encoded characters are detected in strict accordance with RFC3987 guidelines -- http://tools.ietf.org/html/rfc3987.
		/// </summary>
		/// <param name="objThis"></param>
		/// <returns></returns>
		private static Regex _rxStrictUrlEncodeMatch = @"%[0-9A-Fa-f]{2}".ToRegex(RegexOptions.Compiled);
		public static bool IsUrlEncodedStrict(this String objThis)
		{
			return _rxStrictUrlEncodeMatch.IsMatch(objThis);
		}


		/// <summary>
		/// The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.
		/// </summary>
		private static readonly string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };

		/// <summary>
		/// Encode the string into a valid Url escaped value according to the Strict rules defined in RFC3986.
		/// 
		/// NOTE:  This is RFC3986 compliant (http://tools.ietf.org/html/rfc3986) and it will only encode characters necessary into true valid percent encoded hex values.
		///        Thereforem, even if you double encode, the consumer may still double decode which with will result in invariant base decodings 
		///        (i.e. ONLY Hex encodings parsed out until none are left).  
		/// 
		/// NOTE: A distinct advantage to using this Strict Implementation is the ability to consistently determine if the string is already encoded or not
		///       without the concern of varying impelmentations for characters (ie. spaces -> +, etc.)
		///
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static String ToUrlEncodedStrict(this String objThis)
		{
			if (!objThis.IsNullOrEmpty())
			{
				//Note: Start with RFC 2396 escaping by calling the .NET method to do the work.
				//      This MAY sometimes exhibit RFC 3986 behavior (according to the documentation).
				//      If it does, the escaping we do that follows it will be a no-op since the
				//      characters we search for to replace can't possibly exist in the string.
				//Note: By testing this I have found that there is a String limit (ie. 2000 chars) for which the
				//      Uri.EscapeDataString() works with.  Therefore we must BLOCk out Large Strings
				//      manually so that we don't have to implement a custom encoding algorithm.
				//OLD Quick Code: 
				//StringBuilder sb = new StringBuilder(Uri.EscapeDataString(objThis));

				int uriLengthLimit = 2000;
				StringBuilder sb = new StringBuilder();
				int loopCount = objThis.Length / uriLengthLimit;
				for (int i = 0; i <= loopCount; i++)
				{
					if (i < loopCount)
					{
						sb.Append(Uri.EscapeDataString(objThis.Substring(uriLengthLimit * i, uriLengthLimit)));
					}
					else
					{
						sb.Append(Uri.EscapeDataString(objThis.Substring(uriLengthLimit * i)));
					}
				}

				//Now we need to complete the process by Upgrading the escaping to RFC 3986, if necessary.
				//Note:  The StringBuilder replace function will ensure our loop is optimized.
				for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
				{
					sb.Replace(UriRfc3986CharsToEscape[i], Uri.HexEscape(UriRfc3986CharsToEscape[i][0]));
				}

				// Return the fully-RFC3986-escaped string.
				return sb.ToString();
			}

			return String.Empty;
		}

		/// <summary>
		/// Decode the string into a valid Url escaped value.
		/// 
		/// NOTE:  This is not strictly RFC3986 compliant or Internationally compliant as it will
		/// also decode '+' characters as spaces (and perhaps other characters).  Therefore double decoding with these will
		/// not result in an invariant base decodings (i.e. ONLY Hex encodings parsed out).  
		/// 
		/// To Decode into ONLY valid Hex digits (i.e. ONLY Percent decoding) and support decoding into base invariant see the ToUrlDecodedStrict() method;
		/// which is more strictly compliant with RFC3986 guidelines -- http://tools.ietf.org/html/rfc3986.
		/// 
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static String ToUrlDecoded(this String objThis)
		{
			//NOTE:  This is not 100% W3C compliant as it will also decode '+' as spaces (and perhaps other characters).  
			//       Therefore double decoding with these will not result in an invariant. 
			//       To decode ONLY valid Hex digits and support decoding into base invariant see the ToUrlDecodedInvariant() method.
			return HttpUtility.UrlDecode(objThis);
		}

		/// <summary>
		/// Decode the string from a valid Url escaped value.
		/// 
		/// NOTE:  This is RFC3986 compliant (http://tools.ietf.org/html/rfc3986) and it will only decode characters necessary from true valid percent encoded hex values.
		/// Thereforem if you double decoding will result in invariant base decodings (i.e. ONLY Hex encodings parsed out until none are left).  
		/// 
		/// NOTE:  No special case handling is necessary because the UnescapeDataString will alreayd restore ANY valid true percent encoded characters found,
		///        even those that are not actually created/encoded correctly by the EscapeDataString() method -- see ToUrlEncodedStrict() method for more information.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static string ToUrlDecodedStrict(this String objThis)
		{
			//NOTE:  No special case handling is necessary because the UnescapeDataString will already restore ANY valid true percent encoded characters found,
			//       even those that are not actually created/encoded correctly by the EscapeDataString() method -- see ToUrlEncodedStrict() method for more information.
			return Uri.UnescapeDataString(objThis);
		}

		/// <summary>
		/// Decode the string from a valid Url escaped value safely, with auto-detection for if the String is currently encoded.
		/// 
		/// NOTE:  This is RFC3986 compliant (http://tools.ietf.org/html/rfc3986) and it will only decode characters necessary from true valid percent encoded hex values.
		/// Thereforem if you double decoding will result in invariant base decodings (i.e. ONLY Hex encodings parsed out until none are left).  
		/// 
		/// NOTE:  No special case handling is necessary because the UnescapeDataString will alreayd restore ANY valid true percent encoded characters found,
		///        even those that are not actually created/encoded correctly by the EscapeDataString() method -- see ToUrlEncodedStrict() method for more information.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static string ToUrlDecodedStrictSafely(this String objThis)
		{
			//NOTE:  No special case handling is necessary because the UnescapeDataString will alreayd restore ANY valid true percent encoded characters found,
			//       even those that are not actually created/encoded correctly by the EscapeDataString() method -- see ToUrlEncodedStrict() method for more information.
			return objThis.IsUrlEncodedStrict() ? objThis.ToUrlDecodedStrict() : objThis;
		}
		
		static readonly IDictionary<String, Char> invalidFileNameChars = Path.GetInvalidFileNameChars().ToDictionary(charTemp => charTemp.ToString());
		public static string ToFileSystemSafeString(this String objThis)
		{
			return new String(objThis.Where(charTemp => !invalidFileNameChars.ContainsKey(charTemp.ToString())).ToArray());
		}

		public static string ToFileSystemSafeString(this String objThis, IDictionary<char, char> charReplacementMapping)
		{
			//Note:  If a Character Map is specified then we FIRST apply the Map and THEN we remove invalid
			//       filesystem characters so this is safe even if the Mapping provides invalid characters!
			//Note:  The Linq query is optimized for performance because the base filename string
			//       is only Looped 1 time!
			var sanitaryChars = (from charTemp in objThis.ToCharArray()
								 select charReplacementMapping.ContainsKey(charTemp) 
										? charReplacementMapping[charTemp] 
										: charTemp).ToArray();
			
			string sanitaryFileName = new String(sanitaryChars).ToFileSystemSafeString();

			return sanitaryFileName;
		}

		public static string ToFileSystemSafeString(this String objThis, Boolean bSwapSpacesWithUnderscore)
		{

			if (bSwapSpacesWithUnderscore)
			{
				IDictionary<char, char> charMap = new Dictionary<char, char>(1);
				charMap.Add(' ', '_');
				return objThis.ToFileSystemSafeString(charMap);
			}
			else
			{
				return objThis.ToFileSystemSafeString();
			}
		}

		// Parses this string into a Map of Key/Value pairs by using the given delimiter strings.  
		// This will return a map whereby the items are determined by the data separated by the itemPairDelimiter.
		// Each item is split by the keyValueDelimiter whereby the Key is derived from the specified Index/Ordinal position (Zero based),
		// with the first position as the default (i.e. Index=0).  And the Value is derived by any remaining data.
		// 
		// The values for the itemPairDelimiter and keyValueDelimiter can be treated as literals and included in the data if
		// they are escaped by a preceding backslash "\".
		// 
		// If the Key is taken from any index > 0 then the data is collapsed into a clean string, delimited by the itemPairDelimiter.
		// But with out any empty slots left over from the removal of the Key value; this allows further safe parsing of any remaining values as needed.

		public static Dictionary<String, String> TokenizeToDictionary(this String objThis, String itemPairDelimiter, String keyValueDelimiter)
		{
			return objThis.TokenizeToDictionary(itemPairDelimiter, keyValueDelimiter, false);
		}

		public static Dictionary<String, String> TokenizeToDictionary(this String objThis, String itemPairDelimiter, String keyValueDelimiter, Boolean stringIsUrlEncoded)
		{
			return objThis.TokenizeToDictionary(itemPairDelimiter, keyValueDelimiter, 0, stringIsUrlEncoded, false, false);
		}

		public static Dictionary<String, String> TokenizeToDictionary(this String objThis, String itemPairDelimiter, String keyValueDelimiter, int indexOfTokenForKey, Boolean stringIsUrlEncoded, Boolean keysAreUrlEncoded, Boolean valuesAreUrlEncoded)
		{
			return objThis.TokenizeToDictionary(itemPairDelimiter, keyValueDelimiter, 0, stringIsUrlEncoded, false, false, "\\");
		}

		public static Dictionary<String, String> TokenizeToDictionary(this String objThis, String itemPairDelimiter, String keyValueDelimiter, int indexOfTokenForKey, Boolean stringIsUrlEncoded, Boolean keysAreUrlEncoded, Boolean valuesAreUrlEncoded, String escapeSequenceString) 
		{
		
			if(String.IsNullOrEmpty(itemPairDelimiter) || String.IsNullOrEmpty(keyValueDelimiter)) {
				throw new ArgumentNullException("Delimiter is null or empty; An invalid delimiter argument was specified for either the itemPairDelimiter or keyValueDelimiter.");
			}
		
			if(indexOfTokenForKey < 0) {
				throw new ArgumentOutOfRangeException("Invalid index specified; an index of Zero or greater must be specified for the location of the \"Key\" token.");
			}
		
			//Note:  We initialize our Dictionary to return here so that our code is safe and does not return Null.
			//		 If no tokens are identified then we simply return an empty Dictionary for which the length property will be Zero.
			String strValue = objThis;
			Dictionary<String, String> keyValueMap = new Dictionary<String, String>();
		
			if(!String.IsNullOrEmpty(strValue)) {

				//First Split the string into Item or Key/Value pair tokens
				//Note:  We need to clone the STring value because we will Mutate it during our processing
				String stringToParse = stringIsUrlEncoded ? HttpUtility.UrlDecode(strValue, Encoding.UTF8) : strValue;
				
				//Now that the escaped sequences have been replaced we can split/tokenize and process the values
				//NOTE:  We use a negative Look-behind here to ensure that we match only Delimiters that are NOT preceded by a literal "\" character.
				Regex rxItemSplitter = itemPairDelimiter.RegexPreparePatternWithEscapeSequence(escapeSequenceString).ToRegex();
				Regex rxKeyValueSplitter = keyValueDelimiter.RegexPreparePatternWithEscapeSequence(escapeSequenceString).ToRegex();
				
				//NOTE:  We use a standard match here to ensure that we match only Delimiters that ARE escaped by a preceding literal "\" character.
				//		 but the match will include (i.e. not a zero length assertion) the escape character so it is easily replaced by the escaped
				//		 component of the match.
				//NOTE:  The ORDER of the Or conditional matches is IMPORTANT because RegEx will short circuit once a match is found!


				//Process all Item tokens
				var itemTokens = rxItemSplitter.Split(stringToParse);
				foreach(var item in itemTokens) {
						
					//Second Split each Item pair into Key & Value tokens
					var keyValueTokens = rxKeyValueSplitter.Split(item);
					if (keyValueTokens.Length > indexOfTokenForKey)
					{
						String itemKey = String.Empty;
						String itemValue = String.Empty;

						for (int x = 0; x < keyValueTokens.Length; x++)
						{
							String keyValueItem = (keyValueTokens[x] ?? String.Empty);

							//For maximum flexibility we allow the index (Zero based) of the Key token to be specified in 
							//case the KeyValue pair component actually contains series of items. 
							if (x == indexOfTokenForKey)
							{
								//We know we are processing the Key
								itemKey = keysAreUrlEncoded ? HttpUtility.UrlDecode(keyValueItem, Encoding.UTF8) : keyValueItem;
							}
							else
							{
								//We know we are processing part of the Value
								keyValueItem = valuesAreUrlEncoded ? HttpUtility.UrlDecode(keyValueItem, Encoding.UTF8) : keyValueItem;

								//If the item Value is compiled from multiple parts then we re-combine by prefixing with the defined delimiter
								//otherwise our logic will leave the value without any prefix as the only value.
								itemValue = String.Format("{0}{1}{2}", itemValue, (String.IsNullOrEmpty(itemValue) ? String.Empty : keyValueDelimiter), keyValueItem);
							}
						}

						//Now we need to finish cleaning up any remaining escape character sequences by eliminating the escape character
						//NOTE:  i.e. "\abc" becomes "abc",  "\\" becomes "\"
						itemKey = itemKey.RegexProcessEscapeSequences(escapeSequenceString);
						itemValue = itemValue.RegexProcessEscapeSequences(escapeSequenceString);

						//Add the Key and the Value (possibly compiled) to the ResultMap
						keyValueMap.Add(itemKey, itemValue);
					}
					else
					{
						throw new ArgumentOutOfRangeException("Invalid index specified; the index cannot be greater than the number of parsed token items.");
					}
				}

			
			}
		
			//Finally return the resulting tokenized string as a Map of Key/Value items.
			return keyValueMap;
		}

	}

	public static class SystemNumericCustomExtensions
	{
		public static void LoopFor(this Int32 loopCount, Action fn)
		{
			if (loopCount >= 0)
			{
				for (var x = 0; x < loopCount; x++)
				{
					fn.Invoke();
				}
			}
		}
	}

	public static class Enum<TEnum>
	{
		public static TEnum Parse(String value)
		{
			return value.ParseAsEnum<TEnum>(false);
		}

		public static TEnum Parse(String value, bool ignoreCase)
		{
			return value.ParseAsEnum<TEnum>(ignoreCase);
		}

		public static TEnum Parse(Int64 value)
		{
			return value.ParseAsEnum<TEnum>();
		}

		public static IList<TEnum> GetValues()
		{
			return Enum.GetValues(typeof(TEnum)).OfType<TEnum>().ToList();
		}
	}

	public static class SystemEnumCustomExtensions
	{

		public static Int16 ToInteger(this Enum enumerator)
		{
			return enumerator.ChangeType<Int16>();
		}

		public static Int64 ToLong(this Enum enumerator)
		{
			return enumerator.ChangeType<Int64>();
		}

		public static TEnum ParseAsEnum<TEnum>(this String value)
		{
			return value.ParseAsEnum<TEnum>(false);
		}

		public static TEnum ParseAsEnum<TEnum>(this String value, bool ignoreCase)
		{
			if (String.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException("Can't parse an empty string");
			}

			Type enumType = typeof(TEnum);
			if (!enumType.IsEnum)
			{
				throw new InvalidOperationException("Enum Parse failed; [{0}] is not a valid Enum Type.".FormatArgs(enumType.Name));
			}

			return (TEnum)Enum.Parse(enumType, value, ignoreCase);
		}

		public static TEnum ParseAsEnum<TEnum>(this Int64 value)
		{
			Type enumType = ValidateEnumType<TEnum>();
			return (TEnum)Enum.ToObject(enumType, value);
		}

		private static Type ValidateEnumType<TEnum>()
		{
			Type enumType = typeof(TEnum);
			if (!enumType.IsEnum)
			{
				throw new InvalidOperationException("Enum Parse failed; [{0}] is not a valid Enum Type.".FormatArgs(enumType.Name));
			}
			return enumType;
		}

	}

	public static class SystemDateTimeCustomExtensions
	{
		public static DateTime ParseDateTime(this String input)
		{
			return DateTime.Parse(input);
		}

		public static String FormatDate(this DateTime dateTime, String format)
		{
			//For chaining, return a safe String when possible.
			if (dateTime == null) return String.Empty;
			return dateTime.ToString(format);
		}
		
		public static String FormatDate(this DateTime dateTime, String format, IFormatProvider provider)
		{
			//For chaining, return a safe String when possible.
			if (dateTime == null) return String.Empty;
			return dateTime.ToString(format, provider);
		}

		public static String ToLongDateAndTimeString(this DateTime dateTime)
		{
			return "{0} {1}".FormatArgs(dateTime.ToLongDateString(), dateTime.ToLongTimeString());
		}

		public static String ToShortDateAndTimeString(this DateTime dateTime)
		{
			return "{0} {1}".FormatArgs(dateTime.ToShortDateString(), dateTime.ToShortTimeString());
		}

		public static String ToW3CFormat(this DateTime dateTime)
		{
			return dateTime.ToW3CFormat(false);
		}
	
		public static String ToW3CFormat(this DateTime dateTime, bool includeFractionalSecondPrecision)
		{
			if (includeFractionalSecondPrecision)
			{
				return dateTime.FormatDate("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
			}
			else
			{
				return dateTime.FormatDate("yyyy-MM-ddTHH:mm:ss.FFFzzz", CultureInfo.InvariantCulture);
			}
		}

		public static DateTime GetFirstDayOfWeekInMonth(this DateTime dateTime, DayOfWeek dayToFind)
		{
			//Compute the Anchor date as the First Day of the current Month
			var firstDayOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
			//Now we shift the First day of the Month to find the Relative Day we are looking for.
			//This computes the magic mathmatical number to shift the date appropriately as a Modulus of 7 days per week!
			var magicNum = (((int)dayToFind + 7) - (int)firstDayOfMonth.DayOfWeek) % 7;
			//Now we shift the current day of the Week by our Magic Number
			var resultingDate = firstDayOfMonth.AddDays(magicNum);
			//Return the resulting date which will be the First Occurrence of the dayToFind specified 
			//  in the Month defined by dateTime:
			return resultingDate;
		}

		public static DateTime GetLastDayOfWeekInMonth(this DateTime dateTime, DayOfWeek dayToFind)
		{
			//Compute the Anchor date as the First Day of the Next/Following Month
			var firstDayOfNextMonth = new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1);
			//This computes the magic mathmatical number to shift the date appropriately as a Modulus of 7 days per week!
			var magicNum = (((int)dayToFind - 7) - (int)firstDayOfNextMonth.DayOfWeek) % 7;
			//Now we shift the First day of the Month by our Magic Number
			var resultingDate = firstDayOfNextMonth.AddDays(magicNum);
			//Return the resulting date which will be the Last Occurrence of the dayToFind specified 
			//  in the Month defined by dateTime:
			return resultingDate;
		}
	}

	public static class SystemTypeConversionExtesions
	{
		/// <summary>
		/// Intelligently converts data between types taking much more into account than the Convert Class -- ie. Handles Enums, Null, DBNull, and Nullable (Generic) types).
		/// Though, the Convert.ChangeType() class is used as the low level coverter for everything that it appropriately handles.
		/// 
		/// Perfect for converting between most Primitive types.
		/// 
		/// NOTE:  This method may have some performance overhead, so if the object is known to be an Object reference then the As&lt;TReturn&gt;() is recommended for direct conversion performance.
		/// </summary>
		/// <typeparam name="TR"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TR ChangeType<TR>(this Object value)
		{
			return (TR)ChangeType(value, typeof(TR));
		}

		/// <summary>
		/// Intelligently converts data between types, returnign the default value specified if null.
		/// 
		/// Takes much more into account than the Convert Class -- ie. Handles Enums, Null, DBNull, and Nullable (Generic) types).
		/// Though, the Convert.ChangeType() class is used as the low level coverter for everything that it appropriately handles.
		/// 
		/// Perfect for converting between most Primitive types.
		/// 
		/// NOTE:  This method may have some performance overhead, so if the object is known to be an Object reference then the As&lt;TReturn&gt;() is recommended for direct conversion performance.
		/// </summary>
		/// <typeparam name="TR"></typeparam>
		/// <param name="value"></param>
		/// <param name="whenNull"></param>
		/// <returns></returns>
		public static TR ChangeType<TR>(this Object value, TR whenNull)
		{
			return (value == null || value is DBNull || (value is String && value.As<String>().IsNullOrEmpty()))
				? whenNull
				: (TR)ChangeType(value, typeof(TR));
		}

		/// <summary>
		/// Intelligently converts data between types taking much more into account than the Convert Class -- ie. Handles Enums, Null, DBNull, and Nullable (Generic) types).
		/// Though, the Convert.ChangeType() class is used as the low level coverter for everything that it appropriately handles.
		/// 
		/// Perfect for converting between most Primitive types.
		/// 
		/// NOTE:  This method may have some performance overhead, so if the object is known to be an Object reference then the As&lt;TReturn&gt;() is recommended for direct conversion performance.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="convertToType"></param>
		/// <returns></returns>
		public static object ChangeType(this Object value, Type convertToType)
		{
			if (convertToType == null)
			{
				throw new ArgumentNullException("convertToType");
			}

			// return null if the value is null or DBNull
			//NOTE:  This is our primary NULL case handler as we will return
			//		 the default() value for the convertToType which will
			//		 handle classes as well as primitives.
			if (value == null || value is DBNull)
			{
				return convertToType.GetDefault();
			}

			// non-nullable types, which are not supported by Convert.ChangeType(),
			// unwrap the types to determine the underlying time
			if (convertToType.IsGenericType && convertToType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
			{
				convertToType = Nullable.GetUnderlyingType(convertToType);
			}

			if (value.GetType().IsPrimitive)
			{
				// deal with intelligent conversion to primitive types

				// intelligently convert enum types when input is a integer primitive
				//Note:  For optimzation we test Int32 (ie. regular int) FIRST to short circuit other tests
				if (convertToType.IsEnum && ((value is Int32) || (value is Int16) || (value is Int64)))
				{
					return Enum.ToObject(convertToType, value);
				}
			}
			else
			{
				// deal with intelligent conversion to reference types

				// deal with conversion to enum types when input is a string (ie. immutable class)
				if (convertToType.IsEnum && value is String)
				{
					return Enum.Parse(convertToType, value as String);
				}

				//Get the current TypeCode so we can process intelligently.
				var converToTypeCode = Type.GetTypeCode(convertToType);

				// intelligently convert Booleans when input is a String (class)
				if (converToTypeCode == TypeCode.Boolean && (value is String))
				{
					var valueAsString = value.As<String>().ToNullSafe().ToLower();
					if (valueAsString.IsNumeric())
					{
						var valueAsNumeric = Convert.ChangeType(value, typeof(double), CultureInfo.InvariantCulture);
						return Convert.ChangeType(valueAsNumeric, typeof(bool), CultureInfo.InvariantCulture);
					}
					else
					{
						return valueAsString.IsInIgnoreCase("yes", "true");
					}
				}
			}

			// use Convert.ChangeType() to do all other conversions
			return Convert.ChangeType(value, convertToType, CultureInfo.InvariantCulture);
		}
	}    

	public static class SystemEnumerableCustomExtensions
	{
		/// <summary>
		/// Easily join all items in the IEnumerable Generics List.  The ToString() method will
		/// be used to render all items to their String form.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="delimiter"></param>
		/// <returns></returns>
		public static String JoinString<T>(this IEnumerable<T> source, String delimiter)
		{
			//if (source == null) throw new ArgumentNullException("source");
			//return String.Join(delimiter, source.Select(i => i.ToString()).ToArray());
			return source.JoinString(delimiter, (T item) => item.ToString());
		}

		/// <summary>
		/// Easily join all items in the IEnumerable Generics List.  The value selector function/lambda
		/// specified will be used to render all items to their String form.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="delimiter"></param>
		/// <param name="valueSelector"></param>
		/// <returns></returns>
		public static String JoinString<T>(this IEnumerable<T> source, String delimiter, Func<T, String> valueSelector)
		{
			if (source == null) throw new ArgumentException("source");
			return String.Join(delimiter, source.Select(item => valueSelector(item)).ToArray());
		}
	}

	public static class SystemArrayCustomExtensions
	{
		#region Array Class (Non-Generic) Extensions
		/// <summary>
		/// Dynamically push the new item onto the end/bottom of the array.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="objThis"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static T[] Push<T>(this T[] objThis, T item)
		{
			int newSize = objThis.Length + 1;

			Array.Resize(ref objThis, newSize);
			objThis[newSize - 1] = item;

			return objThis;
		}

		/// <summary>
		/// Dynamically push all new items onto the end/bottom of the array.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="objThis"></param>
		/// <param name="items"></param>
		/// <returns></returns>
		public static T[] Push<T>(this T[] objThis, T[] items)
		{
			int newSize = objThis.Length + items.Length;

			Array.Resize(ref objThis, newSize);
			items.CopyTo(objThis, newSize - items.Length);

			return objThis;
		}

		/// <summary>
		/// Slice the array from the top retrieve the specified number of items; taking a slice off the top.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public static T[] SliceTop<T>(this T[] source, int count)
		{
			return source.Slice(0, count);
		}

		/// <summary>
		/// Slice the array from the bottom to retrieve the specified count, taking a slice off the bottom.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public static T[] SliceBottom<T>(this T[] source, int count)
		{
			if (count >= source.Length)
			{
				//Can't slice if not big enough
				return source;
			}
			else
			{
				return source.Slice((source.Length - 1) - count, source.Length - 1);
			}
		}

		/// <summary>
		/// Get the array slice between the two indexes.
		/// ... Inclusive for start index, exclusive for end index.
		/// </summary>
		public static T[] Slice<T>(this T[] source, int start, int end)
		{
			// Handles negative ends.
			if (end < 0)
			{
				end = source.Length + end;
			}
			int len = end - start;

			// Return new array.
			T[] res = new T[len];
			for (int i = 0; i < len; i++)
			{
				res[i] = source[i + start];
			}
			return res;
		}

		#endregion
	}

	public static class SystemEventHandlerDelegateCustomExtensions
	{

		//Note:  We do not implement overloads that take in only an Object param as teh sender because this lends to easy bugs since
		//       mistaking the EventHandler<T> as the sender object is easy and yields Runtime errors.
		public static bool Raise(this EventHandler _this, object sender)
		{
			return _this.Raise(sender, null);
		}

		//Note:  We do not implement overloads that take in only an Object param as teh sender because this lends to easy bugs since
		//       mistaking the EventHandler<T> as the sender object is easy and yields Runtime errors.
		public static bool Raise(this EventHandler _this, object sender, EventArgs eventArgs)
		{
			if (_this != null)
			{
				_this(sender, eventArgs ?? EventArgs.Empty);
				return true;
			}
			return false;
		}

		//Note:  We do not implement overloads that take in only an Object param as teh sender because this lends to easy bugs since
		//       mistaking the EventHandler<T> as the sender object is easy and yields Runtime errors.
		public static bool Raise(this EventHandler<EventArgs> _this, object sender)
		{
			return _this.Raise(sender, EventArgs.Empty);
		}

		//Note:  We do not implement overloads that take in only an Object param as teh sender because this lends to easy bugs since
		//       mistaking the EventHandler<T> as the sender object is easy and yields Runtime errors.
		public static bool Raise<TEventArgs>(this EventHandler<TEventArgs> _this, object sender) where TEventArgs : EventArgs
		{
			return _this.Raise<TEventArgs>(sender, null);
		}

		//Note:  We do not implement overloads that take in only an Object param as teh sender because this lends to easy bugs since
		//       mistaking the EventHandler<T> as the sender object is easy and yields Runtime errors.
		public static bool Raise<TEventArgs>(this EventHandler<TEventArgs> _this, object sender, TEventArgs eventArgs) where TEventArgs : EventArgs
		{
			try
			{
				if (_this != null)
				{
					_this(sender, eventArgs);
					return true;
				}
			}
			catch(Exception exc)
			{
				System.Diagnostics.Debug.Write(exc.GetMessages());
			}
			return false;
		}

		//public static EventHandler<TEventArgs> AddHandler<TEventArgs>(this EventHandler<TEventArgs> _this, EventHandler<TEventArgs> fnNewHandler) where TEventArgs : EventArgs
		//{
		//    if (fnNewHandler != null)
		//    {
		//        _this += fnNewHandler;
		//    }
		//    return _this;
		//}

		//public static EventHandler<TEventArgs> RemoveHandler<TEventArgs>(this EventHandler<TEventArgs> _this, EventHandler<TEventArgs> fnNewHandler) where TEventArgs : EventArgs
		//{
		//    if (fnNewHandler != null)
		//    {
		//        _this -= fnNewHandler;
		//    }
		//    return _this;
		//}
	}

}
	



