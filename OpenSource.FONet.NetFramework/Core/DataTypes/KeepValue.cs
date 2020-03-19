namespace Fonet.DataTypes
{
    internal class KeepValue
    {
        public const string KEEP_WITH_ALWAYS = "KEEP_WITH_ALWAYS";
        public const string KEEP_WITH_AUTO = "KEEP_WITH_AUTO";
        public const string KEEP_WITH_VALUE = "KEEP_WITH_VALUE";

        private string type = KEEP_WITH_AUTO;
        private int value = 0;

        public KeepValue(string type, int val)
        {
            this.type = type;
            this.value = val;
        }

        public int GetValue()
        {
            return value;
        }

        public string GetKeepType()
        {
            return type;
        }

        public override string ToString()
        {
            return type;
        }
    }
}