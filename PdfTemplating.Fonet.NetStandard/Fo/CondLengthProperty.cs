using Fonet.DataTypes;

namespace Fonet.Fo
{
    internal class CondLengthProperty : Property
    {
        internal class Maker : PropertyMaker
        {
            public Maker(string name) : base(name) { }

        }

        private CondLength condLength = null;

        public CondLengthProperty(CondLength condLength)
        {
            this.condLength = condLength;
        }

        public override CondLength GetCondLength()
        {
            return this.condLength;
        }

        public override Length GetLength()
        {
            return this.condLength.GetLength().GetLength();
        }

        public override object GetObject()
        {
            return this.condLength;
        }

    }
}