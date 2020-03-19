namespace Fonet.Pdf
{
    using System;
    using System.Collections;
    using System.Drawing;
    using System.IO;
    using Fonet.DataTypes;
    using Fonet.Image;
    using Fonet.Layout;
    using Fonet.Pdf.Filter;
    using Fonet.Pdf.Security;
    using Fonet.Render.Pdf;

    internal sealed class PdfCreator
    {
        private PdfDocument doc;

        // list of objects to write in the trailer.
        private ArrayList trailerObjects = new ArrayList();

        // the objects themselves
        // These objects are buffered and then written to the
        // PDF stream after each page has been rendered.  Adding
        // an object to this array does not mean it is ready for
        // writing, but it does mean that there is no need to 
        // wait until the end of the PDF stream.  The trigger
        // to write these objects out is pulled by PdfRenderer,
        // at the end of it's render page method.
        private ArrayList objects = new ArrayList();

        // The root outline object
        private PdfOutline outlineRoot;

        // the /Resources object
        private PdfResources resources;

        // the documents idReferences
        private IDReferences idReferences;

        // the XObjects Map.
        private Hashtable xObjectsMap = new Hashtable();

        // The cross-reference table.
        private XRefTable xrefTable;

        // The PDF information dictionary.
        private PdfInfo info;

        // The PDF encryption dictionary.
        private PdfDictionary encrypt;

        // Links wiating for internal document references
        //private ArrayList pendingLinks;

        public PdfCreator(Stream stream)
        {
            // Create the underlying PDF document.
            doc = new PdfDocument(stream);
            doc.Version = PdfVersion.V13;

            resources = new PdfResources(doc.NextObjectId());
            addTrailerObject(resources);
            this.xrefTable = new XRefTable();
        }

        public void setIDReferences(IDReferences idReferences)
        {
            this.idReferences = idReferences;
        }

        public PdfDocument Doc
        {
            get
            {
                return doc;
            }
        }

        public void AddObject(PdfObject obj)
        {
            objects.Add(obj);
        }

        public PdfXObject AddImage(FonetImage img)
        {
            // check if already created
            string url = img.Uri;
            PdfXObject xObject = (PdfXObject)this.xObjectsMap[url];
            if (xObject == null)
            {
                PdfICCStream iccStream = null;

                ColorSpace cs = img.ColorSpace;
                if (cs.HasICCProfile())
                {
                    iccStream = new PdfICCStream(doc.NextObjectId(), cs.GetICCProfile());
                    iccStream.NumComponents = new PdfNumeric(cs.GetNumComponents());
                    iccStream.AddFilter(new FlateFilter());
                    this.objects.Add(iccStream);
                }

                // else, create a new one
                PdfName name = new PdfName("XO" + xObjectsMap.Count);
                xObject = new PdfXObject(img.Bitmaps, name, doc.NextObjectId());
                xObject.SubType = PdfName.Names.Image;
                xObject.Dictionary[PdfName.Names.Width] = new PdfNumeric(img.Width);
                xObject.Dictionary[PdfName.Names.Height] = new PdfNumeric(img.Height);
                xObject.Dictionary[PdfName.Names.BitsPerComponent] = new PdfNumeric(img.BitsPerPixel);

                // Check for ICC color space
                if (iccStream != null)
                {
                    PdfArray ar = new PdfArray();
                    ar.Add(PdfName.Names.ICCBased);
                    ar.Add(iccStream.GetReference());

                    xObject.Dictionary[PdfName.Names.ColorSpace] = ar;
                }
                else
                {
                    xObject.Dictionary[PdfName.Names.ColorSpace] = new PdfName(img.ColorSpace.GetColorSpacePDFString());
                }

                xObject.AddFilter(img.Filter);

                this.objects.Add(xObject);
                this.xObjectsMap.Add(url, xObject);
            }
            return xObject;
        }

        public PdfPage makePage(PdfResources resources, PdfContentStream contents,
                                int pagewidth, int pageheight, Page currentPage)
        {
            PdfPage page = new PdfPage(
                resources, contents,
                pagewidth, pageheight,
                doc.NextObjectId());

            if (currentPage != null)
            {
                foreach (string id in currentPage.getIDList())
                {
                    idReferences.setInternalGoToPageReference(id, page.GetReference());
                }
            }

            /* add it to the list of objects */
            this.objects.Add(page);

            page.SetParent(doc.Pages);
            doc.Pages.Kids.Add(page.GetReference());

            return page;
        }

        public PdfLink makeLink(Rectangle rect, string destination, int linkType)
        {
            PdfLink link = new PdfLink(doc.NextObjectId(), rect);
            this.objects.Add(link);

            if (linkType == LinkSet.EXTERNAL)
            {
                if (destination.EndsWith(".pdf"))
                { // FileSpec
                    PdfFileSpec fileSpec = new PdfFileSpec(doc.NextObjectId(), destination);
                    this.objects.Add(fileSpec);
                    PdfGoToRemote gotoR = new PdfGoToRemote(fileSpec, doc.NextObjectId());
                    this.objects.Add(gotoR);
                    link.SetAction(gotoR);
                }
                else
                { // URI
                    PdfUri uri = new PdfUri(destination);
                    link.SetAction(uri);
                }
            }
            else
            {
                PdfObjectReference goToReference = getGoToReference(destination);
                PdfInternalLink internalLink = new PdfInternalLink(goToReference);
                link.SetAction(internalLink);
            }
            return link;
        }

        private PdfObjectReference getGoToReference(string destination)
        {
            PdfGoTo goTo;
            // Have we seen this 'id' in the document yet?
            if (idReferences.doesIDExist(destination))
            {
                if (idReferences.doesGoToReferenceExist(destination))
                {
                    goTo = idReferences.getInternalLinkGoTo(destination);
                }
                else
                {
                    goTo = idReferences.createInternalLinkGoTo(destination, doc.NextObjectId());
                    addTrailerObject(goTo);
                }
            }
            else
            {
                // id was not found, so create it
                idReferences.CreateUnvalidatedID(destination);
                idReferences.AddToIdValidationList(destination);
                goTo = idReferences.createInternalLinkGoTo(destination, doc.NextObjectId());
                addTrailerObject(goTo);
            }
            return goTo.GetReference();
        }

        private void addTrailerObject(PdfObject obj)
        {
            this.trailerObjects.Add(obj);
        }

        public PdfContentStream makeContentStream()
        {
            PdfContentStream obj = new PdfContentStream(doc.NextObjectId());
            obj.AddFilter(new FlateFilter());
            this.objects.Add(obj);
            return obj;
        }

        public PdfAnnotList makeAnnotList()
        {
            PdfAnnotList obj = new PdfAnnotList(doc.NextObjectId());
            this.objects.Add(obj);
            return obj;
        }

        public void SetOptions(PdfRendererOptions options)
        {
            // Configure the /Info dictionary.
            info = new PdfInfo(doc.NextObjectId());
            if (options.Title != null)
            {
                info.Title = new PdfString(options.Title);
            }
            if (options.Author != null)
            {
                info.Author = new PdfString(options.Author);
            }
            if (options.Subject != null)
            {
                info.Subject = new PdfString(options.Subject);
            }
            if (options.Keywords != String.Empty)
            {
                info.Keywords = new PdfString(options.Keywords);
            }
            if (options.Creator != null)
            {
                info.Creator = new PdfString(options.Creator);
            }
            if (options.Producer != null)
            {
                info.Producer = new PdfString(options.Producer);
            }
            info.CreationDate = new PdfString(PdfDate.Format(DateTime.Now));
            this.objects.Add(info);

            // Configure the security options.
            if (options.UserPassword != null ||
                options.OwnerPassword != null ||
                options.HasPermissions)
            {
                SecurityOptions securityOptions = new SecurityOptions();
                securityOptions.UserPassword = options.UserPassword;
                securityOptions.OwnerPassword = options.OwnerPassword;
                securityOptions.EnableAdding(options.EnableAdd);
                securityOptions.EnableChanging(options.EnableModify);
                securityOptions.EnableCopying(options.EnableCopy);
                securityOptions.EnablePrinting(options.EnablePrinting);

                doc.SecurityOptions = securityOptions;
                encrypt = doc.Writer.SecurityManager.GetEncrypt(doc.NextObjectId());
                this.objects.Add(encrypt);
            }

        }

        public PdfOutline getOutlineRoot()
        {
            if (outlineRoot != null)
            {
                return outlineRoot;
            }

            outlineRoot = new PdfOutline(doc.NextObjectId(), null, null);
            addTrailerObject(outlineRoot);
            doc.Catalog.Outlines = outlineRoot;
            return outlineRoot;
        }

        public PdfOutline makeOutline(PdfOutline parent, string label,
                                      string destination)
        {
            PdfObjectReference goToRef = getGoToReference(destination);

            PdfOutline obj = new PdfOutline(doc.NextObjectId(), label, goToRef);

            if (parent != null)
            {
                parent.AddOutline(obj);
            }
            this.objects.Add(obj);
            return obj;

        }

        public PdfResources getResources()
        {
            return this.resources;
        }

        private void WritePdfObject(PdfObject obj)
        {
            xrefTable.Add(obj.ObjectId, doc.Writer.Position);
            doc.Writer.WriteLine(obj);
        }

        public void output()
        {
            foreach (PdfObject obj in this.objects)
            {
                WritePdfObject(obj);
            }
            objects.Clear();
        }

        public void outputHeader()
        {
            doc.WriteHeader();
        }

        public void outputTrailer()
        {
            output();

            foreach (PdfXObject xobj in xObjectsMap.Values)
            {
                resources.AddXObject(xobj);
            }

            xrefTable.Add(doc.Catalog.ObjectId, doc.Writer.Position);
            doc.Writer.WriteLine(doc.Catalog);

            xrefTable.Add(doc.Pages.ObjectId, doc.Writer.Position);
            doc.Writer.WriteLine(doc.Pages);

            foreach (PdfObject o in trailerObjects)
            {
                WritePdfObject(o);
            }

            // output the xref table
            long xrefOffset = doc.Writer.Position;
            xrefTable.Write(doc.Writer);

            // output the file trailer
            PdfFileTrailer trailer = new PdfFileTrailer();
            trailer.Size = new PdfNumeric(doc.ObjectCount + 1);
            trailer.Root = doc.Catalog.GetReference();
            trailer.Id = doc.FileIdentifier;
            if (info != null)
            {
                trailer.Info = info.GetReference();
            }
            if (info != null && encrypt != null)
            {
                trailer.Encrypt = encrypt.GetReference();
            }
            trailer.XRefOffset = xrefOffset;
            doc.Writer.Write(trailer);
        }
    }
}