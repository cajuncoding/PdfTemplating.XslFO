using System;
using System.Collections;
using Fonet.Pdf.Filter;
using Fonet.Pdf.Security;

namespace Fonet.Pdf
{
    public class PdfStream : PdfObject
    {
        protected byte[] data;

        protected PdfDictionary dictionary = new PdfDictionary();

        private IList filters;

        public PdfStream() { }

        public PdfStream(PdfObjectId objectId) : base(objectId) { }

        public PdfStream(byte[] data)
        {
            this.data = data;
        }

        public PdfStream(byte[] data, PdfObjectId objectId)
            : base(objectId)
        {
            this.data = data;
        }

        public void AddFilter(IFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            if (filters == null)
            {
                filters = new ArrayList();
            }
            filters.Add(filter);
        }

        private PdfObject FilterName
        {
            get
            {
                if (!HasFilters)
                {
                    return PdfNull.Null;
                }
                else if (filters.Count == 1)
                {
                    IFilter filter = (IFilter)filters[0];
                    return filter.Name;
                }
                else
                {
                    PdfArray names = new PdfArray();
                    foreach (IFilter filter in filters)
                    {
                        names.Add(filter.Name);
                    }
                    return names;
                }
            }
        }

        private PdfObject FilterDecodeParms
        {
            get
            {
                if (!HasFilters)
                {
                    return PdfNull.Null;
                }
                else if (filters.Count == 1)
                {
                    IFilter filter = (IFilter)filters[0];
                    return filter.DecodeParms;
                }
                else
                {
                    PdfArray decodeParams = new PdfArray();
                    foreach (IFilter filter in filters)
                    {
                        decodeParams.Add(filter.DecodeParms);
                    }
                    return decodeParams;
                }
            }
        }

        private bool HasFilters
        {
            get
            {
                if (filters != null)
                {
                    return filters.Count > 0;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool HasDecodeParams
        {
            get
            {
                if (filters == null)
                {
                    return false;
                }
                foreach (IFilter filter in filters)
                {
                    if (filter.HasDecodeParams)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private byte[] ApplyFilters(byte[] data)
        {
            if (filters == null)
            {
                return data;
            }

            byte[] encoded = data;
            for (int x = filters.Count - 1; x >= 0; x--)
            {
                IFilter filter = (IFilter)filters[x];
                encoded = filter.Encode(encoded);
            }
            return encoded;
        }

        protected internal override void Write(PdfWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (data == null)
            {
                throw new InvalidOperationException("No data for stream.");
            }

            // Prepare the stream's data.
            byte[] bytes = (byte[])data.Clone();

            // Apply any filters.
            if (HasFilters)
            {
                bytes = ApplyFilters(data);
            }

            // Encrypt the data if required.
            SecurityManager sm = writer.SecurityManager;
            if (sm != null)
            {
                bytes = sm.Encrypt(bytes, writer.EnclosingIndirect.ObjectId);
            }

            // Create the stream's dictionary.
            dictionary[PdfName.Names.Length] = new PdfNumeric(bytes.Length);
            if (HasFilters)
            {
                dictionary[PdfName.Names.Filter] = FilterName;
                if (HasDecodeParams)
                {
                    dictionary[PdfName.Names.DecodeParams] = FilterDecodeParms;
                }
            }

            // Write out the dictionary.
            writer.WriteLine(dictionary);

            // Write out the stream data.
            writer.WriteKeywordLine(Keyword.Stream);
            writer.WriteLine(bytes);
            writer.WriteKeyword(Keyword.EndStream);
        }

    }
}