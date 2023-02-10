using System.Text;
using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     This class takes any PDF and returns exactly the same but
///     encrypted. All the content, links, outlines, etc, are kept.
///     It is also possible to change the info dictionary.
/// </summary>
public static class PdfEncryptor
{
    /// <summary>
    ///     Entry point to encrypt a PDF document. The encryption parameters are the same as in
    ///     PdfWriter . The userPassword and the
    ///     ownerPassword can be null or have zero length. In this case the ownerPassword
    ///     is replaced by a random string. The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="reader">the read PDF</param>
    /// <param name="os">the output destination</param>
    /// <param name="userPassword">the user password. Can be null or empty</param>
    /// <param name="ownerPassword">the owner password. Can be null or empty</param>
    /// <param name="permissions">the user permissions</param>
    /// <param name="strength128Bits"> true  for 128 bit key length,  false  for 40 bit key length</param>
    public static void Encrypt(PdfReader reader, Stream os, byte[] userPassword, byte[] ownerPassword, int permissions,
                               bool strength128Bits)
    {
        var stamper = new PdfStamper(reader, os);
        stamper.SetEncryption(userPassword, ownerPassword, permissions, strength128Bits);
        stamper.Close();
    }

    /// <summary>
    ///     Entry point to encrypt a PDF document. The encryption parameters are the same as in
    ///     PdfWriter . The userPassword and the
    ///     ownerPassword can be null or have zero length. In this case the ownerPassword
    ///     is replaced by a random string. The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     the info dictionary. Entries with  null
    ///     values delete the key in the original info dictionary
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="reader">the read PDF</param>
    /// <param name="os">the output destination</param>
    /// <param name="userPassword">the user password. Can be null or empty</param>
    /// <param name="ownerPassword">the owner password. Can be null or empty</param>
    /// <param name="permissions">the user permissions</param>
    /// <param name="strength128Bits"> true  for 128 bit key length,  false  for 40 bit key length</param>
    /// <param name="newInfo">an optional  String  map to add or change</param>
    public static void Encrypt(PdfReader reader, Stream os, byte[] userPassword, byte[] ownerPassword, int permissions,
                               bool strength128Bits, INullValueDictionary<string, string> newInfo)
    {
        var stamper = new PdfStamper(reader, os);
        stamper.SetEncryption(userPassword, ownerPassword, permissions, strength128Bits);
        stamper.MoreInfo = newInfo;
        stamper.Close();
    }

    /// <summary>
    ///     Entry point to encrypt a PDF document. The encryption parameters are the same as in
    ///     PdfWriter . The userPassword and the
    ///     ownerPassword can be null or have zero length. In this case the ownerPassword
    ///     is replaced by a random string. The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="reader">the read PDF</param>
    /// <param name="os">the output destination</param>
    /// <param name="strength"> true  for 128 bit key length,  false  for 40 bit key length</param>
    /// <param name="userPassword">the user password. Can be null or empty</param>
    /// <param name="ownerPassword">the owner password. Can be null or empty</param>
    /// <param name="permissions">the user permissions</param>
    public static void Encrypt(PdfReader reader, Stream os, bool strength, string userPassword, string ownerPassword,
                               int permissions)
    {
        var stamper = new PdfStamper(reader, os);
        stamper.SetEncryption(strength, userPassword, ownerPassword, permissions);
        stamper.Close();
    }

    /// <summary>
    ///     Entry point to encrypt a PDF document. The encryption parameters are the same as in
    ///     PdfWriter . The userPassword and the
    ///     ownerPassword can be null or have zero length. In this case the ownerPassword
    ///     is replaced by a random string. The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     the info dictionary. Entries with  null
    ///     values delete the key in the original info dictionary
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="reader">the read PDF</param>
    /// <param name="os">the output destination</param>
    /// <param name="strength"> true  for 128 bit key length,  false  for 40 bit key length</param>
    /// <param name="userPassword">the user password. Can be null or empty</param>
    /// <param name="ownerPassword">the owner password. Can be null or empty</param>
    /// <param name="permissions">the user permissions</param>
    /// <param name="newInfo">an optional  String  map to add or change</param>
    public static void Encrypt(PdfReader reader, Stream os, bool strength, string userPassword, string ownerPassword,
                               int permissions, INullValueDictionary<string, string> newInfo)
    {
        var stamper = new PdfStamper(reader, os);
        stamper.SetEncryption(strength, userPassword, ownerPassword, permissions);
        stamper.MoreInfo = newInfo;
        stamper.Close();
    }

