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
using System.CustomExtensions;
using System.Web;

namespace PdfTemplating.XslFO.ApacheFOP.Serverless
{
    public class ApacheFOPServerlessXslFORenderOptions
    {
        public const string DefaultApacheFopServerlessApiPath = "api/apache-fop/xslfo";

        public ApacheFOPServerlessXslFORenderOptions(Uri apacheFopServlessHostUri)
        {
            
            apacheFopServlessHostUri.AssertArgumentIsNotNull(nameof(apacheFopServlessHostUri), "A valid Uri to the Apache FOP service must be specified.");

            //Safely parse the Host as we expect from the provided Uri.
            var hostUrlString = apacheFopServlessHostUri
                .GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped)
                .AssertArgumentIsNotNullOrBlank(nameof(apacheFopServlessHostUri), "The Apache FOP service Uri provided is invalid; the Server Host and Scheme of the Uri are blank/empty.");
            
            this.ApacheFOPServiceHost = new Uri(hostUrlString);

            //Safely parse the API Path as we expect from the provided Uri if included.
            var apiPath = apacheFopServlessHostUri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
            if (!string.IsNullOrWhiteSpace(apiPath))
                this.ApacheFOPApi = apiPath;

            //Safely initialize any pre-defined Querystring Params from the original Uri
            var query = HttpUtility.ParseQueryString(apacheFopServlessHostUri.Query);
            foreach (var key in query.AllKeys)
            {
                this.QuerystringParams[key] = query[key];
            }
        }

        public bool EnableGzipCompressionForRequests { get; set; } = false;

        public bool EnableGzipCompressionForResponses { get; set; } = false;

        public Uri ApacheFOPServiceHost { get; private set; }

        public String ApacheFOPApi { get; set; } = DefaultApacheFopServerlessApiPath;

        public Dictionary<string, string> QuerystringParams { get; } = new Dictionary<string, string>();

        public Dictionary<string, string> RequestHeaders { get; } = new Dictionary<string, string>();

    }
}
