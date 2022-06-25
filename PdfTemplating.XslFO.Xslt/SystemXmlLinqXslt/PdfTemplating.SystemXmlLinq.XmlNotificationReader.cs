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
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using PdfTemplating.SystemXmlLinqXsltCustomExtensions;
using System.Text;


namespace PdfTemplating.SystemXmlLinqXsltCustomExtensions
{
    public class XmlNotificationReader : XmlReader
    {
        #region Additional / Utility Classes

        public class XmlReadNotifier
        {
            public XName ElementNameMatcher { get; set; }
            public Func<XElement, XElement> NotificationCallback { get; set; }
        }
        
        #endregion

        #region Constructors

        public XmlNotificationReader(XmlReader baseXmlReader)
        {
            if (baseXmlReader == null)
            {
                throw new ArgumentNullException("baseXmlReader", "A valid XmlReader must be specified to initialize the XmlNotificationReader.");
            }

            this.BaseReader = baseXmlReader;
            this.NotificationsEnabled = true;
        }

        public static XmlNotificationReader Create(XmlReader baseXmlReader)
        {
            return new XmlNotificationReader(baseXmlReader);
        }

        #endregion
        
        #region Custom Public Properties

        public XmlReader BaseReader { get; private set; }
        public Dictionary<XName, XmlReadNotifier> NotifierSubscriptions { get; set; }
        public bool NotificationsEnabled { get; set; }

        #endregion

        #region Custom Notification / Event Subscription Methods
        public void AddNotification(XName xElementName, Func<XElement, XElement> fnNotificationCallback)
        {
            var notifier = new XmlReadNotifier()
            {
                ElementNameMatcher = xElementName,
                NotificationCallback = fnNotificationCallback
            };

            AddNotification(notifier);
        }

        public void AddNotification(XmlReadNotifier notifier)
        {
            this.NotifierSubscriptions.Add(notifier.ElementNameMatcher, notifier);
        }

        #endregion

        #region Implement Abstract class Properties/Methods

        public override int AttributeCount
        {
            get { return this.BaseReader.AttributeCount; }
        }

        public override string BaseURI
        {
            get { return this.BaseReader.BaseURI; }
        }

        public override void Close()
        {
            this.BaseReader.Close();
        }

        public override int Depth
        {
            get { return this.BaseReader.Depth; }
        }

        public override bool EOF
        {
            get { return this.BaseReader.EOF; }
        }

        public override string GetAttribute(int i)
        {
            return this.BaseReader.GetAttribute(i);
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            return this.BaseReader.GetAttribute(name, namespaceURI);
        }

        public override string GetAttribute(string name)
        {
            return this.BaseReader.GetAttribute(name);
        }

        public override bool IsEmptyElement
        {
            get { return this.BaseReader.IsEmptyElement; }
        }

        public override string LocalName
        {
            get { return this.BaseReader.LocalName; }
        }

        public override string LookupNamespace(string prefix)
        {
            return this.BaseReader.LookupNamespace(prefix);
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            return this.BaseReader.MoveToAttribute(name, ns);
        }

        public override bool MoveToAttribute(string name)
        {
            return this.BaseReader.MoveToAttribute(name);
        }

        public override bool MoveToElement()
        {
            return this.BaseReader.MoveToElement();
        }

        public override bool MoveToFirstAttribute()
        {
            return this.BaseReader.MoveToFirstAttribute();
        }

        public override bool MoveToNextAttribute()
        {
            return this.BaseReader.MoveToNextAttribute();
        }

        public override XmlNameTable NameTable
        {
            get { return this.BaseReader.NameTable; }
        }

        public override string NamespaceURI
        {
            get { return this.BaseReader.NamespaceURI; }
        }

        public override XmlNodeType NodeType
        {
            get { return this.BaseReader.NodeType; }
        }

        public override string Prefix
        {
            get { return this.BaseReader.Prefix; }
        }

        public override bool Read()
        {
            bool readState = this.BaseReader.Read();

            if (this.NotificationsEnabled)
            {
                XName xNodeName = XName.Get(this.Name, this.NamespaceURI);
                XmlReadNotifier notifier;
                if(this.NotifierSubscriptions.TryGetValue(xNodeName, out notifier))
                {
                    XmlReader subTreeReader = this.ReadSubtree();
                    XElement xSubTree = XElement.Load(this.BaseReader.ReadSubtree());
                    notifier.NotificationCallback(xSubTree);
                }
            }

            return readState;
        }

        public override bool ReadAttributeValue()
        {
            return this.BaseReader.ReadAttributeValue();
        }

        public override ReadState ReadState
        {
            get { return this.BaseReader.ReadState; }
        }

        public override void ResolveEntity()
        {
            this.BaseReader.ResolveEntity();
        }

        public override string Value
        {
            get { return this.BaseReader.Value; }
        }

        #endregion

    }
}
