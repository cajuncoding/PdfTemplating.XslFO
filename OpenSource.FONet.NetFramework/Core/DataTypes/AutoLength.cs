namespace Fonet.DataTypes
{
    internal class AutoLength : Length
    {
        public override bool IsAuto()
        {
            return true;
        }

        public override string ToString()
        {
            return "auto";
        }
    }
}