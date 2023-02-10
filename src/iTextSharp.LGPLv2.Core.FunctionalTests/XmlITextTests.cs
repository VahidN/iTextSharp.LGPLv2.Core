using System.IO;
using System.Xml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests;

[TestClass]
public class XmlITextTests
{
	/// <summary>
	///     iTextXML can be used as a report generator
	/// </summary>
	[TestMethod]
    public void Verify_iTextXML_To_PDF_File_CanBeCreated()
    {
        var img = Path.Combine(TestUtils.GetBaseDir(), @"iTextExamples", "resources", "img", "hitchcock.png");
        var iTextXML = $@"
<itext creationdate='1395/06/14' producer='iTextSharp.LGPLv2.Core'>

	<paragraph style='font-family:Helvetica;font-size:18;font-weight:bold;font-style:italic;'>
		<chunk red='255' green='0' blue='0'>iText XML Sample</chunk>
	</paragraph>

	<table width='100%' cellpadding='1.0' cellspacing='1.0'  widths='5;5'>
		<row>
			<cell borderwidth='0.5' left='false' right='false' top='false' bottom='true'>User Name</cell>
			<cell borderwidth='0.5' left='false' right='false' top='false' bottom='true' horizontalalign='right'>Number</cell>
		</row>
	</table>

	<image url='{img}'/>

	<newline />

	<list numbered='true' symbolindent='15' font='unknown'>
		<listitem leading='18.0' font='Times' size='12.0' align='Default' indentationleft='15.0'>
			<chunk font='Times' size='12.0'>List item</chunk>
		</listitem>
	</list>

	<anchor fontstyle='normal, underline' red='0' green='0' blue='255' reference='http://site.com/'>
	My blog
	</anchor>

</itext>
";

        var pdfFilePath = TestUtils.GetOutputFileName();
        ConverITextXmlToPdfFile(iTextXML, pdfFilePath);
        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }


