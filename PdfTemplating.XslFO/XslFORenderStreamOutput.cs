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
using System.Xml.Linq;

namespace PdfTemplating.XslFO
{
    public class XslFORenderStreamOutput : XslFORenderOutput, IDisposable
    {
        public XslFORenderStreamOutput(XDocument xXslFOXmlDoc, MemoryStream xslFOStream)
            : base(xXslFOXmlDoc)
        {
            this.PdfStream = xslFOStream;
        }

        public MemoryStream PdfStream { get; set; }

        public byte[] ReadBytes()
        {
            byte[] pdfBytes = this.PdfStream?.ToArray();
            return pdfBytes;
        }

        //NOTE:  We implement IDisposable to make sure that our Stream Reference is Disposed correctly!
        public void Dispose()
        {
            this.PdfStream?.Dispose();
        }
    }
}
