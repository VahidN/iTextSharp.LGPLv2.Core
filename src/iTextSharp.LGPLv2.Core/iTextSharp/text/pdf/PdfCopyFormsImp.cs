using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     Allows you to add one (or more) existing PDF document(s)
///     and add the form(s) of (an)other PDF document(s).
///     @since 2.1.5
/// </summary>
internal class PdfCopyFormsImp : PdfCopyFieldsImp
{
    /// <summary>
    ///     This sets up the output document
    ///     @throws DocumentException
    /// </summary>
    /// <param name="os">The Outputstream pointing to the output document</param>
    internal PdfCopyFormsImp(Stream os) : base(os)
    {
    }

    /// <summary>
    ///     This method feeds in the source document
    ///     @throws DocumentException
    /// </summary>
    /// <param name="reader">The PDF reader containing the source document</param>
    public void CopyDocumentFields(PdfReader reader)
    {
        if (!reader.IsOpenedWithFullPermissions)
        {
            throw new BadPasswordException("PdfReader not opened with owner password");
        }

        if (Readers2Intrefs.ContainsKey(reader))
        {
            reader = new PdfReader(reader);
        }
        else
        {
            if (reader.Tampered)
            {
                throw new DocumentException("The document was reused.");
            }

            reader.ConsolidateNamedDestinations();
            reader.Tampered = true;
        }

        reader.ShuffleSubsetNames();
        Readers2Intrefs[reader] = new NullValueDictionary<int, int>();
        Fields.Add(reader.AcroFields);
        UpdateCalculationOrder(reader);
    }

    /// <summary>
    ///     This merge fields is slightly different from the mergeFields method
    ///     of PdfCopyFields.
    /// </summary>
    internal override void MergeFields()
    {
        for (var k = 0; k < Fields.Count; ++k)
        {
            var fd = Fields[k].Fields;
            MergeWithMaster(fd);
        }
    }
}