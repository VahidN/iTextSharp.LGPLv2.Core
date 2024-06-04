using System.Drawing;

namespace iTextSharp.text.pdf.codec.wmf;

public class MetaState
{
    public static int Alternate = 1;
    public static int Opaque = 2;
    public static int TaBaseline = 24;
    public static int TaBottom = 8;
    public static int TaCenter = 6;
    public static int TaLeft = 0;
    public static int TaNoupdatecp = 0;
    public static int TaRight = 2;
    public static int TaTop = 0;
    public static int TaUpdatecp = 1;
    public static int Transparent = 1;
    public static int Winding = 2;

    public int backgroundMode = Opaque;
    public BaseColor currentBackgroundColor = BaseColor.White;
    public MetaBrush currentBrush;
    public MetaFont currentFont;
    public MetaPen currentPen;
    public Point currentPoint;
    public BaseColor currentTextColor = BaseColor.Black;
    public int extentWx;
    public int extentWy;
    public int LineJoin = 1;
    public List<MetaObject> MetaObjects;
    public int offsetWx;
    public int offsetWy;
    public int polyFillMode = Alternate;
    public Stack<MetaState> SavedStates;
    public float scalingX;
    public float scalingY;
    public int textAlign;

    /// <summary>
    ///     Creates new MetaState
    /// </summary>
    public MetaState()
    {
        SavedStates = new Stack<MetaState>();
        MetaObjects = new List<MetaObject>();
        currentPoint = new Point(0, 0);
        currentPen = new MetaPen();
        currentBrush = new MetaBrush();
        currentFont = new MetaFont();
    }

    public MetaState(MetaState state) => metaState = state;

    /// <summary>
    ///     Getter for property backgroundMode.
    /// </summary>
    /// <returns>Value of property backgroundMode.</returns>
    public int BackgroundMode
    {
        get => backgroundMode;

        set => backgroundMode = value;
    }

    /// <summary>
    ///     Getter for property currentBackgroundColor.
    /// </summary>
    /// <returns>Value of property currentBackgroundColor.</returns>
    public BaseColor CurrentBackgroundColor
    {
        get => currentBackgroundColor;

        set => currentBackgroundColor = value;
    }

    public MetaBrush CurrentBrush => currentBrush;

    public MetaFont CurrentFont => currentFont;

    public MetaPen CurrentPen => currentPen;

    public Point CurrentPoint
    {
        get => currentPoint;

        set => currentPoint = value;
    }

    /// <summary>
    ///     Getter for property currentTextColor.
    /// </summary>
    /// <returns>Value of property currentTextColor.</returns>
    public BaseColor CurrentTextColor
    {
        get => currentTextColor;

        set => currentTextColor = value;
    }

    public int ExtentWx
    {
        set => extentWx = value;
    }

    public int ExtentWy
    {
        set => extentWy = value;
    }

    public PdfContentByte LineJoinPolygon
    {
        set
        {
            if (LineJoin == 0)
            {
                LineJoin = 1;
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                value.SetLineJoin(1);
            }
        }
    }

    public PdfContentByte LineJoinRectangle
    {
        set
        {
            if (LineJoin != 0)
            {
                LineJoin = 0;
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                value.SetLineJoin(0);
            }
        }
    }

    public bool LineNeutral => LineJoin == 0;

