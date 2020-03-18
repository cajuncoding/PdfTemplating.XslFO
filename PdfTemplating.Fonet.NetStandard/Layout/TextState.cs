namespace Fonet.Layout
{
    internal class TextState
    {
        protected bool underlined;

        protected bool overlined;

        protected bool linethrough;

        public TextState()
        {
        }

        public bool getUnderlined()
        {
            return underlined;
        }

        public void setUnderlined(bool ul)
        {
            this.underlined = ul;
        }

        public bool getOverlined()
        {
            return overlined;
        }

        public void setOverlined(bool ol)
        {
            this.overlined = ol;
        }

        public bool getLineThrough()
        {
            return linethrough;
        }

        public void setLineThrough(bool lt)
        {
            this.linethrough = lt;
        }

    }
}