namespace Fonet.Fo.Flow
{
    using System.Collections;
    using Fonet.Layout;

    internal class Footnote : FObj
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new Footnote(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public Footnote(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:footnote";
        }

        public override Status Layout(Area area)
        {
            FONode inline = null;
            FONode fbody = null;
            if (this.marker == MarkerStart)
            {
                this.marker = 0;
            }
            int numChildren = this.children.Count;
            for (int i = this.marker; i < numChildren; i++)
            {
                FONode fo = (FONode)children[i];
                if (fo is Inline)
                {
                    inline = fo;
                    Status status = fo.Layout(area);
                    if (status.isIncomplete())
                    {
                        return status;
                    }
                }
                else if (inline != null && fo is FootnoteBody)
                {
                    fbody = fo;
                    if (area is BlockArea)
                    {
                        ((BlockArea)area).addFootnote((FootnoteBody)fbody);
                    }
                    else
                    {
                        Page page = area.getPage();
                        LayoutFootnote(page, (FootnoteBody)fbody, area);
                    }
                }
            }
            if (fbody == null)
            {
                FonetDriver.ActiveDriver.FireFonetWarning(
                    "No footnote-body in footnote");
            }
            if (area is BlockArea) { }
            return new Status(Status.OK);
        }

        public static bool LayoutFootnote(Page p, FootnoteBody fb, Area area)
        {
            try
            {
                BodyAreaContainer bac = p.getBody();
                AreaContainer footArea = bac.getFootnoteReferenceArea();
                footArea.setIDReferences(bac.getIDReferences());
                int basePos = footArea.GetCurrentYPosition()
                    - footArea.GetHeight();
                int oldHeight = footArea.GetHeight();
                if (area != null)
                {
                    footArea.setMaxHeight(area.getMaxHeight() - area.GetHeight()
                        + footArea.GetHeight());
                }
                else
                {
                    footArea.setMaxHeight(bac.getMaxHeight()
                        + footArea.GetHeight());
                }
                Status status = fb.Layout(footArea);
                if (status.isIncomplete())
                {
                    return false;
                }
                else
                {
                    if (area != null)
                    {
                        area.setMaxHeight(area.getMaxHeight()
                            - footArea.GetHeight() + oldHeight);
                    }
                    if (bac.getFootnoteState() == 0)
                    {
                        Area ar = bac.getMainReferenceArea();
                        DecreaseMaxHeight(ar, footArea.GetHeight() - oldHeight);
                        footArea.setYPosition(basePos + footArea.GetHeight());
                    }
                }
            }
            catch (FonetException)
            {
                return false;
            }
            return true;
        }

        protected static void DecreaseMaxHeight(Area ar, int change)
        {
            ar.setMaxHeight(ar.getMaxHeight() - change);
            ArrayList childs = ar.getChildren();
            foreach (object obj in childs)
            {
                if (obj is Area)
                {
                    Area childArea = (Area)obj;
                    DecreaseMaxHeight(childArea, change);
                }
            }
        }
    }
}