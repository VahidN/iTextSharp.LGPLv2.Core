using System;
using System.IO;
using System.Collections;
using iTextSharp.LGPLv2.Core.System.Encodings;
using iTextSharp.LGPLv2.Core.System.NetUtils;

namespace iTextSharp.text.pdf.codec.wmf
{
    /// <summary>
    /// Summary description for MetaDo.
    /// </summary>
    public class MetaDo
    {

        public const int META_ANIMATEPALETTE = 0x0436;
        public const int META_ARC = 0x0817;
        public const int META_BITBLT = 0x0922;
        public const int META_CHORD = 0x0830;
        public const int META_CREATEBRUSHINDIRECT = 0x02FC;
        public const int META_CREATEFONTINDIRECT = 0x02FB;
        public const int META_CREATEPALETTE = 0x00f7;
        public const int META_CREATEPATTERNBRUSH = 0x01F9;
        public const int META_CREATEPENINDIRECT = 0x02FA;
        public const int META_CREATEREGION = 0x06FF;
        public const int META_DELETEOBJECT = 0x01f0;
        public const int META_DIBBITBLT = 0x0940;
        public const int META_DIBCREATEPATTERNBRUSH = 0x0142;
        public const int META_DIBSTRETCHBLT = 0x0b41;
        public const int META_ELLIPSE = 0x0418;
        public const int META_ESCAPE = 0x0626;
        public const int META_EXCLUDECLIPRECT = 0x0415;
        public const int META_EXTFLOODFILL = 0x0548;
        public const int META_EXTTEXTOUT = 0x0a32;
        public const int META_FILLREGION = 0x0228;
        public const int META_FLOODFILL = 0x0419;
        public const int META_FRAMEREGION = 0x0429;
        public const int META_INTERSECTCLIPRECT = 0x0416;
        public const int META_INVERTREGION = 0x012A;
        public const int META_LINETO = 0x0213;
        public const int META_MOVETO = 0x0214;
        public const int META_OFFSETCLIPRGN = 0x0220;
        public const int META_OFFSETVIEWPORTORG = 0x0211;
        public const int META_OFFSETWINDOWORG = 0x020F;
        public const int META_PAINTREGION = 0x012B;
        public const int META_PATBLT = 0x061D;
        public const int META_PIE = 0x081A;
        public const int META_POLYGON = 0x0324;
        public const int META_POLYLINE = 0x0325;
        public const int META_POLYPOLYGON = 0x0538;
        public const int META_REALIZEPALETTE = 0x0035;
        public const int META_RECTANGLE = 0x041B;
        public const int META_RESIZEPALETTE = 0x0139;
        public const int META_RESTOREDC = 0x0127;
        public const int META_ROUNDRECT = 0x061C;
        public const int META_SAVEDC = 0x001E;
        public const int META_SCALEVIEWPORTEXT = 0x0412;
        public const int META_SCALEWINDOWEXT = 0x0410;
        public const int META_SELECTCLIPREGION = 0x012C;
        public const int META_SELECTOBJECT = 0x012D;
        public const int META_SELECTPALETTE = 0x0234;
        public const int META_SETBKCOLOR = 0x0201;
        public const int META_SETBKMODE = 0x0102;
        public const int META_SETDIBTODEV = 0x0d33;
        public const int META_SETMAPMODE = 0x0103;
        public const int META_SETMAPPERFLAGS = 0x0231;
        public const int META_SETPALENTRIES = 0x0037;
        public const int META_SETPIXEL = 0x041F;
        public const int META_SETPOLYFILLMODE = 0x0106;
        public const int META_SETRELABS = 0x0105;
        public const int META_SETROP2 = 0x0104;
        public const int META_SETSTRETCHBLTMODE = 0x0107;
        public const int META_SETTEXTALIGN = 0x012E;
        public const int META_SETTEXTCHAREXTRA = 0x0108;
        public const int META_SETTEXTCOLOR = 0x0209;
        public const int META_SETTEXTJUSTIFICATION = 0x020A;
        public const int META_SETVIEWPORTEXT = 0x020E;
        public const int META_SETVIEWPORTORG = 0x020D;
        public const int META_SETWINDOWEXT = 0x020C;
        public const int META_SETWINDOWORG = 0x020B;
        public const int META_STRETCHBLT = 0x0B23;
        public const int META_STRETCHDIB = 0x0f43;
        public const int META_TEXTOUT = 0x0521;
        public PdfContentByte Cb;
        public InputMeta Meta;
        readonly MetaState _state = new MetaState();
        int _bottom;
        int _inch;
        int _left;
        int _right;
        int _top;

