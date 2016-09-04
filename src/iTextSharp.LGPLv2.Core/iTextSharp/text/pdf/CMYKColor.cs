namespace iTextSharp.text.pdf
{
    /// <summary>
    /// @author  Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public class CmykColor : ExtendedColor
    {
        public CmykColor(int intCyan, int intMagenta, int intYellow, int intBlack) :
            this(intCyan / 255f, intMagenta / 255f, intYellow / 255f, intBlack / 255f)
        { }

        public CmykColor(float floatCyan, float floatMagenta, float floatYellow, float floatBlack) :
            base(TYPE_CMYK, 1f - floatCyan - floatBlack, 1f - floatMagenta - floatBlack, 1f - floatYellow - floatBlack)
        {
            Cyan = Normalize(floatCyan);
            Magenta = Normalize(floatMagenta);
            Yellow = Normalize(floatYellow);
            Black = Normalize(floatBlack);
        }

        public new float Black { get; }

        public new float Cyan { get; }

        public new float Magenta { get; }

        public new float Yellow { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is CmykColor))
                return false;
            CmykColor c2 = (CmykColor)obj;
            return (Cyan == c2.Cyan && Magenta == c2.Magenta && Yellow == c2.Yellow && Black == c2.Black);
        }

        public override int GetHashCode()
        {
            return Cyan.GetHashCode() ^ Magenta.GetHashCode() ^ Yellow.GetHashCode() ^ Black.GetHashCode();
        }
    }
}