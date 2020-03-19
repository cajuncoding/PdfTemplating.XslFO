using System;
using System.Collections;
using System.Xml;
using Fonet.DataTypes;
using Fonet.Layout;

namespace Fonet.Fo
{
    internal abstract class XMLObj : FObj
    {
        protected string tagName = "";

        protected XmlNode element;

        protected XmlDocument doc;

        protected const string NS = "http://www.codeplex.com/fonet";

        public XMLObj(FObj parent, PropertyList propertyList, string tag)
            : base(parent, propertyList)
        {
            tagName = tag;
        }

        public abstract string GetNameSpace();

        protected static Hashtable ns = new Hashtable();

        public void addGraphic(XmlDocument doc, XmlNode parent)
        {
            this.doc = doc;
        }

        public void buildTopLevel(XmlDocument doc, XmlNode svgRoot) { }

        public XmlDocument CreateBasicDocument()
        {
            try
            {
                doc = new XmlDocument();
                doc.AppendChild(doc.CreateElement("graph", NS));
                element = doc.DocumentElement;
                buildTopLevel(doc, element);
            }
            catch (Exception e)
            {
                FonetDriver.ActiveDriver.FireFonetError(e.ToString());
            }
            return doc;
        }

        protected internal override void AddChild(FONode child)
        {
            if (child is XMLObj)
            {
                ((XMLObj)child).addGraphic(doc, element);
            }
        }

        protected internal override void AddCharacters(char[] data, int start, int length)
        {
            string str = new string(data, start, length - start);
            doc.DocumentElement.AppendChild(doc.CreateTextNode(str));
        }

        public override Status Layout(Area area)
        {
            FonetDriver.ActiveDriver.FireFonetError(
                this.name + " outside foreign xml");

            return new Status(Status.OK);
        }

        public override void RemoveID(IDReferences idReferences) { }

        public override void SetIsInTableCell() { }

        public override void ForceStartOffset(int offset) { }

        public override void ForceWidth(int width) { }

        public override void ResetMarker() { }

        public override void SetLinkSet(LinkSet linkSet) { }

        public override ArrayList getMarkerSnapshot(ArrayList snapshot)
        {
            return snapshot;
        }

        public override void Rollback(ArrayList snapshot) { }

        protected override void SetWritingMode() { }
    }

}