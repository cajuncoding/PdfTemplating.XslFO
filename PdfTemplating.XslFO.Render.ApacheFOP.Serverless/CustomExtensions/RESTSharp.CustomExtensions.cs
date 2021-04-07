using System;
using System.CustomExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace RestSharp.CustomExtensions
{
    public static class ContentType
    {
        public const string Xml = "text/xml";
        public const string Json = "application/json";
        public const string PlainText = "text/plain";
        public const string BinaryOctetStream = "application/octet-stream";
    }

    //BBernard - Added custom extensions for REST Sharp to provide similar simlified methods for Raw & Plain Text as the out-of-the-box package only provides convenience methods for Json & Xml
    public static class RestRequestCustomExtensions
    {

        public static IRestRequest AddPlainTextBody(this IRestRequest request, string body)
        {
            request.AddRawTextBody(body, ContentType.PlainText);
            return request;
        }

        public static IRestRequest AddRawBytesBody(this IRestRequest request, byte[] body, string contentType = ContentType.BinaryOctetStream)
        {
            #pragma warning disable CS0618 // Type or member is obsolete
            request.RequestFormat = DataFormat.None;
            #pragma warning restore CS0618 // Type or member is obsolete

            request.AddParameter(contentType, body, ParameterType.RequestBody);
            return request;
        }

        public static IRestRequest AddRawBytesAsBase64Body(this IRestRequest request, byte[] body, string contentType = ContentType.PlainText)
        {
            var base64Body = Convert.ToBase64String(body);
            request.AddRawTextBody(base64Body, contentType);
            return request;
        }

        public static IRestRequest AddRawTextBody(this IRestRequest request, string body, string contentType = ContentType.PlainText)
        {
            //BBernard
            //NOTE: When using ParameterType.RequestBody then the `name` parameter is automatically used as the Content Type by RESTSharp;
            //      This isn't clearly documented but it is the behaviour when adding a Body only parameter.
            //NOTE: This is an acknowledged concern that will not be fixed in RESTSharp per Issue:
            //      https://github.com/restsharp/RestSharp/issues/901#issuecomment-380584525
            //NOTE: Therefore we must explicitly use the overload that takes in a 'name' and not a 'contentType'; If we use the
            //      method that takes in a 'contentType' it will not be used even if we specify name to null/empty.
            
            #pragma warning disable CS0618 // Type or member is obsolete
            request.RequestFormat = DataFormat.None;
            #pragma warning restore CS0618 // Type or member is obsolete
            
            request.AddParameter(contentType, body, ParameterType.RequestBody);
            return request;
        }
    }

    public static class RestResponseCustomExtensions
    {
        public static bool IsOk(this IRestResponse response)
        {
            return response?.StatusCode == HttpStatusCode.OK && response?.ErrorException == null;
        }

        public static bool IsError(this IRestResponse response)
        {
            return (response?.StatusCode == HttpStatusCode.InternalServerError);
        }

        public static bool IsNotFound(this IRestResponse response)
        {
            return (response?.StatusCode == HttpStatusCode.NotFound);
        }

        public static IRestResponse AssertRestSharpResponseIsOk(this IRestResponse response)
        {
            response.AssertArgumentIsNotNull(nameof(response));

            //Handle Errors with RESTSharp responses that are not valid...
            int responseStatus = (int)response.StatusCode;
            if (!response.IsOk() || !(responseStatus >= 200 && responseStatus <= 299))
            {
                throw new RestSharpHttpException(response);
            }

            return response;
        }

        public static Dictionary<string, string> GetHeadersAsDictionary(this IRestResponse response)
        {
            var headersDictionary = response.Headers
                .Where(p => p.Type == ParameterType.HttpHeader && p.Value != null)
                .ToDictionary(p => p.Name, p => p.Value?.ToString());
            
            return headersDictionary;
        }
    }

    public static class RestClientCustomExtensions
    {

        public static async Task<IRestResponse> ExecuteWithExceptionHandlingAsync(this IRestClient client, IRestRequest request)
        {
            IRestResponse response = null;
            Uri requestUri = null;
            try
            {
                requestUri = client.BuildUri(request);
                response = await client.ExecuteAsync(request).ConfigureAwait(false);
            }
            catch (Exception exc)
            {
                throw new RestSharpHttpException(
                    HttpStatusCode.InternalServerError, 
                    requestUri,
                    $"Http Request failed for unknown reason calling [{requestUri?.AbsoluteUri}].",
                    exc
                );
            }

            //BBernard - 02/20/2018
            //Handle Http Response and enforce .Net Exeption handling for responses that are not 200 OK, by raising HttpExcpetions.
            if (response == null)
            {
                throw new RestSharpHttpException(
                    HttpStatusCode.InternalServerError,
                    requestUri,
                    $"Http Request failed for unknown reason calling [{requestUri?.AbsoluteUri}]; IRestReponse is null."
                );
            }
            else
            {
                //BBernard - 02/20/2018
                //Handle Http Response issues when not OK StatusCode!
                if (!response.IsOk())
                {
                    throw new RestSharpHttpException(
                        response,
                        requestUri,
                        response.ErrorException
                    );
                }
            }

            //BBernard - 02/20/2018
            //Finally return the Response if valid!
            return response;
        }
    }


    public class RestSharpHttpException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; private set; }
        public string HttpStatusDescription { get; private set; }
        public Uri RequestUri{ get; private set; }
        public Uri ResponseUri { get; private set; }
        public string ErrorMessage { get; set; }
        public string ErrorResponseBody { get; private set; }
        public Exception ResponseException{ get; private set; }

        public RestSharpHttpException(HttpStatusCode httpStatusCode, Uri requestUri, string message, Exception innerException = null)
            : base(message, innerException)
        {
            this.HttpStatusCode = httpStatusCode;
            this.HttpStatusDescription = httpStatusCode.ToString();
            this.RequestUri = requestUri;
        }

        public RestSharpHttpException(IRestResponse response, Uri requestUri = null, Exception innerException = null)
            : base(response.ErrorMessage, 
                //Nest some helpful info. in the inner exception, and pass along any other possible inner exceptions as children also...
                new Exception($"Http Request failed for [{response.ResponseUri}] with [StatusCode={(int)response.StatusCode}-{response.StatusDescription}]; {response?.ErrorException?.Message ?? response.Content}", 
                      innerException ?? response?.ErrorException
                )
            )
        {
            response.AssertArgumentIsNotNull(nameof(response), "RestRequest exception yielded a null RestResponse object.");

            this.HttpStatusCode = response.StatusCode;
            this.HttpStatusDescription = response.StatusCode.ToString();
            this.RequestUri = requestUri;
            this.ResponseUri = response.ResponseUri;
            this.ErrorMessage = response.ErrorMessage;
            this.ErrorResponseBody = response.Content;
            this.ResponseException = response.ErrorException;
        }
    }
}
