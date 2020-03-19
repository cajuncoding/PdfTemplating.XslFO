using System;
using System.Collections;

namespace Fonet.Pdf
{
    public class PdfDictionary : PdfObject, IEnumerable
    {
        protected Hashtable entries = new Hashtable();

        public PdfDictionary()
        {
        }

        public PdfDictionary(PdfObjectId objectId)
            : base(objectId)
        {
        }

        public void Add(PdfName key, PdfObject value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (entries.ContainsKey(key))
            {
                throw new ArgumentException("Already contains entry " + key);
            }

            entries.Add(key, value);
        }

        public void Clear()
        {
            entries.Clear();
        }

        public bool Contains(PdfName key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            return entries.ContainsKey(key);
        }

        public void Remove(PdfName key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            entries.Remove(key);
        }

        public IEnumerator GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        public PdfObject this[PdfName key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                return (PdfObject)entries[key];
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                entries[key] = value;
            }
        }

        public ICollection Keys
        {
            get
            {
                return entries.Keys;
            }
        }

        public ICollection Values
        {
            get
            {
                return entries.Values;
            }
        }

        public int Count
        {
            get
            {
                return entries.Count;
            }
        }

        protected internal override void Write(PdfWriter writer)
        {
            writer.WriteKeywordLine(Keyword.DictionaryBegin);
            foreach (DictionaryEntry e in entries)
            {
                writer.Write((PdfName)e.Key);
                writer.WriteSpace();
                writer.WriteLine((PdfObject)e.Value);
            }
            writer.WriteKeyword(Keyword.DictionaryEnd);
        }

    }
}