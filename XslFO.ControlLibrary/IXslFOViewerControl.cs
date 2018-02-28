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
using System.Xml.Xsl;
using System.Xml.Linq;
using System.Xml.Linq.CustomExtensions;
using System.Xml.Linq.XslFO.CustomExtensions;
using System.IO;
using System.Text;

namespace XslFO.ControlLibrary
{
    public enum XslFOViewerControlState
    {
        Unavailable,
        Unloaded,
        Loading,
        LoadCompleted
    }

    public interface IXslFOViewerControl
    {
        void LoadXslt(FileInfo xmlSourceFile, FileInfo xslFOXsltSourceFile, XslFOPdfOptions pdfOptions);
        void LoadXslt(XDocument xmlInputDoc, XDocument xslFOXsltDoc, XslFOPdfOptions pdfOptions);
        void LoadXslt(XDocument xmlInputDoc, XslTransformEngine xslTransformEngine, XslFOPdfOptions pdfOptions);

        void LoadPdf(FileInfo pdfFileInfo);

        void LoadXslFO(FileInfo xslFOSourceFile, XslFOPdfOptions pdfOptions);
        void LoadXslFO(XDocument xXslFODoc, XslFOPdfOptions pdfOptions);

        void RefreshReport();

        void ClearReport();

        FileInfo LoadedFile
        {
            get;
        }
    }
}
