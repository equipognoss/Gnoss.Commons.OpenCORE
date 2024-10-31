namespace Es.Riam.Util.Test
{
    /// <summary>
    /// Esta clase se encarga de hacer pruebas de los métodos de la clase UtilCadenas
    /// </summary>
    public class UtilCadenasTest
    {
        [SetUp]
        public void Setup()
        {

        }

        #region LimpiarInyeccionCodigo

        [Test]
        public void LimpiarInyeccionCodigo_BassicXSSTestWithoutFilterEvasion()
        {
            string test = "<SCRIPT SRC=https://cdn.jsdelivr.net/gh/Moksh45/host-xss.rocks/index.js></SCRIPT>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_XSSLocatorPolyglot()
        {
            string test = "javascript:/*--></title></style></textarea></script></xmp>\r\n<svg/onload='+/\"`/+/onmouseover=1/+/[*/[]/+alert(42);//'>";
            string expected = "javascript:/*--&gt;\n";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_MalformedTags()
        {
            string test = "\\<a onmouseover=\"alert(document.cookie)\"\\>xxs link\\</a\\>";
            string expected = "\\<a>xxs link\\</a>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_MalformedIMGTags()
        {
            string test = "<IMG \"\"\"><SCRIPT>alert(\"XSS\")</SCRIPT>\"\\>";
            string expected = "<img>\"\\&gt;";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_FromCharCode()
        {
            string test = "<IMG SRC=javascript:alert(String.fromCharCode(88,83,83))>";
            string expected = "<img>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_DefaultSRCTagGetPastFiltersCheckSRCDomain()
        {
            string test = "<IMG SRC=# onmouseover=\"alert('xxs')\">";
            string expected = "<img src=\"#\">";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_DefaultSRCTagLeavingItEmpty()
        {
            string test = "<IMG SRC= onmouseover=\"alert('xxs')\">";
            string expected = "<img src=\"onmouseover=&quot;alert('xxs')&quot;\">";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_DefaultSRCTagLLeavingOutEntirely()
        {
            string test = "<IMG onmouseover=\"alert('xxs')\">";
            string expected = "<img>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_OnErrorAlert()
        {
            string test = "<IMG SRC=/ onerror=\"alert(String.fromCharCode(88,83,83))\"></img>";
            string expected = "<img src=\"/\">";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_IMGOnErrorJavaScriptAlertEncode()
        {
            string test = "<img src=x onerror=\"&#0000106&#0000097&#0000118&#0000097&#0000115&#0000099&#0000114&#0000105&#0000112&#0000116&#0000058&#0000097&#0000108&#0000101&#0000114&#0000116&#0000040&#0000039&#0000088&#0000083&#0000083&#0000039&#0000041\">";
            string expected = "<img src=\"x\">";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_DecimalHTMLCharacterReferences()
        {
            string test = "<IMG SRC=&#106;&#97;&#118;&#97;&#115;&#99;&#114;&#105;&#112;&#116;&#58;&#97;&#108;&#101;&#114;&#116;&#40;&#39;&#88;&#83;&#83;&#39;&#41;>";
            string expected = "<img>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_DecimalHTMLCharacterReferencesWithoutTrailingSemicolons()
        {
            string test = "<IMG SRC=&#0000106&#0000097&#0000118&#0000097&#0000115&#0000099&#0000114&#0000105&#0000112&#0000116&#0000058&#0000097&#0000108&#0000101&#0000114&#0000116&#0000040&#0000039&#0000088&#0000083&#0000083&#0000039&#0000041>";
            string expected = "<img>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_HexadecimalHTMLCharacterReferencesWithoutTrailingSemicolons()
        {
            string test = "<IMG SRC=&#x6A&#x61&#x76&#x61&#x73&#x63&#x72&#x69&#x70&#x74&#x3A&#x61&#x6C&#x65&#x72&#x74&#x28&#x27&#x58&#x53&#x53&#x27&#x29>";
            string expected = "<img>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_EmbeddedTab()
        {
            string test = "<IMG SRC=\"jav   ascript:alert('XSS');\">";
            string expected = "<img>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_EmbeddedEncodedTab()
        {
            string test = "<IMG SRC=\"jav&#x09;ascript:alert('XSS');\">";
            string expected = "<img>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_EmbeddedNexlineBreakUpXSS()
        {
            string test = "<IMG SRC=\"jav&#x0A;ascript:alert('XSS');\">";
            string expected = "<img>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_EmbeddedNexlineBreakUpXSSCarriageReturn()
        {
            string test = "<IMG SRC=\"jav&#x0D;ascript:alert('XSS');\">";
            string expected = "<img>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_EmbeddedNexlineBreakUpXSSJavaScriptDirectiveWithNull()
        {
            string test = "perl -e 'print \"<IMG SRC=java\\0script:alert(\\\"XSS\\\")>\";' > out";
            string expected = "perl -e 'print \"<img>\";' &gt; out";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_EmbeddedNexlineBreakUpXSSMetaCharsBeforeJavaScriptImages()
        {
            string test = "<IMG SRC=\" &#14;  javascript:alert('XSS');\">";
            string expected = "<img>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_EmbeddedNexlineBreakUpXSSNonAlphaNonDigit()
        {
            string test = "<SCRIPT/XSS SRC=\"http://xss.rocks/xss.js\"></SCRIPT>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_EmbeddedNexlineBreakUpXSSNonAlphaNonDigitOtherChasrs()
        {
            string test = "<BODY onload!#$%&()*~+-_.,:;?@[/|\\]^`=alert(\"XSS\")>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_EmbeddedNexlineBreakUpXSSNonAlphaNonDigitWithNoSpaces()
        {
            string test = "<SCRIPT/SRC=\"http://xss.rocks/xss.js\"></SCRIPT>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_ExtraneousOpenBrackets()
        {
            string test = "<<SCRIPT>alert(\"XSS\");//\\<</SCRIPT>";
            string expected = "&lt;";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_NoClosingScrptTags()
        {
            string test = "<SCRIPT SRC=http://xss.rocks/xss.js?< B >";
            string expected = "";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_ProtocolResolutionScriptTags()
        {
            string test = "<SCRIPT SRC=//xss.rocks/.j>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_HalfOpenHTMLJavaScriptXSSVector()
        {
            string test = "<IMG SRC=\"`<javascript:alert>`('XSS')\"";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_EscapingJavaScriptEscapesFinishScriptBlock()
        {
            string test = "</script><script>alert('XSS');</script>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_EndTitleTag()
        {
            string test = "</TITLE><SCRIPT>alert(\"XSS\");</SCRIPT>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_InputImage()
        {
            string test = "<INPUT TYPE=\"IMAGE\" SRC=\"javascript:alert('XSS');\">";
            string expected = "<input type=\"IMAGE\">";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_BodyImage()
        {
            string test = "<BODY BACKGROUND=\"javascript:alert('XSS')\">";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_IMGDynsrc()
        {
            string test = "<IMG DYNSRC=\"javascript:alert('XSS')\">";
            string expected = "<img>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_IMGLowsrc()
        {
            string test = "<IMG LOWSRC=\"javascript:alert('XSS')\">";
            string expected = "<img>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_ListStyleImage()
        {
            string test = "<STYLE>li {list-style-image: url(\"javascript:alert('XSS')\");}</STYLE><UL><LI>XSS</br>";
            string expected = "<ul><li>XSS<br></li></ul>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_VBScriptInAnImage()
        {
            string test = "<IMG SRC='vbscript:msgbox(\"XSS\")'>";
            string expected = "<img>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_BodyTag()
        {
            string test = "<BODY ONLOAD=alert('XSS')>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_Bgsound()
        {
            string test = "<BGSOUND SRC=\"javascript:alert('XSS');\">";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_AmpersandJavaScriptIncludes()
        {
            string test = "<BR SIZE=\"&{alert('XSS')}\">";
            string expected = "<br>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_StyleSheet()
        {
            string test = "<LINK REL=\"stylesheet\" HREF=\"javascript:alert('XSS');\">";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_RemoteStyleSheet()
        {
            string test = "<STYLE>@import'http://xss.rocks/xss.css';</STYLE>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_RemoteStyleSheetGecko()
        {
            string test = "<STYLE>BODY{-moz-binding:url(\"http://xss.rocks/xssmoz.xml#xss\")}</STYLE>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_StyleTagsBreaksUpJavaScript()
        {
            string test = "<STYLE>@im\\port'\\ja\\vasc\\ript:alert(\"XSS\")';</STYLE>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_StyleAttributeBreaksUpExpression()
        {
            string test = "<IMG STYLE=\"xss:expr/*XSS*/ession(alert('XSS'))\">";
            string expected = "<img>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_IMGStyleExpressions()
        {
            string test = "exp/*<A STYLE='no\\xss:noxss(\"*//*\");\r\nxss:ex/*XSS*//*/*/pression(alert(\"XSS\"))'>";
            string expected = "exp/*<a></a>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_StyleTagUsingBackgroundImage()
        {
            string test = "<STYLE>.XSS{background-image:url(\"javascript:alert('XSS')\");}</STYLE><A CLASS=XSS></A>";
            string expected = "<a class=\"XSS\"></a>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_StyleTagUsingBackground()
        {
            string test = "<STYLE type=\"text/css\">BODY{background:url(\"javascript:alert('XSS')\")}</STYLE>\r\n<STYLE type=\"text/css\">BODY{background:url(\"<javascript:alert>('XSS')\")}</STYLE>";
            string expected = "\n";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_AnonymusHTMLWithStyleAttribute()
        {
            string test = "<XSS STYLE=\"xss:expression(alert('XSS'))\">";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_LocalHtcFile()
        {
            string test = "<XSS STYLE=\"behavior: url(xss.htc);\">";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_Meta()
        {
            string test = "<META HTTP-EQUIV=\"refresh\" CONTENT=\"0;url=javascript:alert('XSS');\">";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_MetaUsingData()
        {
            string test = "<META HTTP-EQUIV=\"refresh\" CONTENT=\"0;url=data:text/html base64,PHNjcmlwdD5hbGVydCgnWFNTJyk8L3NjcmlwdD4K\">";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_MetaWithAdditionalUrlParameter()
        {
            string test = "<META HTTP-EQUIV=\"refresh\" CONTENT=\"0; URL=http://;URL=javascript:alert('XSS');\">";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_IFrame()
        {
            string test = "<IFRAME SRC=\"javascript:alert('XSS');\"></IFRAME>";
            string expected = "<iframe></iframe>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_IFrameEventBased()
        {
            string test = "<IFRAME SRC=# onmouseover=\"alert(document.cookie)\"></IFRAME>";
            string expected = "<iframe src=\"#\"></iframe>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_Frame()
        {
            string test = "<FRAMESET><FRAME SRC=\"javascript:alert('XSS');\"></FRAMESET>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_Table()
        {
            string test = "<TABLE BACKGROUND=\"javascript:alert('XSS')\">";
            string expected = "<table></table>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_Td()
        {
            string test = "<TABLE><TD BACKGROUND=\"javascript:alert('XSS')\">";
            string expected = "<table><tbody><tr><td></td></tr></tbody></table>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_DivBackgroundImage()
        {
            string test = "<DIV STYLE=\"background-image: url(javascript:alert('XSS'))\">";
            string expected = "<div></div>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_DivBackgroundImageWithUnicodeXSSExploit()
        {
            string test = "<DIV STYLE=\"background-image:\\0075\\0072\\006C\\0028'\\006a\\0061\\0076\\0061\\0073\\0063\\0072\\0069\\0070\\0074\\003a\\0061\\006c\\0065\\0072\\0074\\0028.1027\\0058.1053\\0053\\0027\\0029'\\0029\">";
            string expected = "<div></div>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_DivBackgroundImagePlusExtraCharacters()
        {
            string test = "<DIV STYLE=\"background-image: url(\u0001javascript:alert('XSS'))\">";
            string expected = "<div></div>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_DivExpression()
        {
            string test = "<DIV STYLE=\"width: expression(alert('XSS'));\">";
            string expected = "<div></div>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_DownllevelHiddenBlock()
        {
            string test = "<!--[if gte IE 4]>\r\n<SCRIPT>alert('XSS');</SCRIPT>\r\n<![endif]-->";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_BaseTag()
        {
            string test = "<BASE HREF=\"javascript:alert('XSS');//\">";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_ObjectTag()
        {
            string test = "<OBJECT TYPE=\"text/x-scriptlet\" DATA=\"http://xss.rocks/scriptlet.html\"></OBJECT>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_EmbedSvgWhichContainsXSSVector()
        {
            string test = "<EMBED SRC=\"data:image/svg+xml;base64,PHN2ZyB4bWxuczpzdmc9Imh0dH A6Ly93d3cudzMub3JnLzIwMDAvc3ZnIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcv MjAwMC9zdmciIHhtbG5zOnhsaW5rPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5L3hs aW5rIiB2ZXJzaW9uPSIxLjAiIHg9IjAiIHk9IjAiIHdpZHRoPSIxOTQiIGhlaWdodD0iMjAw IiBpZD0ieHNzIj48c2NyaXB0IHR5cGU9InRleHQvZWNtYXNjcmlwdCI+YWxlcnQoIlh TUyIpOzwvc2NyaXB0Pjwvc3ZnPg==\" type=\"image/svg+xml\" AllowScriptAccess=\"always\"></EMBED>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_XmlDataIslandWithCdataObfuscation()
        {
            string test = "<XML ID=\"xss\"><I><B><IMG SRC=\"javas<!-- -->cript:alert('XSS')\"></B></I></XML>\r\n<SPAN DATASRC=\"#xss\" DATAFLD=\"B\" DATAFORMATAS=\"HTML\"></SPAN>";
            string expected = "\n<span></span>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_LocallyHostedXmlEmbeddedJavaScriptGeneratedUsingXmlDataIsland()
        {
            string test = "<XML SRC=\"xsstest.xml\" ID=I></XML>\r\n<SPAN DATASRC=#I DATAFLD=C DATAFORMATAS=HTML></SPAN>";
            string expected = "\n<span></span>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_HtmlTimeXml()
        {
            string test = "<HTML><BODY>\r\n<?xml:namespace prefix=\"t\" ns=\"urn:schemas-microsoft-com:time\">\r\n<?import namespace=\"t\" implementation=\"#default#time2\">\r\n<t:set attributeName=\"innerHTML\" to=\"XSS<SCRIPT DEFER>alert(\"XSS\")</SCRIPT>\">\r\n</BODY></HTML>";
            string expected = "\n\n\n";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_AssumingCanFitFewCharactersAdFiltersAgainstJS()
        {
            string test = "<SCRIPT SRC=\"http://xss.rocks/xss.jpg\"></SCRIPT>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_ServerSideIncudes()
        {
            string test = "<!--#exec cmd=\"/bin/echo '<SCR'\"--><!--#exec cmd=\"/bin/echo 'IPT SRC=http://xss.rocks/xss.js></SCRIPT>'\"-->";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_Php()
        {
            string test = "<? echo('<SCR)';\r\necho('IPT>alert(\"XSS\")</SCRIPT>'); ?>";
            string expected = "alert(\"XSS\")'); ?&gt;";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_ImgEmbeddedCommandsWithoutIdentifiers()
        {
            string test = "Redirect 302 /a.jpg http://victimsite.com/admin.asp&deleteuser";
            string expected = "Redirect 302 /a.jpg http://victimsite.com/admin.asp&amp;deleteuser";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_CookieManipulation()
        {
            string test = "<META HTTP-EQUIV=\"Set-Cookie\" Content=\"USERID=<SCRIPT>alert('XSS')</SCRIPT>\">";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_XssUsingHtmlQuoteEncapsulation()
        {
            string test = "<SCRIPT a=\">\" SRC=\"httx://xss.rocks/xss.js\"></SCRIPT>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_XssUsingHtmlQuoteEncapsulationPart2()
        {
            string test = "<SCRIPT =\">\" SRC=\"httx://xss.rocks/xss.js\"></SCRIPT>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_XssUsingHtmlQuoteEncapsulationPart3()
        {
            string test = "<SCRIPT a=\">\" '' SRC=\"httx://xss.rocks/xss.js\"></SCRIPT>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_XssUsingHtmlQuoteEncapsulationPart4()
        {
            string test = "<SCRIPT \"a='>'\" SRC=\"httx://xss.rocks/xss.js\"></SCRIPT>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_XssUsingHtmlQuoteEncapsulationPart5()
        {
            string test = "<SCRIPT a=\">'>\" SRC=\"httx://xss.rocks/xss.js\"></SCRIPT>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_JavaScriptLinkLocation()
        {
            string test = "<A HREF=\"javascript:document.location='http://www.google.com/'\">XSS</A>";
            string expected = "<a>XSS</a>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_SharePageSourceCode()
        {
            string test = "<script>\r\nvar contentType = <%=Request.getParameter(\"content_type\")%>;\r\nvar title = \"<%=Encode.forJavaScript(request.getParameter(\"title\"))%>\";\r\n...\r\n//some user agreement and sending to server logic might be here\r\n...\r\n</script>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_ContentPageOutput()
        {
            string test = "<a href=\"/share?content_type=1&title=This is a regular title&amp;content_type=1;alert(1)\">Share</a>";
            string expected = "<a href=\"/share?content_type=1&amp;title=This is a regular title&amp;content_type=1;alert(1)\">Share</a>";
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LimpiarInyeccionCodigo_SharePageOutput()
        {
            string test = "<script>\r\nvar contentType = 1; alert(1);\r\nvar title = \"This is a regular title\";\r\n…\r\n//some user agreement and sending to server logic might be here\r\n…\r\n</script>";
            string expected = string.Empty;
            string result = UtilCadenas.LimpiarInyeccionCodigo(test);
            Assert.That(result, Is.EqualTo(expected));
        }

		[Test]
		public void LimpiarInyeccionCodigo_LimpiarFormularios()
		{
			//Codificado 3 veces con codificación de url
			string test = "Esto es un formulario <form action=\"change-password\"><input type=\"text\"/><input type=\"submit\" value=\"Enviar\"/></form>";
			string expected = "Esto es un formulario ";
			string result = UtilCadenas.LimpiarInyeccionCodigo(test);
			Assert.That(result, Is.EqualTo(expected));
		}

		[Test]
		public void LimpiarInyeccionCodigo_MultipleUrlEncodedText()
		{
            //Codificado 3 veces con codificación de url
			string test = "%25253Cform%252520id%25253D%252522testvuln%252522%252520action%25253D%252522https%25253A%25252F%25252Fdominio.prueba.gnos.com%252522%252520method%25253D%252522POST%252522%252520enctype%25253D%252522multipart%25252Fform-data%252522%25253E%25253Cbr%252520%25252F%25253E%25253Cinput%252520type%25253D%252522hidden%252522%252520name%25253D%252522peticionAJAX%252522%252520value%25253D%252522true%252522%252520%25252F%25253E%25253Cbr%252520%25252F%25253E%25253Cinput%252520type%25253D%252522hidden%252522%252520name%25253D%252522ProfilePersonal%252526%25252346%25253BName%252522%252520value%25253D%252522RT%252522%252520%25252F%25253E%25253Cbr%252520%25252F%25253E%25253Cinput%252520type%25253D%252522hidden%252522%252520name%25253D%252522ProfilePersonal%252526%25252346%25253BLastName%252522%252520value%25253D%252522Modified%252522%252520%25252F%25253E%25253Cbr%252520%25252F%25253E%25253Cinput%252520type%25253D%252522hidden%252522%252520name%25253D%252522ProfilePersonal%252526%25252346%25253BCountry%252522%252520value%25253D%2525226fa9b8c0%252526%25252345%25253Becb9%252526%25252345%25253B4e63%252526%25252345%25253Ba1ac%252526%25252345%25253B6bef0299e56a%252522%252520%25252F%25253E%25253Cbr%252520%25252F%25253E%25253Cinput%252520type%25253D%252522hidden%252522%252520name%25253D%252522ProfilePersonal%252526%25252346%25253BLang%252522%252520value%25253D%252522es%252522%252520%25252F%25253E%25253Cbr%252520%25252F%25253E%25253Cinput%252520type%25253D%252522hidden%252522%252520name%25253D%252522ProfilePersonal%252526%25252346%25253BRegion%252522%252520value%25253D%252522wewew%252522%252520%25252F%25253E%25253Cbr%252520%25252F%25253E%25253Cinput%252520type%25253D%252522hidden%252522%252520name%25253D%252522ProfilePersonal%252526%25252346%25253BSex%252522%252520value%25253D%252522M%252522%252520%25252F%25253E%25253Cbr%252520%25252F%25253E%25253Cinput%252520type%25253D%252522hidden%252522%252520name%25253D%252522ProfilePersonal%252526%25252346%25253BPostalCode%252522%252520value%25253D%25252228033%252522%252520%25252F%25253E%25253Cbr%252520%25252F%25253E%25253Cinput%252520type%25253D%252522hidden%252522%252520name%25253D%252522ProfilePersonal%252526%25252346%25253BLocation%252522%252520value%25253D%252522wqwqwq%252522%252520%25252F%25253E%25253Cbr%252520%25252F%25253E%25253Cinput%252520type%25253D%252522hidden%252522%252520name%25253D%252522ProfilePersonal%252526%25252346%25253BBornDate%252522%252520value%25253D%25252214%252526%25252347%25253B03%252526%25252347%25253B1995%252522%252520%25252F%25253E%25253Cbr%252520%25252F%25253E%25253Cinput%252520type%25253D%252522hidden%252522%252520name%25253D%252522ProfilePersonal%252526%25252346%25253BEmail%252522%252520value%25253D%252522joxesaj776%252526%25252364%25253Bbsomek%252526%25252346%25253Bcom%252522%252520%25252F%25253E%25253Cbr%252520%25252F%25253E%25253Cinput%252520type%25253D%252522hidden%252522%252520name%25253D%252522a3f03440%252526%25252345%25253B790b%252526%25252345%25253B43ec%252526%25252345%25253B94a5%252526%25252345%25253B48ea8fcdeb42%252522%252520value%25253D%252522a8b2d414%252526%25252345%25253Bbf02%252526%25252345%25253B4b2b%252526%25252345%25253B9686%252526%25252345%25253B1c3bab84c362%252522%252520%25252F%25253E%25253Cbr%252520%25252F%25253E%25253Cbr%252520%25252F%25253E%25253C%25252Fform%25253E%25253Cimg%252520src%25253Dx%252520onerror%25253D%252527document.getElementById%252528%252522testvuln%252522%252529.submit%252528%252529%252527%25253E";
			string expected = "<img src=\"x\">";
			string result = UtilCadenas.LimpiarInyeccionCodigo(test);
			Assert.That(result, Is.EqualTo(expected));
		}

		#endregion

		#region Decodificar texto codificado multiples veces
		[Test]
		public void DecodificarTextoCodificadoMultiplesVeces_UnaCodificacion()
		{
			string test = "Esto%20es%20un%20formulario%20%3Cform%20action%3D%5C%22change-password%5C%22%3E%3Cinput%20type%3D%5C%22text%5C%22%2F%3E%3Cinput%20type%3D%5C%22submit%5C%22%20value%3D%5C%22Enviar%5C%22%2F%3E%3C%2Fform%3E";
			string expected = "Esto es un formulario ";
			string result = UtilCadenas.LimpiarInyeccionCodigo(test);
			Assert.That(result, Is.EqualTo(expected));
		}

		[Test]
		public void DecodificarTextoCodificadoMultiplesVeces_TresCodificaciones()
		{
			string test = "Esto%252520es%252520un%252520formulario%252520%25253Cform%252520action%25253D%25255C%252522change-password%25255C%252522%25253E%25253Cinput%252520type%25253D%25255C%252522text%25255C%252522%25252F%25253E%25253Cinput%252520type%25253D%25255C%252522submit%25255C%252522%252520value%25253D%25255C%252522Enviar%25255C%252522%25252F%25253E%25253C%25252Fform%25253E";
			string expected = "Esto es un formulario ";
			string result = UtilCadenas.LimpiarInyeccionCodigo(test);
			Assert.That(result, Is.EqualTo(expected));
		}
		#endregion

		#region LimpiarCaracteresNombreCortoRegistro

		/// <summary>
		/// Se comprueba que el método limpia y remplaza correctamente todos los caracteres ascii y remplaza y deja únicamente los permitidos
		/// </summary>
		[Test]
        public void LimpiarCaracteresNombreCortoRegistro_ASCIICharacters()
        {
            string test = "";
            //Creamos test con los 255 caracteres ASCII
            for (int i = 0; i <= 255; i++)
            {
                test += (char)i;
            }

            //Letras numeros y guion
            string expected = "--0123456789abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzaaaaaaceeeeiiiionoooooouuuuyaaaaaaceeeeiiiionoooooouuuuyy";

            string result = UtilCadenas.LimpiarCaracteresNombreCortoRegistro(test);
            Assert.That(result, Is.EqualTo(expected));
        }

        /// <summary>
        /// Este test prueba un caso de registro real de nombre y apellido para generar el nombre corto.
        /// </summary>
        [Test]
        public void LimpiarCaracteresNombreCortoRegistro_TestNombreRegistro()
        {
            string test = "Santiago-López de Luzuriaga Olmos";

            //Letras numeros y guion
            string expected = "santiago-lopez-de-luzuriaga-olmos";

            string result = UtilCadenas.LimpiarCaracteresNombreCortoRegistro(test);
            Assert.That(result, Is.EqualTo(expected));
        }
        #endregion

    }
}