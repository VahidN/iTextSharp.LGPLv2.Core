namespace iTextSharp.text
{
    /// <summary>
    /// Signals an attempt to create an Element that hasn't got the right form.
    /// </summary>
    /// <seealso cref="T:iTextSharp.text.Cell"/>
    /// <seealso cref="T:iTextSharp.text.Table"/>
    public class BadElementException : DocumentException
    {
        public BadElementException()
        { }

        public BadElementException(string message) : base(message) { }
    }
}
