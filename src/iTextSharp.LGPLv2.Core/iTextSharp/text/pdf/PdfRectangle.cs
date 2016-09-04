namespace iTextSharp.text.pdf
{

    /// <summary>
    ///  PdfRectangle  is the PDF Rectangle object.
    ///
    /// Rectangles are used to describe locations on the page and bounding boxes for several
    /// objects in PDF, such as fonts. A rectangle is represented as an  array  of
    /// four numbers, specifying the lower lef <I>x</I>, lower left <I>y</I>, upper right <I>x</I>,
    /// and upper right <I>y</I> coordinates of the rectangle, in that order.
    /// This object is described in the 'Portable Document Format Reference Manual version 1.3'
    /// section 7.1 (page 183).
    /// @see     iTextSharp.text.Rectangle
    /// @see     PdfArray
    /// </summary>
    public class PdfRectangle : PdfArray
    {

        /// <summary>
        /// membervariables
        /// </summary>

        /// <summary>
        /// lower left x
        /// </summary>
        private readonly float _llx;

        /// <summary>
        /// lower left y
        /// </summary>
        private readonly float _lly;

        /// <summary>
        /// upper right x
        /// </summary>
        private readonly float _urx;

        /// <summary>
        /// upper right y
        /// </summary>
        private readonly float _ury;

        /// <summary>
        /// constructors
        /// </summary>
        /// <summary>
        /// Constructs a  PdfRectangle -object.
        /// @since       rugPdf0.10
        /// </summary>
        /// <param name="llx">lower left x</param>
        /// <param name="lly">lower left y</param>
        /// <param name="urx">upper right x</param>
        /// <param name="ury">upper right y</param>
        /// <param name="rotation"></param>
        public PdfRectangle(float llx, float lly, float urx, float ury, int rotation)
        {
            if (rotation == 90 || rotation == 270)
            {
                _llx = lly;
                _lly = llx;
                _urx = ury;
                _ury = urx;
            }
            else
            {
                _llx = llx;
                _lly = lly;
                _urx = urx;
                _ury = ury;
            }
            base.Add(new PdfNumber(_llx));
            base.Add(new PdfNumber(_lly));
            base.Add(new PdfNumber(_urx));
            base.Add(new PdfNumber(_ury));
        }

        public PdfRectangle(float llx, float lly, float urx, float ury) : this(llx, lly, urx, ury, 0) { }

        /// <summary>
        /// Constructs a  PdfRectangle -object starting from the origin (0, 0).
        /// </summary>
        /// <param name="urx">upper right x</param>
        /// <param name="ury">upper right y</param>
        /// <param name="rotation"></param>
        public PdfRectangle(float urx, float ury, int rotation) : this(0, 0, urx, ury, rotation) { }

        public PdfRectangle(float urx, float ury) : this(0, 0, urx, ury, 0) { }

        /// <summary>
        /// Constructs a  PdfRectangle -object with a  Rectangle -object.
        /// </summary>
        /// <param name="rectangle">a  Rectangle </param>
        /// <param name="rotation"></param>
        public PdfRectangle(Rectangle rectangle, int rotation) : this(rectangle.Left, rectangle.Bottom, rectangle.Right, rectangle.Top, rotation) { }

        public PdfRectangle(Rectangle rectangle) : this(rectangle.Left, rectangle.Bottom, rectangle.Right, rectangle.Top, 0) { }

        /// <summary>
        /// methods
        /// </summary>

        public float Bottom
        {
            get
            {
                return _lly;
            }
        }

        public float Height
        {
            get
            {
                return _ury - _lly;
            }
        }

        public float Left
        {
            get
            {
                return _llx;
            }
        }

        /// <summary>
        /// Returns the high level version of this PdfRectangle
        /// </summary>
        /// <returns>this PdfRectangle translated to class Rectangle</returns>
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle(Left, Bottom, Right, Top);
            }
        }

        public float Right
        {
            get
            {
                return _urx;
            }
        }

        public PdfRectangle Rotate
        {
            get
            {
                return new PdfRectangle(_lly, _llx, _ury, _urx, 0);
            }
        }

        public float Top
        {
            get
            {
                return _ury;
            }
        }

        public float Width
        {
            get
            {
                return _urx - _llx;
            }
        }

        public override bool Add(PdfObject obj)
        {
            return false;
        }

        /// <summary>
        /// Block changes to the underlying PdfArray
        /// @since 2.1.5
        /// </summary>
        /// <param name="values">stuff we'll ignore. Ha!</param>
        /// <returns>false. You can't add anything to a PdfRectangle</returns>

        public override bool Add(float[] values)
        {
            return false;
        }

        /// <summary>
        /// Block changes to the underlying PdfArray
        /// @since 2.1.5
        /// </summary>
        /// <param name="values">stuff we'll ignore. Ha!</param>
        /// <returns>false. You can't add anything to a PdfRectangle</returns>

        public override bool Add(int[] values)
        {
            return false;
        }

        /// <summary>
        /// Block changes to the underlying PdfArray
        /// @since 2.1.5
        /// </summary>
        /// <param name="obj">Ignored.</param>
        public override void AddFirst(PdfObject obj)
        {
        }

        /// <summary>
        /// Returns the lower left x-coordinate.
        /// </summary>
        /// <returns>the lower left x-coordinaat</returns>
        /// <summary>
        /// Returns the upper right x-coordinate.
        /// </summary>
        /// <returns>the upper right x-coordinate</returns>
        /// <summary>
        /// Returns the upper right y-coordinate.
        /// </summary>
        /// <returns>the upper right y-coordinate</returns>
        /// <summary>
        /// Returns the lower left y-coordinate.
        /// </summary>
        /// <returns>the lower left y-coordinate</returns>
        /// <summary>
        /// Returns the lower left x-coordinate, considering a given margin.
        /// </summary>
        /// <param name="margin">a margin</param>
        /// <returns>the lower left x-coordinate</returns>

        public float GetBottom(int margin)
        {
            return _lly + margin;
        }

        public float GetLeft(int margin)
        {
            return _llx + margin;
        }

        /// <summary>
        /// Returns the upper right x-coordinate, considering a given margin.
        /// </summary>
        /// <param name="margin">a margin</param>
        /// <returns>the upper right x-coordinate</returns>

        public float GetRight(int margin)
        {
            return _urx - margin;
        }

        /// <summary>
        /// Returns the upper right y-coordinate, considering a given margin.
        /// </summary>
        /// <param name="margin">a margin</param>
        /// <returns>the upper right y-coordinate</returns>

        public float GetTop(int margin)
        {
            return _ury - margin;
        }
    }
}