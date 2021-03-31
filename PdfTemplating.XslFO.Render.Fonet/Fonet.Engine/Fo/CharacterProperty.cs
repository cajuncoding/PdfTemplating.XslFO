namespace Fonet.Fo
{
    internal class CharacterProperty : Property
    {
        internal class Maker : PropertyMaker
        {
            public Maker(string propName) : base(propName) { }

            public override Property Make(
                PropertyList propertyList, string value, FObj fo)
            {
                char c = value[0];
                return new CharacterProperty(c);
            }

        }

        private char character;

        public CharacterProperty(char character)
        {
            this.character = character;
        }

        public override object GetObject()
        {
            return character;
        }

        public override char GetCharacter()
        {
            return this.character;
        }

        public override string GetString()
        {
            return character.ToString();
        }

    }
}