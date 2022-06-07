using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace PdfTemplating.XslFO.Render.ApacheFOP.Serverless
{

    public class ImageConversion
    {
        public const string DefaultImageWidth = "100%";
        public const string DefaultImageHeight = "100%";
        public const string DefaultPageMargin = ".25in";
        public const string DefaultPageMarginImperial = "6mm";
    }

    public enum ImageType
    {
        [Description("image/bmp")]
        Bmp,
        [Description("image/gif")]
        Gif,
        [Description("image/jpeg")]
        Jpeg,
        [Description("image/png")]
        Png,
        [Description("image/svg+xml")]
        Svg,
        [Description("image/tiff")]
        Tiff
    }

    public enum ImageConversionPageLayout
    {
        ImagesInFlowSequence,
        ImagePerPage,
        ImagePerPageCentered
    }

    public enum ImageConversionPageOrientation
    {
        Default,
        Portrait,
        Landscape,
        DynamicallyOptimize
    }

    public static class ImageConversionCustomExtensions
    {
        private static readonly ILookup<ImageType, DescriptionAttribute> ImageDescriptionLookupCache = Enum.GetValues(typeof(ImageType)).Cast<ImageType>().ToLookup(
            e => (ImageType)e,
            e => typeof(ImageType).GetField(e.ToString()).GetCustomAttribute<DescriptionAttribute>()
        );

        public static string GetDescription(this ImageType enumValue)
        {
            return ImageDescriptionLookupCache[enumValue].FirstOrDefault()?.Description;
        }
    }
}
