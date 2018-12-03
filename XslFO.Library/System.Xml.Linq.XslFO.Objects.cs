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

using System.IO;

namespace System.Xml.Linq.XslFO.CustomExtensions
{
    public class XslFORenderOutput
    {
        public XslFORenderOutput(XDocument xXslFOXmlDoc)
        {
            this.XslFODocument = xXslFOXmlDoc;
        }

        public XDocument XslFODocument { get; set; }

        public override String ToString()
        {
            return XslFODocument == null ? String.Empty : this.XslFODocument.ToString();
        }
    }

    public class XslFORenderFileOutput : XslFORenderOutput
    {
        public XslFORenderFileOutput(XDocument xXslFOXmlDoc, FileInfo xslFOFileInfo) 
            : base(xXslFOXmlDoc)
        {
            this.PdfFileInfo = xslFOFileInfo;
        }

        public FileInfo PdfFileInfo { get; set; }
    }

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

    public class XslFOPdfOptions
    {
        private DirectoryInfo _baseDirectory = null;
        public DirectoryInfo BaseDirectory
        {
            get { return _baseDirectory; }
            set { _baseDirectory = value; }
        }
        
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
