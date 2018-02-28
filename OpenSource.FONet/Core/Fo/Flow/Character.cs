namespace Fonet.Fo.Flow
{
    using Fonet.DataTypes;
    using Fonet.Fo.Properties;
    using Fonet.Layout;

    internal class Character : FObj
    {
        public const int OK = 0;

        public const int DOESNOT_FIT = 1;

        public Character(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:character";
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new Character(parent, propertyList);
            }
        }

        public override Status Layout(Area area)
        {
            BlockArea blockArea;
            if (!(area is BlockArea))
            {
                FonetDriver.ActiveDriver.FireFonetWarning(
                    "Currently Character can only be in a BlockArea");
                return new Status(Status.OK);
            }
            blockArea = (BlockArea)area;
            bool textDecoration;

            AuralProps mAurProps = propMgr.GetAuralProps();
            BorderAndPadding bap = propMgr.GetBorderAndPadding();
            BackgroundProps bProps = propMgr.GetBackgroundProps();
            HyphenationProps mHyphProps = propMgr.GetHyphenationProps();
            MarginInlineProps mProps = propMgr.GetMarginInlineProps();
            RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();
            ColorType c = this.properties.GetProperty("color").GetColorType();
            float red = c.Red;
            float green = c.Green;
            float blue = c.Blue;

            int whiteSpaceCollapse =
                this.properties.GetProperty("white-space-collapse").GetEnum();
            int wrapOption = this.parent.properties.GetProperty("wrap-option").GetEnum();

            int tmp = this.properties.GetProperty("text-decoration").GetEnum();
            if (tmp == TextDecoration.UNDERLINE)
            {
                textDecoration = true;
            }
            else
            {
                textDecoration = false;
            }

            char characterValue = this.properties.GetProperty("character").GetCharacter();
            string id = this.properties.GetProperty("id").GetString();
            blockArea.getIDReferences().InitializeID(id, blockArea);

            LineArea la = blockArea.getCurrentLineArea();
            if (la == null)
            {
                return new Status(Status.AREA_FULL_NONE);
            }
            la.changeFont(propMgr.GetFontState(area.getFontInfo()));
            la.changeColor(red, green, blue);
            la.changeWrapOption(wrapOption);
            la.changeWhiteSpaceCollapse(whiteSpaceCollapse);
            blockArea.setupLinkSet(this.GetLinkSet());
            int result = la.addCharacter(characterValue, this.GetLinkSet(),
                                         textDecoration);
            if (result == Character.DOESNOT_FIT)
            {
                la = blockArea.createNextLineArea();
                if (la == null)
                {
                    return new Status(Status.AREA_FULL_NONE);
                }
                la.changeFont(propMgr.GetFontState(area.getFontInfo()));
                la.changeColor(red, green, blue);
                la.changeWrapOption(wrapOption);
                la.changeWhiteSpaceCollapse(whiteSpaceCollapse);
                blockArea.setupLinkSet(this.GetLinkSet());
                la.addCharacter(characterValue, this.GetLinkSet(),
                                textDecoration);
            }
            return new Status(Status.OK);
        }
    }
}