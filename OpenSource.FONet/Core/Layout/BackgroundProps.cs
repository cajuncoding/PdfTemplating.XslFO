using Fonet.DataTypes;
using Fonet.Image;

namespace Fonet.Layout
{
    internal class BackgroundProps
    {
        public int backAttachment = 0;

        public ColorType backColor = null;

        public FonetImage backImage = null;

        public int backRepeat = 0;

        public Length backPosHorizontal = null;

        public Length backPosVertical = null;

        public BackgroundProps()
        {
        }
    }
}