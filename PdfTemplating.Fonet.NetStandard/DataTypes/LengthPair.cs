namespace Fonet.DataTypes
{
    using Fonet.Fo;

    internal class LengthPair : ICompoundDatatype
    {
        private Property ipd;
        private Property bpd;

        public void SetComponent(string sCmpnName, Property cmpnValue,
                                 bool bIsDefault)
        {
            if (sCmpnName.Equals("block-progression-direction"))
            {
                bpd = cmpnValue;
            }
            else if (sCmpnName.Equals("inline-progression-direction"))
            {
                ipd = cmpnValue;
            }
        }

        public Property GetComponent(string sCmpnName)
        {
            if (sCmpnName.Equals("block-progression-direction"))
            {
                return GetBPD();
            }
            else if (sCmpnName.Equals("inline-progression-direction"))
            {
                return GetIPD();
            }
            else
            {
                return null;
            }
        }

        public Property GetIPD()
        {
            return this.ipd;
        }

        public Property GetBPD()
        {
            return this.bpd;
        }
    }
}