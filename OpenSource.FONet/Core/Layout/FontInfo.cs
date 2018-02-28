namespace Fonet.Layout
{
    using System;
    using System.Collections;

    internal class FontInfo
    {
        private Hashtable usedFonts;
        private Hashtable triplets;
        private Hashtable fonts;

        public FontInfo()
        {
            this.triplets = new Hashtable();
            this.fonts = new Hashtable();
            this.usedFonts = new Hashtable();
        }

        public void AddFontProperties(string name, string family, string style, string weight)
        {
            string key = CreateFontKey(family, style, weight);
            triplets.Add(key, name);
        }

        public void AddMetrics(string name, IFontMetric metrics)
        {
            fonts.Add(name, metrics);
        }

        public string FontLookup(string family, string style, string weight)
        {
            return FontLookup(CreateFontKey(family, style, weight));
        }

        private string FontLookup(string key)
        {
            string f = (string)triplets[key];
            if (f == null)
            {
                int i = key.IndexOf(',');
                string s = "any" + key.Substring(i);
                f = (string)triplets[s];
                if (f == null)
                {
                    f = (string)triplets["any,normal,normal"];
                    if (f == null)
                    {
                        throw new FonetException("no default font defined by OutputConverter");
                    }
                    FonetDriver.ActiveDriver.FireFonetInfo(
                        "Defaulted font to any,normal,normal");
                }
                FonetDriver.ActiveDriver.FireFonetWarning(
                    "Unknown font " + key + " so defaulted font to any");
            }

            usedFonts[f] = fonts[f];
            return f;
        }

        private bool HasFont(string family, string style, string weight)
        {
            string key = CreateFontKey(family, style, weight);
            return triplets.ContainsKey(key);
        }

        public static string CreateFontKey(string family, string style, string weight)
        {
            int i;
            try
            {
                if (weight != null && weight.Length > 0 && Char.IsNumber(weight, 0))
                {
                    i = Int32.Parse(weight);
                }
                else
                {
                    i = 0;
                }
            }
            catch (Exception)
            {
                i = 0;
            }

            if (i > 600)
            {
                weight = "bold";
            }
            else if (i > 0)
            {
                weight = "normal";
            }

            return String.Format("{0},{1},{2}", family, style, weight);
        }

        public IDictionary GetFonts()
        {
            return fonts;
        }

        public IFontMetric GetFontByName(string fontName)
        {
            return (IFontMetric)fonts[fontName];
        }

        public Hashtable GetUsedFonts()
        {
            return usedFonts;
        }

        public IFontMetric GetMetricsFor(string fontName)
        {
            usedFonts[fontName] = fonts[fontName];
            return (IFontMetric)fonts[fontName];
        }

    }
}