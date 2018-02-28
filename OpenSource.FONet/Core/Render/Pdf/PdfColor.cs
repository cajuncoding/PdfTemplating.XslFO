using System.Globalization;
using System.Text;
using Fonet.DataTypes;

namespace Fonet.Render.Pdf {
    internal sealed class PdfColor {
        private double red = -1.0;
        private double green = -1.0;
        private double blue = -1.0;

        public PdfColor(ColorType color) {
            this.red = (double) color.Red;
            this.green = (double) color.Green;
            this.blue = (double) color.Blue;
        }

        public PdfColor(double red, double green, double blue) {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        // components from 0 to 255
        public PdfColor(int red, int green, int blue) : this(
            ((double) red)/255d,
            ((double) green)/255d,
            ((double) blue)/255d
            ) {}

        public double getRed() {
            return (this.red);
        }

        public double getGreen() {
            return (this.green);
        }

        public double getBlue() {
            return (this.blue);
        }

        public string getColorSpaceOut(bool fillNotStroke) {
            StringBuilder p = new StringBuilder();

            // according to pdfspec 12.1 p.399
            // if the colors are the same then just use the g or G operator
            bool same = false;
            if (this.red == this.green && this.red == this.blue) {
                same = true;
            }

            // output RGB
            if (fillNotStroke) {
                if (same) {
                    p.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "{0:0.0####} g\n",
                        this.red);
                }
                else {
                    p.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "{0:0.0####} {1:0.0####} {2:0.0####} rg\n",
                        this.red, this.green, this.blue);
                }
            }
            else {
                if (same) {
                    p.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "{0:0.0####} G\n",
                        this.red);
                }
                else {
                    p.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "{0:0.0####} {1:0.0####} {2:0.0####} RG\n",
                        this.red, this.green, this.blue);
                }
            }

            return p.ToString();
        }
    }
}