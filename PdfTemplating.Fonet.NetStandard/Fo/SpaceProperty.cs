using Fonet.DataTypes;

namespace Fonet.Fo
{
    internal class SpaceProperty : Property
    {
        internal class Maker : LengthRangeProperty.Maker
        {
            protected Maker(string name) : base(name) { }

        }

        private Space space;

        public SpaceProperty(Space space)
        {
            this.space = space;
        }

        public override Space GetSpace()
        {
            return this.space;
        }

        public override LengthRange GetLengthRange()
        {
            return this.space;
        }

        public override object GetObject()
        {
            return this.space;
        }
    }
}