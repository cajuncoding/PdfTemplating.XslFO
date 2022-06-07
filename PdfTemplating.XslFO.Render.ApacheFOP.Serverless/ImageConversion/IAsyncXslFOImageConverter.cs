using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PdfTemplating.XslFO.Render.ApacheFOP.Serverless
{
    /// <summary>
    /// BBernard
    /// Interface abstraction for Converting Images to PDF (e.g. using ApacheFOP Serverless)
    /// </summary>
    public interface IAsyncXslFOImageConverter
    {
        Task<ApacheFOPServerlessResponse> ConvertImageToPdfAsync(
            Uri imageUrl,
            ImageType imageType,
            PageSizeInfo pageSizeInfo,
            string imageWidth = ImageConversion.DefaultImageWidth, 
            string imageHeight = ImageConversion.DefaultImageHeight,
            ImageConversionPageLayout pageLayout = ImageConversionPageLayout.ImagePerPageCentered,
            ImageConversionPageOrientation pageOrientation = ImageConversionPageOrientation.DynamicallyOptimize
        );

        Task<ApacheFOPServerlessResponse> ConvertImageToPdfAsync(
            byte[] imageBytes,
            ImageType imageType,
            PageSizeInfo pageSizeInfo,
            string imageWidth = ImageConversion.DefaultImageWidth,
            string imageHeight = ImageConversion.DefaultImageHeight,
            ImageConversionPageLayout pageLayout = ImageConversionPageLayout.ImagePerPageCentered,
            ImageConversionPageOrientation pageOrientation = ImageConversionPageOrientation.DynamicallyOptimize
        );
    }
}
