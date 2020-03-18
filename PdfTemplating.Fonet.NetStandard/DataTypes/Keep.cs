using Fonet.Fo;

namespace Fonet.DataTypes
{
    internal class Keep : ICompoundDatatype
    {
        private Property withinLine;

        private Property withinColumn;

        private Property withinPage;

        public Keep()
        {
        }

        public void SetComponent(string sCmpnName, Property cmpnValue,
                                 bool bIsDefault)
        {
            if (sCmpnName.Equals("within-line"))
            {
                setWithinLine(cmpnValue, bIsDefault);
            }
            else if (sCmpnName.Equals("within-column"))
            {
                setWithinColumn(cmpnValue, bIsDefault);
            }
            else if (sCmpnName.Equals("within-page"))
            {
                setWithinPage(cmpnValue, bIsDefault);
            }
        }

        public Property GetComponent(string sCmpnName)
        {
            if (sCmpnName.Equals("within-line"))
            {
                return getWithinLine();
            }
            else if (sCmpnName.Equals("within-column"))
            {
                return getWithinColumn();
            }
            else if (sCmpnName.Equals("within-page"))
            {
                return getWithinPage();
            }
            else
            {
                return null;
            }
        }

        public void setWithinLine(Property withinLine, bool bIsDefault)
        {
            this.withinLine = withinLine;
        }

        protected void setWithinColumn(Property withinColumn,
                                       bool bIsDefault)
        {
            this.withinColumn = withinColumn;
        }

        public void setWithinPage(Property withinPage, bool bIsDefault)
        {
            this.withinPage = withinPage;
        }

        public Property getWithinLine()
        {
            return this.withinLine;
        }

        public Property getWithinColumn()
        {
            return this.withinColumn;
        }

        public Property getWithinPage()
        {
            return this.withinPage;
        }

        public override string ToString()
        {
            return "Keep";
        }
    }
}