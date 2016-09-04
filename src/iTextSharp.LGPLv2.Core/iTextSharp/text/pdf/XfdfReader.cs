using System;
using System.IO;
using System.Collections;
using iTextSharp.text.xml.simpleparser;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// Reads a XFDF.
    /// @author Leonard Rosenthol (leonardr@pdfsages.com)
    /// </summary>
    public class XfdfReader : ISimpleXmlDocHandler
    {
        /// <summary>
        /// storage for the field list and their values
        /// </summary>
        internal Hashtable fields;

        /// <summary>
        /// storage for the path to referenced PDF, if any
        /// </summary>
        internal string fileSpec;

        /// <summary>
        /// Storage for field values if there's more than one value for a field.
        /// @since    2.1.4
        /// </summary>
        protected Hashtable ListFields;

        private readonly Stackr _fieldNames = new Stackr();

        private readonly Stackr _fieldValues = new Stackr();

        /// <summary>
        /// stuff used during parsing to handle state
        /// </summary>
        private bool _foundRoot;

        /// <summary>
        /// Reads an XFDF form.
        /// @throws IOException on error
        /// </summary>
        /// <param name="filename">the file name of the form</param>
        public XfdfReader(string filename)
        {
            FileStream fin = null;
            try
            {
                fin = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                SimpleXmlParser.Parse(this, fin);
            }
            finally
            {
                try
                {
                    fin?.Dispose();
                } catch { }
            }
        }

        /// <summary>
        /// Reads an XFDF form.
        /// @throws IOException on error
        /// </summary>
        /// <param name="xfdfIn">the byte array with the form</param>
        public XfdfReader(byte[] xfdfIn)
        {
            SimpleXmlParser.Parse(this, new MemoryStream(xfdfIn));
        }

        /// <summary>
        /// Gets all the fields. The map is keyed by the fully qualified
        /// field name and the value is a merged  PdfDictionary
        /// with the field content.
        /// </summary>
        /// <returns>all the fields</returns>
        public Hashtable Fields
        {
            get
            {
                return fields;
            }
        }

        /// <summary>
        /// Gets the PDF file specification contained in the FDF.
        /// </summary>
        /// <returns>the PDF file specification contained in the FDF</returns>
        public string FileSpec
        {
            get
            {
                return fileSpec;
            }
        }

        /// <summary>
        /// Called after the document is parsed.
        /// </summary>
        public void EndDocument()
        {

        }

        /// <summary>
        /// Called when an end tag is found.
        /// </summary>
        /// <param name="tag">the tag name</param>
        public void EndElement(string tag)
        {
            if (tag.Equals("value"))
            {
                string fName = "";
                for (int k = 0; k < _fieldNames.Count; ++k)
                {
                    fName += "." + (string)_fieldNames[k];
                }
                if (fName.StartsWith("."))
                    fName = fName.Substring(1);
                string fVal = (string)_fieldValues.Pop();
                string old = (string)fields[fName];
                fields[fName] = fVal;
                if (old != null)
                {
                    ArrayList l = (ArrayList)ListFields[fName];
                    if (l == null)
                    {
                        l = new ArrayList();
                        l.Add(old);
                    }
                    l.Add(fVal);
                    ListFields[fName] = l;
                }
            }
            else if (tag.Equals("field"))
            {
                if (_fieldNames.Count != 0)
                    _fieldNames.Pop();
            }
        }

        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <param name="name">the fully qualified field name</param>
        /// <returns>the field's value</returns>
        public string GetField(string name)
        {
            return (string)fields[name];
        }

        /// <summary>
        /// Gets the field value or  null  if the field does not
        /// exist or has no value defined.
        /// </summary>
        /// <param name="name">the fully qualified field name</param>
        /// <returns>the field value or  null </returns>
        public string GetFieldValue(string name)
        {
            string field = (string)fields[name];
            if (field == null)
                return null;
            else
                return field;
        }

        /// <summary>
        /// Gets the field values for a list or  null  if the field does not
        /// exist or has no value defined.
        /// @since   2.1.4
        /// </summary>
        /// <param name="name">the fully qualified field name</param>
        /// <returns>the field values or  null </returns>
        public ArrayList GetListValues(string name)
        {
            return (ArrayList)ListFields[name];
        }
        /// <summary>
        /// Called when the document starts to be parsed.
        /// </summary>
        public void StartDocument()
        {
            fileSpec = "";  // and this too...
        }

        /// <summary>
        /// Called when a start tag is found.
        /// </summary>
        /// <param name="tag">the tag name</param>
        /// <param name="h">the tag's attributes</param>
        public void StartElement(string tag, Hashtable h)
        {
            if (!_foundRoot)
            {
                if (!tag.Equals("xfdf"))
                    throw new Exception("Root element is not Bookmark.");
                else
                    _foundRoot = true;
            }

            if (tag.Equals("xfdf"))
            {

            }
            else if (tag.Equals("f"))
            {
                fileSpec = (string)h["href"];
            }
            else if (tag.Equals("fields"))
            {
                fields = new Hashtable();     // init it!
                ListFields = new Hashtable();
            }
            else if (tag.Equals("field"))
            {
                string fName = (string)h["name"];
                _fieldNames.Push(fName);
            }
            else if (tag.Equals("value"))
            {
                _fieldValues.Push("");
            }
        }
        /// <summary>
        /// Called when a text element is found.
        /// </summary>
        /// <param name="str">the text element, probably a fragment.</param>
        public void Text(string str)
        {
            if (_fieldNames.Count == 0 || _fieldValues.Count == 0)
                return;

            string val = (string)_fieldValues.Pop();
            val += str;
            _fieldValues.Push(val);
        }

        internal class Stackr : ArrayList
        {
            internal object Pop()
            {
                if (Count == 0)
                    throw new InvalidOperationException("The stack is empty.");
                object obj = this[Count - 1];
                RemoveAt(Count - 1);
                return obj;
            }

            internal void Push(object obj)
            {
                Add(obj);
            }
        }
    }
}