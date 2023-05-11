using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     PdfTextArray  defines an array with displacements and  PdfString -objects.
///     A  TextArray  is used with the operator <VAR>TJ</VAR> in  PdfText .
///     The first object in this array has to be a  PdfString ;
///     see reference manual version 1.3 section 8.7.5, pages 346-347.
///     OR
///     see reference manual version 1.6 section 5.3.2, pages 378-379.
/// </summary>
public class PdfTextArray
{
    private float _lastNum = float.NaN;

    /// <summary>
    ///     To emit a more efficient array, we consolidate
    /// </summary>
    /// <summary>
    ///     repeated numbers or strings into single array entries.
    /// </summary>
    /// <summary>
    ///     "add( 50 ); Add( -50 );" will REMOVE the combined zero from the array.
    /// </summary>
    /// <summary>
    ///     the alternative (leaving a zero in there) was Just Weird.
    /// </summary>
    /// <summary>
    ///     --Mark Storer, May 12, 2008
    /// </summary>
    private string _lastStr;

    /// <summary>
    ///     constructors
    /// </summary>
    public PdfTextArray(string str)
    {
        Add(str);
    }

    public PdfTextArray()
    {
    }

    internal List<object> ArrayList { get; } = new();

    /// <summary>
    ///     Adds a  PdfNumber  to the  PdfArray .
    /// </summary>
    /// <param name="number">displacement of the string</param>
    public void Add(PdfNumber number)
    {
        if (number == null)
        {
            throw new ArgumentNullException(nameof(number));
        }

        Add((float)number.DoubleValue);
    }

    public void Add(float number)
    {
        if (number.ApproxNotEqual(0))
        {
            if (!float.IsNaN(_lastNum))
            {
                _lastNum += number;
                if (_lastNum.ApproxNotEqual(0))
                {
                    replaceLast(_lastNum);
                }
                else
                {
                    ArrayList.RemoveAt(ArrayList.Count - 1);
                }
            }
            else
            {
                _lastNum = number;
                ArrayList.Add(_lastNum);
            }

            _lastStr = null;
        }
        // adding zero doesn't modify the TextArray at all
    }

    public void Add(string str)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        if (str.Length > 0)
        {
            if (_lastStr != null)
            {
                _lastStr = _lastStr + str;
                replaceLast(_lastStr);
            }
            else
            {
                _lastStr = str;
                ArrayList.Add(_lastStr);
            }

            _lastNum = float.NaN;
        }
        // adding an empty string doesn't modify the TextArray at all
    }

    private void replaceLast(object obj)
    {
        // deliberately throw the IndexOutOfBoundsException if we screw up.
        ArrayList[ArrayList.Count - 1] = obj;
    }
}