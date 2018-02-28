using System;
using System.Collections;
using Fonet.Layout;

namespace Fonet.Fo
{
    internal abstract class FONode
    {
        protected FObj parent;

        protected string areaClass = AreaClass.UNASSIGNED;

        protected ArrayList children = new ArrayList();

        public const int MarkerStart = -1000;

        public const int MarkerBreakAfter = -1001;

        protected int marker = MarkerStart;

        protected bool isInTableCell = false;

        protected int forcedStartOffset = 0;

        protected int forcedWidth = 0;

        protected int widows = 0;

        protected int orphans = 0;

        protected LinkSet linkSet;

        public int areasGenerated = 0;

        protected FONode(FObj parent)
        {
            this.parent = parent;

            if (null != parent)
            {
                this.areaClass = parent.areaClass;
            }
        }

        public virtual void SetIsInTableCell()
        {
            isInTableCell = true;
            foreach (FONode child in children)
            {
                child.SetIsInTableCell();
            }
        }

        public virtual void ForceStartOffset(int offset)
        {
            forcedStartOffset = offset;
            foreach (FONode child in children)
            {
                child.ForceStartOffset(offset);
            }
        }

        public virtual void ForceWidth(int width)
        {
            forcedWidth = width;
            foreach (FONode child in children)
            {
                child.ForceWidth(width);
            }
        }

        public virtual void ResetMarker()
        {
            marker = MarkerStart;
            foreach (FONode child in children)
            {
                child.ResetMarker();
            }
        }

        public void SetWidows(int wid)
        {
            widows = wid;
        }

        public void SetOrphans(int orph)
        {
            orphans = orph;
        }

        public void RemoveAreas()
        {
        }

        protected internal virtual void AddChild(FONode child)
        {
            children.Add(child);
        }

        public FObj getParent()
        {
            return parent;
        }

        public virtual void SetLinkSet(LinkSet linkSet)
        {
            this.linkSet = linkSet;
            foreach (FONode child in children)
            {
                child.SetLinkSet(linkSet);
            }
        }

        public virtual LinkSet GetLinkSet()
        {
            return linkSet;
        }

        public abstract Status Layout(Area area);

        public virtual Property GetProperty(String name)
        {
            return null;
        }

        public virtual ArrayList getMarkerSnapshot(ArrayList snapshot)
        {
            snapshot.Add(marker);

            if (marker < 0)
            {
                return snapshot;
            }
            else if (children.Count == 0)
            {
                return snapshot;
            }
            else
            {
                return ((FONode)children[this.marker]).getMarkerSnapshot(snapshot);
            }
        }

        public virtual void Rollback(ArrayList snapshot)
        {
            this.marker = (Int32)snapshot[0];
            snapshot.RemoveAt(0);

            if (this.marker == MarkerStart)
            {
                ResetMarker();
                return;
            }
            else if ((this.marker == -1) || children.Count == 0)
            {
                return;
            }

            if (this.marker <= MarkerStart)
            {
                return;
            }

            int numChildren = children.Count;
            for (int i = this.marker + 1; i < numChildren; i++)
            {
                FONode fo = (FONode)children[i];
                fo.ResetMarker();
            }
            ((FONode)children[this.marker]).Rollback(snapshot);
        }

        public virtual bool MayPrecedeMarker()
        {
            return false;
        }
    }
}