using System.IO;
using Fonet.Pdf.Security;

namespace Fonet.Pdf
{
    /// <summary>
    ///     A class that enables a well structured PDF document to be generated.
    /// </summary>
    /// <remarks>
    ///     Responsible for allocating object identifiers.
    /// </remarks>
    public class PdfDocument
    {
        private PdfWriter writer;

        private PdfVersion version = PdfVersion.V14;

        private FileIdentifier fileId = new FileIdentifier();

        private PdfCatalog catalog;

        private PdfPageTree pages;

        private uint nextObjectNumber = 1;

        public PdfDocument(Stream stream) : this(new PdfWriter(stream)) { }

        public PdfDocument(PdfWriter writer)
        {
            this.writer = writer;
            this.catalog = new PdfCatalog(NextObjectId());
            this.pages = new PdfPageTree(NextObjectId());
            this.catalog.Pages = pages;
        }

        public PdfVersion Version
        {
            get { return version; }
            set { this.version = value; }
        }

        public FileIdentifier FileIdentifier
        {
            get { return fileId; }
            set { fileId = value; }
        }

        public SecurityOptions SecurityOptions
        {
            set { writer.SecurityManager = new SecurityManager(value, fileId); }
        }

        public PdfCatalog Catalog
        {
            get { return catalog; }
        }

        public PdfPageTree Pages
        {
            get { return pages; }
        }

        public PdfObjectId NextObjectId()
        {
            return new PdfObjectId(nextObjectNumber++, 0);
        }

        public uint ObjectCount
        {
            get { return nextObjectNumber - 1; }
        }

        public PdfWriter Writer
        {
            get { return this.writer; }
        }

        public void WriteHeader()
        {
            writer.WriteHeader(version);
            writer.WriteBinaryComment();
        }

    }
}