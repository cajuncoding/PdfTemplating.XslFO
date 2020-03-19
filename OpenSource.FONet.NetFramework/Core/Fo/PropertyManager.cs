using System;
using Fonet.Fo.Properties;
using Fonet.Image;
using Fonet.Layout;

namespace Fonet.Fo
{
    internal class PropertyManager
    {
        private PropertyList properties;
        private FontState fontState = null;
        private BorderAndPadding borderAndPadding = null;
        private HyphenationProps hyphProps = null;
        private BackgroundProps bgProps = null;

        private string saLeft;
        private string saRight;
        private string saTop;
        private string saBottom;

        private static string msgColorFmt = "border-{0}-color";
        private static string msgStyleFmt = "border-{0}-style";
        private static string msgWidthFmt = "border-{0}-width";
        private static string msgPaddingFmt = "padding-{0}";

        public PropertyManager(PropertyList pList)
        {
            this.properties = pList;
        }

        private void InitDirections()
        {
            saTop = properties.wmAbsToRel(PropertyList.TOP);
            saBottom = properties.wmAbsToRel(PropertyList.BOTTOM);
            saLeft = properties.wmAbsToRel(PropertyList.LEFT);
            saRight = properties.wmAbsToRel(PropertyList.RIGHT);
        }

        public FontState GetFontState(FontInfo fontInfo)
        {
            if (fontState == null)
            {
                string fontFamily = properties.GetProperty("font-family").GetString();
                string fontStyle = properties.GetProperty("font-style").GetString();
                string fontWeight = properties.GetProperty("font-weight").GetString();
                int fontSize = properties.GetProperty("font-size").GetLength().MValue();
                int fontVariant = properties.GetProperty("font-variant").GetEnum();
                fontState = new FontState(fontInfo, fontFamily, fontStyle,
                                          fontWeight, fontSize, fontVariant);
            }
            return fontState;
        }


        public BorderAndPadding GetBorderAndPadding()
        {
            if (borderAndPadding == null)
            {
                this.borderAndPadding = new BorderAndPadding();
                InitDirections();

                InitBorderInfo(BorderAndPadding.TOP, saTop);
                InitBorderInfo(BorderAndPadding.BOTTOM, saBottom);
                InitBorderInfo(BorderAndPadding.LEFT, saLeft);
                InitBorderInfo(BorderAndPadding.RIGHT, saRight);
            }
            return borderAndPadding;
        }

        private void InitBorderInfo(int whichSide, string saSide)
        {
            borderAndPadding.setPadding(
                whichSide, properties.GetProperty(String.Format(msgPaddingFmt, saSide)).GetCondLength());
            int style = properties.GetProperty(String.Format(msgStyleFmt, saSide)).GetEnum();
            if (style != Constants.NONE)
            {
                borderAndPadding.setBorder(whichSide, style,
                                           properties.GetProperty(String.Format(msgWidthFmt, saSide)).GetCondLength(),
                                           properties.GetProperty(String.Format(msgColorFmt, saSide)).GetColorType());
            }
        }

        public HyphenationProps GetHyphenationProps()
        {
            if (hyphProps == null)
            {
                this.hyphProps = new HyphenationProps();
                hyphProps.hyphenate = this.properties.GetProperty("hyphenate").GetEnum();
                hyphProps.hyphenationChar =
                    this.properties.GetProperty("hyphenation-character").GetCharacter();
                hyphProps.hyphenationPushCharacterCount =
                    this.properties.GetProperty("hyphenation-push-character-count").GetNumber().IntValue();
                hyphProps.hyphenationRemainCharacterCount =
                    this.properties.GetProperty("hyphenation-remain-character-count").GetNumber().IntValue();
                hyphProps.language = this.properties.GetProperty("language").GetString();
                hyphProps.country = this.properties.GetProperty("country").GetString();
            }
            return hyphProps;
        }

        public int CheckBreakBefore(Area area)
        {
            if (!(area is ColumnArea))
            {
                switch (properties.GetProperty("break-before").GetEnum())
                {
                    case BreakBefore.PAGE:
                        return Status.FORCE_PAGE_BREAK;
                    case BreakBefore.ODD_PAGE:
                        return Status.FORCE_PAGE_BREAK_ODD;
                    case BreakBefore.EVEN_PAGE:
                        return Status.FORCE_PAGE_BREAK_EVEN;
                    case BreakBefore.COLUMN:
                        return Status.FORCE_COLUMN_BREAK;
                    default:
                        return Status.OK;
                }
            }
            else
            {
                ColumnArea colArea = (ColumnArea)area;
                switch (properties.GetProperty("break-before").GetEnum())
                {
                    case BreakBefore.PAGE:
                        if (!colArea.hasChildren() && (colArea.getColumnIndex() == 1))
                        {
                            return Status.OK;
                        }
                        else
                        {
                            return Status.FORCE_PAGE_BREAK;
                        }
                    case BreakBefore.ODD_PAGE:
                        if (!colArea.hasChildren() && (colArea.getColumnIndex() == 1)
                            && (colArea.getPage().getNumber() % 2 != 0))
                        {
                            return Status.OK;
                        }
                        else
                        {
                            return Status.FORCE_PAGE_BREAK_ODD;
                        }
                    case BreakBefore.EVEN_PAGE:
                        if (!colArea.hasChildren() && (colArea.getColumnIndex() == 1)
                            && (colArea.getPage().getNumber() % 2 == 0))
                        {
                            return Status.OK;
                        }
                        else
                        {
                            return Status.FORCE_PAGE_BREAK_EVEN;
                        }
                    case BreakBefore.COLUMN:
                        if (!area.hasChildren())
                        {
                            return Status.OK;
                        }
                        else
                        {
                            return Status.FORCE_COLUMN_BREAK;
                        }
                    default:
                        return Status.OK;
                }
            }
        }

