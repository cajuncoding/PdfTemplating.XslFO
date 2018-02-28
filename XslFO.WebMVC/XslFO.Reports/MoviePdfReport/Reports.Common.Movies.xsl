<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" exclude-result-prefixes=""
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:fo="http://www.w3.org/1999/XSL/Format"
	xmlns:csharp="urn:Xslt.System.Extensions">

	<xsl:variable name="FontFamily.Default" select="'Cambria'" />
	<xsl:variable name="FontSize.Default" select="'10pt'" />
	<xsl:variable name="FontSize.Medium" select="'9pt'" />
	<xsl:variable name="FontSize.Small" select="'8pt'" />
	<xsl:variable name="FontSize.H1" select="'18pt'" />
	<xsl:variable name="FontSize.H2" select="'16pt'" />
	<xsl:variable name="FontSize.H3" select="'10pt'" />

	<xsl:variable name="FontFamily.Label" select="'Calibri'" />
	<xsl:variable name="FontSize.Label" select="'9pt'" />

	<xsl:variable name="FontFamily.Footer" select="'Verdana'" />
	<xsl:variable name="FontSize.Footer" select="'9pt'" />

	<xsl:variable name="Table.Border.Report" select="'1pt solid #C4C4C4'" />
	<xsl:variable name="Table.Border.Separator" select="'.25pt solid #A0A0A0'" />
	<xsl:variable name="Table.Padding.Report" select="'5px'" />
	
	<xsl:variable name="Header.SpaceAfter" select="'.25in'" />
	<xsl:variable name="Paragraph.SpaceBefore" select="'.15in'" />

	<xsl:variable name="Border.HorizontalRule" select="'.25pt solid #A0A0A0'" />
	
	<!-- Custom table settings for Reports -->
	<xsl:variable name="Table.EdgeSpacing.Default" select="'2px'" />
	<xsl:variable name="Table.BorderWidth.Default" select="'1px'" />
	<xsl:variable name="Table.BorderWidth.Bold" select="'2px'" />
	<xsl:variable name="Table.BackgroundColor" select="'#F2F2F2'" />
	<xsl:variable name="Table.HighlightRowColor" select="'#FFFBCC'" />
	<xsl:variable name="Table.EvenRowColor" select="'#F2F2F2'" />
	<xsl:variable name="Table.OddRowColor" select="'#FFFFFF'" />

	<xsl:variable name="Color.NeonYellow" select="'#F3F315'" />
	<xsl:variable name="Color.HighlighterYellow" select="'#FFFBCC'" />
	
	<!-- NOTE: List of Char Codes can be found at: https://en.wikipedia.org/wiki/List_of_XML_and_HTML_character_entity_references -->
	<xsl:variable name="Char.Divide" select="'&#247;'" />
	
</xsl:stylesheet>