using Fonet.DataTypes;

namespace Fonet.Fo
{
    internal class LengthRangeProperty : Property
    {
        internal class Maker : LengthProperty.Maker
        {
            protected Maker(string name) : base(name) { }

        }

        private LengthRange lengthRange;

        public LengthRangeProperty(LengthRange lengthRange)
        {
            this.lengthRange = lengthRange;
        }

        public override LengthRange GetLengthRange()
        {
            return this.lengthRange;
        }

        public override object GetObject()
        {
            return this.lengthRange;
        }

    }
}