        public int CheckBreakAfter(Area area)
        {
            switch (properties.GetProperty("break-after").GetEnum())
            {
                case BreakAfter.PAGE:
                    return Status.FORCE_PAGE_BREAK;
                case BreakAfter.ODD_PAGE:
                    return Status.FORCE_PAGE_BREAK_ODD;
                case BreakAfter.EVEN_PAGE:
                    return Status.FORCE_PAGE_BREAK_EVEN;
                case BreakAfter.COLUMN:
                    return Status.FORCE_COLUMN_BREAK;
                default:
                    return Status.OK;
            }
        }

        public MarginProps GetMarginProps()
        {
            MarginProps props = new MarginProps();

            props.marginTop =
                this.properties.GetProperty("margin-top").GetLength().MValue();
            props.marginBottom =
                this.properties.GetProperty("margin-bottom").GetLength().MValue();
            props.marginLeft =
                this.properties.GetProperty("margin-left").GetLength().MValue();
            props.marginRight =
                this.properties.GetProperty("margin-right").GetLength().MValue();
            return props;
        }

        public BackgroundProps GetBackgroundProps()
        {
            if (bgProps == null)
            {
                bgProps = new BackgroundProps();

                bgProps.backColor =
                    properties.GetProperty("background-color").GetColorType();

                string src = properties.GetProperty("background-image").GetString();
                if (src == "none")
                {
                    bgProps.backImage = null;
                }
                else if (src == "inherit")
                {
                    bgProps.backImage = null;
                }
                else
                {
                    try
                    {
                        bgProps.backImage = FonetImageFactory.Make(src);
                    }
                    catch (FonetImageException imgex)
                    {
                        bgProps.backImage = null;
                        FonetDriver.ActiveDriver.FireFonetError(imgex.Message);
                    }
                }

                bgProps.backRepeat = properties.GetProperty("background-repeat").GetEnum();
            }
            return bgProps;
        }

        public MarginInlineProps GetMarginInlineProps()
        {
            MarginInlineProps props = new MarginInlineProps();
            return props;
        }

        public AccessibilityProps GetAccessibilityProps()
        {
            AccessibilityProps props = new AccessibilityProps();
            string str;
            str = this.properties.GetProperty("source-document").GetString();
            if (!"none".Equals(str))
            {
                props.sourceDoc = str;
            }
            str = this.properties.GetProperty("role").GetString();
            if (!"none".Equals(str))
            {
                props.role = str;
            }
            return props;
        }

        public AuralProps GetAuralProps()
        {
            AuralProps props = new AuralProps();
            return props;
        }

        public RelativePositionProps GetRelativePositionProps()
        {
            RelativePositionProps props = new RelativePositionProps();
            return props;
        }

        public AbsolutePositionProps GetAbsolutePositionProps()
        {
            AbsolutePositionProps props = new AbsolutePositionProps();
            return props;
        }

        public TextState getTextDecoration(FObj parent)
        {
            TextState tsp = null;
            bool found = false;

            do
            {
                string fname = parent.GetName();
                if (fname.Equals("fo:flow") || fname.Equals("fo:static-content"))
                {
                    found = true;
                }
                else if (fname.Equals("fo:block") || fname.Equals("fo:inline"))
                {
                    FObjMixed fom = (FObjMixed)parent;
                    tsp = fom.getTextState();
                    found = true;
                }
                parent = parent.getParent();
            } while (!found);

            TextState ts = new TextState();

            if (tsp != null)
            {
                ts.setUnderlined(tsp.getUnderlined());
                ts.setOverlined(tsp.getOverlined());
                ts.setLineThrough(tsp.getLineThrough());
            }

            int textDecoration = this.properties.GetProperty("text-decoration").GetEnum();

            if (textDecoration == TextDecoration.UNDERLINE)
            {
                ts.setUnderlined(true);
            }
            if (textDecoration == TextDecoration.OVERLINE)
            {
                ts.setOverlined(true);
            }
            if (textDecoration == TextDecoration.LINE_THROUGH)
            {
                ts.setLineThrough(true);
            }
            if (textDecoration == TextDecoration.NO_UNDERLINE)
            {
                ts.setUnderlined(false);
            }
            if (textDecoration == TextDecoration.NO_OVERLINE)
            {
                ts.setOverlined(false);
            }
            if (textDecoration == TextDecoration.NO_LINE_THROUGH)
            {
                ts.setLineThrough(false);
            }

            return ts;
        }
    }
}