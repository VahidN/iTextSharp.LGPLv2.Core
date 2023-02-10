using Org.BouncyCastle.X509;

namespace iTextSharp.text.pdf;

/// <summary>
/// </summary>
public class PdfPublicKeyRecipient
{
    public PdfPublicKeyRecipient(X509Certificate certificate, int permission)
    {
        Certificate = certificate;
        Permission = permission;
    }

    public X509Certificate Certificate { get; }

    public int Permission { get; }

    protected internal byte[] Cms { set; get; }
}