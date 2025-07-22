using System;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Flurl.Http;
using PdfTemplating.XslFO.ApacheFOP.Serverless;

namespace PdfTemplating.XslFO.Render.ApacheFOP.Serverless
{
    public class ApacheFOPServerlessApiException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; }

        public string RequestBody { get; }

        public string ResponseBody { get; }

        /// <summary>
        /// Returns the Request Uri without the 'code' querystring param if it exists.
        /// </summary>
        public Uri RequestUriSecured { get; }

        /// <summary>
        /// Returns the complete raw Uri including secured query params such as 'code'.
        /// </summary>
        public Uri RequestUriRaw { get; }

        private JsonNode _responseJson = null;
        public JsonNode ToJson() => _responseJson ?? (_responseJson = ParseJsonSafely(ResponseBody));

        public static async Task<ApacheFOPServerlessApiException> FromFlurlHttpExceptionAsync(FlurlHttpException flurlHttpException, string requestBody = null)
        {
            var httpStatusCode = HttpStatusCode.InternalServerError;
            var responseBody = await flurlHttpException.GetResponseStringAsync().ConfigureAwait(false);
            var responseJson = ParseJsonSafely(responseBody);
            
            var requestUrl = flurlHttpException.Call?.Request?.Url;
            var requestUriRaw = requestUrl.ToUri();
            var requestUriSecured = requestUrl.RemoveQueryParam(ApacheFOPServerlessXslFORenderOptions.AzureFunctionsApiTokenQueryParamName).ToUri();

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

                if(responseJson is JsonObject jsonObject)
                    errorMessage = jsonObject["detailMessage"]?.GetValue<string>() ?? "Unknown Error occurred rendering the Xsl FO markup; error message could not be parsed from response.";
            }

            return new ApacheFOPServerlessApiException(
                httpStatusCode, 
                errorMessage,
                requestUriRaw: requestUriRaw,
                requestUriSecured: requestUriSecured,
                requestPayloadBody: requestBody, 
                errorResponseBody: responseBody, 
                errorResponseJson: responseJson, 
                innerException: flurlHttpException
            );
        }

        public ApacheFOPServerlessApiException(
            HttpStatusCode httpStatusCode,
            string message,
            Uri requestUriRaw,
            Uri requestUriSecured,
            string requestPayloadBody, 
            string errorResponseBody, 
            Exception innerException = null
        ) : base(message, innerException)
        {
            HttpStatusCode = httpStatusCode;
            RequestBody = requestPayloadBody;
            ResponseBody = errorResponseBody;
            RequestUriRaw = requestUriRaw;
            RequestUriSecured = requestUriSecured;
        }

        /// <summary>
        /// Protected Constructor accepting pre-parsed Json if available.
        /// </summary>
        protected ApacheFOPServerlessApiException(
            HttpStatusCode httpStatusCode, 
            string message,
            Uri requestUriRaw,
            Uri requestUriSecured,
            string requestPayloadBody, 
            string errorResponseBody, 
            JsonNode errorResponseJson, 
            Exception innerException = null
        ) : this(httpStatusCode, message, requestUriRaw, requestUriSecured, requestPayloadBody, errorResponseBody, innerException)
        {
            _responseJson = errorResponseJson;
        }

        protected static JsonNode ParseJsonSafely(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                return JsonNode.Parse(json, new JsonNodeOptions() { PropertyNameCaseInsensitive = true });
            }
            catch (Exception)
            {
                //DO NOTHING if there is a Parse Error...
                return null;
            }
        }
    }
}
