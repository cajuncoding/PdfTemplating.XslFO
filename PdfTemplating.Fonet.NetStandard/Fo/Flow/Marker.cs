namespace Fonet.Fo.Flow
{
    using Fonet.Layout;

    internal class Marker : FObjMixed
    {
        private string markerClassName;

        private Area registryArea;

        private bool isFirst;

        private bool isLast;

        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new Marker(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public Marker(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:marker";

            this.markerClassName =
                this.properties.GetProperty("marker-class-name").GetString();
            ts = propMgr.getTextDecoration(parent);

            try
            {
                parent.AddMarker(this.markerClassName);
            }
            catch (FonetException)
            {
            }
        }

        public override Status Layout(Area area)
        {
            this.registryArea = area;
            area.getPage().registerMarker(this);
            return new Status(Status.OK);
        }

        public Status LayoutMarker(Area area)
        {
            if (this.marker == MarkerStart)
            {
                this.marker = 0;
            }

            int numChildren = this.children.Count;
            for (int i = this.marker; i < numChildren; i++)
            {
                FONode fo = (FONode)children[i];

                Status status;
                if ((status = fo.Layout(area)).isIncomplete())
                {
                    this.marker = i;
                    return status;
                }
            }

            return new Status(Status.OK);
        }

        public string GetMarkerClassName()
        {
            return markerClassName;
        }

        public Area GetRegistryArea()
        {
            return registryArea;
        }

        public void releaseRegistryArea()
        {
            isFirst = registryArea.isFirst();
            isLast = registryArea.isLast();
            registryArea = null;
        }

        public void resetMarker()
        {
            if (registryArea != null)
            {
                Page page = registryArea.getPage();
                if (page != null)
                {
                    page.unregisterMarker(this);
                }
            }
        }

        public void resetMarkerContent()
        {
            base.ResetMarker();
        }

        public override bool MayPrecedeMarker()
        {
            return true;
        }
    }
}