    [TestMethod]
    public void Verify_iTextXML_CanBeParsed()
    {
        var img = Path.Combine(TestUtils.GetBaseDir(), @"iTextExamples", "resources", "img", "hitchcock.png");
        var iTextXML = $@"
<itext creationdate='Thu Jun 26 14:25:52 CEST 2003' producer='iTextXML by lowagie.com'>
	<paragraph leading='18.0' font='unknown' align='Default'>
		Please visit my
		<anchor leading='18.0' font='Helvetica' size='12.0' fontstyle='normal, underline' red='0' green='0' blue='255' name='top' reference='http://www.lowagie.com/iText/'>
			<chunk font='Helvetica' size='12.0' fontstyle='normal, underline' red='0' green='0' blue='255'>website (external reference)</chunk>
		</anchor>
	</paragraph>
	<paragraph leading='18.0' font='unknown' align='Default'>
		These are some special characters: &lt;, &gt;, &amp;, &quot; and &apos;
	</paragraph>
	<paragraph leading='18.0' font='unknown' align='Default'>
		some books I really like:
	</paragraph>
	<list numbered='true' symbolindent='15' font='unknown'>
		<listitem leading='18.0' font='Times' size='12.0' align='Default' indentationleft='15.0'>
			<chunk font='Times' size='12.0'>When Harlie was one</chunk>
			<chunk font='Times' size='11.0' fontstyle='normal' subsupscript='8.0'> by David Gerrold</chunk>
		</listitem>
		<listitem leading='18.0' font='Times' size='12.0' align='Default' indentationleft='15.0'>
			<chunk font='Times' size='12.0'>The World according to Garp</chunk>
			<chunk font='Times' size='11.0' fontstyle='normal' subsupscript='-8.0'> by John Irving</chunk>
		</listitem>
		<listitem leading='18.0' font='Times' size='12.0' align='Default' indentationleft='15.0'>
			<chunk font='Times' size='12.0'>Decamerone</chunk>
			<chunk font='Times' size='11.0' fontstyle='normal'> by Giovanni Boccaccio</chunk>
		</listitem>
	</list>
	<paragraph leading='18.0' font='unknown' align='Default'>
		some movies I really like:
		<list numbered='false' symbolindent='10' listsymbol='-' font='unknown'>
			<listitem leading='18.0' font='unknown' align='Default' indentationleft='10.0'>
				Wild At Heart
			</listitem>
			<listitem leading='18.0' font='unknown' align='Default' indentationleft='10.0'>
				Casablanca
			</listitem>
			<listitem leading='18.0' font='unknown' align='Default' indentationleft='10.0'>
				When Harry met Sally
			</listitem>
			<listitem leading='18.0' font='unknown' align='Default' indentationleft='10.0'>
				True Romance
			</listitem>
			<listitem leading='18.0' font='unknown' align='Default' indentationleft='10.0'>
				Le mari de la coiffeuse
			</listitem>
		</list>
	</paragraph>
	<paragraph leading='18.0' font='unknown' align='Default'>
		Some authors I really like:
	</paragraph>
	<list numbered='false' symbolindent='20' first='-2' listsymbol='*' font='Helvetica' size='20.0' fontstyle='normal'>
		<listitem leading='18.0' font='unknown' align='Default' indentationleft='20.0'>
			Isaac Asimov
		</listitem>
		<list numbered='true' symbolindent='10' indentationleft='20.0' font='Helvetica' size='8.0'>
			<listitem leading='18.0' font='unknown' align='Default' indentationleft='10.0'>
				The Foundation Trilogy
			</listitem>
			<listitem leading='18.0' font='unknown' align='Default' indentationleft='10.0'>
				The Complete Robot
			</listitem>
			<listitem leading='18.0' font='unknown' align='Default' indentationleft='10.0'>
				Caves of Steel
			</listitem>
			<listitem leading='18.0' font='unknown' align='Default' indentationleft='10.0'>
				The Naked Sun
			</listitem>
		</list>
		<listitem leading='18.0' font='unknown' align='Default' indentationleft='20.0'>
			John Irving
		</listitem>
		<list numbered='true' symbolindent='10' indentationleft='20.0' font='Helvetica' size='8.0'>
			<listitem leading='18.0' font='unknown' align='Default' indentationleft='10.0'>
				The World according to Garp
			</listitem>
			<listitem leading='18.0' font='unknown' align='Default' indentationleft='10.0'>
				Hotel New Hampshire
			</listitem>
			<listitem leading='18.0' font='unknown' align='Default' indentationleft='10.0'>
				A prayer for Owen Meany
			</listitem>
			<listitem leading='18.0' font='unknown' align='Default' indentationleft='10.0'>
				Widow for a year
			</listitem>
		</list>
		<listitem leading='18.0' font='unknown' align='Default' indentationleft='20.0'>
			Kurt Vonnegut
		</listitem>
		<list numbered='true' symbolindent='10' indentationleft='20.0' font='Helvetica' size='8.0'>
			<listitem leading='18.0' font='unknown' align='Default' indentationleft='10.0'>
				Slaughterhouse 5
			</listitem>
			<listitem leading='18.0' font='unknown' align='Default' indentationleft='10.0'>
				Welcome to the Monkey House
			</listitem>
			<listitem leading='18.0' font='unknown' align='Default' indentationleft='10.0'>
				The great pianola
			</listitem>
			<listitem leading='18.0' font='unknown' align='Default' indentationleft='10.0'>
				Galapagos
			</listitem>
		</list>
	</list>
	<paragraph leading='18.0' font='unknown' align='Default'>
		<newline />
<newline />

	</paragraph>
	<table columns='3' width='80.0%' align='Center' cellpadding='5.0' cellspacing='5.0' widths='33.333332;33.333332;33.333332' borderwidth='1.0' left='true' right='true' top='true' bottom='true' red='0' green='0' blue='255'>
		<row>
			<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='3' header='true' leading='18.0'>
				<paragraph leading='18.0' font='unknown' align='Default'>
					header
				</paragraph>
			</cell>
		</row>
		<row>
			<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' red='255' green='0' blue='0' horizontalalign='Default' verticalalign='Default' rowspan='2' leading='18.0'>
				<paragraph leading='18.0' font='unknown' align='Default'>
					example cell with colspan 1 and rowspan 2
				</paragraph>
			</cell>
			<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
				<phrase leading='18.0' font='unknown'>
					1.1
				</phrase>
			</cell>
			<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
				<phrase leading='18.0' font='unknown'>
					2.1
				</phrase>
			</cell>
		</row>
		<row>
			<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
				<phrase leading='18.0' font='unknown'>
					1.2
				</phrase>
			</cell>
			<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
				<phrase leading='18.0' font='unknown'>
					2.2
				</phrase>
			</cell>
		</row>
		<row>
			<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
				<phrase leading='18.0' font='unknown'>
					cell test1
				</phrase>
			</cell>
			<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='2' rowspan='2' leading='18.0'>
				<paragraph leading='18.0' font='unknown' align='Default'>
					big cell
				</paragraph>
			</cell>
		</row>
		<row>
			<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
				<phrase leading='18.0' font='unknown'>
					cell test2
				</phrase>
			</cell>
		</row>
	</table>
	<image url='{img}' plainwidth='194.0' plainheight='202.0' />
	<image url='{img}' plainwidth='171.0' plainheight='250.0' />
	<anchor leading='18.0' font='Helvetica' size='12.0' fontstyle='normal' red='0' green='0' blue='255' reference='#top'>
		<chunk font='Helvetica' size='12.0' fontstyle='normal' red='0' green='0' blue='255'>please jump to a local destination</chunk>
	</anchor>
	<paragraph leading='18.0' font='unknown' align='Default'>
		<newline />
<newline />

	</paragraph>
	<chapter numberdepth='1' depth='1' indent='0.0'>
		<title leading='36.0' align='Default' font='Helvetica' size='24.0' fontstyle='normal' red='255' green='0' blue='0'>
			<chunk font='Helvetica' size='24.0' fontstyle='normal' red='255' green='0' blue='0'>This is chapter 1</chunk>
		</title>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 1 in chapter 1</chunk>
			</title>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
		</section>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 2 in chapter 1</chunk>
			</title>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
		</section>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 3 in chapter 1</chunk>
			</title>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blaah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
				<table columns='3' width='80.0%' align='Center' cellpadding='5.0' cellspacing='5.0' widths='33.333332;33.333332;33.333332' borderwidth='1.0' left='true' right='true' top='true' bottom='true' red='0' green='0' blue='255'>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='3' header='true' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								header
							</paragraph>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' red='255' green='0' blue='0' horizontalalign='Default' verticalalign='Default' rowspan='2' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								example cell with colspan 1 and rowspan 2
							</paragraph>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								1.1
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								2.1
							</phrase>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								1.2
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								2.2
							</phrase>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								cell test1
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='2' rowspan='2' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								big cell
							</paragraph>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								cell test2
							</phrase>
						</cell>
					</row>
				</table>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
		</section>
	</chapter>
	<chapter numberdepth='1' depth='1' indent='0.0'>
		<title leading='36.0' align='Default' font='Helvetica' size='24.0' fontstyle='normal' red='255' green='0' blue='0'>
			<chunk font='Helvetica' size='24.0' fontstyle='normal' red='255' green='0' blue='0'>This is chapter 2</chunk>
		</title>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 1 in chapter 2</chunk>
			</title>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
		</section>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 2 in chapter 2</chunk>
			</title>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
		</section>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 3 in chapter 2</chunk>
			</title>
			<paragraph leading='18.0' font='unknown' align='Default'>
				blah blah blah blah blah blah blaah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah
			</paragraph>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blaah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
				<table columns='3' width='80.0%' align='Center' cellpadding='5.0' cellspacing='5.0' widths='33.333332;33.333332;33.333332' borderwidth='1.0' left='true' right='true' top='true' bottom='true' red='0' green='0' blue='255'>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='3' header='true' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								header
							</paragraph>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' red='255' green='0' blue='0' horizontalalign='Default' verticalalign='Default' rowspan='2' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								example cell with colspan 1 and rowspan 2
							</paragraph>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								1.1
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								2.1
							</phrase>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								1.2
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								2.2
							</phrase>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								cell test1
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='2' rowspan='2' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								big cell
							</paragraph>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								cell test2
							</phrase>
						</cell>
					</row>
				</table>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
		</section>
	</chapter>
	<chapter numberdepth='1' depth='1' indent='0.0'>
		<title leading='36.0' align='Default' font='Helvetica' size='24.0' fontstyle='normal' red='255' green='0' blue='0'>
			<chunk font='Helvetica' size='24.0' fontstyle='normal' red='255' green='0' blue='0'>This is chapter 3</chunk>
		</title>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 1 in chapter 3</chunk>
			</title>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
		</section>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 2 in chapter 3</chunk>
			</title>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<paragraph leading='18.0' font='unknown' align='Default'>
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blaah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
			</paragraph>
			<table columns='3' width='80.0%' align='Center' cellpadding='5.0' cellspacing='5.0' widths='33.333332;33.333332;33.333332' borderwidth='1.0' left='true' right='true' top='true' bottom='true' red='0' green='0' blue='255'>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='3' header='true' leading='18.0'>
						<paragraph leading='18.0' font='unknown' align='Default'>
							header
						</paragraph>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' red='255' green='0' blue='0' horizontalalign='Default' verticalalign='Default' rowspan='2' leading='18.0'>
						<paragraph leading='18.0' font='unknown' align='Default'>
							example cell with colspan 1 and rowspan 2
						</paragraph>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							1.1
						</phrase>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							2.1
						</phrase>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							1.2
						</phrase>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							2.2
						</phrase>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							cell test1
						</phrase>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='2' rowspan='2' leading='18.0'>
						<paragraph leading='18.0' font='unknown' align='Default'>
							big cell
						</paragraph>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							cell test2
						</phrase>
					</cell>
				</row>
			</table>
		</section>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 3 in chapter 3</chunk>
			</title>
			<paragraph leading='18.0' font='unknown' align='Default'>
				blah blah blah blah blah blah blaah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah
			</paragraph>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blaah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
				<table columns='3' width='80.0%' align='Center' cellpadding='5.0' cellspacing='5.0' widths='33.333332;33.333332;33.333332' borderwidth='1.0' left='true' right='true' top='true' bottom='true' red='0' green='0' blue='255'>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='3' header='true' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								header
							</paragraph>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' red='255' green='0' blue='0' horizontalalign='Default' verticalalign='Default' rowspan='2' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								example cell with colspan 1 and rowspan 2
							</paragraph>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								1.1
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								2.1
							</phrase>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								1.2
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								2.2
							</phrase>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								cell test1
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='2' rowspan='2' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								big cell
							</paragraph>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								cell test2
							</phrase>
						</cell>
					</row>
				</table>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Default'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
		</section>
	</chapter>
	<chapter numberdepth='1' depth='1' indent='0.0'>
		<title leading='36.0' align='Default' font='Helvetica' size='24.0' fontstyle='normal' red='255' green='0' blue='0'>
			<chunk font='Helvetica' size='24.0' fontstyle='normal' red='255' green='0' blue='0'>This is chapter 4</chunk>
		</title>
		<paragraph leading='18.0' font='unknown' align='Justify'>
			blah blah blah blah blah blah blaah blah blah blah blah blah blah
			blah blah blah blah blah blah blah blah blah blah blah blah blah
			blah blah blah blah blah blah blah blah blah blah blah blah blah
			blah blah blah blah blah blah blah blah blah blah blah blah
		</paragraph>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 1 in chapter 4</chunk>
			</title>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
		</section>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 2 in chapter 4</chunk>
			</title>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<paragraph leading='18.0' font='unknown' align='Justify'>
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blaah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
			</paragraph>
			<table columns='3' width='80.0%' align='Center' cellpadding='5.0' cellspacing='5.0' widths='33.333332;33.333332;33.333332' borderwidth='1.0' left='true' right='true' top='true' bottom='true' red='0' green='0' blue='255'>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='3' header='true' leading='18.0'>
						<paragraph leading='18.0' font='unknown' align='Default'>
							header
						</paragraph>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' red='255' green='0' blue='0' horizontalalign='Default' verticalalign='Default' rowspan='2' leading='18.0'>
						<paragraph leading='18.0' font='unknown' align='Default'>
							example cell with colspan 1 and rowspan 2
						</paragraph>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							1.1
						</phrase>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							2.1
						</phrase>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							1.2
						</phrase>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							2.2
						</phrase>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							cell test1
						</phrase>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='2' rowspan='2' leading='18.0'>
						<paragraph leading='18.0' font='unknown' align='Default'>
							big cell
						</paragraph>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							cell test2
						</phrase>
					</cell>
				</row>
			</table>
		</section>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 3 in chapter 4</chunk>
			</title>
			<paragraph leading='18.0' font='unknown' align='Justify'>
				blah blah blah blah blah blah blaah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah
			</paragraph>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blaah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
				<table columns='3' width='80.0%' align='Center' cellpadding='5.0' cellspacing='5.0' widths='33.333332;33.333332;33.333332' borderwidth='1.0' left='true' right='true' top='true' bottom='true' red='0' green='0' blue='255'>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='3' header='true' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								header
							</paragraph>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' red='255' green='0' blue='0' horizontalalign='Default' verticalalign='Default' rowspan='2' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								example cell with colspan 1 and rowspan 2
							</paragraph>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								1.1
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								2.1
							</phrase>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								1.2
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								2.2
							</phrase>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								cell test1
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='2' rowspan='2' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								big cell
							</paragraph>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								cell test2
							</phrase>
						</cell>
					</row>
				</table>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
		</section>
	</chapter>
	<chapter numberdepth='1' depth='1' indent='0.0'>
		<title leading='36.0' align='Default' font='Helvetica' size='24.0' fontstyle='normal' red='255' green='0' blue='0'>
			<chunk font='Helvetica' size='24.0' fontstyle='normal' red='255' green='0' blue='0'>This is chapter 5</chunk>
		</title>
		<paragraph leading='18.0' font='unknown' align='Right'>
			blah blah blah blah blah blah blaah blah blah blah blah blah blah
			blah blah blah blah blah blah blah blah blah blah blah blah blah
			blah blah blah blah blah blah blah blah blah blah blah blah blah
			blah blah blah blah blah blah blah blah blah blah blah blah
		</paragraph>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 1 in chapter 5</chunk>
			</title>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Right'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Right'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Right'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
		</section>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 2 in chapter 5</chunk>
			</title>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Right'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Right'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Right'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<paragraph leading='18.0' font='unknown' align='Center'>
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blaah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
			</paragraph>
			<table columns='3' width='80.0%' align='Center' cellpadding='5.0' cellspacing='5.0' widths='33.333332;33.333332;33.333332' borderwidth='1.0' left='true' right='true' top='true' bottom='true' red='0' green='0' blue='255'>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='3' header='true' leading='18.0'>
						<paragraph leading='18.0' font='unknown' align='Default'>
							header
						</paragraph>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' red='255' green='0' blue='0' horizontalalign='Default' verticalalign='Default' rowspan='2' leading='18.0'>
						<paragraph leading='18.0' font='unknown' align='Default'>
							example cell with colspan 1 and rowspan 2
						</paragraph>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							1.1
						</phrase>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							2.1
						</phrase>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							1.2
						</phrase>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							2.2
						</phrase>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							cell test1
						</phrase>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='2' rowspan='2' leading='18.0'>
						<paragraph leading='18.0' font='unknown' align='Default'>
							big cell
						</paragraph>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							cell test2
						</phrase>
					</cell>
				</row>
			</table>
		</section>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 3 in chapter 5</chunk>
			</title>
			<paragraph leading='18.0' font='unknown' align='Right'>
				blah blah blah blah blah blah blaah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah
			</paragraph>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Center'>
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blaah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
				<table columns='3' width='80.0%' align='Center' cellpadding='5.0' cellspacing='5.0' widths='33.333332;33.333332;33.333332' borderwidth='1.0' left='true' right='true' top='true' bottom='true' red='0' green='0' blue='255'>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='3' header='true' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								header
							</paragraph>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' red='255' green='0' blue='0' horizontalalign='Default' verticalalign='Default' rowspan='2' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								example cell with colspan 1 and rowspan 2
							</paragraph>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								1.1
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								2.1
							</phrase>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								1.2
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								2.2
							</phrase>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								cell test1
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='2' rowspan='2' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								big cell
							</paragraph>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								cell test2
							</phrase>
						</cell>
					</row>
				</table>
				<paragraph leading='18.0' font='unknown' align='Right'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Right'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Right'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
		</section>
	</chapter>
	<chapter numberdepth='1' depth='1' indent='0.0'>
		<title leading='36.0' align='Default' font='Helvetica' size='24.0' fontstyle='normal' red='255' green='0' blue='0'>
			<chunk font='Helvetica' size='24.0' fontstyle='normal' red='255' green='0' blue='0'>This is chapter 6</chunk>
		</title>
		<table columns='3' width='80.0%' align='Center' cellpadding='5.0' cellspacing='5.0' widths='33.333332;33.333332;33.333332' borderwidth='1.0' left='true' right='true' top='true' bottom='true' red='0' green='0' blue='255'>
			<row>
				<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='3' header='true' leading='18.0'>
					<paragraph leading='18.0' font='unknown' align='Default'>
						header
					</paragraph>
				</cell>
			</row>
			<row>
				<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' red='255' green='0' blue='0' horizontalalign='Default' verticalalign='Default' rowspan='2' leading='18.0'>
					<paragraph leading='18.0' font='unknown' align='Default'>
						example cell with colspan 1 and rowspan 2
					</paragraph>
				</cell>
				<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
					<phrase leading='18.0' font='unknown'>
						1.1
					</phrase>
				</cell>
				<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
					<phrase leading='18.0' font='unknown'>
						2.1
					</phrase>
				</cell>
			</row>
			<row>
				<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
					<phrase leading='18.0' font='unknown'>
						1.2
					</phrase>
				</cell>
				<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
					<phrase leading='18.0' font='unknown'>
						2.2
					</phrase>
				</cell>
			</row>
			<row>
				<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
					<phrase leading='18.0' font='unknown'>
						cell test1
					</phrase>
				</cell>
				<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='2' rowspan='2' leading='18.0'>
					<paragraph leading='18.0' font='unknown' align='Default'>
						big cell
					</paragraph>
				</cell>
			</row>
			<row>
				<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
					<phrase leading='18.0' font='unknown'>
						cell test2
					</phrase>
				</cell>
			</row>
		</table>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 1 in chapter 6</chunk>
			</title>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
		</section>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 2 in chapter 6</chunk>
			</title>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<paragraph leading='18.0' font='unknown' align='Center'>
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blaah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
			</paragraph>
			<table columns='3' width='80.0%' align='Center' cellpadding='5.0' cellspacing='5.0' widths='33.333332;33.333332;33.333332' borderwidth='1.0' left='true' right='true' top='true' bottom='true' red='0' green='0' blue='255'>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='3' header='true' leading='18.0'>
						<paragraph leading='18.0' font='unknown' align='Default'>
							header
						</paragraph>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' red='255' green='0' blue='0' horizontalalign='Default' verticalalign='Default' rowspan='2' leading='18.0'>
						<paragraph leading='18.0' font='unknown' align='Default'>
							example cell with colspan 1 and rowspan 2
						</paragraph>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							1.1
						</phrase>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							2.1
						</phrase>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							1.2
						</phrase>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							2.2
						</phrase>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							cell test1
						</phrase>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='2' rowspan='2' leading='18.0'>
						<paragraph leading='18.0' font='unknown' align='Default'>
							big cell
						</paragraph>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							cell test2
						</phrase>
					</cell>
				</row>
			</table>
		</section>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 3 in chapter 6</chunk>
			</title>
			<paragraph leading='18.0' font='unknown' align='Justify'>
				blah blah blah blah blah blah blaah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah
			</paragraph>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Center'>
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blaah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
				<table columns='3' width='80.0%' align='Center' cellpadding='5.0' cellspacing='5.0' widths='33.333332;33.333332;33.333332' borderwidth='1.0' left='true' right='true' top='true' bottom='true' red='0' green='0' blue='255'>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='3' header='true' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								header
							</paragraph>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' red='255' green='0' blue='0' horizontalalign='Default' verticalalign='Default' rowspan='2' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								example cell with colspan 1 and rowspan 2
							</paragraph>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								1.1
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								2.1
							</phrase>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								1.2
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								2.2
							</phrase>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								cell test1
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='2' rowspan='2' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								big cell
							</paragraph>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								cell test2
							</phrase>
						</cell>
					</row>
				</table>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
		</section>
	</chapter>
	<chapter numberdepth='1' depth='1' indent='0.0'>
		<title leading='36.0' align='Default' font='Helvetica' size='24.0' fontstyle='normal' red='255' green='0' blue='0'>
			<chunk font='Helvetica' size='24.0' fontstyle='normal' red='255' green='0' blue='0'>This is chapter 7</chunk>
		</title>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 1 in chapter 7</chunk>
			</title>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 1</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
		</section>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 2 in chapter 7</chunk>
			</title>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 2</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<paragraph leading='18.0' font='unknown' align='Center'>
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blaah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
			</paragraph>
			<table columns='3' width='80.0%' align='Center' cellpadding='5.0' cellspacing='5.0' widths='33.333332;33.333332;33.333332' borderwidth='1.0' left='true' right='true' top='true' bottom='true' red='0' green='0' blue='255'>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='3' header='true' leading='18.0'>
						<paragraph leading='18.0' font='unknown' align='Default'>
							header
						</paragraph>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' red='255' green='0' blue='0' horizontalalign='Default' verticalalign='Default' rowspan='2' leading='18.0'>
						<paragraph leading='18.0' font='unknown' align='Default'>
							example cell with colspan 1 and rowspan 2
						</paragraph>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							1.1
						</phrase>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							2.1
						</phrase>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							1.2
						</phrase>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							2.2
						</phrase>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							cell test1
						</phrase>
					</cell>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='2' rowspan='2' leading='18.0'>
						<paragraph leading='18.0' font='unknown' align='Default'>
							big cell
						</paragraph>
					</cell>
				</row>
				<row>
					<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
						<phrase leading='18.0' font='unknown'>
							cell test2
						</phrase>
					</cell>
				</row>
			</table>
		</section>
		<section numberdepth='1' depth='2' indent='0.0'>
			<title leading='30.0' align='Default' font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>
				<chunk font='Helvetica' size='20.0' fontstyle='normal' red='0' green='0' blue='255'>This is section 3 in chapter 7</chunk>
			</title>
			<paragraph leading='18.0' font='unknown' align='Justify'>
				blah blah blah blah blah blah blaah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah blah
				blah blah blah blah blah blah blah blah blah blah blah blah
			</paragraph>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 1 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Center'>
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blaah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
				<table columns='3' width='80.0%' align='Center' cellpadding='5.0' cellspacing='5.0' widths='33.333332;33.333332;33.333332' borderwidth='1.0' left='true' right='true' top='true' bottom='true' red='0' green='0' blue='255'>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='3' header='true' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								header
							</paragraph>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' red='255' green='0' blue='0' horizontalalign='Default' verticalalign='Default' rowspan='2' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								example cell with colspan 1 and rowspan 2
							</paragraph>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								1.1
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								2.1
							</phrase>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								1.2
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								2.2
							</phrase>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								cell test1
							</phrase>
						</cell>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' colspan='2' rowspan='2' leading='18.0'>
							<paragraph leading='18.0' font='unknown' align='Default'>
								big cell
							</paragraph>
						</cell>
					</row>
					<row>
						<cell borderwidth='0.5' left='true' right='true' top='true' bottom='true' horizontalalign='Default' verticalalign='Default' leading='18.0'>
							<phrase leading='18.0' font='unknown'>
								cell test2
							</phrase>
						</cell>
					</row>
				</table>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 2 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
			<section numberdepth='3' depth='3' indent='0.0'>
				<title leading='27.0' align='Default' font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>
					<chunk font='Helvetica' size='18.0' fontstyle='normal' red='0' green='64' blue='64'>This is subsection 3 of section 3</chunk>
				</title>
				<paragraph leading='18.0' font='unknown' align='Justify'>
					blah blah blah blah blah blah blaah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah blah
					blah blah blah blah blah blah blah blah blah blah blah blah
				</paragraph>
			</section>
		</section>
	</chapter>
</itext>
";
        var pdfFilePath = TestUtils.GetOutputFileName();
        ConverITextXmlToPdfFile(iTextXML, pdfFilePath);
        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }


    private static void ConverITextXmlToPdfFile(string iTextXML, string pdfFilePath)
    {
        using var fileStream = new FileStream(pdfFilePath, FileMode.Create);
        using var document = new Document();
        PdfWriter.GetInstance(document, fileStream);

        document.AddAuthor(TestUtils.Author);
        document.Open();

        using var xmlReader = XmlReader.Create(new StringReader(iTextXML));

        var parser = new XmlParser();
        parser.Go(document, xmlReader);
    }
}