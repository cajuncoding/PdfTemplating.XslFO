using System;
using System.IO;
using Fonet.Pdf.Gdi.Font;

namespace Fonet.Pdf.Gdi {
    /// <summary>
    ///     Retrieves all pertinent TrueType tables by invoking GetFontData.
    /// </summary>
    public class GdiFontCreator {
        private const int NumTables = 11;

        private GdiDeviceContent dc;
        private TrueTypeHeader header;
        private MemoryStream ms;
        private FontFileStream fs;

        private int offset;

        public GdiFontCreator(GdiDeviceContent dc) {
            this.dc = dc;
            this.header = new TrueTypeHeader();
            this.ms = new MemoryStream();
            this.fs = new FontFileStream(ms);
        }

        public byte[] Build() {
            byte[] headData = ReadTableData(TableNames.Head);
            byte[] maxpData = ReadTableData(TableNames.Maxp);
            byte[] hheaData = ReadTableData(TableNames.Hhea);
            byte[] hmtxData = ReadTableData(TableNames.Hmtx);
            byte[] cvtData = ReadTableData(TableNames.Cvt);
            byte[] prepData = ReadTableData(TableNames.Prep);
            byte[] fpgmData = ReadTableData(TableNames.Fpgm);
            byte[] glyfData = ReadTableData(TableNames.Glyf);
            byte[] locaData = ReadTableData(TableNames.Loca);
            byte[] os2Data = ReadTableData(TableNames.Os2);
            byte[] postData = ReadTableData(TableNames.Post);

            // Write TrueType header
            fs.WriteFixed(0x00010000); // sfnt Version
            fs.WriteUShort(11);
            fs.WriteUShort(0); // search range
            fs.WriteUShort(0); // entry selector
            fs.WriteUShort(0); // range shift

            // Offsets begin from end of table directory
            offset = (int) fs.Position + NumTables*(PrimitiveSizes.ULong*4);

            // Write directory entry for each table
            WriteDirectoryEntry(TableNames.Head, headData);
            WriteDirectoryEntry(TableNames.Maxp, maxpData);
            WriteDirectoryEntry(TableNames.Hhea, hheaData);
            WriteDirectoryEntry(TableNames.Hmtx, hmtxData);
            WriteDirectoryEntry(TableNames.Cvt, cvtData);
            WriteDirectoryEntry(TableNames.Prep, prepData);
            WriteDirectoryEntry(TableNames.Fpgm, fpgmData);
            WriteDirectoryEntry(TableNames.Glyf, glyfData);
            WriteDirectoryEntry(TableNames.Loca, locaData);
            WriteDirectoryEntry(TableNames.Os2, os2Data);
            WriteDirectoryEntry(TableNames.Post, postData);

            fs.Write(headData, 0, headData.Length);
            fs.Write(maxpData, 0, maxpData.Length);
            fs.Write(hheaData, 0, hheaData.Length);
            fs.Write(hmtxData, 0, hmtxData.Length);
            fs.Write(cvtData, 0, cvtData.Length);
            fs.Write(prepData, 0, prepData.Length);
            fs.Write(fpgmData, 0, fpgmData.Length);
            fs.Write(glyfData, 0, glyfData.Length);
            fs.Write(locaData, 0, locaData.Length);
            fs.Write(os2Data, 0, os2Data.Length);
            fs.Write(postData, 0, postData.Length);

            return ms.ToArray();
        }

        private void WriteTable(byte[] data) {
            fs.Write(data, 0, data.Length);

            // Align table on 4-byte boundary
            fs.Pad();
        }

        private void WriteDirectoryEntry(string tableName, byte[] data) {
            fs.WriteByte((byte) tableName[0]);
            fs.WriteByte((byte) tableName[1]);
            fs.WriteByte((byte) tableName[2]);
            fs.WriteByte((byte) tableName[3]);
            fs.WriteULong(0);
            fs.WriteULong((uint) offset);
            fs.WriteULong((uint) data.Length);

            offset += data.Length;
        }

        private byte[] ReadTableData(string tableName) {
            uint tag = TableNames.ToUint(tableName);
            uint size = LibWrapper.GetFontData(dc.Handle, tag, 0, null, 0);

            byte[] data = new byte[size];
            uint rv = LibWrapper.GetFontData(dc.Handle, tag, 0, data, (uint) data.Length);
            if (rv == GdiFontMetrics.GDI_ERROR) {
                throw new Exception("Failed to retrieve table " + tableName);
            }

            return data;
        }
    }
}