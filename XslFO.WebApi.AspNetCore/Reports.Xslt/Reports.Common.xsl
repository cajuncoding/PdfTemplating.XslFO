<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" exclude-result-prefixes=""
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:fo="http://www.w3.org/1999/XSL/Format"
	xmlns:csharp="urn:Xslt.System.Extensions">
	
	<xsl:variable name="DateFormat.Short" select="'MM/dd/yyyy'" />
	<xsl:variable name="DateFormat.Long" select="'MM/dd/yyyy hh:mm tt'" />
	<xsl:variable name="DateFormat.Extended" select="'MM/dd/yyyy hh:mm:ss tt'" />
	<xsl:variable name="CurrencyFormat.US" select="'$###,###,##0.00'" />
	<xsl:variable name="NumberFormat.Decimal" select="'0.00'" />
	
	<xsl:variable name="space"><xsl:text>&#x20;</xsl:text></xsl:variable>
	<xsl:variable name="non_breaking_space"><xsl:text>&#xA0;</xsl:text></xsl:variable>
	<xsl:variable name="date_time_stamp"><xsl:value-of select="csharp:DateNow($DateFormat.Extended)" /></xsl:variable>
	<xsl:variable name="date_now" select="csharp:DateNow()" />

		<xsl:variable name="Color.Gray" select="'#777777'" />

	<!-- US-Letter-Portrait Content Width -->
	<xsl:variable name="ContentWidth.Default.Inches" select="7.5" />
	
	<!-- BBernard - 09/24/2012
		 NOTE: Generic Function to create a Layout Master Set with specified Parameters & Default Values 
	-->
	<xsl:template name="printLayoutMasterSet">
		<xsl:param name="headerHeight" select="'0in'" />
		<xsl:param name="footerHeight" select="'.5in'" />
		<xsl:param name="leftWidth" select="'0in'" />
		<xsl:param name="rightWidth" select="'0in'" />
		<xsl:param name="orientation" select="'portrait'" />

		<!-- 
			NOTE: Calculating Width of Body and Left/Right areas:
					The width of the body in can be calculated by subtracting the left and right margins from the page and the left and right margins from the region-body itself.
					The region-start/region-end are NOT factored into the width of the body because they always extend from the sides and overlap the body.  Therefore to create
					a column on the left/right side the region-body must have a margin size greater than or equal to the size of the respective extent.
		-->
		<fo:layout-master-set>
			<fo:simple-page-master master-name="US-Letter-Portrait" page-width="8.5in" page-height="11in" margin-top=".25in" margin-bottom=".25in" margin-left=".5in" margin-right=".5in">
				<fo:region-body margin-top="{$headerHeight}" margin-bottom="{$footerHeight}" margin-left="{$leftWidth}" margin-right="{$rightWidth}" />
				<fo:region-before extent="{$headerHeight}" />
				<fo:region-after extent="{$footerHeight}" />
				<fo:region-start extent="{$leftWidth}" />
				<fo:region-end extent="{$rightWidth}" />
			</fo:simple-page-master>
			
			<fo:simple-page-master master-name="US-Letter-Landscape" page-width="11in" page-height="8.5in" margin-top=".25in" margin-bottom=".25in" margin-left=".25in" margin-right=".25in">
				<fo:region-body margin-top="{$headerHeight}" margin-bottom="{$footerHeight}" margin-left="{$leftWidth}" margin-right="{$rightWidth}" />
				<fo:region-before extent="{$headerHeight}" />
				<fo:region-after extent="{$footerHeight}" />
				<fo:region-start extent="{$leftWidth}" />
				<fo:region-end extent="{$rightWidth}" />
			</fo:simple-page-master>
			<!--
			<fo:simple-page-master master-name="US-A-Portrait-Last" page-width="8.5in" page-height="11in" margin-top=".25in" margin-bottom=".1in" margin-left=".4in" margin-right=".4in">
				<fo:region-before extent="0in" />
				<fo:region-after extent="2in" />
				<fo:region-start extent="0in" />
				<fo:region-end extent="0in" />
				<fo:region-body margin-top="0in" margin-bottom=".7in" margin-left="0in" margin-right="0in" />
			</fo:simple-page-master>
			-->
			<xsl:variable name="pageMasterToUse">
				<xsl:choose>
					<!--Do Not force a page break before for the very first item (i.e. value of 'auto')-->
					<xsl:when test="$orientation = 'landscape'">US-Letter-Landscape</xsl:when>
					<!--Alwauys force a page break before for all remaining items (i.e. value of 'page')-->
					<xsl:otherwise>US-Letter-Portrait</xsl:otherwise>
				</xsl:choose>
			</xsl:variable>
			
			<fo:page-sequence-master master-name="report-master">
				<fo:repeatable-page-master-alternatives maximum-repeats="no-limit">
					<fo:conditional-page-master-reference master-reference="{$pageMasterToUse}" page-position="first" />
					<!--<fo:conditional-page-master-reference master-reference="US-A-Portrait-Last" page-position="last" />-->
					<fo:conditional-page-master-reference master-reference="{$pageMasterToUse}" page-position="rest" />
				</fo:repeatable-page-master-alternatives>
			</fo:page-sequence-master>
			
		</fo:layout-master-set>
	
	</xsl:template>
	
	<!-- BBernard - 09/24/2012
		 NOTE: Generic Function wrap content so that it is ALWAYS kept together even through a Page Break, so all
				content flows to the second page without being split.
	-->
	<xsl:template name="keepBlockContentTogether">
		<xsl:param name="content" />
		<!-- KEEP THE CONTENT TOGETHER FOR CLEANER FORMATTING WHEN MULTIPLE PAGES ARE GENERATED
			Note:  http://fonet.codeplex.com/workitem/1552
			Keep-together is implemented in FO.NET. I use it heavily. It only works when set as an attribute
			on a table-row. To make use of this, one can create "blind tables," which are tables created solely
			for the keep-together.
			http://xmlgraphics.apache.org/fop/faq.html#keep-with
		-->
		<fo:table>
			<fo:table-column />
			<fo:table-body>
				<fo:table-row keep-together="always">
					<fo:table-cell>
						<xsl:copy-of select="$content" />
					</fo:table-cell>
				</fo:table-row>
			</fo:table-body>
		</fo:table>
	</xsl:template>

	<!--BBernard - 09/24/2012
		Generic Function to help with aligning content by wrapping in surrounding Layout Table with defined Width of center content.
		Supports parameters to easily generate the surrounding Table Markup for the specified content that needs to be aligned completely
		Left, Center, Right, etc.
		NOTE:  WAS NEVER NOT ABLE TO GET THIS dynamic centering template to work in all cases... in some cases you may still have to resort 
				to manual left shift of known spacing since tables are soo rigid and have defined widths, we usually know the width of the 
				page and width of the table so we can shift with padding of a block element.
	-->
	<xsl:template name="alignBlockContent">
		<xsl:param name="content" select="''" />
		<xsl:param name="align" select="'left'" />
		<xsl:param name="border" select="''" />
		<xsl:param name="contentWidth" select="concat($ContentWidth.Default.Inches, 'in')" />
		<!--
		<xsl:param name="totalWidth" select="$ContentWidth.Default.Inches" />
		<xsl:param name="widthUnits" select="in" />
		
		<xsl:variable name="remainderWidth" select="csharp:Max($totalWidth - $contentWidth, 0)" />
		<xsl:variable name="halfRemainderWidth" select="csharp:Max($remainderWidth div 2, 0)" />

		<xsl:comment>
			Centering Element
			Width Units = <xsl:value-of select="$widthUnits" />
			Total Width = <xsl:value-of select="$totalWidth"/>
			Content Width = <xsl:value-of select="$contentWidth"/>
			Remainder Width = <xsl:value-of select="$remainderWidth"/>
			Half Remainder Width = <xsl:value-of select="$halfRemainderWidth"/>
		</xsl:comment>
		-->
		
		<xsl:comment>WRAPPING CONTENT with Table to support Manual Alignment [<xsl:value-of select="$align" />].</xsl:comment>
		<fo:table table-layout="fixed">
		<!-- <fo:table table-layout="fixed" border="red 1px solid"> -->
			<xsl:choose>
				<xsl:when test="$align='center'">
				<!--
					<fo:table-column column-width="{$halfRemainderWidth}{$widthUnits}"/>
					<fo:table-column column-width="{$contentWidth}{$widthUnits}" />
					<fo:table-column column-width="{$halfRemainderWidth}{$widthUnits}"/>
				-->
					<fo:table-column column-width="proportional-column-width(1)" />
					<fo:table-column column-width="{$contentWidth}" />
					<fo:table-column column-width="proportional-column-width(1)" />
				</xsl:when>
				<xsl:when test="$align='left' or $align='start'">
					<fo:table-column column-width="{$contentWidth}" />
					<fo:table-column column-width="proportional-column-width(1)" />
				</xsl:when>
				<xsl:when test="$align='right' or $align='end'">
					<fo:table-column column-width="proportional-column-width(1)" />
					<fo:table-column column-width="{$contentWidth}" />
				</xsl:when>
			</xsl:choose>
			<fo:table-body>
				<fo:table-row>
					<xsl:choose>
						<xsl:when test="$align='center'">
							<fo:table-cell />
							<fo:table-cell border="{$border}">
								<xsl:copy-of select="$content" />
							</fo:table-cell>
							<fo:table-cell />
						</xsl:when>
						<xsl:when test="$align='left' or $align='start'">
							<fo:table-cell border="{$border}">
								<xsl:copy-of select="$content" />
							</fo:table-cell>
							<fo:table-cell />
						</xsl:when>
						<xsl:when test="$align='right' or $align='end'">
							<fo:table-cell />
							<fo:table-cell border="{$border}">
								<xsl:copy-of select="$content" />
							</fo:table-cell>
						</xsl:when>
					</xsl:choose>
				</fo:table-row>
			</fo:table-body>
		</fo:table>

	</xsl:template>

	<!-- BBernard - 09/24/2012
		 NOTE: Generic Function print a Message when there is No Data . . . helpful when the Xml data
				is empty and you do not want to render a plain blank Pdf, or risk error in Rendering.
	-->
		<xsl:template name="printNoDataMessage">
				<xsl:param name="messageContent"  />
				<xsl:param name="contentAlign" select="'center'" />
				<xsl:param name="border" select="''" />
				<xsl:param name="padding" select="'20px'" />
				<xsl:param name="contentWidth" select="'5in'" />
				<xsl:param name="foreColor" select="'white'" />
				<xsl:param name="backColor" select="$Color.Gray" />


				<xsl:call-template name="alignBlockContent">
						<xsl:with-param name="align" select="$contentAlign" />
						<xsl:with-param name="contentWidth" select="$contentWidth" />
						<xsl:with-param name="content">

								<fo:table>
										<fo:table-column column-width="{$contentWidth}" />
										<fo:table-body>
												<fo:table-row>
														<fo:table-cell border="{$border}">

																<fo:block background-color="{$backColor}" color="{$foreColor}" padding-top="{$padding}" padding-right="{$padding}" padding-bottom="{$padding}" padding-left="{$padding}">
																		<xsl:copy-of select="$messageContent" />
																</fo:block>

														</fo:table-cell>
												</fo:table-row>
										</fo:table-body>
								</fo:table>

						</xsl:with-param>
				</xsl:call-template>

		</xsl:template>


	<!-- BBernard - 09/24/2012 
		 Generate Layout tables that don't normally formatting themselves but are 
		 wrappers for layout such as side by side tables, 4 x 4 tables, etc.
		 NOTE: This renders a 2-Col x 1-Row table
	-->
	<xsl:template name="printLayoutTable-2x1">
		<xsl:param name="content_1x1" select="''" />
		<xsl:param name="contentWidth_1x1" select="'proportional-column-width(1)'" />
		<xsl:param name="content_2x1" select="''" />
		<xsl:param name="contentWidth_2x1" select="'proportional-column-width(1)'" />
		<xsl:param name="border" select="''" />
		
		<fo:table>
			<fo:table-column column-width="{$contentWidth_1x1}" />
			<fo:table-column column-width="{$contentWidth_2x1}" />
			<fo:table-body>
				<fo:table-row>
					<fo:table-cell border="{$border}">
						<fo:block>
							<xsl:copy-of select="$content_1x1" />
						</fo:block>
					</fo:table-cell>
					<fo:table-cell border="{$border}">
						<fo:block>
							<xsl:copy-of select="$content_2x1" />
						</fo:block>
					</fo:table-cell>
				</fo:table-row>
			</fo:table-body>
		</fo:table>
	
	</xsl:template>

	<!-- BBernard - 09/24/2012 
		 Function to duplicate the specified content over and over the specified number of times.
		 Helpful when generating lists, empty tables, repeating content, etc.
	-->
	<xsl:template name="duplicateContent">
		<xsl:param name="counter" select="0" />
		<xsl:param name="numberOfTimes" select="0" />
		<xsl:param name="content" />
		
		<xsl:if test="$counter &lt; $numberOfTimes">
			
			<xsl:copy-of select="$content" />
			
			<!-- Use Re-cursion to iterate and continue duplication -->
			<xsl:call-template name="duplicateContent">
				<xsl:with-param name="counter" select="$counter + 1"/>
				<xsl:with-param name="numberOfTimes" select="$numberOfTimes"/>
				<xsl:with-param name="content" select="$content"/>
			</xsl:call-template>
		</xsl:if>
		
	</xsl:template>
	
	<!-- BBernard - 09/24/2012 
		Generic function to merge all values of the provided node set into a single concatenated 
		String value using the specified separator string. 
	-->
	<xsl:template name="string-join">
		<xsl:param name="nodeSet" select="''" />
		<xsl:param name="separator" select="','" />

		<xsl:for-each select="$nodeSet">
			<xsl:if test="not(position() = 1)"><xsl:value-of select="$separator" /></xsl:if>
			<xsl:value-of select="."/> 
		</xsl:for-each>
	</xsl:template>	
	
</xsl:stylesheet>