using System.Runtime.InteropServices;

namespace Fonet.Pdf.Gdi {
    /// <summary>
    ///     The Point structure defines the x- and y- coordinates of a point. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Point {
        public int x;
        public int y;
    } ;

}