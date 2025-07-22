using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PdfTemplating.WebApp.Common
{
    public static class SystemExceptionCustomExtensions
    {
        private const string _nestedExceptionFormatString = "[{0}] {1}";

        /// <summary>
        /// Retrieve a string containing ALL descendent Exception Messages using the specified format string; traversing all nested exceptions as necessary.
        /// </summary>
        /// <param name="thisException"></param>
        /// <returns></returns>
        public static String GetMessagesRecursively(this Exception thisException)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string exceptionMessage = String.Empty;

            if (thisException != null)
            {
                exceptionMessage = TerminateMessageHelper(thisException.Message);
                stringBuilder.AppendFormat(_nestedExceptionFormatString, thisException.GetType().Name, exceptionMessage);

                //Traverse all InnerExceptions
                Exception innerException = thisException.InnerException;
                while (innerException != null)
                {
                    exceptionMessage = TerminateMessageHelper(innerException.Message);
                    stringBuilder.AppendFormat(_nestedExceptionFormatString, innerException.GetType().Name, exceptionMessage);
                    innerException = innerException.InnerException;
                }

                //Handle New .Net 4.0 Aggregate Exception Type as a special Case because
                //AggregateExceptions contain a list of Exceptions thrown by background threads.
                if (thisException is AggregateException aggregateExc)
                {
                    foreach (var exc in aggregateExc.InnerExceptions)
                    {
                        //exceptionMessage = TerminateMessageHelper(exc.Message);
                        exceptionMessage = TerminateMessageHelper(exc.GetMessagesRecursively());
                        stringBuilder.AppendFormat(_nestedExceptionFormatString, exc.GetType().Name, exceptionMessage);
                    }
                }
            }

            return stringBuilder.ToString();
        }

        private static string TerminateMessageHelper(string message)
        {
            if (!string.IsNullOrEmpty(message) && !message.EndsWith(".") && !message.EndsWith(";"))
            {
                return string.Concat(message, ";");
            }

            return message;
        }

        /// <summary>
		/// Provides a simplified Json object model that can be safely used and serialized with no risk of
		/// recursive reference errors that the Exception class.
		/// </summary>
		/// <param name="exc"></param>
		/// <param name="includeStackTrace"></param>
		/// <returns></returns>
        public static JsonObject ToSimplifiedJsonObjectModel(this Exception exc, bool includeStackTrace = false)
        {
            var type = exc.GetType();
            var exceptionJson = JsonSerializer.SerializeToNode(new
            {
                ExceptionType = type.FullName,
                ExceptionTypeName = type.Name,
                AllMessages = exc.GetMessagesRecursively(),
                Data = exc.Data,
                HResult = exc.HResult,
                Source = exc.Source,
            });

            if (includeStackTrace)
                exceptionJson[nameof(exc.StackTrace)] = exc.StackTrace;

            return (JsonObject)exceptionJson;
        }

        /// <summary>
        /// Safely convert the Exception to Json without issues relating to recursive references;
        /// handles all nested inner exceptions and ensures that we get all relevant data and messages with
        /// optional StackTraces included.
        /// </summary>
        /// <param name="exc"></param>
        /// <param name="includeStackTrace"></param>
        /// <returns></returns>
        public static string ToJson(this Exception exc, bool includeStackTrace = false)
        {
            //Simplify and map the Exception Model for Serialization...
            var exceptionJson = ToSimplifiedJsonObjectModel(exc, includeStackTrace);

            var innerException = exc.InnerException;
            var parentExceptionJson = exceptionJson;
            while (innerException != null)
            {
                var newInnerException = ToSimplifiedJsonObjectModel(innerException, includeStackTrace);
                //Set the Inner Exception into the Parent...
                parentExceptionJson[nameof(exc.InnerException)] = newInnerException;
                //Now Re-set the Parent to the new Inner Exception, and see if there are any further Inner Exceptions to handle; so we keep crawling the tree...
                parentExceptionJson = newInnerException;
                innerException = innerException.InnerException;
            }

            return exceptionJson.ToString();
        }
    }
}
