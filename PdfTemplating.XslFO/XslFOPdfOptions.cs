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
using System.IO;

namespace PdfTemplating.XslFO
{
    public class XslFOPdfOptions
    {
        public DirectoryInfo BaseDirectory { get; set; }
        public String Author { get; set; }
        public String Title { get; set; }
        public String Subject { get; set; }
        public String OwnerPassword { get; set; }
        public String UserPassword { get; set; }
        public bool EnableAdd { get; set; }
        public bool EnableCopy { get; set; }
        public bool EnableModify { get; set; }
        public bool EnablePrinting { get; set; }
    }
}
