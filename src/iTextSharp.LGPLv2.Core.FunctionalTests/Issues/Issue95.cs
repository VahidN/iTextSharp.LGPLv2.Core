using System;
using System.Text;
using System.Xml;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/95
/// </summary>
[TestClass]
public class Issue95
{
    public const string XmlRootElementName = "form1";
    public const string XmlChildElementName = "childElement";

    public const string XmlDeclarationFragment = "<?xml version=";
    public const string XmlElementRootFragment = "<form1";

    [TestMethod]
    public void Verify_Issue95_XfaForm_SerializeDoc_Does_Not_Add_Xml_Declaration()
    {
        var xmlDoc = new XmlDocument();

        var form1 = xmlDoc.CreateElement(XmlRootElementName);
        xmlDoc.AppendChild(form1);

        // Add some child elements
        form1.AppendChild(xmlDoc.CreateElement(XmlChildElementName));
        form1.AppendChild(xmlDoc.CreateElement(XmlChildElementName));
        form1.AppendChild(xmlDoc.CreateElement(XmlChildElementName));

        var bytes = XfaForm.SerializeDoc(xmlDoc);
        var text = Encoding.UTF8.GetString(bytes);

        // We check for the presence of the "<?xml ... " fragment in the serialized document.
        // It should not be there, as its presence causes the parsing of the XFA data to fail.
        var containsXmlDeclaration = text.IndexOf(
                                                  XmlDeclarationFragment,
                                                  StringComparison.InvariantCultureIgnoreCase) == 0;

        // We should have the root element at the beginning of the serialized text ; this also
        // makes sure that we're not mstakenly adding the UTF-8 identifier to the beginning of
        // the serialized text
        var startsWithRootelement = text.IndexOf(
                                                 XmlElementRootFragment,
                                                 StringComparison.InvariantCulture) == 0;

        Assert.IsFalse(containsXmlDeclaration);
        Assert.IsTrue(startsWithRootelement);
    }
}