    /// <summary>
    ///     Entry point to encrypt a PDF document. The encryption parameters are the same as in
    ///     PdfWriter . The userPassword and the
    ///     ownerPassword can be null or have zero length. In this case the ownerPassword
    ///     is replaced by a random string. The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
    ///     the info dictionary. Entries with  null
    ///     values delete the key in the original info dictionary
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="reader">the read PDF</param>
    /// <param name="os">the output destination</param>
    /// <param name="type">
    ///     the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128 or
    ///     ENCRYPTION_AES128.
    /// </param>
    /// <param name="userPassword">the user password. Can be null or empty</param>
    /// <param name="ownerPassword">the owner password. Can be null or empty</param>
    /// <param name="permissions">the user permissions</param>
    /// <param name="newInfo">an optional  String  map to add or change</param>
    public static void Encrypt(PdfReader reader, Stream os, int type, string userPassword, string ownerPassword,
                               int permissions, INullValueDictionary<string, string> newInfo)
    {
        var stamper = new PdfStamper(reader, os);
        stamper.SetEncryption(type, userPassword, ownerPassword, permissions);
        stamper.MoreInfo = newInfo;
        stamper.Close();
    }

    /// <summary>
    ///     Entry point to encrypt a PDF document. The encryption parameters are the same as in
    ///     PdfWriter . The userPassword and the
    ///     ownerPassword can be null or have zero length. In this case the ownerPassword
    ///     is replaced by a random string. The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
    ///     values delete the key in the original info dictionary
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="reader">the read PDF</param>
    /// <param name="os">the output destination</param>
    /// <param name="type">
    ///     the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128 or
    ///     ENCRYPTION_AES128.
    /// </param>
    /// <param name="userPassword">the user password. Can be null or empty</param>
    /// <param name="ownerPassword">the owner password. Can be null or empty</param>
    /// <param name="permissions">the user permissions</param>
    public static void Encrypt(PdfReader reader, Stream os, int type, string userPassword, string ownerPassword,
                               int permissions)
    {
        var stamper = new PdfStamper(reader, os);
        stamper.SetEncryption(type, userPassword, ownerPassword, permissions);
        stamper.Close();
    }

    /// <summary>
    ///     Give you a verbose analysis of the permissions.
    /// </summary>
    /// <param name="permissions">the permissions value of a PDF file</param>
    /// <returns>a String that explains the meaning of the permissions value</returns>
    public static string GetPermissionsVerbose(int permissions)
    {
        var buf = new StringBuilder("Allowed:");
        if ((PdfWriter.ALLOW_PRINTING & permissions) == PdfWriter.ALLOW_PRINTING)
        {
            buf.Append(" Printing");
        }

        if ((PdfWriter.ALLOW_MODIFY_CONTENTS & permissions) == PdfWriter.ALLOW_MODIFY_CONTENTS)
        {
            buf.Append(" Modify contents");
        }

        if ((PdfWriter.ALLOW_COPY & permissions) == PdfWriter.ALLOW_COPY)
        {
            buf.Append(" Copy");
        }

        if ((PdfWriter.ALLOW_MODIFY_ANNOTATIONS & permissions) == PdfWriter.ALLOW_MODIFY_ANNOTATIONS)
        {
            buf.Append(" Modify annotations");
        }

        if ((PdfWriter.ALLOW_FILL_IN & permissions) == PdfWriter.ALLOW_FILL_IN)
        {
            buf.Append(" Fill in");
        }

        if ((PdfWriter.ALLOW_SCREENREADERS & permissions) == PdfWriter.ALLOW_SCREENREADERS)
        {
            buf.Append(" Screen readers");
        }

        if ((PdfWriter.ALLOW_ASSEMBLY & permissions) == PdfWriter.ALLOW_ASSEMBLY)
        {
            buf.Append(" Assembly");
        }

        if ((PdfWriter.ALLOW_DEGRADED_PRINTING & permissions) == PdfWriter.ALLOW_DEGRADED_PRINTING)
        {
            buf.Append(" Degraded printing");
        }

        return buf.ToString();
    }

