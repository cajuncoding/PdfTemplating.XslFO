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
using System.Text;
using System.Xml.Linq;
using PdfTemplating.XslFO;
using PdfTemplating.XslFO.Fonet.CustomExtensions;

namespace PdfTemplating.ControlLibrary
{
    public class XslFOViewerEventArgs : EventArgs
    {
        public XslFOViewerEventArgs(String message)
            : this(message, null as XslFORenderOutput)
        {
        }

        public XslFOViewerEventArgs(String message, String renderSource)
            : this(message, null as XslFORenderOutput)
        {
            this.RenderSource = renderSource;
        }

        public XslFOViewerEventArgs(String message, XslFORenderOutput renderOutput)
        {
            this.Message = message;
            this.RenderOutput = renderOutput;
            this.RenderSource = renderOutput != null ? renderOutput.ToString() : String.Empty;
        }

        public String Message { get; protected set; }
        public XslFORenderOutput RenderOutput { get; protected set; }
        public String RenderSource { get; protected set; }
        public List<XslFOViewerEventArgs> ErrorEvents { get; set; }

        //Note:  For easier Implementation we provide an easy Empty Property here for quick throws or Events that don't require other data;
        public new static readonly XslFOViewerEventArgs Empty = new XslFOViewerEventArgs(String.Empty);
    }
}
