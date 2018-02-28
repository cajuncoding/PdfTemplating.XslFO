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
using System.Web;
using RestSharp;
using System.Net;
using System.Threading.Tasks;

namespace RestSharp.CustomExtensions
{
    public static class RestClientCustomExtensions
    {
        public static bool IsOk(this IRestResponse response)
        {
            return (response?.StatusCode == HttpStatusCode.OK && response?.ErrorException == null);
        }

        public static bool IsError(this IRestResponse response)
        {
            return (response?.StatusCode == HttpStatusCode.InternalServerError);
        }

        public static bool IsNotFound(this IRestResponse response)
        {
            return (response?.StatusCode == HttpStatusCode.NotFound);
        }

        public static async Task<IRestResponse> ExecuteWithExceptionHandlingAsync(this IRestClient client, IRestRequest request)
        {
            IRestResponse response = null;
            Uri requestUri = null;
            try
            {
                requestUri = client.BuildUri(request);
                response = await client.ExecuteTaskAsync(request);
            }
            catch (Exception exc)
            {
                throw new HttpException(
                    (int)HttpStatusCode.InternalServerError, 
                    $"Unknown error occurred while processing the Rest request [{requestUri?.AbsoluteUri ?? request.Resource}].", exc
                );
            }

            //BBernard - 02/20/2018
            //Handle Http Response and enforce .Net Exeption handling for responses that are not 200 OK, by raising HttpExcpetions.
            if (response == null)
            {
                throw new HttpException(
                    (int)HttpStatusCode.InternalServerError, 
                    $"Unknown error occurred while processing the Rest request [{requestUri?.AbsoluteUri}]; IRestReponse is null."
                );
            }
            else
            {
                //BBernard - 02/20/2018
                //Handle Http Response issues when not OK StatusCode!
                if (!response.IsOk())
                {
                    var statusCode = response.StatusCode;
                    throw new HttpException(
                        (int)response.StatusCode,
                        $"Http Error occurred with [StatusCode={(int)statusCode} {statusCode.ToString()}]; {response?.ErrorException?.Message ?? response.Content}",
                        response.ErrorException
                    );
                }
            }

            //BBernard - 02/20/2018
            //Finally return the Response if valid!
            return response;
        }
    }
}