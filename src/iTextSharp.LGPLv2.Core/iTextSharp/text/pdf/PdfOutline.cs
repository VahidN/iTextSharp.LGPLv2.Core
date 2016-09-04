using System.Text;
using System.IO;
using System.Collections;

namespace iTextSharp.text.pdf
{

    /// <summary>
    ///  PdfOutline  is an object that represents a PDF outline entry.
    ///
    /// An outline allows a user to access views of a document by name.
    /// This object is described in the 'Portable Document Format Reference Manual version 1.3'
    /// section 6.7 (page 104-106)
    /// @see     PdfDictionary
    /// </summary>
    public class PdfOutline : PdfDictionary
    {

        /// <summary>
        /// membervariables
        /// </summary>

        protected ArrayList kids = new ArrayList();

        protected PdfWriter Writer;

        /// <summary>
        /// The  PdfAction  for this outline.
        /// </summary>
        private readonly PdfAction _action;

        /// <summary>
        /// value of the <B>Destination</B>-key
        /// </summary>
        private readonly PdfDestination _destination;

        /// <summary>
        /// Holds value of property color.
        /// </summary>
        private BaseColor _color;

        /// <summary>
        /// value of the <B>Count</B>-key
        /// </summary>
        private int _count;

        /// <summary>
        /// Holds value of property open.
        /// </summary>
        private bool _open;

        /// <summary>
        /// value of the <B>Parent</B>-key
        /// </summary>
        private PdfOutline _parent;

        /// <summary>
        /// the  PdfIndirectReference  of this object
        /// </summary>
        private PdfIndirectReference _reference;
        /// <summary>
        /// Holds value of property style.
        /// </summary>
        private int _style;

        /// <summary>
        /// Holds value of property tag.
        /// </summary>
        private string _tag;
        /// <summary>
        /// constructors
        /// </summary>

        /// <summary>
        /// Constructs a  PdfOutline .
        ///
        /// This is the constructor for the  outlines object .
        /// </summary>

        public PdfOutline(PdfOutline parent, PdfAction action, string title) : this(parent, action, title, true) { }

        /// <summary>
        /// Constructs a  PdfOutline .
        ///
        /// This is the constructor for an  outline entry .
        /// </summary>
        /// <param name="parent">the parent of this outline item</param>
        /// <param name="action">the  PdfAction  for this outline item</param>
        /// <param name="title">the title of this outline item</param>
        /// <param name="open"> true  if the children are visible</param>
        public PdfOutline(PdfOutline parent, PdfAction action, string title, bool open)
        {
            _action = action;
            InitOutline(parent, title, open);
        }

        public PdfOutline(PdfOutline parent, PdfDestination destination, string title) : this(parent, destination, title, true) { }

        /// <summary>
        /// Constructs a  PdfOutline .
        ///
        /// This is the constructor for an  outline entry .
        /// </summary>
        /// <param name="parent">the parent of this outline item</param>
        /// <param name="destination">the destination for this outline item</param>
        /// <param name="title">the title of this outline item</param>
        /// <param name="open"> true  if the children are visible</param>
        public PdfOutline(PdfOutline parent, PdfDestination destination, string title, bool open)
        {
            _destination = destination;
            InitOutline(parent, title, open);
        }

        /// <summary>
        /// Constructs a  PdfOutline .
        ///
        /// This is the constructor for an  outline entry . The open mode is
        ///  true .
        /// </summary>
        /// <param name="parent">the parent of this outline item</param>
        /// <param name="action">the  PdfAction  for this outline item</param>
        /// <param name="title">the title of this outline item</param>
        public PdfOutline(PdfOutline parent, PdfAction action, PdfString title) : this(parent, action, title, true) { }

        /// <summary>
        /// Constructs a  PdfOutline .
        ///
        /// This is the constructor for an  outline entry .
        /// </summary>
        /// <param name="parent">the parent of this outline item</param>
        /// <param name="action">the  PdfAction  for this outline item</param>
        /// <param name="title">the title of this outline item</param>
        /// <param name="open"> true  if the children are visible</param>
        public PdfOutline(PdfOutline parent, PdfAction action, PdfString title, bool open) : this(parent, action, title.ToString(), open) { }

        public PdfOutline(PdfOutline parent, PdfDestination destination, PdfString title) : this(parent, destination, title, true) { }

        /// <summary>
        /// Constructs a  PdfOutline .
        ///
        /// This is the constructor for an  outline entry .
        /// </summary>
        /// <param name="parent">the parent of this outline item</param>
        /// <param name="destination">the destination for this outline item</param>
        /// <param name="title">the title of this outline item</param>
        /// <param name="open"> true  if the children are visible</param>
        public PdfOutline(PdfOutline parent, PdfDestination destination, PdfString title, bool open) : this(parent, destination, title.ToString(), true) { }

        public PdfOutline(PdfOutline parent, PdfAction action, Paragraph title) : this(parent, action, title, true) { }