        public MetaDo(Stream meta, PdfContentByte cb)
        {
            Cb = cb;
            Meta = new InputMeta(meta);
        }

        public static byte[] WrapBmp(Image image)
        {
            if (image.OriginalType != Image.ORIGINAL_BMP)
                throw new IOException("Only BMP can be wrapped in WMF.");
            Stream imgIn;
            byte[] data = null;
            if (image.OriginalData == null)
            {
                imgIn = image.Url.GetResponseStream();
                MemoryStream outp = new MemoryStream();
                int b = 0;
                while ((b = imgIn.ReadByte()) != -1)
                    outp.WriteByte((byte)b);
                imgIn.Dispose();
                data = outp.ToArray();
            }
            else
                data = image.OriginalData;
            int sizeBmpWords = (data.Length - 14 + 1) >> 1;
            MemoryStream os = new MemoryStream();
            // write metafile header
            WriteWord(os, 1);
            WriteWord(os, 9);
            WriteWord(os, 0x0300);
            WriteDWord(os, 9 + 4 + 5 + 5 + (13 + sizeBmpWords) + 3); // total metafile size
            WriteWord(os, 1);
            WriteDWord(os, 14 + sizeBmpWords); // max record size
            WriteWord(os, 0);
            // write records
            WriteDWord(os, 4);
            WriteWord(os, META_SETMAPMODE);
            WriteWord(os, 8);

            WriteDWord(os, 5);
            WriteWord(os, META_SETWINDOWORG);
            WriteWord(os, 0);
            WriteWord(os, 0);

            WriteDWord(os, 5);
            WriteWord(os, META_SETWINDOWEXT);
            WriteWord(os, (int)image.Height);
            WriteWord(os, (int)image.Width);

            WriteDWord(os, 13 + sizeBmpWords);
            WriteWord(os, META_DIBSTRETCHBLT);
            WriteDWord(os, 0x00cc0020);
            WriteWord(os, (int)image.Height);
            WriteWord(os, (int)image.Width);
            WriteWord(os, 0);
            WriteWord(os, 0);
            WriteWord(os, (int)image.Height);
            WriteWord(os, (int)image.Width);
            WriteWord(os, 0);
            WriteWord(os, 0);
            os.Write(data, 14, data.Length - 14);
            if ((data.Length & 1) == 1)
                os.WriteByte(0);
            //        WriteDWord(os, 14 + sizeBmpWords);
            //        WriteWord(os, META_STRETCHDIB);
            //        WriteDWord(os, 0x00cc0020);
            //        WriteWord(os, 0);
            //        WriteWord(os, (int)image.Height);
            //        WriteWord(os, (int)image.Width);
            //        WriteWord(os, 0);
            //        WriteWord(os, 0);
            //        WriteWord(os, (int)image.Height);
            //        WriteWord(os, (int)image.Width);
            //        WriteWord(os, 0);
            //        WriteWord(os, 0);
            //        os.Write(data, 14, data.length - 14);
            //        if ((data.length & 1) == 1)
            //            os.Write(0);

            WriteDWord(os, 3);
            WriteWord(os, 0);
            return os.ToArray();
        }

        public static void WriteDWord(Stream os, int v)
        {
            WriteWord(os, v & 0xffff);
            WriteWord(os, (v >> 16) & 0xffff);
        }

        public static void WriteWord(Stream os, int v)
        {
            os.WriteByte((byte)(v & 0xff));
            os.WriteByte((byte)((v >> 8) & 0xff));
        }