    /// <summary>
    ///     Tells you if document assembly is allowed.
    ///     @since 2.0.7
    /// </summary>
    /// <param name="permissions">the permissions value of a PDF file</param>
    /// <returns>true if document assembly is allowed</returns>
    public static bool IsAssemblyAllowed(int permissions) =>
        (PdfWriter.ALLOW_ASSEMBLY & permissions) == PdfWriter.ALLOW_ASSEMBLY;

    /// <summary>
    ///     Tells you if copying is allowed.
    ///     @since 2.0.7
    /// </summary>
    /// <param name="permissions">the permissions value of a PDF file</param>
    /// <returns>true if copying is allowed</returns>
    public static bool IsCopyAllowed(int permissions) => (PdfWriter.ALLOW_COPY & permissions) == PdfWriter.ALLOW_COPY;

    /// <summary>
    ///     Tells you if degraded printing is allowed.
    ///     @since 2.0.7
    /// </summary>
    /// <param name="permissions">the permissions value of a PDF file</param>
    /// <returns>true if degraded printing is allowed</returns>
    public static bool IsDegradedPrintingAllowed(int permissions) =>
        (PdfWriter.ALLOW_DEGRADED_PRINTING & permissions) == PdfWriter.ALLOW_DEGRADED_PRINTING;

    /// <summary>
    ///     Tells you if filling in fields is allowed.
    ///     @since 2.0.7
    /// </summary>
    /// <param name="permissions">the permissions value of a PDF file</param>
    /// <returns>true if filling in fields is allowed</returns>
    public static bool IsFillInAllowed(int permissions) =>
        (PdfWriter.ALLOW_FILL_IN & permissions) == PdfWriter.ALLOW_FILL_IN;

    /// <summary>
    ///     Tells you if modifying annotations is allowed.
    ///     @since 2.0.7
    /// </summary>
    /// <param name="permissions">the permissions value of a PDF file</param>
    /// <returns>true if modifying annotations is allowed</returns>
    public static bool IsModifyAnnotationsAllowed(int permissions) =>
        (PdfWriter.ALLOW_MODIFY_ANNOTATIONS & permissions) == PdfWriter.ALLOW_MODIFY_ANNOTATIONS;

    /// <summary>
    ///     Tells you if modifying content is allowed.
    ///     @since 2.0.7
    /// </summary>
    /// <param name="permissions">the permissions value of a PDF file</param>
    /// <returns>true if modifying content is allowed</returns>
    public static bool IsModifyContentsAllowed(int permissions) =>
        (PdfWriter.ALLOW_MODIFY_CONTENTS & permissions) == PdfWriter.ALLOW_MODIFY_CONTENTS;

    /// <summary>
    ///     Tells you if printing is allowed.
    ///     @since 2.0.7
    /// </summary>
    /// <param name="permissions">the permissions value of a PDF file</param>
    /// <returns>true if printing is allowed</returns>
    public static bool IsPrintingAllowed(int permissions) =>
        (PdfWriter.ALLOW_PRINTING & permissions) == PdfWriter.ALLOW_PRINTING;

    /// <summary>
    ///     Tells you if repurposing for screenreaders is allowed.
    ///     @since 2.0.7
    /// </summary>
    /// <param name="permissions">the permissions value of a PDF file</param>
    /// <returns>true if repurposing for screenreaders is allowed</returns>
    public static bool IsScreenReadersAllowed(int permissions) =>
        (PdfWriter.ALLOW_SCREENREADERS & permissions) == PdfWriter.ALLOW_SCREENREADERS;
}