        /// <summary>
        /// Constructs a  PdfOutline .
        ///
        /// This is the constructor for an  outline entry .
        /// </summary>
        /// <param name="parent">the parent of this outline item</param>
        /// <param name="action">the  PdfAction  for this outline item</param>
        /// <param name="title">the title of this outline item</param>
        /// <param name="open"> true  if the children are visible</param>
        public PdfOutline(PdfOutline parent, PdfAction action, Paragraph title, bool open)
        {
            StringBuilder buf = new StringBuilder();
            foreach (Chunk chunk in title.Chunks)
            {
                buf.Append(chunk.Content);
            }
            _action = action;
            InitOutline(parent, buf.ToString(), open);
        }

        public PdfOutline(PdfOutline parent, PdfDestination destination, Paragraph title) : this(parent, destination, title, true) { }

        /// <summary>
        /// Constructs a  PdfOutline .
        ///
        /// This is the constructor for an  outline entry .
        /// </summary>
        /// <param name="parent">the parent of this outline item</param>
        /// <param name="destination">the destination for this outline item</param>
        /// <param name="title">the title of this outline item</param>
        /// <param name="open"> true  if the children are visible</param>
        public PdfOutline(PdfOutline parent, PdfDestination destination, Paragraph title, bool open)
        {
            StringBuilder buf = new StringBuilder();
            foreach (Chunk chunk in title.Chunks)
            {
                buf.Append(chunk.Content);
            }
            _destination = destination;
            InitOutline(parent, buf.ToString(), open);
        }

        internal PdfOutline(PdfWriter writer) : base(Outlines)
        {
            _open = true;
            _parent = null;
            Writer = writer;
        }
        /// <summary>
        /// methods
        /// </summary>

        public BaseColor Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        public PdfIndirectReference IndirectReference
        {
            get
            {
                return _reference;
            }

            set
            {
                _reference = value;
            }
        }

        public ArrayList Kids
        {
            get
            {
                return kids;
            }

            set
            {
                kids = value;
            }
        }

        public int Level
        {
            get
            {
                if (_parent == null)
                {
                    return 0;
                }
                return (_parent.Level + 1);
            }
        }

        /// <summary>
        /// Setter for property open.
        /// </summary>
        public bool Open
        {
            set
            {
                _open = value;
            }
            get
            {
                return _open;
            }
        }

        public PdfOutline Parent
        {
            get
            {
                return _parent;
            }
        }

        /// <summary>
        /// Gets the destination for this outline.
        /// </summary>
        /// <returns>the destination</returns>
        public PdfDestination PdfDestination
        {
            get
            {
                return _destination;
            }
        }

        public int Style
        {
            get
            {
                return _style;
            }
            set
            {
                _style = value;
            }
        }

        /// <summary>
        /// Getter for property tag.
        /// </summary>
        /// <returns>Value of property tag.</returns>
        public string Tag
        {
            get
            {
                return _tag;
            }

            set
            {
                _tag = value;
            }
        }

        public string Title
        {
            get
            {
                PdfString title = (PdfString)Get(PdfName.Title);
                return title.ToString();
            }

            set
            {
                Put(PdfName.Title, new PdfString(value, TEXT_UNICODE));
            }
        }

        internal int Count
        {
            get
            {
                return _count;
            }

            set
            {
                _count = value;
            }
        }

        public void AddKid(PdfOutline outline)
        {
            kids.Add(outline);
        }

        public bool SetDestinationPage(PdfIndirectReference pageReference)
        {
            if (_destination == null)
            {
                return false;
            }
            return _destination.AddPage(pageReference);
        }

        public override void ToPdf(PdfWriter writer, Stream os)
        {
            if (_color != null && !_color.Equals(BaseColor.Black))
            {
                Put(PdfName.C, new PdfArray(new[] { _color.R / 255f, _color.G / 255f, _color.B / 255f }));
            }
            int flag = 0;
            if ((_style & text.Font.BOLD) != 0)
                flag |= 2;
            if ((_style & text.Font.ITALIC) != 0)
                flag |= 1;
            if (flag != 0)
                Put(PdfName.F, new PdfNumber(flag));
            if (_parent != null)
            {
                Put(PdfName.Parent, _parent.IndirectReference);
            }
            if (_destination != null && _destination.HasPage())
            {
                Put(PdfName.Dest, _destination);
            }
            if (_action != null)
                Put(PdfName.A, _action);
            if (_count != 0)
            {
                Put(PdfName.Count, new PdfNumber(_count));
            }
            base.ToPdf(writer, os);
        }

        /// <summary>
        /// Helper for the constructors.
        /// </summary>
        /// <param name="parent">the parent outline</param>
        /// <param name="title">the title for this outline</param>
        /// <param name="open"> true  if the children are visible</param>
        internal void InitOutline(PdfOutline parent, string title, bool open)
        {
            _open = open;
            _parent = parent;
            Writer = parent.Writer;
            Put(PdfName.Title, new PdfString(title, TEXT_UNICODE));
            parent.AddKid(this);
            if (_destination != null && !_destination.HasPage()) // bugfix Finn Bock
                SetDestinationPage(Writer.CurrentPage);
        }
    }
}