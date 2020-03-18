namespace Fonet.Fo.Flow
{
    using Fonet.Layout;

    internal class InlineContainer : ToBeImplementedElement
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new InlineContainer(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected InlineContainer(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:inline-container";

            BorderAndPadding bap = propMgr.GetBorderAndPadding();
            BackgroundProps bProps = propMgr.GetBackgroundProps();
            MarginInlineProps mProps = propMgr.GetMarginInlineProps();
            RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();
        }
    }
}