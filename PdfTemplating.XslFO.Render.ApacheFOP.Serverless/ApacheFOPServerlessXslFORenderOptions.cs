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

namespace PdfTemplating.XslFO.ApacheFOP.Serverless
{
    public class ApacheFOPServerlessXslFORenderOptions
    {
        public ApacheFOPServerlessXslFORenderOptions(Uri apacheFopServlessHostUri)
        {
            this.ApacheFOPServiceHost = apacheFopServlessHostUri.AssertArgumentIsNotNull(nameof(apacheFopServlessHostUri), "A valid Uri to the Apache FOP service must be specified.");
        }

        public bool EnableGzipCompression { get; set; } = false;

        public Uri ApacheFOPServiceHost { get; private set; }

        public String ApacheFOPApi { get; set; } = "api/apache-fop/xslfo";

        public Dictionary<string, string> QuerystringParams { get; } = new Dictionary<string, string>();

        public Dictionary<string, string> RequestHeaders { get; } = new Dictionary<string, string>();

    }
}
