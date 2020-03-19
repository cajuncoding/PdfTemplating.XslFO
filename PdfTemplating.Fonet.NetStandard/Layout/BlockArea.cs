using System.Collections;
using Fonet.Fo.Flow;
using Fonet.Render.Pdf;

namespace Fonet.Layout
{
    internal class BlockArea : Area
    {
        protected int startIndent;
        protected int endIndent;
        protected int textIndent;
        protected int lineHeight;
        protected int halfLeading;
        protected int align;
        protected int alignLastLine;
        protected LineArea currentLineArea;
        protected LinkSet currentLinkSet;
        protected bool hasLines = false;
        protected HyphenationProps hyphProps;
        protected ArrayList pendingFootnotes = null;

        public BlockArea(FontState fontState, int allocationWidth, int maxHeight,
                         int startIndent, int endIndent, int textIndent,
                         int align, int alignLastLine, int lineHeight)
            : base(fontState, allocationWidth, maxHeight)
        {
            this.startIndent = startIndent;
            this.endIndent = endIndent;
            this.textIndent = textIndent;
            this.contentRectangleWidth = allocationWidth - startIndent
                - endIndent;
            this.align = align;
            this.alignLastLine = alignLastLine;
            this.lineHeight = lineHeight;

            if (fontState != null)
            {
                this.halfLeading = (lineHeight - fontState.FontSize) / 2;
            }
        }

        public override void render(PdfRenderer renderer)
        {
            renderer.RenderBlockArea(this);
        }

        protected void addLineArea(LineArea la)
        {
            if (!la.isEmpty())
            {
                la.verticalAlign();
                this.addDisplaySpace(this.halfLeading);
                int size = la.GetHeight();
                this.addChild(la);
                this.increaseHeight(size);
                this.addDisplaySpace(this.halfLeading);
            }
            if (pendingFootnotes != null)
            {
                foreach (FootnoteBody fb in pendingFootnotes)
                {
                    Page page = getPage();
                    if (!Footnote.LayoutFootnote(page, fb, this))
                    {
                        page.addPendingFootnote(fb);
                    }
                }
                pendingFootnotes = null;
            }
        }

        public LineArea getCurrentLineArea()
        {
            if (currentHeight + lineHeight > maxHeight)
            {
                return null;
            }
            this.currentLineArea.changeHyphenation(hyphProps);
            this.hasLines = true;
            return this.currentLineArea;
        }

        public LineArea createNextLineArea()
        {
            if (this.hasLines)
            {
                this.currentLineArea.align(this.align);
                this.addLineArea(this.currentLineArea);
            }
            this.currentLineArea = new LineArea(fontState, lineHeight,
                                                halfLeading, allocationWidth,
                                                startIndent, endIndent,
                                                currentLineArea);
            this.currentLineArea.changeHyphenation(hyphProps);
            if (currentHeight + lineHeight > maxHeight)
            {
                return null;
            }
            return this.currentLineArea;
        }

        public void setupLinkSet(LinkSet ls)
        {
            if (ls != null)
            {
                this.currentLinkSet = ls;
                ls.setYOffset(currentHeight);
            }
        }

        public override void end()
        {
            if (this.hasLines)
            {
                this.currentLineArea.addPending();
                this.currentLineArea.align(this.alignLastLine);
                this.addLineArea(this.currentLineArea);
            }
        }

        public override void start()
        {
            currentLineArea = new LineArea(fontState, lineHeight, halfLeading,
                                           allocationWidth,
                                           startIndent + textIndent, endIndent,
                                           null);
        }

        public int getEndIndent()
        {
            return endIndent;
        }

        public int getStartIndent()
        {
            return startIndent;
        }

        public void setIndents(int startIndent, int endIndent)
        {
            this.startIndent = startIndent;
            this.endIndent = endIndent;
            this.contentRectangleWidth = allocationWidth - startIndent
                - endIndent;
        }

        public override int spaceLeft()
        {
            return maxHeight - currentHeight -
                (getPaddingTop() + getPaddingBottom()
                    + getBorderTopWidth() + getBorderBottomWidth());
        }

        public int getHalfLeading()
        {
            return halfLeading;
        }

        public void setHyphenation(HyphenationProps hyphProps)
        {
            this.hyphProps = hyphProps;
        }

        public void addFootnote(FootnoteBody fb)
        {
            if (pendingFootnotes == null)
            {
                pendingFootnotes = new ArrayList();
            }
            pendingFootnotes.Add(fb);
        }

    }
}