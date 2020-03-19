using System;

namespace Fonet.Pdf.Gdi {
    /// <summary>
    ///     Maps a Unicode character to a WinAnsi codepoint value.
    /// </summary>
    internal class WinAnsiMapping {
        /// <summary>
        ///     First column is codepoint value.  Second column is unicode value.
        /// </summary>
        private static readonly int[] winAnsiEncoding
            = {
                0x20, 0x0020, // space
                0x20, 0x00A0, // space
                0x21, 0x0021, // exclam
                0x22, 0x0022, // quotedbl
                0x23, 0x0023, // numbersign
                0x24, 0x0024, // dollar
                0x25, 0x0025, // percent
                0x26, 0x0026, // ampersand
                0x27, 0x0027, // quotesingle
                0x28, 0x0028, // parenleft
                0x29, 0x0029, // parenright
                0x2a, 0x002A, // asterisk
                0x2b, 0x002B, // plus
                0x2c, 0x002C, // comma
                0x2d, 0x002D, // hyphen
                0x2d, 0x00AD, // hyphen
                0x2e, 0x002E, // period
                0x2f, 0x002F, // slash
                0x30, 0x0030, // zero
                0x31, 0x0031, // one
                0x32, 0x0032, // two
                0x33, 0x0033, // three
                0x34, 0x0034, // four
                0x35, 0x0035, // five
                0x36, 0x0036, // six
                0x37, 0x0037, // seven
                0x38, 0x0038, // eight
                0x39, 0x0039, // nine
                0x3a, 0x003A, // colon
                0x3b, 0x003B, // semicolon
                0x3c, 0x003C, // less
                0x3d, 0x003D, // equal
                0x3e, 0x003E, // greater
                0x3f, 0x003F, // question
                0x40, 0x0040, // at
                0x41, 0x0041, // A
                0x42, 0x0042, // B
                0x43, 0x0043, // C
                0x44, 0x0044, // D
                0x45, 0x0045, // E
                0x46, 0x0046, // F
                0x47, 0x0047, // G
                0x48, 0x0048, // H
                0x49, 0x0049, // I
                0x4a, 0x004A, // J
                0x4b, 0x004B, // K
                0x4c, 0x004C, // L
                0x4d, 0x004D, // M
                0x4e, 0x004E, // N
                0x4f, 0x004F, // O
                0x50, 0x0050, // P
                0x51, 0x0051, // Q
                0x52, 0x0052, // R
                0x53, 0x0053, // S
                0x54, 0x0054, // T
                0x55, 0x0055, // U
                0x56, 0x0056, // V
                0x57, 0x0057, // W
                0x58, 0x0058, // X
                0x59, 0x0059, // Y
                0x5a, 0x005A, // Z
                0x5b, 0x005B, // bracketleft
                0x5c, 0x005C, // backslash
                0x5d, 0x005D, // bracketright
                0x5e, 0x005E, // asciicircum
                0x5f, 0x005F, // underscore
                0x60, 0x0060, // grave
                0x61, 0x0061, // a
                0x62, 0x0062, // b
                0x63, 0x0063, // c
                0x64, 0x0064, // d
                0x65, 0x0065, // e
                0x66, 0x0066, // f
                0x67, 0x0067, // g
                0x68, 0x0068, // h
                0x69, 0x0069, // i
                0x6a, 0x006A, // j
                0x6b, 0x006B, // k
                0x6c, 0x006C, // l
                0x6d, 0x006D, // m
                0x6e, 0x006E, // n
                0x6f, 0x006F, // o
                0x70, 0x0070, // p
                0x71, 0x0071, // q
                0x72, 0x0072, // r
                0x73, 0x0073, // s
                0x74, 0x0074, // t
                0x75, 0x0075, // u
                0x76, 0x0076, // v
                0x77, 0x0077, // w
                0x78, 0x0078, // x
                0x79, 0x0079, // y
                0x7a, 0x007A, // z
                0x7b, 0x007B, // braceleft
                0x7c, 0x007C, // bar
                0x7d, 0x007D, // braceright
                0x7e, 0x007E, // asciitilde
                0x80, 0x20AC, // Euro
                0x82, 0x201A, // quotesinglbase
                0x83, 0x0192, // florin
                0x84, 0x201E, // quotedblbase
                0x85, 0x2026, // ellipsis
                0x86, 0x2020, // dagger
                0x87, 0x2021, // daggerdbl
                0x88, 0x02C6, // circumflex
                0x89, 0x2030, // perthousand
                0x8a, 0x0160, // Scaron
                0x8b, 0x2039, // guilsinglleft
                0x8c, 0x0152, // OE
                0x8e, 0x017D, // Zcaron
                0x91, 0x2018, // quoteleft
                0x92, 0x2019, // quoteright
                0x93, 0x201C, // quotedblleft
                0x94, 0x201D, // quotedblright
                0x95, 0x2022, // bullet
                0x96, 0x2013, // endash
                0x97, 0x2014, // emdash
                0x98, 0x02DC, // tilde
                0x99, 0x2122, // trademark
                0x9a, 0x0161, // scaron
                0x9b, 0x203A, // guilsinglright
                0x9c, 0x0153, // oe
                0x9e, 0x017E, // zcaron
                0x9f, 0x0178, // Ydieresis
                0xa1, 0x00A1, // exclamdown
                0xa2, 0x00A2, // cent
                0xa3, 0x00A3, // sterling
                0xa4, 0x00A4, // currency
                0xa5, 0x00A5, // yen
                0xa6, 0x00A6, // brokenbar
                0xa7, 0x00A7, // section
                0xa8, 0x00A8, // dieresis
                0xa9, 0x00A9, // copyright
                0xaa, 0x00AA, // ordfeminine
                0xab, 0x00AB, // guillemotleft
                0xac, 0x00AC, // logicalnot
                0xae, 0x00AE, // registered
                0xaf, 0x00AF, // macron
                0xaf, 0x02C9, // macron
                0xb0, 0x00B0, // degree
                0xb1, 0x00B1, // plusminus
                0xb2, 0x00B2, // twosuperior
                0xb3, 0x00B3, // threesuperior
                0xb4, 0x00B4, // acute
                0xb5, 0x00B5, // mu
                0xb5, 0x03BC, // mu
                0xb6, 0x00B6, // paragraph
                0xb7, 0x00B7, // periodcentered
                0xb7, 0x2219, // periodcentered
                0xb8, 0x00B8, // cedilla
                0xb9, 0x00B9, // onesuperior
                0xba, 0x00BA, // ordmasculine
                0xbb, 0x00BB, // guillemotright
                0xbc, 0x00BC, // onequarter
                0xbd, 0x00BD, // onehalf
                0xbe, 0x00BE, // threequarters
                0xbf, 0x00BF, // questiondown
                0xc0, 0x00C0, // Agrave
                0xc1, 0x00C1, // Aacute
                0xc2, 0x00C2, // Acircumflex
                0xc3, 0x00C3, // Atilde
                0xc4, 0x00C4, // Adieresis
                0xc5, 0x00C5, // Aring
                0xc6, 0x00C6, // AE
                0xc7, 0x00C7, // Ccedilla
                0xc8, 0x00C8, // Egrave
                0xc9, 0x00C9, // Eacute
                0xca, 0x00CA, // Ecircumflex
                0xcb, 0x00CB, // Edieresis
                0xcc, 0x00CC, // Igrave
                0xcd, 0x00CD, // Iacute
                0xce, 0x00CE, // Icircumflex
                0xcf, 0x00CF, // Idieresis
                0xd0, 0x00D0, // Eth
                0xd1, 0x00D1, // Ntilde
                0xd2, 0x00D2, // Ograve
                0xd3, 0x00D3, // Oacute
                0xd4, 0x00D4, // Ocircumflex
                0xd5, 0x00D5, // Otilde
                0xd6, 0x00D6, // Odieresis
                0xd7, 0x00D7, // multiply
                0xd8, 0x00D8, // Oslash
                0xd9, 0x00D9, // Ugrave
                0xda, 0x00DA, // Uacute
                0xdb, 0x00DB, // Ucircumflex
                0xdc, 0x00DC, // Udieresis
                0xdd, 0x00DD, // Yacute
                0xde, 0x00DE, // Thorn
                0xdf, 0x00DF, // germandbls
                0xe0, 0x00E0, // agrave
                0xe1, 0x00E1, // aacute
                0xe2, 0x00E2, // acircumflex
                0xe3, 0x00E3, // atilde
                0xe4, 0x00E4, // adieresis
                0xe5, 0x00E5, // aring
                0xe6, 0x00E6, // ae
                0xe7, 0x00E7, // ccedilla
                0xe8, 0x00E8, // egrave
                0xe9, 0x00E9, // eacute
                0xea, 0x00EA, // ecircumflex
                0xeb, 0x00EB, // edieresis
                0xec, 0x00EC, // igrave
                0xed, 0x00ED, // iacute
                0xee, 0x00EE, // icircumflex
                0xef, 0x00EF, // idieresis
                0xf0, 0x00F0, // eth
                0xf1, 0x00F1, // ntilde
                0xf2, 0x00F2, // ograve
                0xf3, 0x00F3, // oacute
                0xf4, 0x00F4, // ocircumflex
                0xf5, 0x00F5, // otilde
                0xf6, 0x00F6, // odieresis
                0xf7, 0x00F7, // divide
                0xf8, 0x00F8, // oslash
                0xf9, 0x00F9, // ugrave
                0xfa, 0x00FA, // uacute
                0xfb, 0x00FB, // ucircumflex
                0xfc, 0x00FC, // udieresis
                0xfd, 0x00FD, // yacute
                0xfe, 0x00FE, // thorn
                0xff, 0x00FF, // ydieresis
            };

        public static readonly WinAnsiMapping Mapping = new WinAnsiMapping();

        private ushort[] latin1Map;

        private WinAnsiMapping() {
            latin1Map = new ushort[256];
            for (int i = 0; i < winAnsiEncoding.Length; i += 2) {
                if (winAnsiEncoding[i + 1] < 256) {
                    latin1Map[winAnsiEncoding[i + 1]] = (char) winAnsiEncoding[i];
                }
            }
        }

        public ushort MapCharacter(char c) {
            if (c > Byte.MaxValue) {
                return 0;
            }
            return latin1Map[c];
        }
    }
}