using System;
using System.Collections;
using Fonet.DataTypes;
using Fonet.Fo.Expr;

namespace Fonet.Fo
{
    internal class Property
    {
        private string specVal;

        public string SpecifiedValue
        {
            get { return specVal; }
            set { specVal = value; }
        }

        public virtual Length GetLength()
        {
            return null;
        }

        public virtual ColorType GetColorType()
        {
            return null;
        }

        public virtual CondLength GetCondLength()
        {
            return null;
        }

        public virtual LengthRange GetLengthRange()
        {
            return null;
        }

        public virtual LengthPair GetLengthPair()
        {
            return null;
        }

        public virtual Space GetSpace()
        {
            return null;
        }

        public virtual Keep GetKeep()
        {
            return null;
        }

        public virtual int GetEnum()
        {
            return 0;
        }

        public virtual char GetCharacter()
        {
            return (char)0;
        }

        public virtual ArrayList GetList()
        {
            return null;
        }

        public virtual Number GetNumber()
        {
            return null;
        }

        public virtual Numeric GetNumeric()
        {
            return null;
        }

        public virtual string GetNCname()
        {
            return null;
        }

        public virtual object GetObject()
        {
            return null;
        }

        public virtual String GetString()
        {
            object o = GetObject();
            return (o == null) ? null : o.ToString();
        }
    }
}