        public bool IsNullStrokeFill(bool isRectangle)
        {
            MetaPen pen = _state.CurrentPen;
            MetaBrush brush = _state.CurrentBrush;
            bool noPen = (pen.Style == MetaPen.PS_NULL);
            int style = brush.Style;
            bool isBrush = (style == MetaBrush.BS_SOLID || (style == MetaBrush.BS_HATCHED && _state.BackgroundMode == MetaState.Opaque));
            bool result = noPen && !isBrush;
            if (!noPen)
            {
                if (isRectangle)
                    _state.LineJoinRectangle = Cb;
                else
                    _state.LineJoinPolygon = Cb;
            }
            return result;
        }

        public void OutputText(int x, int y, int flag, int x1, int y1, int x2, int y2, string text)
        {
            MetaFont font = _state.CurrentFont;
            float refX = _state.TransformX(x);
            float refY = _state.TransformY(y);
            float angle = _state.TransformAngle(font.Angle);
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);
            float fontSize = font.GetFontSize(_state);
            BaseFont bf = font.Font;
            int align = _state.TextAlign;
            float textWidth = bf.GetWidthPoint(text, fontSize);
            float tx = 0;
            float ty = 0;
            float descender = bf.GetFontDescriptor(BaseFont.DESCENT, fontSize);
            float ury = bf.GetFontDescriptor(BaseFont.BBOXURY, fontSize);
            Cb.SaveState();
            Cb.ConcatCtm(cos, sin, -sin, cos, refX, refY);
            if ((align & MetaState.TaCenter) == MetaState.TaCenter)
                tx = -textWidth / 2;
            else if ((align & MetaState.TaRight) == MetaState.TaRight)
                tx = -textWidth;
            if ((align & MetaState.TaBaseline) == MetaState.TaBaseline)
                ty = 0;
            else if ((align & MetaState.TaBottom) == MetaState.TaBottom)
                ty = -descender;
            else
                ty = -ury;
            BaseColor textColor;
            if (_state.BackgroundMode == MetaState.Opaque)
            {
                textColor = _state.CurrentBackgroundColor;
                Cb.SetColorFill(textColor);
                Cb.Rectangle(tx, ty + descender, textWidth, ury - descender);
                Cb.Fill();
            }
            textColor = _state.CurrentTextColor;
            Cb.SetColorFill(textColor);
            Cb.BeginText();
            Cb.SetFontAndSize(bf, fontSize);
            Cb.SetTextMatrix(tx, ty);
            Cb.ShowText(text);
            Cb.EndText();
            if (font.IsUnderline())
            {
                Cb.Rectangle(tx, ty - fontSize / 4, textWidth, fontSize / 15);
                Cb.Fill();
            }
            if (font.IsStrikeout())
            {
                Cb.Rectangle(tx, ty + fontSize / 3, textWidth, fontSize / 15);
                Cb.Fill();
            }
            Cb.RestoreState();
        }

