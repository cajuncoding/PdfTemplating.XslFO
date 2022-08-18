using System;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PdfTemplating.XslFO.Render.ApacheFOP.Serverless
{
    public class ApacheFOPServerlessApiException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; }

        public string RequestBody { get; }

        public string ResponseBody { get; }

        private JObject _responseJson = null;
        public JObject ToJsonJObject() => _responseJson ?? (_responseJson = ParseJsonSafely(ResponseBody));

        public static async Task<ApacheFOPServerlessApiException> FromFlurlHttpExceptionAsync(FlurlHttpException flurlHttpException, string requestBody = null)
        {
            var httpStatusCode = HttpStatusCode.InternalServerError;
            var responseBody = await flurlHttpException.GetResponseStringAsync().ConfigureAwait(false);
            var responseJson = ParseJsonSafely(responseBody);
            string errorMessage = null;

            //Handle Timeout exception...
            if (flurlHttpException is FlurlHttpTimeoutException flurlTimeoutException)
            {
                httpStatusCode = HttpStatusCode.RequestTimeout;
                errorMessage = flurlTimeoutException.Message;
            }
            //Generically handle other Flurl Http Exceptions...
            else
            {
                if (flurlHttpException.StatusCode.HasValue)
                    httpStatusCode = (HttpStatusCode)flurlHttpException.StatusCode;

                errorMessage = responseJson.GetValue("detailMessage", StringComparison.OrdinalIgnoreCase)?.Value<string>() 
                                ?? "Unknown Error occurred rendering the Xsl FO markup; error message could not be parsed from response.";
            }

            return new ApacheFOPServerlessApiException(
                httpStatusCode, 
                errorMessage, 
                requestPayloadBody: requestBody, 
                errorResponseBody: responseBody, 
                errorResponseJson: responseJson, 
                innerException: flurlHttpException
            );
        }

        public ApacheFOPServerlessApiException(
            HttpStatusCode httpStatusCode,
            string message, 
            string requestPayloadBody, 
            string errorResponseBody, 
            Exception innerException = null
        ) : base(message, innerException)
        {
            HttpStatusCode = httpStatusCode;
            RequestBody = requestPayloadBody;
            ResponseBody = errorResponseBody;
        }

        /// <summary>
        /// Protected Constructor accepting pre-parsed Json if available.
        /// </summary>
        protected ApacheFOPServerlessApiException(
            HttpStatusCode httpStatusCode, 
            string message, 
            string requestPayloadBody, 
            string errorResponseBody, 
            JObject errorResponseJson, 
            Exception innerException = null
        ) : this(httpStatusCode, message, requestPayloadBody, errorResponseBody, innerException)
        {
            _responseJson = errorResponseJson;
        }

        protected static JObject ParseJsonSafely(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                return JObject.Parse(json);
            }
            catch (JsonReaderException)
            {
                //DO NOTHING if there is a Parse Error...
                return null;
            }
        }
    }
}
