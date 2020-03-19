using System;

namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     List of all TrueType and OpenType tables
    /// </summary>
    internal abstract class TableNames {
        // TrueType font collection
        public const string Ttcf = "ttcf";

        // Required tables
        public const string Cmap = "cmap";
        public const string Head = "head";
        public const string Hhea = "hhea";
        public const string Hmtx = "hmtx";
        public const string Maxp = "maxp";
        public const string Name = "name";
        public const string Os2 = "OS/2";
        public const string Post = "post";

        // s Related to TrueType Outlines 
        public const string Cvt = "cvt ";
        public const string Fpgm = "fpgm";
        public const string Glyf = "glyf";
        public const string Loca = "loca";
        public const string Prep = "prep";

        // s Related to PostScript Outlines 
        public const string CFF = "CFF ";
        public const string VORG = "VORG";

        // s Related to Bitmap Glyphs 
        public const string BASE = "BASE";
        public const string GDEF = "GDEF";
        public const string GPOS = "GPOS";
        public const string GSUB = "GSUB";
        public const string JSTF = "JSTF";

        // Other OpenType s 
        public const string DSIG = "DSIG";
        public const string Gasp = "gasp";
        public const string Hdmx = "hdmx";
        public const string Kern = "kern";
        public const string LTSH = "LTSH";
        public const string PCLT = "PCLT";
        public const string VDMX = "VDMX";
        public const string Vhea = "vhea";
        public const string Vmtx = "vmtx";

        /// <summary>
        ///     Converts one of the predefined table names to an unsigned integer.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static uint ToUint(string tableName) {
            if (tableName == null) {
                throw new ArgumentNullException("tableName", "tableName cannot be null.");
            }
            if (tableName.Length != 4) {
                throw new ArgumentException("tableName must be 4 characters in length.");
            }

            return (uint) (((byte) tableName[3] << 24) |
                ((byte) tableName[2] << 16) |
                ((byte) tableName[1] << 8) |
                ((byte) tableName[0]));
        }
    }
}