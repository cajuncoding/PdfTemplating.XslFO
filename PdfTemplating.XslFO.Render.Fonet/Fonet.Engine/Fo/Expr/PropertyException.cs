using System;

namespace Fonet.Fo.Expr
{
    internal class PropertyException : Exception
    {
        public PropertyException(string detail) : base(detail)
        {
        }
    }
}