    public MetaState metaState
    {
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            SavedStates = value.SavedStates;
            MetaObjects = value.MetaObjects;
            currentPoint = value.currentPoint;
            currentPen = value.currentPen;
            currentBrush = value.currentBrush;
            currentFont = value.currentFont;
            currentBackgroundColor = value.currentBackgroundColor;
            currentTextColor = value.currentTextColor;
            backgroundMode = value.backgroundMode;
            polyFillMode = value.polyFillMode;
            textAlign = value.textAlign;
            LineJoin = value.LineJoin;
            offsetWx = value.offsetWx;
            offsetWy = value.offsetWy;
            extentWx = value.extentWx;
            extentWy = value.extentWy;
            scalingX = value.scalingX;
            scalingY = value.scalingY;
        }
    }

    public int OffsetWx
    {
        set => offsetWx = value;
    }

    public int OffsetWy
    {
        set => offsetWy = value;
    }

    /// <summary>
    ///     Getter for property polyFillMode.
    /// </summary>
    /// <returns>Value of property polyFillMode.</returns>
    public int PolyFillMode
    {
        get => polyFillMode;

        set => polyFillMode = value;
    }

    public float ScalingX
    {
        set => scalingX = value;
    }

    public float ScalingY
    {
        set => scalingY = value;
    }

    /// <summary>
    ///     Getter for property textAlign.
    /// </summary>
    /// <returns>Value of property textAlign.</returns>
    public int TextAlign
    {
        get => textAlign;

        set => textAlign = value;
    }

    public void AddMetaObject(MetaObject obj)
    {
        for (var k = 0; k < MetaObjects.Count; ++k)
        {
            if (MetaObjects[k] == null)
            {
                MetaObjects[k] = obj;
                return;
            }
        }

        MetaObjects.Add(obj);
    }

    public void Cleanup(PdfContentByte cb)
    {
        if (cb == null)
        {
            throw new ArgumentNullException(nameof(cb));
        }

        var k = SavedStates.Count;
        while (k-- > 0)
        {
            cb.RestoreState();
        }
    }

    public void DeleteMetaObject(int index)
    {
        MetaObjects[index] = null;
    }

    public void RestoreState(int index, PdfContentByte cb)
    {
        if (cb == null)
        {
            throw new ArgumentNullException(nameof(cb));
        }

        int pops;
        if (index < 0)
        {
            pops = Math.Min(-index, SavedStates.Count);
        }
        else
        {
            pops = Math.Max(SavedStates.Count - index, 0);
        }

        if (pops == 0)
        {
            return;
        }

        MetaState state = null;
        while (pops-- != 0)
        {
            cb.RestoreState();
            state = SavedStates.Pop();
        }

        metaState = state;
    }

    public void SaveState(PdfContentByte cb)
    {
        if (cb == null)
        {
            throw new ArgumentNullException(nameof(cb));
        }

        cb.SaveState();
        var state = new MetaState(this);
        SavedStates.Push(state);
    }

    public void SelectMetaObject(int index, PdfContentByte cb)
    {
        if (cb == null)
        {
            throw new ArgumentNullException(nameof(cb));
        }

        var obj = MetaObjects[index];
        if (obj == null)
        {
            return;
        }

        int style;
        switch (obj.Type)
        {
            case MetaObject.META_BRUSH:
                currentBrush = (MetaBrush)obj;
                style = currentBrush.Style;
                if (style == MetaBrush.BS_SOLID)
                {
                    var color = currentBrush.Color;
                    cb.SetColorFill(color);
                }
                else if (style == MetaBrush.BS_HATCHED)
                {
                    var color = currentBackgroundColor;
                    cb.SetColorFill(color);
                }

                break;
            case MetaObject.META_PEN:
            {
                currentPen = (MetaPen)obj;
                style = currentPen.Style;
                if (style != MetaPen.PS_NULL)
                {
                    var color = currentPen.Color;
                    cb.SetColorStroke(color);
                    cb.SetLineWidth(Math.Abs(currentPen.PenWidth * scalingX / extentWx));
                    switch (style)
                    {
                        case MetaPen.PS_DASH:
                            cb.SetLineDash(18, 6, 0);
                            break;
                        case MetaPen.PS_DASHDOT:
                            cb.SetLiteral("[9 6 3 6]0 d\n");
                            break;
                        case MetaPen.PS_DASHDOTDOT:
                            cb.SetLiteral("[9 3 3 3 3 3]0 d\n");
                            break;
                        case MetaPen.PS_DOT:
                            cb.SetLineDash(3, 0);
                            break;
                        default:
                            cb.SetLineDash(0);
                            break;
                    }
                }

                break;
            }
            case MetaObject.META_FONT:
            {
                currentFont = (MetaFont)obj;
                break;
            }
        }
    }

    public float TransformAngle(float angle)
    {
        var ta = scalingY < 0 ? -angle : angle;
        return (float)(scalingX < 0 ? Math.PI - ta : ta);
    }

    public float TransformX(int x) => ((float)x - offsetWx) * scalingX / extentWx;

    public float TransformY(int y) => (1f - ((float)y - offsetWy) / extentWy) * scalingY;
}