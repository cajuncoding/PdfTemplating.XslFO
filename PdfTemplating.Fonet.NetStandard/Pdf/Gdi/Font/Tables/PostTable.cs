using System;

namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     Class that represents the PostScript ('post') table
    /// </summary>
    /// <remarks>
    ///     http://www.microsoft.com/typography/otspec/post.htm
    /// </remarks>
    internal class PostTable : FontTable {
        /// <summary>
        ///     0x00010000 for version 1.0 
        ///     0x00020000 for version 2.0 
        ///     0x00025000 for version 2.5 (deprecated) 
        ///     0x00030000 for version 3.0 
        /// </summary>
        private int version;

        /// <summary>
        ///     Italic angle in counter-clockwise degrees from the vertical. 
        ///     Zero for upright text, negative for text that leans to the 
        ///     right (forward). 
        /// </summary>
        private float italicAngle;

        /// <summary>
        ///     This is the suggested distance of the top of the underline from 
        ///     the baseline (negative values indicate below baseline). 
        /// </summary>
        private short underlinePosition;

        /// <summary>
        ///     Suggested values for the underline thickness. 
        /// </summary>
        private short underlineThickness;

        /// <summary>
        ///     Set to 0 if the font is proportionally spaced, non-zero if the 
        ///     font is not proportionally spaced (i.e. monospaced). 
        /// </summary>
        private uint fixedPitch;

        /// <summary>
        ///     Minimum memory usage when an OpenType font is downloaded. 
        /// </summary>
        private uint minMemType42;

        /// <summary>
        ///     Maximum memory usage when an OpenType font is downloaded. 
        /// </summary>
        private uint maxMemType42;

        /// <summary>
        ///     Minimum memory usage when an OpenType font is downloaded 
        ///     as a Type 1 font. 
        /// </summary>
        private uint minMemType1;

        /// <summary>
        ///     Maximum memory usage when an OpenType font is downloaded 
        ///     as a Type 1 font. 
        /// </summary>
        private uint maxMemType1;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="entry"></param>
        public PostTable(DirectoryEntry entry) : base(TableNames.Post, entry) {}

        /// <summary>
        ///     Gets a boolean value that indicates whether this font is 
        ///     proportionally spaced (fixed pitch) or not.
        /// </summary>
        public bool IsFixedPitch {
            get { return (fixedPitch == 1); }
        }

        public float ItalicAngle {
            get { return italicAngle; }
        }

        /// <summary>
        ///     Reads the contents of the "post" table from the supplied stream 
        ///     at the current position.
        /// </summary>
        /// <param name="reader"></param>
        protected internal override void Read(FontFileReader reader) {
            FontFileStream stream = reader.Stream;
            version = stream.ReadFixed();

            // The italic angle is stored in the stupid fixed field format.
            italicAngle = (float) stream.ReadFixed()/65536.0f;

            underlinePosition = stream.ReadFWord();
            underlineThickness = stream.ReadFWord();
            fixedPitch = stream.ReadULong();
            minMemType42 = stream.ReadULong();
            maxMemType42 = stream.ReadULong();
            minMemType1 = stream.ReadULong();
            maxMemType1 = stream.ReadULong();
        }

        protected internal override void Write(FontFileWriter writer) {
            throw new NotImplementedException("Write is not implemented.");
        }
    }
}