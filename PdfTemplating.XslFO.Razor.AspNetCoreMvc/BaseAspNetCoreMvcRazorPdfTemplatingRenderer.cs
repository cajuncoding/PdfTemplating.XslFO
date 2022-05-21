/*
Copyright 2012 Brandon Bernard

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
using System;
using Microsoft.AspNetCore.Mvc;

namespace PdfTemplating.XslFO.Razor.AspNetCoreMvc
{
    public abstract class BaseAspNetCoreMvcRazorPdfTemplatingRenderer<TViewModel>
    {
        protected BaseAspNetCoreMvcRazorPdfTemplatingRenderer(String razorViewPath, Controller mvcController)
        {
            if (string.IsNullOrWhiteSpace(razorViewPath))
                throw new ArgumentNullException(nameof(razorViewPath), "The virtual path to the Razor is null/empty; a valid virtual path must be specified.");

            //NOTE: The Local FileInfo for the Razor View template/file will have it's Directory used
            //  as the BaseDirectory for resolving locally referenced files/images within the XSL-FO processing.
            this.RazorViewVirtualPath = razorViewPath;
            this.MvcController = mvcController;
        }

        /// <summary>
        /// The original VirtualPath to the Razor View File.
        /// NOTE: REQUIRED for underlying MVC ViewEngine to work as expected!
        /// </summary>
        public string RazorViewVirtualPath { get; protected set; }

        /// <summary>
        /// Returns a valid ControllerContext to use when executing the MVC Razor View; this is an 
        /// abstract method that must be implemented by inheriting classes.
        /// </summary>
        /// <returns></returns>
        public Controller MvcController { get; protected set; }
    }
}