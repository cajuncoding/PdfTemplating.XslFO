using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace PdfTemplating.XslFO.Render.ApacheFOP.Serverless
{
    public class ImageConversionXslFOBuilder
    {
        public string CreateImageSrcContent(Uri imageUri)
        {
            return $@"url('{SecurityElement.Escape(imageUri.ToString())}')";
        }

        public string CreateImageSrcContent(byte[] imageBytes, ImageType imageType)
        {
            var base64Data = Convert.ToBase64String(imageBytes);
            var xmlEncodedBase64Data = SecurityElement.Escape(base64Data);
            var imageMimeType = imageType.GetDescription();
            var imageSrcContent = $@"data:{imageMimeType};base64,{xmlEncodedBase64Data}";
            return imageSrcContent;
        }

        public string CreateImageConversionXslFOContentInternal(
            string imageSrcContent,
            PageSizeInfo pageSizeInfo,
            string imageWidth = ImageConversion.DefaultImageWidth,
            string imageHeight = ImageConversion.DefaultImageHeight,
            ImageConversionPageLayout pageLayout = ImageConversionPageLayout.ImagePerPageCentered,
            ImageConversionPageOrientation pageOrientation = ImageConversionPageOrientation.DynamicallyOptimize
        )
        {
            string breakBeforeMarkup = string.Empty;
            if (pageLayout == ImageConversionPageLayout.ImagePerPage || pageLayout == ImageConversionPageLayout.ImagePerPageCentered)
            {
                breakBeforeMarkup = @"break-before=""page""";
            }

            string imageBlockContainerCenteringMarkup = string.Empty;
            if (pageLayout == ImageConversionPageLayout.ImagePerPageCentered)
            {
                imageBlockContainerCenteringMarkup = @"height=""100%"" width=""100%"" display-align=""center"" text-align=""center""";
            }
            
            //TODO: Add Support for customized Scaling, etc.
            var xslfoContent = $@"
                <fo:root xmlns:fo=""http://www.w3.org/1999/XSL/Format"">
                    <fo:layout-master-set>
                        <fo:simple-page-master master-name=""Image-Portrait"" page-width=""{pageSizeInfo.Width}"" page-height=""{pageSizeInfo.Height}"" 
                            margin-top=""{pageSizeInfo.MarginTop}"" margin-bottom=""{pageSizeInfo.MarginBottom}"" margin-left=""{pageSizeInfo.MarginLeft}"" margin-right=""{pageSizeInfo.MarginRight}"">
                            <fo:region-body />
                        </fo:simple-page-master>
                        <fo:simple-page-master master-name=""Image-Landscape"" page-width=""{pageSizeInfo.Height}"" page-height=""{pageSizeInfo.Width}""
                            margin-top=""{pageSizeInfo.MarginTop}"" margin-bottom=""{pageSizeInfo.MarginBottom}"" margin-left=""{pageSizeInfo.MarginLeft}"" margin-right=""{pageSizeInfo.MarginRight}"">
                            <fo:region-body />
                        </fo:simple-page-master>
                    </fo:layout-master-set>
                    <fo:page-sequence master-reference=""Image-Portrait"">
                        <fo:flow flow-name=""xsl-region-body"" font-size=""10pt"">
                            <fo:block-container {breakBeforeMarkup} {imageBlockContainerCenteringMarkup}>
                                <fo:block>
                                    <fo:external-graphic width=""{imageWidth}"" height=""{imageHeight}"" scaling=""uniform"" content-width=""scale-to-fit"" src=""{imageSrcContent}"" />
                                </fo:block>
                            </fo:block-container>
                        </fo:flow>
                    </fo:page-sequence>
                </fo:root>
            ";

            return xslfoContent;
        }
    }
}
