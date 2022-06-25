/*
Copyright 2020 Brandon Bernard

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
using PdfTemplating.SystemCustomExtensions;
using Microsoft.AspNetCore.WebUtilities;
using Flurl;

namespace PdfTemplating.XslFO.ApacheFOP.Serverless
{
    public class ApacheFOPServerlessXslFORenderOptions
    {
        public const string DefaultXslFoCommandName = "xslfo";
        public const string DefaultGzipCommandName = "gzip";
        public const string DefaultXslFoApiPath = "api/apache-fop/xslfo";
        public const string DefaultGzipApiPath = "api/apache-fop/gzip";

        public ApacheFOPServerlessXslFORenderOptions(Uri apacheFopServlessUriWithToken)
        {
            
            apacheFopServlessUriWithToken.AssertArgumentIsNotNull(nameof(apacheFopServlessUriWithToken), "A valid Uri to the Apache FOP service must be specified.");

            //Safely parse the Host as we expect from the provided Uri.
            var hostUrlString = apacheFopServlessUriWithToken
                .GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped)
                .AssertArgumentIsNotNullOrBlank(nameof(apacheFopServlessUriWithToken), "The Apache FOP service Uri provided is invalid; the Server Host and Scheme of the Uri are blank/empty.");
            
            this.ApacheFOPServiceHost = new Uri(hostUrlString);

            //Safely parse the API Path as we expect from the provided Uri if included.
            var apiPath = apacheFopServlessUriWithToken.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
            if (!string.IsNullOrWhiteSpace(apiPath))
            {
                this.ApacheFopXslFoApi = apiPath;
                //TODO: Make this more robust with Regex Replace...
                this.ApacheFopGzipApi = apiPath.Replace(DefaultXslFoCommandName, DefaultGzipCommandName);
            }

            //Safely initialize any pre-defined Querystring Params from the original Uri
            var query = QueryHelpers.ParseQuery(apacheFopServlessUriWithToken.Query);
            foreach (var key in query.Keys)
            {
                this.QuerystringParams[key] = query[key].ToString();
            }
        }

        public ApacheFOPServerlessXslFORenderOptions(Uri apacheFopServlessHostUri, string azFuncAuthTokenCode)
            : this(apacheFopServlessHostUri.SetQueryParam("code", azFuncAuthTokenCode).ToUri())
        { }

        public int? RequestWaitTimeoutMillis { get; set; }

        public bool EnableGzipCompressionForRequests { get; set; } = false;

        public bool EnableGzipCompressionForResponses { get; set; } = false;

        public Uri ApacheFOPServiceHost { get; private set; }

        public string ApacheFopXslFoApi { get; set; } = DefaultXslFoApiPath;

        public string ApacheFopGzipApi { get; set; } = DefaultGzipApiPath;

        public string GetApacheFopApiPath()
        {
            return this.EnableGzipCompressionForRequests
                ? this.ApacheFopGzipApi
                : this.ApacheFopXslFoApi;
        }
        
        public Dictionary<string, string> QuerystringParams { get; } = new Dictionary<string, string>();

        public Dictionary<string, string> RequestHeaders { get; } = new Dictionary<string, string>();

    }
}
