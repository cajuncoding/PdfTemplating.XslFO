namespace Fonet.Layout.Inline
{
    internal class PageNumberInlineArea : WordArea
    {
        public PageNumberInlineArea(
            FontState fontState, float red, float green,
            float blue, string refid, int width)
            : base(fontState, red, green, blue, "?", width)
        {
            this.pageNumberId = refid;
        }

    }
}