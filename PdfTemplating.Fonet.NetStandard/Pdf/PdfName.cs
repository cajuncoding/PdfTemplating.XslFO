using System;
using System.IO;
using System.Text;

namespace Fonet.Pdf
{
    public sealed class PdfName : PdfObject
    {
        private string name;

        private byte[] bytes;

        public PdfName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this.name = name;
        }

        public PdfName(string name, PdfObjectId objectId)
            : base(objectId)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }

        protected internal override void Write(PdfWriter writer)
        {
            writer.Write(NameBytes);
        }

        private static readonly byte[] HexDigits = {
            0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
            0x38, 0x39, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66
        };

        private byte[] NameBytes
        {
            get
            {
                if (bytes == null)
                {
                    // Create a memory stream to hold the results.
                    // We guess the size, based on the most likely outcome
                    // (i.e. all ASCII characters with no escapes.
                    MemoryStream ms = new MemoryStream(name.Length + 1);

                    // The forward slash introduces a name.
                    ms.WriteByte((byte)'/');

                    // The PDF specification recommends encoding name objects using UTF8.
                    byte[] data = Encoding.UTF8.GetBytes(name);
                    for (int x = 0; x < data.Length; x++)
                    {
                        byte b = data[x];

                        // The PDF specification recommends using a special #hh syntax 
                        // for any bytes that are outside the range 33 to 126 and for
                        // the # character itself (35).
                        if (b < 34 || b > 125 || b == 35)
                        {
                            ms.WriteByte((byte)'#');
                            ms.WriteByte(HexDigits[b >> 4]);
                            ms.WriteByte(HexDigits[b & 0x0f]);
                        }
                        else
                        {
                            ms.WriteByte(b);
                        }
                    }
                    ms.Close();
                    bytes = ms.ToArray();
                }
                return bytes;
            }
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            PdfName pobj = obj as PdfName;
            if (pobj == null)
            {
                return false;
            }
            return name.Equals(pobj.Name);
        }

        //        public static bool operator ==(PdfName o1, PdfName o2) {
        //            return o1.Equals(o2);
        //        }
        //
        //        public static bool operator !=(PdfName o1, PdfName o2) {
        //            return !(o1 == o2);
        //        }

        /// <summary>
        ///     Well-known PDF name objects.
        /// </summary>
        public class Names
        {
            public static readonly PdfName Catalog = new PdfName("Catalog");
            public static readonly PdfName Type = new PdfName("Type");
            public static readonly PdfName Subtype = new PdfName("Subtype");
            public static readonly PdfName Pages = new PdfName("Pages");
            public static readonly PdfName Outlines = new PdfName("Outlines");
            public static readonly PdfName Kids = new PdfName("Kids");
            public static readonly PdfName Count = new PdfName("Count");

            public static readonly PdfName Title = new PdfName("Title");
            public static readonly PdfName Author = new PdfName("Author");
            public static readonly PdfName Subject = new PdfName("Subject");
            public static readonly PdfName Keywords = new PdfName("Keywords");
            public static readonly PdfName Creator = new PdfName("Creator");
            public static readonly PdfName Producer = new PdfName("Producer");
            public static readonly PdfName CreationDate = new PdfName("CreationDate");
            public static readonly PdfName ModDate = new PdfName("ModDate");

            public static readonly PdfName Size = new PdfName("Size");
            public static readonly PdfName Prev = new PdfName("Prev");
            public static readonly PdfName Root = new PdfName("Root");
            public static readonly PdfName Encrypt = new PdfName("Encrypt");
            public static readonly PdfName Info = new PdfName("Info");
            public static readonly PdfName Id = new PdfName("ID");

            public static readonly PdfName Encoding = new PdfName("Encoding");
            public static readonly PdfName BaseEncoding = new PdfName("BaseEncoding");
            public static readonly PdfName MacRomanEncoding = new PdfName("MacRomanEncoding");
            public static readonly PdfName MacExpertEncoding = new PdfName("MacExpertEncoding");
            public static readonly PdfName WinAnsiEncoding = new PdfName("WinAnsiEncoding");

            public static readonly PdfName FileSpec = new PdfName("FileSpec");
            public static readonly PdfName F = new PdfName("F");

            public static readonly PdfName Annot = new PdfName("Annot");
            public static readonly PdfName Action = new PdfName("Action");
            public static readonly PdfName Link = new PdfName("Link");
            public static readonly PdfName H = new PdfName("H");
            public static readonly PdfName I = new PdfName("I");
            public static readonly PdfName A = new PdfName("A");
            public static readonly PdfName Border = new PdfName("Border");
            public static readonly PdfName Rect = new PdfName("Rect");
            public static readonly PdfName C = new PdfName("C");
            public static readonly PdfName S = new PdfName("S");
            public static readonly PdfName GoTo = new PdfName("GoTo");
            public static readonly PdfName GoToR = new PdfName("GoToR");
            public static readonly PdfName D = new PdfName("D");
            public static readonly PdfName XYZ = new PdfName("XYZ");
            public static readonly PdfName URI = new PdfName("URI");

            public static readonly PdfName Font = new PdfName("Font");
            public static readonly PdfName FontName = new PdfName("FontName");
            public static readonly PdfName FontDescriptor = new PdfName("FontDescriptor");
            public static readonly PdfName Flags = new PdfName("Flags");
            public static readonly PdfName FontBBox = new PdfName("FontBBox");
            public static readonly PdfName ItalicAngle = new PdfName("ItalicAngle");
            public static readonly PdfName Ascent = new PdfName("Ascent");
            public static readonly PdfName Descent = new PdfName("Descent");
            public static readonly PdfName Leading = new PdfName("Leading");
            public static readonly PdfName CapHeight = new PdfName("CapHeight");
            public static readonly PdfName XHeight = new PdfName("XHeight");
            public static readonly PdfName StemV = new PdfName("StemV");
            public static readonly PdfName StemH = new PdfName("StemH");
            public static readonly PdfName AvgWidth = new PdfName("AvgWidth");
            public static readonly PdfName MaxWidth = new PdfName("MaxWidth");
            public static readonly PdfName MissingWidth = new PdfName("MissingWidth");
            public static readonly PdfName FontFile = new PdfName("FontFile");
            public static readonly PdfName FontFile2 = new PdfName("FontFile2");
            public static readonly PdfName FontFile3 = new PdfName("FontFile3");
            public static readonly PdfName CharSet = new PdfName("CharSet");
            public static readonly PdfName CIDToGIDMap = new PdfName("CIDToGIDMap");
            public static readonly PdfName Identity = new PdfName("Identity");

            public static readonly PdfName Length1 = new PdfName("Length1");
            public static readonly PdfName Length2 = new PdfName("Length2");
            public static readonly PdfName Length3 = new PdfName("Length3");

            public static readonly PdfName ToUnicode = new PdfName("ToUnicode");
            public static readonly PdfName CMap = new PdfName("CMap");
            public static readonly PdfName CMapName = new PdfName("CMapName");
            public static readonly PdfName WMode = new PdfName("WMode");

            public static readonly PdfName Type0 = new PdfName("Type0");
            public static readonly PdfName Type1 = new PdfName("Type1");
            public static readonly PdfName TrueType = new PdfName("TrueType");
            public static readonly PdfName Name = new PdfName("Name");
            public static readonly PdfName BaseFont = new PdfName("BaseFont");
            public static readonly PdfName XObject = new PdfName("XObject");

            public static readonly PdfName CIDFontType0 = new PdfName("CIDFontType0");
            public static readonly PdfName CIDFontType2 = new PdfName("CIDFontType2");
            public static readonly PdfName CIDSystemInfo = new PdfName("CIDSystemInfo");
            public static readonly PdfName DescendantFonts = new PdfName("DescendantFonts");

            public static readonly PdfName Registry = new PdfName("Registry");
            public static readonly PdfName Ordering = new PdfName("Ordering");
            public static readonly PdfName Supplement = new PdfName("Supplement");

            public static readonly PdfName DW = new PdfName("DW");
            public static readonly PdfName W = new PdfName("W");

            public static readonly PdfName Page = new PdfName("Page");
            public static readonly PdfName PageMode = new PdfName("PageMode");
            public static readonly PdfName UseOutlines = new PdfName("UseOutlines");
            public static readonly PdfName Resources = new PdfName("Resources");
            public static readonly PdfName Contents = new PdfName("Contents");
            public static readonly PdfName MediaBox = new PdfName("MediaBox");
            public static readonly PdfName Parent = new PdfName("Parent");
            public static readonly PdfName Annots = new PdfName("Annots");

            public static readonly PdfName Image = new PdfName("Image");
            public static readonly PdfName Width = new PdfName("Width");
            public static readonly PdfName Height = new PdfName("Height");
            public static readonly PdfName BitsPerComponent = new PdfName("BitsPerComponent");
            public static readonly PdfName ColorSpace = new PdfName("ColorSpace");

            public static readonly PdfName ProcSet = new PdfName("ProcSet");
            public static readonly PdfName PDF = new PdfName("PDF");
            public static readonly PdfName Text = new PdfName("Text");
            public static readonly PdfName ImageB = new PdfName("ImageB");
            public static readonly PdfName ImageC = new PdfName("ImageC");
            public static readonly PdfName ImageI = new PdfName("ImageI");

            public static readonly PdfName Length = new PdfName("Length");
            public static readonly PdfName Filter = new PdfName("Filter");
            public static readonly PdfName DecodeParams = new PdfName("DecodeParams");

            public static readonly PdfName ASCII85Decode = new PdfName("ASCII85Decode");
            public static readonly PdfName ASCIIHexDecode = new PdfName("ASCIIHexDecode");
            public static readonly PdfName CCITTFaxDecode = new PdfName("CCITTFaxDecode");
            public static readonly PdfName DCTDecode = new PdfName("DCTDecode");
            public static readonly PdfName FlateDecode = new PdfName("FlateDecode");
            public static readonly PdfName JBIG2Decode = new PdfName("JBIG2Decode");
            public static readonly PdfName LZWDecode = new PdfName("LZWDecode");
            public static readonly PdfName RunLengthDecode = new PdfName("RunLengthDecode");

            public static readonly PdfName Standard = new PdfName("Standard");
            public static readonly PdfName V = new PdfName("V");
            public static readonly PdfName R = new PdfName("R");
            public static readonly PdfName O = new PdfName("O");
            public static readonly PdfName U = new PdfName("U");
            public static readonly PdfName P = new PdfName("P");

            public static readonly PdfName FirstChar = new PdfName("FirstChar");
            public static readonly PdfName LastChar = new PdfName("LastChar");
            public static readonly PdfName Widths = new PdfName("Widths");

            public static readonly PdfName First = new PdfName("First");
            public static readonly PdfName Last = new PdfName("Last");
            public static readonly PdfName Next = new PdfName("Next");

            public static readonly PdfName Alternate = new PdfName("Alternate");
            public static readonly PdfName ICCBased = new PdfName("ICCBased");
            public static readonly PdfName N = new PdfName("N");
        }
    }
}