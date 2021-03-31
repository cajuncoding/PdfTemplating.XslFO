using System;
using System.Globalization;
using System.Text;

namespace Fonet.Render.Pdf {
    internal sealed class PdfNumber {
        private PdfNumber() {}

        internal static string doubleOut(double doubleDown) {
            StringBuilder p = new StringBuilder();
            if (doubleDown < 0) {
                doubleDown = -doubleDown;
                p.Append("-");
            }
            double trouble = doubleDown%1;

            if (trouble > 0.950) {
                p.Append((int) doubleDown + 1);
            }
            else if (trouble < 0.050) {
                p.Append((int) doubleDown);
            }
            else {
                string doubleString = doubleDown.ToString(CultureInfo.InvariantCulture.NumberFormat);
                int d = doubleString.IndexOf(".");
                if (d != -1) {
                    p.Append(doubleString.Substring(0, d));

                    if ((doubleString.Length - d) > 6) {
                        p.Append(doubleString.Substring(d, 6));
                    }
                    else {
                        p.Append(doubleString.Substring(d));
                    }
                }
                else {
                    p.Append(doubleString);
                }
            }
            return (p.ToString());
        }

        internal static string doubleOut(double doubleDown, int dec) {
            StringBuilder p = new StringBuilder();
            if (doubleDown < 0) {
                doubleDown = -doubleDown;
                p.Append("-");
            }
            double trouble = doubleDown%1;

            if (trouble > (1.0 - (5.0/(Math.Pow(10.0, dec))))) {
                p.Append((int) doubleDown + 1);
            }
            else if (trouble < (5.0/(Math.Pow(10.0, dec)))) {
                p.Append((int) doubleDown);
            }
            else {
                string doubleString = doubleDown.ToString(CultureInfo.InvariantCulture.NumberFormat);
                int d = doubleString.IndexOf(".");
                if (d != -1) {
                    p.Append(doubleString.Substring(0, d));

                    if ((doubleString.Length - d) > dec) {
                        p.Append(doubleString.Substring(d, dec));
                    }
                    else {
                        p.Append(doubleString.Substring(d));
                    }
                }
                else {
                    p.Append(doubleString);
                }
            }
            return p.ToString();
        }

    }
}