<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" exclude-result-prefixes=""
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:fo="http://www.w3.org/1999/XSL/Format"
	xmlns:csharp="urn:Xslt.System.Extensions">

	<xsl:import href="../Reports.Common.xsl" />
	<xsl:import href="Reports.Common.Movies.xsl" />
	<xsl:output method="xml" encoding="utf-8" indent="yes"/>

	<!-- Initialize the Xslt - this is the Xstl Entry Point/Main processing Selector
			NOTE: This almost always matches the Root Element
	-->
	<xsl:template match="/MovieSearchResponse">

		<!-- XslFO MUST HAVE A ROOT -->
		<fo:root>

			<!-- MASTER SETS MUST BE DEFINED BEFORE WE RENDER CONTENT
					NOTE: We use shared Template to print out the complex markup via simple params for 
					normal US Portrait/Landscape Page layouts 
			-->
			<xsl:call-template name="printLayoutMasterSet">
				<xsl:with-param name="headerHeight" select="'.25in'" />
				<xsl:with-param name="footerHeight" select="'.25in'" />
			</xsl:call-template>

			<!-- OUTPUT PAGE SEQUENCE
					NOTE: Use different Page Sequence Blocks if you need to Change Layout/Orientation of Pages 
			-->
			<fo:page-sequence master-reference="report-master">

				<!-- RENDER STATIC FOOTER REGION AS STATIC CONTENT; APPEARS ON EVERY PAGE-->
				<!-- NOTE:  FOOTER AND HEADERS MUST BE DEFINED BEFORE THE BODY FOR VALID XslFO PROCESSING -->
				<xsl:call-template name="printReportFooter">
					<xsl:with-param name="description" select="concat('Movie Search for [', ./SearchTitle, '] on [', $date_time_stamp, ']')" />
				</xsl:call-template>

				<!-- RENDER BODY REGION-->
				<fo:flow flow-name="xsl-region-body" font-size="{$FontSize.Default}" font-family="{$FontFamily.Default}">

					<!-- Root Block Element -->
					<fo:block font-size="{$FontSize.Medium}">

						<!-- Render the Cover Page from the root Node Match -->
						<xsl:apply-templates select="." mode="cover-page" />

						<!-- Render all Search Results by applying the Template that Matches 
							 NOTE: Each of the Search rendered items will be on it's own Page by including Page Breaks
							 NOTE: We Sort the order by Year Ascending and then apply the template to each item!
							 
						-->
						<xsl:for-each select="./SearchResults/SearchResult">
							<xsl:sort select="Year" data-type="number" order="ascending"/>
							<xsl:apply-templates select="." mode="default-mode" />
						</xsl:for-each>

						<!--OUTPUT REFERENCES FOR LAST PAGE - MUST APPEAR AFTER ALL CONTENT HAS BEEN RENDERED via TEMPLATES -->
						<fo:block>
							<fo:marker marker-class-name="Marker.LastPageFooterContent"></fo:marker>
						</fo:block>
						<fo:block id="ref-last-page"/>

					</fo:block>
				</fo:flow>

			</fo:page-sequence>
		</fo:root>
	</xsl:template>

	<!--******************************************************************************************************************************-->
	<!--***TEMPLATE: Create the Notes for Appraisers page for the Target Account -->
	<!--******************************************************************************************************************************-->
	<xsl:template match="SearchResult" mode="default-mode">
		<fo:block break-before="page">

			<!--Output Heading info-->
			<fo:block text-align="center">
				<fo:block font-size="{$FontSize.H1}" border-bottom="{$Border.HorizontalRule}">
					<xsl:value-of select="./Title"/>
				</fo:block>
			</fo:block>

			<fo:block font-size="{$FontSize.H3}">
				<fo:table width="100%">
					<fo:table-column column-width="proportional-column-width(1)" />
					<fo:table-column column-width="proportional-column-width(.75)" />
					<fo:table-body>
						<fo:table-row>
							<fo:table-cell padding="10px">

								<!-- Render Image -->
								<xsl:variable name="posterImageUrl" select="./Poster" />
								<xsl:choose>
									<xsl:when test="$posterImageUrl = ''">
										<fo:block font-size="{$FontSize.H1}">No Image</fo:block>
									</xsl:when>
									<xsl:otherwise>
										<fo:block>

											<xsl:choose>
												<xsl:when test="//FonetCompatibilityEnabled = 'true'">
													<!-- Render External graphic without the content-width attribute; scaling is not configurable in legacy Fonet -->
													<fo:external-graphic src="url('{$posterImageUrl}')" width="3in" scaling="uniform" />
												</xsl:when>
												<xsl:otherwise>
													<fo:external-graphic src="url('{$posterImageUrl}')" width="3in" scaling="uniform" content-width="scale-to-fit" />
												</xsl:otherwise>
											</xsl:choose>
										</fo:block>
									</xsl:otherwise>
								</xsl:choose>

							</fo:table-cell>
							<fo:table-cell padding="10px">

								<!-- Render Fields/Info-->
								<fo:block space-before="{$Paragraph.SpaceBefore}">
									Title: <xsl:value-of select="./Title"/>
								</fo:block>
								<fo:block space-before="{$Paragraph.SpaceBefore}">
									Year: <xsl:value-of select="./Year"/>
								</fo:block>
								<fo:block space-before="{$Paragraph.SpaceBefore}">
									IMDB ID: <xsl:value-of select="./ImdbID"/>
								</fo:block>

							</fo:table-cell>
						</fo:table-row>
					</fo:table-body>
				</fo:table>
			</fo:block>

		</fo:block>
	</xsl:template>

	<!--******************************************************************************************************************************-->
	<!--***TEMPLATE: Create the Cover Page for the Target Account -->
	<!--******************************************************************************************************************************-->
	<xsl:template match="MovieSearchResponse" mode="cover-page">

		<fo:block break-before="auto">

			<!-- Vertical Alignment requires a full sized width/height Table with 1 Cell: https://sites.cs.ucsb.edu/~pconrad/github/ucsb-cs56-tutorials-fop/fop-1.1/docs/fo.html#fo-center-vertical -->
			<fo:table table-layout="fixed" width="100%">
				<fo:table-column column-width="proportional-column-width(1)"/>
				<fo:table-body>
					<fo:table-row height="8.5in">
						<fo:table-cell display-align="center">

							<!--Output Title Page info-->
							<fo:block text-align="center">
								<!-- Generate a Horizontal Rule -->
								<fo:block font-size="{$FontSize.H1}" border-bottom="{$Border.HorizontalRule}"></fo:block>

								<fo:block font-size="{$FontSize.H1}" space-before=".5in">
									Movie Search for "<xsl:value-of select="./SearchTitle" />"
								</fo:block>
								<fo:block font-size="{$FontSize.H2}" space-before=".5in">
									BY: Brandon Bernard
								</fo:block>
								<fo:block font-size="{$FontSize.H1}" space-before=".5in">
									<!-- Use the DateTime stamp that is already taken & formatted in Reports.Common initialization! -->
									Executed on: <xsl:value-of select="$date_time_stamp" />
								</fo:block>

								<!-- Generate a Horizontal Rule -->
								<fo:block space-before=".5in" border-bottom="{$Border.HorizontalRule}"></fo:block>
							</fo:block>

						</fo:table-cell>
					</fo:table-row>
				</fo:table-body>
			</fo:table>

		</fo:block>
	</xsl:template>


	<!--******************************************************************************************************************************-->
	<!--***UTILITY/HELPER FUNCTIONS for re-usable outputs -->
	<!--******************************************************************************************************************************-->
	<xsl:template name="printReportFooter">
		<xsl:param name="description" select="''" />

		<!-- RENDER FOOTER REGION AS STATIC CONTENT; APPEARS ON EVERY PAGE-->
		<!-- NOTE:  FOOTER AND HEADERS MUST BE DEFINED BEFORE THE BODY FOR VALID XslFO PROCESSING -->
		<fo:static-content flow-name="xsl-region-after"  font-family="{$FontFamily.Footer}" font-size="{$FontSize.Footer}">
			<fo:block padding-top="0px" color="{$Color.Gray}">
				<!-- <fo:table width="100%" border="blue 1px solid"> -->
				<fo:table width="100%">
					<fo:table-column column-width="proportional-column-width(1)" />
					<fo:table-column column-width="proportional-column-width(2)" />
					<fo:table-body>
						<fo:table-row>
							<fo:table-cell>
								<fo:block text-align="left" padding-before="3px">
									<fo:block>
										Page <fo:page-number/> of <fo:page-number-citation ref-id="ref-last-page"/>
									</fo:block>
								</fo:block>
							</fo:table-cell>
							<fo:table-cell padding-before="3px">
								<fo:block text-align="right">
									<!--<fo:block space-before="3px">Report Generated By <xsl:value-of select="$appName" /> at <xsl:value-of select="$date_time_stamp" /></fo:block>-->
									<!--<fo:block space-before="3px">Report Generated on <xsl:value-of select="$date_time_stamp" /></fo:block>-->
									<fo:block space-before="3px"><xsl:value-of select="$description" /></fo:block>
								</fo:block>
							</fo:table-cell>
						</fo:table-row>
					</fo:table-body>
				</fo:table>
			</fo:block>
		</fo:static-content>
	</xsl:template>

</xsl:stylesheet>