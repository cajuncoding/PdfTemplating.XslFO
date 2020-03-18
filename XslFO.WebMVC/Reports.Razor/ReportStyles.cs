namespace XslFO.WebMVC.Reports.Razor
{
    public static class FontFamily
    {
        public static string Default = "Cambria";
        public static string Label = "Calibri";
        public static string Footer = "Verdana";
    }

    public static class FontSize
    {
        public static string Default = "10pt";
        public static string Medium = "9pt";
        public static string Small = "8pt";
        public static string H1 = "18pt";
        public static string H2 = "16pt";
        public static string H3 = "10pt";

        public static string Label = "9pt";
        public static string Footer = "9pt";
    }

    public static class Border
    {
        public static string HorizontalRule = ".25pt solid #A0A0A0";
    }

    public static class Paragraph
    {
        public static string SpaceAfterHeader = ".25in";
        public static string SpaceBefore = ".15in";
    }
    public static class Table
    {
        public static class Border
        {
            public static string Report = "1pt solid #C4C4C4";
            public static string Separator = ".25pt solid #A0A0A0";
            public static string EdgeSpacingDefault = "2px";
        }
        public static class Padding
        {
            public static string Report = "5px";
        }
        public static class BorderWidth
        {
            public static string Default = "1px";
            public static string Bold = "2px";
        }
        public static string BackgroundColor = "#F2F2F2";
        public static string HighlightRowColor = "#FFFBCC";
        public static string EvenRowColor = "#F2F2F2";
        public static string OddRowColor = "#FFFFFF";
    }


    public static class Color
    {
        public static string NeonYellow = "#F3F315";
        public static string HighlighterYellow = "#FFFBCC";
        public static string Gray = "#777777";
    }

    public static class DateFormat
    {
        public static string Short = "MM/dd/yyyy";
        public static string Long = "MM/dd/yyyy hh:mm tt";
        public static string Extended = "MM/dd/yyyy hh:mm:ss tt";
    }

    public static class CurrencyFormat
    {
        public static string US = "$###,###,##0.00";
    }

    public static class NumberFormat
    {
        public static string Decimal = "0.00";
    }

}