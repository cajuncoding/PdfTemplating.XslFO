using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.CustomExtensions;
using System.IO;

namespace PdfTemplating.XslFO.Razor.AspNetCoreMvc
{
    public class RazorPdfTemplatingConfig
    {
        public const string VirtualPathRoot = "~";
            
        private static readonly ViewSearchPathList viewSearchPathsSingleton = new ViewSearchPathList();

        public static IImmutableList<string> ViewSearchPaths
            => viewSearchPathsSingleton.ViewSearchPaths;

        public static ViewSearchPathList ClearViewSearchPaths()
            => viewSearchPathsSingleton.ClearViewSearchPaths();

        public static ViewSearchPathList AddViewSearchPathAsTopPriority(string directoryPath)
            => viewSearchPathsSingleton.AddViewSearchPathAsTopPriority(directoryPath);

        public static ViewSearchPathList AddViewSearchPath(string directoryPath)
            => viewSearchPathsSingleton.AddViewSearchPath(directoryPath);
    }

    public class ViewSearchPathList
    {
        public ViewSearchPathList()
        {
            this.AddViewSearchPath(AppDomain.CurrentDomain.BaseDirectory)
                .AddViewSearchPath(Directory.GetCurrentDirectory());
        }

        protected readonly List<string> viewSearchPathList = new List<string>();

        public IImmutableList<string> ViewSearchPaths
            => viewSearchPathList.ToImmutableList();

        public ViewSearchPathList ClearViewSearchPaths()
        {
            viewSearchPathList.Clear();
            return this;
        }

        public ViewSearchPathList AddViewSearchPathAsTopPriority(string directoryPath)
        {
            AddViewSearchPathInternal(directoryPath, insertAtTop: true);
            return this;
        }

        public ViewSearchPathList AddViewSearchPath(string directoryPath)
        {
            AddViewSearchPathInternal(directoryPath, insertAtTop: false);
            return this;
        }

        protected virtual ViewSearchPathList AddViewSearchPathInternal(string directoryPath, bool insertAtTop)
        {
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                var path = NormalizePath(directoryPath);
                if (!viewSearchPathList.Contains(path))
                {
                    if (insertAtTop) viewSearchPathList.Insert(0, path);
                    else viewSearchPathList.Add(path);
                }
            }

            return this;
        }

        /// <summary>
        /// Normalize the Path for consistency and handle various use cases...
        /// Inspired by StackOverflow post here: https://stackoverflow.com/a/21058121/7293142
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected string NormalizePath(string path)
        {
            var sanitizedPath = new Uri(path).LocalPath.TrimEnd(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            return sanitizedPath;
        }
    }
}
