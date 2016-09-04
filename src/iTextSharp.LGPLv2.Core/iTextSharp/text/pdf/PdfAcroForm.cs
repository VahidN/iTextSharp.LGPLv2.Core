using System.Text;
using System.Collections;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// Each PDF document can contain maximum 1 AcroForm.
    /// </summary>
    public class PdfAcroForm : PdfDictionary
    {

        /// <summary>
        /// This is an array containing the calculationorder of the fields.
        /// </summary>
        private readonly PdfArray _calculationOrder = new PdfArray();

        /// <summary>
        /// This is an array containing DocumentFields.
        /// </summary>
        private readonly PdfArray _documentFields = new PdfArray();

        /// <summary>
        /// This is a map containing FieldTemplates.
        /// </summary>
        private readonly Hashtable _fieldTemplates = new Hashtable();

        private readonly PdfWriter _writer;
        /// <summary>
        /// Contains the signature flags.
        /// </summary>
        private int _sigFlags;

        /// <summary>
        /// Creates new PdfAcroForm
        /// </summary>
        public PdfAcroForm(PdfWriter writer)
        {
            _writer = writer;
        }

        public bool NeedAppearances
        {
            set
            {
                Put(PdfName.Needappearances, value ? PdfBoolean.Pdftrue : PdfBoolean.Pdffalse);
            }
        }


        /// <summary>
        /// Adds fieldTemplates.
        /// </summary>

        public int SigFlags
        {
            set
            {
                _sigFlags |= value;
            }
        }

        public void AddCalculationOrder(PdfFormField formField)
        {
            _calculationOrder.Add(formField.IndirectReference);
        }

        public PdfFormField AddCheckBox(string name, string value, bool status, float llx, float lly, float urx, float ury)
        {
            PdfFormField field = PdfFormField.CreateCheckBox(_writer);
            SetCheckBoxParams(field, name, value, status, llx, lly, urx, ury);
            DrawCheckBoxAppearences(field, value, llx, lly, urx, ury);
            AddFormField(field);
            return field;
        }

        public PdfFormField AddComboBox(string name, string[] options, string defaultValue, bool editable, BaseFont font, float fontSize, float llx, float lly, float urx, float ury)
        {
            PdfFormField choice = PdfFormField.CreateCombo(_writer, editable, options, 0);
            SetChoiceParams(choice, name, defaultValue, llx, lly, urx, ury);
            if (defaultValue == null)
            {
                defaultValue = options[0];
            }
            DrawSingleLineOfText(choice, defaultValue, font, fontSize, llx, lly, urx, ury);
            AddFormField(choice);
            return choice;
        }

        public PdfFormField AddComboBox(string name, string[,] options, string defaultValue, bool editable, BaseFont font, float fontSize, float llx, float lly, float urx, float ury)
        {
            PdfFormField choice = PdfFormField.CreateCombo(_writer, editable, options, 0);
            SetChoiceParams(choice, name, defaultValue, llx, lly, urx, ury);
            string value = null;
            for (int i = 0; i < options.GetLength(0); i++)
            {
                if (options[i, 0].Equals(defaultValue))
                {
                    value = options[i, 1];
                    break;
                }
            }
            if (value == null)
            {
                value = options[0, 1];
            }
            DrawSingleLineOfText(choice, value, font, fontSize, llx, lly, urx, ury);
            AddFormField(choice);
            return choice;
        }

        public void AddDocumentField(PdfIndirectReference piref)
        {
            _documentFields.Add(piref);
        }

        public void AddFieldTemplates(Hashtable ft)
        {
            foreach (object key in ft.Keys)
            {
                _fieldTemplates[key] = ft[key];
            }
        }

        /// <summary>
        /// Adds documentFields.
        /// </summary>
        /// <summary>
        /// Closes the AcroForm.
        /// </summary>

        public void AddFormField(PdfFormField formField)
        {
            _writer.AddAnnotation(formField);
        }

        public PdfFormField AddHiddenField(string name, string value)
        {
            PdfFormField hidden = PdfFormField.CreateEmpty(_writer);
            hidden.FieldName = name;
            hidden.ValueAsName = value;
            AddFormField(hidden);
            return hidden;
        }

        /// <summary>
        /// Adds an object to the calculationOrder.
        /// </summary>
        /// <summary>
        /// Sets the signature flags.
        /// </summary>
        /// <summary>
        /// Adds a formfield to the AcroForm.
        /// </summary>
        public PdfFormField AddHtmlPostButton(string name, string caption, string value, string url, BaseFont font, float fontSize, float llx, float lly, float urx, float ury)
        {
            PdfAction action = PdfAction.CreateSubmitForm(url, null, PdfAction.SUBMIT_HTML_FORMAT);
            PdfFormField button = new PdfFormField(_writer, llx, lly, urx, ury, action);
            SetButtonParams(button, PdfFormField.FF_PUSHBUTTON, name, value);
            DrawButton(button, caption, font, fontSize, llx, lly, urx, ury);
            AddFormField(button);
            return button;
        }

        public PdfFormField AddMap(string name, string value, string url, PdfContentByte appearance, float llx, float lly, float urx, float ury)
        {
            PdfAction action = PdfAction.CreateSubmitForm(url, null, PdfAction.SUBMIT_HTML_FORMAT | PdfAction.SUBMIT_COORDINATES);
            PdfFormField button = new PdfFormField(_writer, llx, lly, urx, ury, action);
            SetButtonParams(button, PdfFormField.FF_PUSHBUTTON, name, null);
            PdfAppearance pa = PdfAppearance.CreateAppearance(_writer, urx - llx, ury - lly);
            pa.Add(appearance);
            button.SetAppearance(PdfAnnotation.AppearanceNormal, pa);
            AddFormField(button);
            return button;
        }

        public PdfFormField AddMultiLineTextField(string name, string text, BaseFont font, float fontSize, float llx, float lly, float urx, float ury)
        {
            PdfFormField field = PdfFormField.CreateTextField(_writer, PdfFormField.MULTILINE, PdfFormField.PLAINTEXT, 0);
            SetTextFieldParams(field, text, name, llx, lly, urx, ury);
            DrawMultiLineOfText(field, text, font, fontSize, llx, lly, urx, ury);
            AddFormField(field);
            return field;
        }

        public PdfFormField AddRadioButton(PdfFormField radiogroup, string value, float llx, float lly, float urx, float ury)
        {
            PdfFormField radio = PdfFormField.CreateEmpty(_writer);
            radio.SetWidget(new Rectangle(llx, lly, urx, ury), PdfAnnotation.HighlightToggle);
            string name = ((PdfName)radiogroup.Get(PdfName.V)).ToString().Substring(1);
            if (name.Equals(value))
            {
                radio.AppearanceState = value;
            }
            else
            {
                radio.AppearanceState = "Off";
            }
            DrawRadioAppearences(radio, value, llx, lly, urx, ury);
            radiogroup.AddKid(radio);
            return radio;
        }

        public void AddRadioGroup(PdfFormField radiogroup)
        {
            AddFormField(radiogroup);
        }

        public PdfFormField AddResetButton(string name, string caption, string value, BaseFont font, float fontSize, float llx, float lly, float urx, float ury)
        {
            PdfAction action = PdfAction.CreateResetForm(null, 0);
            PdfFormField button = new PdfFormField(_writer, llx, lly, urx, ury, action);
            SetButtonParams(button, PdfFormField.FF_PUSHBUTTON, name, value);
            DrawButton(button, caption, font, fontSize, llx, lly, urx, ury);
            AddFormField(button);
            return button;
        }

        public PdfFormField AddSelectList(string name, string[] options, string defaultValue, BaseFont font, float fontSize, float llx, float lly, float urx, float ury)
        {
            PdfFormField choice = PdfFormField.CreateList(_writer, options, 0);
            SetChoiceParams(choice, name, defaultValue, llx, lly, urx, ury);
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < options.Length; i++)
            {
                text.Append(options[i]).Append('\n');
            }
            DrawMultiLineOfText(choice, text.ToString(), font, fontSize, llx, lly, urx, ury);
            AddFormField(choice);
            return choice;
        }

        public PdfFormField AddSelectList(string name, string[,] options, string defaultValue, BaseFont font, float fontSize, float llx, float lly, float urx, float ury)
        {
            PdfFormField choice = PdfFormField.CreateList(_writer, options, 0);
            SetChoiceParams(choice, name, defaultValue, llx, lly, urx, ury);
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < options.GetLength(0); i++)
            {
                text.Append(options[i, 1]).Append('\n');
            }
            DrawMultiLineOfText(choice, text.ToString(), font, fontSize, llx, lly, urx, ury);
            AddFormField(choice);
            return choice;
        }

        public PdfFormField AddSignature(string name, float llx, float lly, float urx, float ury)
        {
            PdfFormField signature = PdfFormField.CreateSignature(_writer);
            SetSignatureParams(signature, name, llx, lly, urx, ury);
            DrawSignatureAppearences(signature, llx, lly, urx, ury);
            AddFormField(signature);
            return signature;
        }

        public PdfFormField AddSingleLinePasswordField(string name, string text, BaseFont font, float fontSize, float llx, float lly, float urx, float ury)
        {
            PdfFormField field = PdfFormField.CreateTextField(_writer, PdfFormField.SINGLELINE, PdfFormField.PASSWORD, 0);
            SetTextFieldParams(field, text, name, llx, lly, urx, ury);
            DrawSingleLineOfText(field, text, font, fontSize, llx, lly, urx, ury);
            AddFormField(field);
            return field;
        }

        public PdfFormField AddSingleLineTextField(string name, string text, BaseFont font, float fontSize, float llx, float lly, float urx, float ury)
        {
            PdfFormField field = PdfFormField.CreateTextField(_writer, PdfFormField.SINGLELINE, PdfFormField.PLAINTEXT, 0);
            SetTextFieldParams(field, text, name, llx, lly, urx, ury);
            DrawSingleLineOfText(field, text, font, fontSize, llx, lly, urx, ury);
            AddFormField(field);
            return field;
        }

        public void DrawButton(PdfFormField button, string caption, BaseFont font, float fontSize, float llx, float lly, float urx, float ury)
        {
            PdfAppearance pa = PdfAppearance.CreateAppearance(_writer, urx - llx, ury - lly);
            pa.DrawButton(0f, 0f, urx - llx, ury - lly, caption, font, fontSize);
            button.SetAppearance(PdfAnnotation.AppearanceNormal, pa);
        }

        public void DrawCheckBoxAppearences(PdfFormField field, string value, float llx, float lly, float urx, float ury)
        {
            BaseFont font = BaseFont.CreateFont(BaseFont.ZAPFDINGBATS, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
            float size = (ury - lly);
            PdfAppearance tpOn = PdfAppearance.CreateAppearance(_writer, urx - llx, ury - lly);
            PdfAppearance tp2 = (PdfAppearance)tpOn.Duplicate;
            tp2.SetFontAndSize(font, size);
            tp2.ResetRgbColorFill();
            field.DefaultAppearanceString = tp2;
            tpOn.DrawTextField(0f, 0f, urx - llx, ury - lly);
            tpOn.SaveState();
            tpOn.ResetRgbColorFill();
            tpOn.BeginText();
            tpOn.SetFontAndSize(font, size);
            tpOn.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "4", (urx - llx) / 2, (ury - lly) / 2 - (size * 0.3f), 0);
            tpOn.EndText();
            tpOn.RestoreState();
            field.SetAppearance(PdfAnnotation.AppearanceNormal, value, tpOn);
            PdfAppearance tpOff = PdfAppearance.CreateAppearance(_writer, urx - llx, ury - lly);
            tpOff.DrawTextField(0f, 0f, urx - llx, ury - lly);
            field.SetAppearance(PdfAnnotation.AppearanceNormal, "Off", tpOff);
        }

        public void DrawMultiLineOfText(PdfFormField field, string text, BaseFont font, float fontSize, float llx, float lly, float urx, float ury)
        {
            PdfAppearance tp = PdfAppearance.CreateAppearance(_writer, urx - llx, ury - lly);
            PdfAppearance tp2 = (PdfAppearance)tp.Duplicate;
            tp2.SetFontAndSize(font, fontSize);
            tp2.ResetRgbColorFill();
            field.DefaultAppearanceString = tp2;
            tp.DrawTextField(0f, 0f, urx - llx, ury - lly);
            tp.BeginVariableText();
            tp.SaveState();
            tp.Rectangle(3f, 3f, urx - llx - 6f, ury - lly - 6f);
            tp.Clip();
            tp.NewPath();
            tp.BeginText();
            tp.SetFontAndSize(font, fontSize);
            tp.ResetRgbColorFill();
            tp.SetTextMatrix(4, 5);
            System.util.StringTokenizer tokenizer = new System.util.StringTokenizer(text, "\n");
            float yPos = ury - lly;
            while (tokenizer.HasMoreTokens())
            {
                yPos -= fontSize * 1.2f;
                tp.ShowTextAligned(PdfContentByte.ALIGN_LEFT, tokenizer.NextToken(), 3, yPos, 0);
            }
            tp.EndText();
            tp.RestoreState();
            tp.EndVariableText();
            field.SetAppearance(PdfAnnotation.AppearanceNormal, tp);
        }

        public void DrawRadioAppearences(PdfFormField field, string value, float llx, float lly, float urx, float ury)
        {
            PdfAppearance tpOn = PdfAppearance.CreateAppearance(_writer, urx - llx, ury - lly);
            tpOn.DrawRadioField(0f, 0f, urx - llx, ury - lly, true);
            field.SetAppearance(PdfAnnotation.AppearanceNormal, value, tpOn);
            PdfAppearance tpOff = PdfAppearance.CreateAppearance(_writer, urx - llx, ury - lly);
            tpOff.DrawRadioField(0f, 0f, urx - llx, ury - lly, false);
            field.SetAppearance(PdfAnnotation.AppearanceNormal, "Off", tpOff);
        }

        /// <summary>
        /// </summary>
        /// <param name="field"></param>
        /// <param name="llx"></param>
        /// <param name="lly"></param>
        /// <param name="urx"></param>
        /// <param name="ury"></param>
        public void DrawSignatureAppearences(PdfFormField field, float llx, float lly, float urx, float ury)
        {
            PdfAppearance tp = PdfAppearance.CreateAppearance(_writer, urx - llx, ury - lly);
            tp.SetGrayFill(1.0f);
            tp.Rectangle(0, 0, urx - llx, ury - lly);
            tp.Fill();
            tp.SetGrayStroke(0);
            tp.SetLineWidth(1);
            tp.Rectangle(0.5f, 0.5f, urx - llx - 0.5f, ury - lly - 0.5f);
            tp.ClosePathStroke();
            tp.SaveState();
            tp.Rectangle(1, 1, urx - llx - 2, ury - lly - 2);
            tp.Clip();
            tp.NewPath();
            tp.RestoreState();
            field.SetAppearance(PdfAnnotation.AppearanceNormal, tp);
        }

        public void DrawSingleLineOfText(PdfFormField field, string text, BaseFont font, float fontSize, float llx, float lly, float urx, float ury)
        {
            PdfAppearance tp = PdfAppearance.CreateAppearance(_writer, urx - llx, ury - lly);
            PdfAppearance tp2 = (PdfAppearance)tp.Duplicate;
            tp2.SetFontAndSize(font, fontSize);
            tp2.ResetRgbColorFill();
            field.DefaultAppearanceString = tp2;
            tp.DrawTextField(0f, 0f, urx - llx, ury - lly);
            tp.BeginVariableText();
            tp.SaveState();
            tp.Rectangle(3f, 3f, urx - llx - 6f, ury - lly - 6f);
            tp.Clip();
            tp.NewPath();
            tp.BeginText();
            tp.SetFontAndSize(font, fontSize);
            tp.ResetRgbColorFill();
            tp.SetTextMatrix(4, (ury - lly) / 2 - (fontSize * 0.3f));
            tp.ShowText(text);
            tp.EndText();
            tp.RestoreState();
            tp.EndVariableText();
            field.SetAppearance(PdfAnnotation.AppearanceNormal, tp);
        }

        public PdfFormField GetRadioGroup(string name, string defaultValue, bool noToggleToOff)
        {
            PdfFormField radio = PdfFormField.CreateRadioButton(_writer, noToggleToOff);
            radio.FieldName = name;
            radio.ValueAsName = defaultValue;
            return radio;
        }

        public bool IsValid()
        {
            if (_documentFields.Size == 0) return false;
            Put(PdfName.Fields, _documentFields);
            if (_sigFlags != 0)
                Put(PdfName.Sigflags, new PdfNumber(_sigFlags));
            if (_calculationOrder.Size > 0)
                Put(PdfName.Co, _calculationOrder);
            if (_fieldTemplates.Count == 0) return true;
            PdfDictionary dic = new PdfDictionary();
            foreach (PdfTemplate template in _fieldTemplates.Keys)
            {
                PdfFormField.MergeResources(dic, (PdfDictionary)template.Resources);
            }
            Put(PdfName.Dr, dic);
            Put(PdfName.Da, new PdfString("/Helv 0 Tf 0 g "));
            PdfDictionary fonts = (PdfDictionary)dic.Get(PdfName.Font);
            if (fonts != null)
            {
                _writer.EliminateFontSubset(fonts);
            }
            return true;
        }
        public void SetButtonParams(PdfFormField button, int characteristics, string name, string value)
        {
            button.Button = characteristics;
            button.Flags = PdfAnnotation.FLAGS_PRINT;
            button.SetPage();
            button.FieldName = name;
            if (value != null) button.ValueAsString = value;
        }
        public void SetCheckBoxParams(PdfFormField field, string name, string value, bool status, float llx, float lly, float urx, float ury)
        {
            field.SetWidget(new Rectangle(llx, lly, urx, ury), PdfAnnotation.HighlightToggle);
            field.FieldName = name;
            if (status)
            {
                field.ValueAsName = value;
                field.AppearanceState = value;
            }
            else
            {
                field.ValueAsName = "Off";
                field.AppearanceState = "Off";
            }
            field.Flags = PdfAnnotation.FLAGS_PRINT;
            field.SetPage();
            field.BorderStyle = new PdfBorderDictionary(1, PdfBorderDictionary.STYLE_SOLID);
        }

        public void SetChoiceParams(PdfFormField field, string name, string defaultValue, float llx, float lly, float urx, float ury)
        {
            field.SetWidget(new Rectangle(llx, lly, urx, ury), PdfAnnotation.HighlightInvert);
            if (defaultValue != null)
            {
                field.ValueAsString = defaultValue;
                field.DefaultValueAsString = defaultValue;
            }
            field.FieldName = name;
            field.Flags = PdfAnnotation.FLAGS_PRINT;
            field.SetPage();
            field.BorderStyle = new PdfBorderDictionary(2, PdfBorderDictionary.STYLE_SOLID);
        }

        /// <summary>
        /// </summary>
        /// <param name="field"></param>
        /// <param name="name"></param>
        /// <param name="llx"></param>
        /// <param name="lly"></param>
        /// <param name="urx"></param>
        /// <param name="ury"></param>
        public void SetSignatureParams(PdfFormField field, string name, float llx, float lly, float urx, float ury)
        {
            field.SetWidget(new Rectangle(llx, lly, urx, ury), PdfAnnotation.HighlightInvert);
            field.FieldName = name;
            field.Flags = PdfAnnotation.FLAGS_PRINT;
            field.SetPage();
            field.MkBorderColor = BaseColor.Black;
            field.MkBackgroundColor = BaseColor.White;
        }

        public void SetTextFieldParams(PdfFormField field, string text, string name, float llx, float lly, float urx, float ury)
        {
            field.SetWidget(new Rectangle(llx, lly, urx, ury), PdfAnnotation.HighlightInvert);
            field.ValueAsString = text;
            field.DefaultValueAsString = text;
            field.FieldName = name;
            field.Flags = PdfAnnotation.FLAGS_PRINT;
            field.SetPage();
        }
    }
}