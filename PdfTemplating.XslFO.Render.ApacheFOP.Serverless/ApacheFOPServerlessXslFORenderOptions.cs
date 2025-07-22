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
using System.Linq;
using PdfTemplating.SystemCustomExtensions;
using Flurl;
using PdfTemplating.SystemLinqCustomExtensions;

namespace PdfTemplating.XslFO.ApacheFOP.Serverless
{
    public class ApacheFOPServerlessXslFORenderOptions
    {
        public const string AzureFunctionsApiTokenQueryParamName = "code";
        public const string DefaultXslFoCommandName = "xslfo";
        public const string DefaultGzipCommandName = "gzip";
        public const string DefaultXslFoApiPath = "api/apache-fop/xslfo";
        public const string DefaultGzipApiPath = "api/apache-fop/gzip";

        public ApacheFOPServerlessXslFORenderOptions(Uri apacheFopServlessUriWithToken)
        {
            apacheFopServlessUriWithToken.AssertArgumentIsNotNull(nameof(apacheFopServlessUriWithToken), "A valid Uri to the Apache FOP service must be specified.");

            var flurlUrl = new Url(apacheFopServlessUriWithToken);

            if (flurlUrl.PathSegments.Count > 0)
            {
                //Safely parse the API Path as we expect from the provided Uri if included.
                this.ApacheFopXslFoApi = flurlUrl.Path;

                var xslfoPathIndex = flurlUrl.PathSegments.ToList().FindIndex(s => s.Equals(DefaultXslFoCommandName, StringComparison.OrdinalIgnoreCase));
                if (xslfoPathIndex >= 0)
                {
                    flurlUrl.PathSegments[xslfoPathIndex] = DefaultGzipCommandName;
                    this.ApacheFopGzipApi = flurlUrl.Path;
                }
            }

            //Safely initialize any pre-defined Querystring Params from the original Uri (using Flurl.Util helpers)...
            this.QuerystringParams = flurlUrl.QueryParams.ToDictionarySafely(qp => qp.Name, qp => qp.Value.ToString());

            //Safely parse the base Host Url as we expect from the provided Uri.
            this.ApacheFOPServiceHost = flurlUrl.RemovePath().RemoveQuery().RemoveFragment().ToUri();
        }

        public ApacheFOPServerlessXslFORenderOptions(Uri apacheFopServlessHostUri, string azFuncAuthTokenCode)
            : this(apacheFopServlessHostUri.SetQueryParam(AzureFunctionsApiTokenQueryParamName, azFuncAuthTokenCode).ToUri())
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
        
        public Dictionary<string, string> QuerystringParams { get; private set; } = new Dictionary<string, string>();

        public Dictionary<string, string> RequestHeaders { get; } = new Dictionary<string, string>();

    }
}