        public void ReadAll()
        {
            if (Meta.ReadInt() != unchecked((int)0x9AC6CDD7))
            {
                throw new DocumentException("Not a placeable windows metafile");
            }
            Meta.ReadWord();
            _left = Meta.ReadShort();
            _top = Meta.ReadShort();
            _right = Meta.ReadShort();
            _bottom = Meta.ReadShort();
            _inch = Meta.ReadWord();
            _state.ScalingX = (_right - _left) / (float)_inch * 72f;
            _state.ScalingY = (_bottom - _top) / (float)_inch * 72f;
            _state.OffsetWx = _left;
            _state.OffsetWy = _top;
            _state.ExtentWx = _right - _left;
            _state.ExtentWy = _bottom - _top;
            Meta.ReadInt();
            Meta.ReadWord();
            Meta.Skip(18);

            int tsize;
            int function;
            Cb.SetLineCap(1);
            Cb.SetLineJoin(1);
            for (;;)
            {
                int lenMarker = Meta.Length;
                tsize = Meta.ReadInt();
                if (tsize < 3)
                    break;
                function = Meta.ReadWord();
                switch (function)
                {
                    case 0:
                        break;
                    case META_CREATEPALETTE:
                    case META_CREATEREGION:
                    case META_DIBCREATEPATTERNBRUSH:
                        _state.AddMetaObject(new MetaObject());
                        break;
                    case META_CREATEPENINDIRECT:
                        {
                            MetaPen pen = new MetaPen();
                            pen.Init(Meta);
                            _state.AddMetaObject(pen);
                            break;
                        }
                    case META_CREATEBRUSHINDIRECT:
                        {
                            MetaBrush brush = new MetaBrush();
                            brush.Init(Meta);
                            _state.AddMetaObject(brush);
                            break;
                        }
                    case META_CREATEFONTINDIRECT:
                        {
                            MetaFont font = new MetaFont();
                            font.Init(Meta);
                            _state.AddMetaObject(font);
                            break;
                        }
                    case META_SELECTOBJECT:
                        {
                            int idx = Meta.ReadWord();
                            _state.SelectMetaObject(idx, Cb);
                            break;
                        }
                    case META_DELETEOBJECT:
                        {
                            int idx = Meta.ReadWord();
                            _state.DeleteMetaObject(idx);
                            break;
                        }
                    case META_SAVEDC:
                        _state.SaveState(Cb);
                        break;
                    case META_RESTOREDC:
                        {
                            int idx = Meta.ReadShort();
                            _state.RestoreState(idx, Cb);
                            break;
                        }
                    case META_SETWINDOWORG:
                        _state.OffsetWy = Meta.ReadShort();
                        _state.OffsetWx = Meta.ReadShort();
                        break;
                    case META_SETWINDOWEXT:
                        _state.ExtentWy = Meta.ReadShort();
                        _state.ExtentWx = Meta.ReadShort();
                        break;
                    case META_MOVETO:
                        {
                            int y = Meta.ReadShort();
                            System.Drawing.Point p = new System.Drawing.Point(Meta.ReadShort(), y);
                            _state.CurrentPoint = p;
                            break;
                        }
                    case META_LINETO:
                        {
                            int y = Meta.ReadShort();
                            int x = Meta.ReadShort();
                            System.Drawing.Point p = _state.CurrentPoint;
                            Cb.MoveTo(_state.TransformX(p.X), _state.TransformY(p.Y));
                            Cb.LineTo(_state.TransformX(x), _state.TransformY(y));
                            Cb.Stroke();
                            _state.CurrentPoint = new System.Drawing.Point(x, y);
                            break;
                        }
                    case META_POLYLINE:
                        {
                            _state.LineJoinPolygon = Cb;
                            int len = Meta.ReadWord();
                            int x = Meta.ReadShort();
                            int y = Meta.ReadShort();
                            Cb.MoveTo(_state.TransformX(x), _state.TransformY(y));
                            for (int k = 1; k < len; ++k)
                            {
                                x = Meta.ReadShort();
                                y = Meta.ReadShort();
                                Cb.LineTo(_state.TransformX(x), _state.TransformY(y));
                            }
                            Cb.Stroke();
                            break;
                        }
                    case META_POLYGON:
                        {
                            if (IsNullStrokeFill(false))
                                break;
                            int len = Meta.ReadWord();
                            int sx = Meta.ReadShort();
                            int sy = Meta.ReadShort();
                            Cb.MoveTo(_state.TransformX(sx), _state.TransformY(sy));
                            for (int k = 1; k < len; ++k)
                            {
                                int x = Meta.ReadShort();
                                int y = Meta.ReadShort();
                                Cb.LineTo(_state.TransformX(x), _state.TransformY(y));
                            }
                            Cb.LineTo(_state.TransformX(sx), _state.TransformY(sy));
                            StrokeAndFill();
                            break;
                        }
                    case META_POLYPOLYGON:
                        {
                            if (IsNullStrokeFill(false))
                                break;
                            int numPoly = Meta.ReadWord();
                            int[] lens = new int[numPoly];
                            for (int k = 0; k < lens.Length; ++k)
                                lens[k] = Meta.ReadWord();
                            for (int j = 0; j < lens.Length; ++j)
                            {
                                int len = lens[j];
                                int sx = Meta.ReadShort();
                                int sy = Meta.ReadShort();
                                Cb.MoveTo(_state.TransformX(sx), _state.TransformY(sy));
                                for (int k = 1; k < len; ++k)
                                {
                                    int x = Meta.ReadShort();
                                    int y = Meta.ReadShort();
                                    Cb.LineTo(_state.TransformX(x), _state.TransformY(y));
                                }
                                Cb.LineTo(_state.TransformX(sx), _state.TransformY(sy));
                            }
                            StrokeAndFill();
                            break;
                        }
                    case META_ELLIPSE:
                        {
                            if (IsNullStrokeFill(_state.LineNeutral))
                                break;
                            int b = Meta.ReadShort();
                            int r = Meta.ReadShort();
                            int t = Meta.ReadShort();
                            int l = Meta.ReadShort();
                            Cb.Arc(_state.TransformX(l), _state.TransformY(b), _state.TransformX(r), _state.TransformY(t), 0, 360);
                            StrokeAndFill();
                            break;
                        }
                    case META_ARC:
                        {
                            if (IsNullStrokeFill(_state.LineNeutral))
                                break;
                            float yend = _state.TransformY(Meta.ReadShort());
                            float xend = _state.TransformX(Meta.ReadShort());
                            float ystart = _state.TransformY(Meta.ReadShort());
                            float xstart = _state.TransformX(Meta.ReadShort());
                            float b = _state.TransformY(Meta.ReadShort());
                            float r = _state.TransformX(Meta.ReadShort());
                            float t = _state.TransformY(Meta.ReadShort());
                            float l = _state.TransformX(Meta.ReadShort());
                            float cx = (r + l) / 2;
                            float cy = (t + b) / 2;
                            float arc1 = GetArc(cx, cy, xstart, ystart);
                            float arc2 = GetArc(cx, cy, xend, yend);
                            arc2 -= arc1;
                            if (arc2 <= 0)
                                arc2 += 360;
                            Cb.Arc(l, b, r, t, arc1, arc2);
                            Cb.Stroke();
                            break;
                        }
                    case META_PIE:
                        {
                            if (IsNullStrokeFill(_state.LineNeutral))
                                break;
                            float yend = _state.TransformY(Meta.ReadShort());
                            float xend = _state.TransformX(Meta.ReadShort());
                            float ystart = _state.TransformY(Meta.ReadShort());
                            float xstart = _state.TransformX(Meta.ReadShort());
                            float b = _state.TransformY(Meta.ReadShort());
                            float r = _state.TransformX(Meta.ReadShort());
                            float t = _state.TransformY(Meta.ReadShort());
                            float l = _state.TransformX(Meta.ReadShort());
                            float cx = (r + l) / 2;
                            float cy = (t + b) / 2;
                            float arc1 = GetArc(cx, cy, xstart, ystart);
                            float arc2 = GetArc(cx, cy, xend, yend);
                            arc2 -= arc1;
                            if (arc2 <= 0)
                                arc2 += 360;
                            ArrayList ar = PdfContentByte.BezierArc(l, b, r, t, arc1, arc2);
                            if (ar.Count == 0)
                                break;
                            float[] pt = (float[])ar[0];
                            Cb.MoveTo(cx, cy);
                            Cb.LineTo(pt[0], pt[1]);
                            for (int k = 0; k < ar.Count; ++k)
                            {
                                pt = (float[])ar[k];
                                Cb.CurveTo(pt[2], pt[3], pt[4], pt[5], pt[6], pt[7]);
                            }
                            Cb.LineTo(cx, cy);
                            StrokeAndFill();
                            break;
                        }
                    case META_CHORD:
                        {
                            if (IsNullStrokeFill(_state.LineNeutral))
                                break;
                            float yend = _state.TransformY(Meta.ReadShort());
                            float xend = _state.TransformX(Meta.ReadShort());
                            float ystart = _state.TransformY(Meta.ReadShort());
                            float xstart = _state.TransformX(Meta.ReadShort());
                            float b = _state.TransformY(Meta.ReadShort());
                            float r = _state.TransformX(Meta.ReadShort());
                            float t = _state.TransformY(Meta.ReadShort());
                            float l = _state.TransformX(Meta.ReadShort());
                            float cx = (r + l) / 2;
                            float cy = (t + b) / 2;
                            float arc1 = GetArc(cx, cy, xstart, ystart);
                            float arc2 = GetArc(cx, cy, xend, yend);
                            arc2 -= arc1;
                            if (arc2 <= 0)
                                arc2 += 360;
                            ArrayList ar = PdfContentByte.BezierArc(l, b, r, t, arc1, arc2);
                            if (ar.Count == 0)
                                break;
                            float[] pt = (float[])ar[0];
                            cx = pt[0];
                            cy = pt[1];
                            Cb.MoveTo(cx, cy);
                            for (int k = 0; k < ar.Count; ++k)
                            {
                                pt = (float[])ar[k];
                                Cb.CurveTo(pt[2], pt[3], pt[4], pt[5], pt[6], pt[7]);
                            }
                            Cb.LineTo(cx, cy);
                            StrokeAndFill();
                            break;
                        }
                    case META_RECTANGLE:
                        {
                            if (IsNullStrokeFill(true))
                                break;
                            float b = _state.TransformY(Meta.ReadShort());
                            float r = _state.TransformX(Meta.ReadShort());
                            float t = _state.TransformY(Meta.ReadShort());
                            float l = _state.TransformX(Meta.ReadShort());
                            Cb.Rectangle(l, b, r - l, t - b);
                            StrokeAndFill();
                            break;
                        }
                    case META_ROUNDRECT:
                        {
                            if (IsNullStrokeFill(true))
                                break;
                            float h = _state.TransformY(0) - _state.TransformY(Meta.ReadShort());
                            float w = _state.TransformX(Meta.ReadShort()) - _state.TransformX(0);
                            float b = _state.TransformY(Meta.ReadShort());
                            float r = _state.TransformX(Meta.ReadShort());
                            float t = _state.TransformY(Meta.ReadShort());
                            float l = _state.TransformX(Meta.ReadShort());
                            Cb.RoundRectangle(l, b, r - l, t - b, (h + w) / 4);
                            StrokeAndFill();
                            break;
                        }
                    case META_INTERSECTCLIPRECT:
                        {
                            float b = _state.TransformY(Meta.ReadShort());
                            float r = _state.TransformX(Meta.ReadShort());
                            float t = _state.TransformY(Meta.ReadShort());
                            float l = _state.TransformX(Meta.ReadShort());
                            Cb.Rectangle(l, b, r - l, t - b);
                            Cb.EoClip();
                            Cb.NewPath();
                            break;
                        }
                    case META_EXTTEXTOUT:
                        {
                            int y = Meta.ReadShort();
                            int x = Meta.ReadShort();
                            int count = Meta.ReadWord();
                            int flag = Meta.ReadWord();
                            int x1 = 0;
                            int y1 = 0;
                            int x2 = 0;
                            int y2 = 0;
                            if ((flag & (MetaFont.ETO_CLIPPED | MetaFont.ETO_OPAQUE)) != 0)
                            {
                                x1 = Meta.ReadShort();
                                y1 = Meta.ReadShort();
                                x2 = Meta.ReadShort();
                                y2 = Meta.ReadShort();
                            }
                            byte[] text = new byte[count];
                            int k;
                            for (k = 0; k < count; ++k)
                            {
                                byte c = (byte)Meta.ReadByte();
                                if (c == 0)
                                    break;
                                text[k] = c;
                            }
                            string s;
                            try
                            {
                                s = EncodingsRegistry.Instance.GetEncoding(1252).GetString(text, 0, k);
                            }
                            catch
                            {
                                s = System.Text.Encoding.ASCII.GetString(text, 0, k);
                            }
                            OutputText(x, y, flag, x1, y1, x2, y2, s);
                            break;
                        }
                    case META_TEXTOUT:
                        {
                            int count = Meta.ReadWord();
                            byte[] text = new byte[count];
                            int k;
                            for (k = 0; k < count; ++k)
                            {
                                byte c = (byte)Meta.ReadByte();
                                if (c == 0)
                                    break;
                                text[k] = c;
                            }
                            string s;
                            try
                            {
                                s = EncodingsRegistry.Instance.GetEncoding(1252).GetString(text, 0, k);
                            }
                            catch
                            {
                                s = System.Text.Encoding.ASCII.GetString(text, 0, k);
                            }
                            count = (count + 1) & 0xfffe;
                            Meta.Skip(count - k);
                            int y = Meta.ReadShort();
                            int x = Meta.ReadShort();
                            OutputText(x, y, 0, 0, 0, 0, 0, s);
                            break;
                        }
                    case META_SETBKCOLOR:
                        _state.CurrentBackgroundColor = Meta.ReadColor();
                        break;
                    case META_SETTEXTCOLOR:
                        _state.CurrentTextColor = Meta.ReadColor();
                        break;
                    case META_SETTEXTALIGN:
                        _state.TextAlign = Meta.ReadWord();
                        break;
                    case META_SETBKMODE:
                        _state.BackgroundMode = Meta.ReadWord();
                        break;
                    case META_SETPOLYFILLMODE:
                        _state.PolyFillMode = Meta.ReadWord();
                        break;
                    case META_SETPIXEL:
                        {
                            BaseColor color = Meta.ReadColor();
                            int y = Meta.ReadShort();
                            int x = Meta.ReadShort();
                            Cb.SaveState();
                            Cb.SetColorFill(color);
                            Cb.Rectangle(_state.TransformX(x), _state.TransformY(y), .2f, .2f);
                            Cb.Fill();
                            Cb.RestoreState();
                            break;
                        }
                    case META_DIBSTRETCHBLT:
                    case META_STRETCHDIB:
                        {
                            int rop = Meta.ReadInt();
                            if (function == META_STRETCHDIB)
                            {
                                /*int usage = */
                                Meta.ReadWord();
                            }
                            int srcHeight = Meta.ReadShort();
                            int srcWidth = Meta.ReadShort();
                            int ySrc = Meta.ReadShort();
                            int xSrc = Meta.ReadShort();
                            float destHeight = _state.TransformY(Meta.ReadShort()) - _state.TransformY(0);
                            float destWidth = _state.TransformX(Meta.ReadShort()) - _state.TransformX(0);
                            float yDest = _state.TransformY(Meta.ReadShort());
                            float xDest = _state.TransformX(Meta.ReadShort());
                            byte[] b = new byte[(tsize * 2) - (Meta.Length - lenMarker)];
                            for (int k = 0; k < b.Length; ++k)
                                b[k] = (byte)Meta.ReadByte();
                            try
                            {
                                MemoryStream inb = new MemoryStream(b);
                                Image bmp = BmpImage.GetImage(inb, true, b.Length);
                                Cb.SaveState();
                                Cb.Rectangle(xDest, yDest, destWidth, destHeight);
                                Cb.Clip();
                                Cb.NewPath();
                                bmp.ScaleAbsolute(destWidth * bmp.Width / srcWidth, -destHeight * bmp.Height / srcHeight);
                                bmp.SetAbsolutePosition(xDest - destWidth * xSrc / srcWidth, yDest + destHeight * ySrc / srcHeight - bmp.ScaledHeight);
                                Cb.AddImage(bmp);
                                Cb.RestoreState();
                            }
                            catch
                            {
                                // empty on purpose
                            }
                            break;
                        }
                }
                Meta.Skip((tsize * 2) - (Meta.Length - lenMarker));
            }
            _state.Cleanup(Cb);
        }
        public void StrokeAndFill()
        {
            MetaPen pen = _state.CurrentPen;
            MetaBrush brush = _state.CurrentBrush;
            int penStyle = pen.Style;
            int brushStyle = brush.Style;
            if (penStyle == MetaPen.PS_NULL)
            {
                Cb.ClosePath();
                if (_state.PolyFillMode == MetaState.Alternate)
                {
                    Cb.EoFill();
                }
                else
                {
                    Cb.Fill();
                }
            }
            else
            {
                bool isBrush = (brushStyle == MetaBrush.BS_SOLID || (brushStyle == MetaBrush.BS_HATCHED && _state.BackgroundMode == MetaState.Opaque));
                if (isBrush)
                {
                    if (_state.PolyFillMode == MetaState.Alternate)
                        Cb.ClosePathEoFillStroke();
                    else
                        Cb.ClosePathFillStroke();
                }
                else
                {
                    Cb.ClosePathStroke();
                }
            }
        }

        internal static float GetArc(float xCenter, float yCenter, float xDot, float yDot)
        {
            double s = Math.Atan2(yDot - yCenter, xDot - xCenter);
            if (s < 0)
                s += Math.PI * 2;
            return (float)(s / Math.PI * 180);
        }
    }
}
