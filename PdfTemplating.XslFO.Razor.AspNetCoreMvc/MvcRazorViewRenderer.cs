using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace PdfTemplating.XslFO.Razor.AspNetCoreMvc
{
    /// <summary>
    /// Class that renders MVC views to a string using the standard MVC View Engine to render the view. 
    /// NOTE: Designed to be used within the context of AspNet Core MVC and requires that ASP.NET Controller & HttpContext is present to work.
    /// </summary>
    public class MvcRazorViewRenderer
    {
        /// <summary>
        /// Initializes the AspNetCoreMvcRazorViewRenderer with a specific Controller as Context.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public MvcRazorViewRenderer()
        {
        }

        /// <summary>
        /// Renders a full MVC view to a string. Will render with the full MVC View engine including running _ViewStart and merging into _Layout        
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="mvcController">The Controller handling/executing the current Request</param>
        /// <param name="model">The model to render the view with</param>
        /// <param name="throwExceptionIfViewNotFound"></param>
        /// <returns>String of the rendered view or null on error</returns>
        public virtual Task<TemplatedRenderResult> RenderViewAsync(string viewPath, Controller mvcController, object? model = null, bool throwExceptionIfViewNotFound = false)
            => RenderViewInternalAsync(viewPath, mvcController, model, false, throwExceptionIfViewNotFound);

        /// <summary>
        /// Renders a partial MVC view to string. Use this method to render a partial view that doesn't merge with _Layout and doesn't fire _ViewStart.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="mvcController">The Controller handling/executing the current Request</param>
        /// <param name="model">The model to pass to the viewRenderer</param>
        /// <param name="throwExceptionIfViewNotFound"></param>
        /// <returns>String of the rendered view or null on error</returns>
        public virtual Task<TemplatedRenderResult> RenderPartialViewAsync(string viewPath, Controller mvcController, object? model = null, bool throwExceptionIfViewNotFound = false)
            => RenderViewInternalAsync(viewPath, mvcController, model, true, throwExceptionIfViewNotFound);

        /// <summary>
        /// Renders a full MVC view to a writer. Will render with the full MVC View engine including running _ViewStart and merging into _Layout        
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="mvcController">The Controller handling/executing the current Request</param>
        /// <param name="model">The model to render the view with</param>
        /// <param name="writer"></param>
        /// <param name="throwExceptionIfViewNotFound"></param>
        /// <returns>String of the rendered view or null on error</returns>
        public virtual Task RenderViewToWriter(string viewPath, Controller mvcController, object model, TextWriter writer, bool throwExceptionIfViewNotFound = false)
            => RenderViewToWriterInternalAsync(viewPath, mvcController, writer, model, false, throwExceptionIfViewNotFound);

        /// <summary>
        /// Renders a partial MVC view to given Writer. Use this method to render a partial view that doesn't merge with _Layout and doesn't fire _ViewStart.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="mvcController">The Controller handling/executing the current Request</param>
        /// <param name="model">The model to pass to the viewRenderer</param>
        /// <param name="writer">Writer to render the view to</param>
        /// <param name="throwExceptionIfViewNotFound"></param>
        public virtual Task RenderPartialViewToWriter(string viewPath, Controller mvcController, object model, TextWriter writer, bool throwExceptionIfViewNotFound = false)
            => RenderViewToWriterInternalAsync(viewPath, mvcController, writer, model, true, throwExceptionIfViewNotFound);

        /// <summary>
        /// Refactored this logic out so that it is not duplicated and can be called independently allowing
        /// consuming code to use the same approach to detecting if Views exist; to support default View fallback logic
        /// when a specific view does not match!
        /// </summary>
        /// <param name="viewPath"></param>
        /// <param name="mvcController">The Controller handling/executing the current Request</param>
        /// <param name="isPartial"></param>
        /// <param name="throwExceptionIfViewNotFound"></param>
        /// <returns></returns>
        public virtual ViewEngineResult FindView(String viewPath, Controller mvcController, bool isPartial = false, bool throwExceptionIfViewNotFound = false)
        {
            var viewEngineResult = this.SearchForViewInternal(viewPath, mvcController, isPartial, throwExceptionIfViewNotFound);
            return viewEngineResult;
        }

        /// <summary>
        /// Support searching for Views from a specified list and returning the first valid view found.
        /// Used to provide generic fallback logic for a series of views that may or may not exist.
        /// Will raise an ArgumentException if no view is found.
        /// </summary>
        /// <param name="viewsToSearch"></param>
        /// <param name="mvcController">The Controller handling/executing the current Request</param>
        /// <param name="isPartial"></param>
        /// <param name="throwExceptionIfViewNotFound"></param>
        /// <returns></returns>
        public virtual ViewEngineResult FindFirstValidView(List<String> viewsToSearch, Controller mvcController, bool isPartial = false, bool throwExceptionIfViewNotFound = false)
        {
            //Search for the first valid view that we can find and return it if possible.
            var validResult = viewsToSearch.Select(v => SearchForViewInternal(v, mvcController, isPartial, throwExceptionIfViewNotFound)).FirstOrDefault(v => v.Success);
            return validResult ?? throw new ArgumentException($"No valid view could be found in the list [count={viewsToSearch.Count}] of views specified.");
        }

        /// <summary>
        /// Refactored this logic out so that it is not duplicated across multiple internal methods; keeping code dry'er for 
        /// retrieving the view and raising exception when not found.
        /// </summary>
        protected virtual ViewEngineResult SearchForViewInternal(String viewPath, Controller mvcController, bool isPartial = false, bool throwExceptionIfViewNotFound = false)
        {
            const string controllerErrorMessageSuffix = "The view renderer must run in the context of an ASP.NET Controller with access to HttpContext, ControllerContext & ViewData.";

            //We MUST ensure that HttpContext is valid for the internal methods that depend on it.
            //NOTE: This is required because AspNet requires use of HttpContext for mapping virtual paths,
            //      and creating default controller contexts, etc.
            if (mvcController == null)
                throw new ArgumentNullException(nameof(mvcController), $"Controller is null; {controllerErrorMessageSuffix}");

            var controllerContext = mvcController.ControllerContext
                ?? throw new ArgumentNullException(nameof(mvcController.HttpContext), $"Controller.ControllerContext is null; {controllerErrorMessageSuffix}");

            var httpContext = mvcController.HttpContext
                ?? throw new ArgumentNullException(nameof(mvcController.HttpContext), $"The Controller.HttpContext is null; {controllerErrorMessageSuffix}");

            //The GetRequiredService will ensure that the ViewEngine dependency is valid & not null!
            var viewEngine = httpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
            
            var viewName = Path.GetFileNameWithoutExtension(viewPath);

            //First Attempt to get/fined the view using the explicit search paths configured...
            var viewFindCommands = RazorPdfTemplatingConfig.ViewSearchPaths
                .Select(path => new Func<ViewEngineResult>(() => viewEngine.GetView(path, viewPath, !isPartial)))
                .ToList();

            //Then (as a fallback or if none defined) we Attempt to search for the View with built in search logic using Controller context data
            // such as: /Views/{ControllerName}, /View/Shared
            viewFindCommands.Add(() => viewEngine.FindView(controllerContext, viewName, !isPartial));

            var viewEngineResult = viewFindCommands.Select(c => c.Invoke()).FirstOrDefault(v => v.Success);

            if (throwExceptionIfViewNotFound && viewEngineResult?.View == null)
                throw new FileNotFoundException($"View could not be found for view path [{viewPath}].");

            return viewEngineResult!;
        }

        /// <summary>
        /// BBernard - 08/02/2016
        /// Updated to take in ViewEngineResult reference and re-use GetView() internal method for greater flexibility and code re-use.
        /// 
        /// Internal method that handles rendering of either partial or 
        /// or full views.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="viewEngineResult"></param>
        /// <param name="mvcController">The Controller handling/executing the current Request</param>
        /// <param name="model">Model to render the view with</param>
        /// <param name="isPartial">Determines whether to render a full or partial view</param>
        /// <param name="throwExceptionIfViewNotFound"></param>
        /// <returns>String of the rendered view</returns>
        protected virtual async Task<TemplatedRenderResult> RenderViewInternalAsync(string viewPath, Controller mvcController, object? model = null, bool isPartial = false, bool throwExceptionIfViewNotFound = false)
        {
            await using var stringWriter = new StringWriter();

            await RenderViewToWriterInternalAsync(viewPath, mvcController, stringWriter, model, isPartial, throwExceptionIfViewNotFound).ConfigureAwait(false);

            var result = stringWriter.ToString();
            
            var renderResult = new TemplatedRenderResult(result);
            return renderResult;
        }

        /// <summary>
        /// Internal method that handles rendering of either partial or 
        /// or full views.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">Model to render the view with</param>
        /// <param name="isPartial">Determines whether to render a full or partial view</param>
        /// <param name="mvcController">The Controller handling/executing the current Request</param>
        /// <param name="writer">Text writer to render view to</param>
        /// <param name="throwExceptionIfViewNotFound"></param>
        protected virtual async Task RenderViewToWriterInternalAsync(string viewPath, Controller mvcController, TextWriter writer, object? model = null, bool isPartial = false, bool throwExceptionIfViewNotFound = false)
        {
            var viewEngineResult = SearchForViewInternal(viewPath, mvcController, isPartial, throwExceptionIfViewNotFound);
            var view = viewEngineResult?.View;
            if (view != null)
            {
                mvcController.ViewData.Model = model;

                var viewContext = new ViewContext(
                    mvcController.ControllerContext,
                    view,
                    mvcController.ViewData,
                    mvcController.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await view.RenderAsync(viewContext).ConfigureAwait(false);
            }
        }
    }
}
