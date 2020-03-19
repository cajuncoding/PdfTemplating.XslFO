namespace Fonet
{

    using System.Collections;
    using Fonet.Apps;
    using Fonet.DataTypes;
    using Fonet.Fo.Flow;
    using Fonet.Fo.Pagination;
    using Fonet.Layout;
    using Fonet.Render.Pdf;

    /// <summary>
    ///     This class acts as a bridge between the XML:FO parser and the 
    ///     formatting/rendering classes. It will queue PageSequences up until 
    ///     all the IDs required by them are satisfied, at which time it will 
    ///     render the pages.
    ///     StreamRenderer is created by Driver and called from FOTreeBuilder 
    ///     when a PageSequence is created, and AreaTree when a Page is formatted.
    /// </summary>
    internal class StreamRenderer
    {
        /// <summary>
        ///     Keep track of the number of pages rendered.
        /// </summary>
        private int pageCount = 0;

        /// <summary>
        ///     The renderer being used.
        /// </summary>
        private PdfRenderer renderer;

        /// <summary>
        ///     The formatting results to be handed back to the caller.
        /// </summary>
        private FormattingResults results = new FormattingResults();

        /// <summary>
        ///     The FontInfo for this renderer.
        /// </summary>
        private FontInfo fontInfo = new FontInfo();

        /// <summary>
        ///     The list of pages waiting to be renderered.
        /// </summary>
        private ArrayList renderQueue = new ArrayList();

        /// <summary>
        ///     The current set of IDReferences, passed to the areatrees 
        ///     and pages. This is used by the AreaTree as a single map of 
        ///     all IDs.
        /// </summary>
        private IDReferences idReferences = new IDReferences();

        /// <summary>
        ///     The list of markers.
        /// </summary>
        private ArrayList documentMarkers;

        private ArrayList currentPageSequenceMarkers;
        private PageSequence currentPageSequence;

        public StreamRenderer(PdfRenderer renderer)
        {
            this.renderer = renderer;
        }

        public IDReferences GetIDReferences()
        {
            return idReferences;
        }

        public FormattingResults getResults()
        {
            return this.results;
        }

        public void StartRenderer()
        {
            pageCount = 0;

            renderer.SetupFontInfo(fontInfo);
            renderer.StartRenderer();
        }

        public void StopRenderer()
        {
            // Force the processing of any more queue elements, even if they 
            // are not resolved.
            ProcessQueue(true);
            renderer.StopRenderer();
        }

        /// <summary>
        ///     Format the PageSequence. The PageSequence formats Pages and adds 
        ///     them to the AreaTree, which subsequently calls the StreamRenderer
        ///     instance (this) again to render the page.  At this time the page 
        ///     might be printed or it might be queued. A page might not be 
        ///     renderable immediately if the IDReferences are not all valid. In 
        ///     this case we defer the rendering until they are all valid.
        /// </summary>
        /// <param name="pageSequence"></param>
        public void Render(PageSequence pageSequence)
        {
            AreaTree a = new AreaTree(this);
            a.setFontInfo(fontInfo);

            var sw = System.Diagnostics.Stopwatch.StartNew();

            pageSequence.Format(a);

            FonetDriver.ActiveDriver.FireFonetInfo(string.Format("Rendered Page Sequence Output in [{0}] seconds.", sw.Elapsed.TotalSeconds));


            this.results.HaveFormattedPageSequence(pageSequence);

            FonetDriver.ActiveDriver.FireFonetInfo(
                "Last page-sequence produced " + pageSequence.PageCount + " page(s).");
        }

        public void QueuePage(Page page)
        {
            // Process markers
            PageSequence pageSequence = page.getPageSequence();
            if (pageSequence != currentPageSequence)
            {
                currentPageSequence = pageSequence;
                currentPageSequenceMarkers = null;
            }
            ArrayList markers = page.getMarkers();
            if (markers != null)
            {
                if (documentMarkers == null)
                {
                    documentMarkers = new ArrayList();
                }
                if (currentPageSequenceMarkers == null)
                {
                    currentPageSequenceMarkers = new ArrayList();
                }
                for (int i = 0; i < markers.Count; i++)
                {
                    Marker marker = (Marker)markers[i];
                    marker.releaseRegistryArea();
                    currentPageSequenceMarkers.Add(marker);
                    documentMarkers.Add(marker);
                }
            }


            // Try to optimise on the common case that there are no pages pending 
            // and that all ID references are valid on the current pages. This 
            // short-cuts the pipeline and renders the area immediately.
            if ((renderQueue.Count == 0) && idReferences.IsEveryIdValid())
            {
                renderer.Render(page);
            }
            else
            {
                AddToRenderQueue(page);
            }

            pageCount++;
        }

        private void AddToRenderQueue(Page page)
        {
            RenderQueueEntry entry = new RenderQueueEntry(this, page);
            renderQueue.Add(entry);

            // The just-added entry could (possibly) resolve the waiting entries, 
            // so we try to process the queue now to see.
            ProcessQueue(false);
        }

        /// <summary>
        ///     Try to process the queue from the first entry forward.  If an 
        ///     entry can't be processed, then the queue can't move forward, 
        ///     so return.
        /// </summary>
        /// <param name="force"></param>
        private void ProcessQueue(bool force)
        {
            while (renderQueue.Count > 0)
            {
                RenderQueueEntry entry = (RenderQueueEntry)renderQueue[0];
                if ((!force) && (!entry.isResolved()))
                {
                    break;
                }

                renderer.Render(entry.getPage());
                renderQueue.RemoveAt(0);
            }
        }

        /// <summary>
        ///     A RenderQueueEntry consists of the Page to be queued, plus a list 
        ///     of outstanding ID references that need to be resolved before the 
        ///     Page can be renderered.
        /// </summary>
        private class RenderQueueEntry
        {
            /// <summary>
            ///     The Page that has outstanding ID references.
            /// </summary>
            private Page page;

            private StreamRenderer outer;

            /// <summary>
            ///     A list of ID references (names).
            /// </summary>
            private ArrayList unresolvedIdReferences = new ArrayList();

            public RenderQueueEntry(StreamRenderer outer, Page page)
            {
                this.outer = outer;
                this.page = page;

                foreach (object o in outer.idReferences.getInvalidElements())
                {
                    unresolvedIdReferences.Add(o);
                }
            }

            public Page getPage()
            {
                return page;
            }

            /// <summary>
            ///     See if the outstanding references are resolved in the current 
            ///     copy of IDReferences.
            /// </summary>
            /// <returns></returns>
            public bool isResolved()
            {
                if ((unresolvedIdReferences.Count == 0) || outer.idReferences.IsEveryIdValid())
                {
                    return true;
                }

                // See if any of the unresolved references are still unresolved.
                foreach (string s in unresolvedIdReferences)
                {
                    if (!outer.idReferences.doesIDExist(s))
                    {
                        return false;
                    }
                }

                unresolvedIdReferences.RemoveRange(0, unresolvedIdReferences.Count);
                return true;
            }
        }

        /// <summary>
        ///     Auxillary function for retrieving markers.
        /// </summary>
        /// <returns></returns>
        public ArrayList GetDocumentMarkers()
        {
            return documentMarkers;
        }

        /// <summary>
        ///     Auxillary function for retrieving markers.
        /// </summary>
        /// <returns></returns>
        public PageSequence GetCurrentPageSequence()
        {
            return currentPageSequence;
        }

        /// <summary>
        ///     Auxillary function for retrieving markers.
        /// </summary>
        /// <returns></returns>
        public ArrayList GetCurrentPageSequenceMarkers()
        {
            return currentPageSequenceMarkers;
        }
    }
}