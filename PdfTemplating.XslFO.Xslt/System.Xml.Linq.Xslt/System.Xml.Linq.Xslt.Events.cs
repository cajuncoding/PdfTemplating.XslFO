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
using System.CustomExtensions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Xml.Linq.CustomExtensions;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.CustomExtensions;
using System.Security;
using System.Web;
using System.IO;
using System.IO.CustomExtensions;

namespace System.Xml.Linq.CustomExtensions
{
    #region EventArg Classes

    public class XsltExtensionEventArgs :EventArgs {}

    public class XsltMessageEventArgs : XsltExtensionEventArgs
    {
        public XsltMessageEventArgs()
        {
            this.Message = String.Empty;
        }

        public XsltMessageEventArgs(String message)
        {
            this.Message = message;
        }

        public XsltMessageEventArgs(String messageFormat, params object[] args)
        {
            this.Message = String.Format(messageFormat, args);
        }

        public String Message { get; set; }

        public String GetMessage()
        {
            return this.Message;
        }
    }

    public class XsltErrorEventArgs : XsltMessageEventArgs
    {
        public XsltErrorEventArgs() : base()
        {
        }

        public XsltErrorEventArgs(String message) : base(message)
        {
        }

        public XsltErrorEventArgs(String messageFormat, params object[] args) : base(messageFormat, args)
        {
        }
    }

    //public class XsltProgressEventArgs : XsltExtensionEventArgs
    //{
    //    public XsltProgressEventArgs(int percentProgress)
    //    {
    //        this.PercentProgress = percentProgress;
    //    }
    //    public int PercentProgress { get; private set; }
    //}

    #endregion

}
