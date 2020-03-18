using System.Collections;

namespace Fonet.Fo.Properties
{
    internal class GenericColor : ColorTypeProperty.Maker
    {
        private static Hashtable s_htKeywords;

        protected GenericColor(string name) : base(name) { }

        static GenericColor()
        {
            s_htKeywords = new Hashtable(147);
            s_htKeywords.Add("aliceblue", "#f0f8ff");
            s_htKeywords.Add("antiquewhite", "#faebd7");
            s_htKeywords.Add("aqua", "#00ffff");
            s_htKeywords.Add("aquamarine", "#7fffd4");
            s_htKeywords.Add("azure", "#f0ffff");
            s_htKeywords.Add("beige", "#f5f5dc");
            s_htKeywords.Add("bisque", "#ffe4c4");
            s_htKeywords.Add("black", "#000000");
            s_htKeywords.Add("blanchedalmond", "#ffebcd");
            s_htKeywords.Add("blue", "#0000ff");
            s_htKeywords.Add("blueviolet", "#8a2be2");
            s_htKeywords.Add("brown", "#a52a2a");
            s_htKeywords.Add("burlywood", "#deb887");
            s_htKeywords.Add("cadetblue", "#5f9ea0");
            s_htKeywords.Add("chartreuse", "#7fff00");
            s_htKeywords.Add("chocolate", "#d2691e");
            s_htKeywords.Add("coral", "#ff7f50");
            s_htKeywords.Add("cornflowerblue", "#6495ed");
            s_htKeywords.Add("cornsilk", "#fff8dc");
            s_htKeywords.Add("crimson", "#dc143c");
            s_htKeywords.Add("cyan", "#00ffff");
            s_htKeywords.Add("darkblue", "#00008b");
            s_htKeywords.Add("darkcyan", "#008b8b");
            s_htKeywords.Add("darkgoldenrod", "#b8860b");
            s_htKeywords.Add("darkgray", "#a9a9a9");
            s_htKeywords.Add("darkgreen", "#006400");
            s_htKeywords.Add("darkgrey", "#a9a9a9");
            s_htKeywords.Add("darkkhaki", "#bdb76b");
            s_htKeywords.Add("darkmagenta", "#8b008b");
            s_htKeywords.Add("darkolivegreen", "#556b2f");
            s_htKeywords.Add("darkorange", "#ff8c00");
            s_htKeywords.Add("darkorchid", "#9932cc");
            s_htKeywords.Add("darkred", "#8b0000");
            s_htKeywords.Add("darksalmon", "#e9967a");
            s_htKeywords.Add("darkseagreen", "#8fbc8f");
            s_htKeywords.Add("darkslateblue", "#483d8b");
            s_htKeywords.Add("darkslategray", "#2f4f4f");
            s_htKeywords.Add("darkslategrey", "#2f4f4f");
            s_htKeywords.Add("darkturquoise", "#00ced1");
            s_htKeywords.Add("darkviolet", "#9400d3");
            s_htKeywords.Add("deeppink", "#ff1493");
            s_htKeywords.Add("deepskyblue", "#00bfff");
            s_htKeywords.Add("dimgray", "#696969");
            s_htKeywords.Add("dimgrey", "#696969");
            s_htKeywords.Add("dodgerblue", "#1e90ff");
            s_htKeywords.Add("firebrick", "#b22222");
            s_htKeywords.Add("floralwhite", "#fffaf0");
            s_htKeywords.Add("forestgreen", "#228b22");
            s_htKeywords.Add("fuchsia", "#ff00ff");
            s_htKeywords.Add("gainsboro", "#dcdcdc");
            s_htKeywords.Add("lightpink", "#ffb6c1");
            s_htKeywords.Add("lightsalmon", "#ffa07a");
            s_htKeywords.Add("lightseagreen", "#20b2aa");
            s_htKeywords.Add("lightskyblue", "#87cefa");
            s_htKeywords.Add("lightslategray", "#778899");
            s_htKeywords.Add("lightslategrey", "#778899");
            s_htKeywords.Add("lightsteelblue", "#b0c4de");
            s_htKeywords.Add("lightyellow", "#ffffe0");
            s_htKeywords.Add("lime", "#00ff00");
            s_htKeywords.Add("limegreen", "#32cd32");
            s_htKeywords.Add("linen", "#faf0e6");
            s_htKeywords.Add("magenta", "#ff00ff");
            s_htKeywords.Add("maroon", "#800000");
            s_htKeywords.Add("mediumaquamarine", "#66cdaa");
            s_htKeywords.Add("mediumblue", "#0000cd");
            s_htKeywords.Add("mediumorchid", "#ba55d3");
            s_htKeywords.Add("mediumpurple", "#9370db");
            s_htKeywords.Add("mediumseagreen", "#3cb371");
            s_htKeywords.Add("mediumslateblue", "#7b68ee");
            s_htKeywords.Add("mediumspringgreen", "#00fa9a");
            s_htKeywords.Add("mediumturquoise", "#48d1cc");
            s_htKeywords.Add("mediumvioletred", "#c71585");
            s_htKeywords.Add("midnightblue", "#191970");
            s_htKeywords.Add("mintcream", "#f5fffa");
            s_htKeywords.Add("mistyrose", "#ffe4e1");
            s_htKeywords.Add("moccasin", "#ffe4b5");
            s_htKeywords.Add("navajowhite", "#ffdead");
            s_htKeywords.Add("navy", "#000080");
            s_htKeywords.Add("oldlace", "#fdf5e6");
            s_htKeywords.Add("olive", "#808000");
            s_htKeywords.Add("olivedrab", "#6b8e23");
            s_htKeywords.Add("orange", "#ffa500");
            s_htKeywords.Add("orangered", "#ff4500");
            s_htKeywords.Add("orchid", "#da70d6");
            s_htKeywords.Add("palegoldenrod", "#eee8aa");
            s_htKeywords.Add("palegreen", "#98fb98");
            s_htKeywords.Add("paleturquoise", "#afeeee");
            s_htKeywords.Add("palevioletred", "#db7093");
            s_htKeywords.Add("papayawhip", "#ffefd5");
            s_htKeywords.Add("peachpuff", "#ffdab9");
            s_htKeywords.Add("peru", "#cd853f");
            s_htKeywords.Add("pink", "#ffc0cb");
            s_htKeywords.Add("plum", "#dda0dd");
            s_htKeywords.Add("powderblue", "#b0e0e6");
            s_htKeywords.Add("purple", "#800080");
            s_htKeywords.Add("red", "#ff0000");
            s_htKeywords.Add("rosybrown", "#bc8f8f");
            s_htKeywords.Add("royalblue", "#4169e1");
            s_htKeywords.Add("saddlebrown", "#8b4513");
            s_htKeywords.Add("salmon", "#fa8072");
            s_htKeywords.Add("ghostwhite", "#f8f8ff");
            s_htKeywords.Add("gold", "#ffd700");
            s_htKeywords.Add("goldenrod", "#daa520");
            s_htKeywords.Add("gray", "#808080");
            s_htKeywords.Add("grey", "#808080");
            s_htKeywords.Add("green", "#008000");
            s_htKeywords.Add("greenyellow", "#adff2f");
            s_htKeywords.Add("honeydew", "#f0fff0");
            s_htKeywords.Add("hotpink", "#ff69b4");
            s_htKeywords.Add("indianred", "#cd5c5c");
            s_htKeywords.Add("indigo", "#4b0082");
            s_htKeywords.Add("ivory", "#fffff0");
            s_htKeywords.Add("khaki", "#f0e68c");
            s_htKeywords.Add("lavender", "#e6e6fa");
            s_htKeywords.Add("lavenderblush", "#fff0f5");
            s_htKeywords.Add("lawngreen", "#7cfc00");
            s_htKeywords.Add("lemonchiffon", "#fffacd");
            s_htKeywords.Add("lightblue", "#add8e6");
            s_htKeywords.Add("lightcoral", "#f08080");
            s_htKeywords.Add("lightcyan", "#e0ffff");
            s_htKeywords.Add("lightgoldenrodyellow", "#fafad2");
            s_htKeywords.Add("lightgray", "#d3d3d3");
            s_htKeywords.Add("lightgreen", "#90ee90");
            s_htKeywords.Add("lightgrey", "#d3d3d3");
            s_htKeywords.Add("sandybrown", "#f4a460");
            s_htKeywords.Add("seagreen", "#2e8b57");
            s_htKeywords.Add("seashell", "#fff5ee");
            s_htKeywords.Add("sienna", "#a0522d");
            s_htKeywords.Add("silver", "#c0c0c0");
            s_htKeywords.Add("skyblue", "#87ceeb");
            s_htKeywords.Add("slateblue", "#6a5acd");
            s_htKeywords.Add("slategray", "#708090");
            s_htKeywords.Add("slategrey", "#708090");
            s_htKeywords.Add("snow", "#fffafa");
            s_htKeywords.Add("springgreen", "#00ff7f");
            s_htKeywords.Add("steelblue", "#4682b4");
            s_htKeywords.Add("tan", "#d2b48c");
            s_htKeywords.Add("teal", "#008080");
            s_htKeywords.Add("thistle", "#d8bfd8");
            s_htKeywords.Add("tomato", "#ff6347");
            s_htKeywords.Add("turquoise", "#40e0d0");
            s_htKeywords.Add("violet", "#ee82ee");
            s_htKeywords.Add("wheat", "#f5deb3");
            s_htKeywords.Add("white", "#ffffff");
            s_htKeywords.Add("whitesmoke", "#f5f5f5");
            s_htKeywords.Add("yellow", "#ffff00");
            s_htKeywords.Add("yellowgreen", "#9acd32");
        }

        new public static PropertyMaker Maker(string propName)
        {
            return new GenericColor(propName);
        }

        protected override string CheckValueKeywords(string keyword)
        {
            string val = (string)s_htKeywords[keyword];
            if (val == null)
            {
                return base.CheckValueKeywords(keyword);
            }
            else
            {
                return val;
            }
        }
    }
}