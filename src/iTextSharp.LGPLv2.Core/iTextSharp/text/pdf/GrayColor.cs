using System.util;

namespace iTextSharp.text.pdf
{

    /// <summary>
    /// @author  Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public class GrayColor : ExtendedColor
    {
        public static readonly GrayColor Grayblack = new GrayColor(0f);
        public static readonly GrayColor Graywhite = new GrayColor(1f);

        public GrayColor(int intGray) : this(intGray / 255f) { }

        public GrayColor(float floatGray) : base(TYPE_GRAY, floatGray, floatGray, floatGray)
        {
            Gray = Normalize(floatGray);
        }

        public new float Gray { get; }

        public override bool Equals(object obj)
        {
            return (obj is GrayColor) && ((GrayColor)obj).Gray.ApproxEquals(Gray);
        }

        public override int GetHashCode()
        {
            return Gray.GetHashCode();
        }
    }
}