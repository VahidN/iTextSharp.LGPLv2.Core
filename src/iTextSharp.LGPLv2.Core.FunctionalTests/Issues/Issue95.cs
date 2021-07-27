using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues
{
    /// <summary>
    /// https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/95
    /// </summary>
    [TestClass]
    public class Issue95
    {
        public const string XmlDeclarationFragment = "<?xml version=";

        [TestMethod]
        public void Verify_Issue95_XfaForm_SerializeDoc_Does_Not_Add_Xml_Declaration()
        {
            var xmlDoc = new XmlDocument();

            var form1 = xmlDoc.CreateElement("form1");
            xmlDoc.AppendChild(form1);

            form1.AppendChild(xmlDoc.CreateElement("subObject"));
            form1.AppendChild(xmlDoc.CreateElement("subObject"));
            form1.AppendChild(xmlDoc.CreateElement("subObject"));

            var bytes = XfaForm.SerializeDoc(xmlDoc);
            var text = Encoding.UTF8.GetString(bytes);

            // We check for the presence of the "<?xml ... " fragment in the serialized document.
            // It should not be there, as its presence causes the parsing of the XFA data to fail.
            var containsXmlDeclaration = text.IndexOf(
                XmlDeclarationFragment, 
                StringComparison.InvariantCultureIgnoreCase) == 0;

            Assert.IsFalse(containsXmlDeclaration);
        }
    }
}