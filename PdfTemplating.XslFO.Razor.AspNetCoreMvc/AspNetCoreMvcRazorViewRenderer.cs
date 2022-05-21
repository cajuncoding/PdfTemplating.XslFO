using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    public class AspNetCoreMvcRazorViewRenderer
    {
        /// <summary>
        /// Required Controller Context
        /// </summary>
        protected Controller MvcController { get; set; }

        protected HttpContext HttpContext { get; set; }

        protected ICompositeViewEngine ViewEngine { get; set; }

        /// <summary>
        /// Initializes the AspNetCoreMvcRazorViewRenderer with a specific Controller as Context.
        /// </summary>
        /// <param name="controller"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AspNetCoreMvcRazorViewRenderer(Controller controller)
        {
            //We MUST ensure that HttpContext is valid for the internal methods that depend on it.
            //NOTE: This is required because AspNet requires use of HttpContext for mapping virtual paths,
            //      and creating default controller contexts, etc.
            this.MvcController = controller 
                ?? throw new ArgumentNullException(nameof(controller), "MVC Controller is null; The view renderer must run in the context of an ASP.NET Controller.");

            this.HttpContext = controller.HttpContext
                ?? throw new ArgumentNullException(nameof(controller.HttpContext),"Valid HttpContext is missing on the Controller.");

            this.ViewEngine = HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
        }

        /// <summary>
        /// Renders a full MVC view to a string. Will render with the full MVC View engine including running _ViewStart and merging into _Layout        
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to render the view with</param>
        /// <returns>String of the rendered view or null on error</returns>
        public virtual Task<TemplatedRenderResult> RenderViewAsync(string viewPath, object? model = null)
            => RenderViewInternalAsync(viewPath, model, false);

        /// <summary>
        /// Renders a partial MVC view to string. Use this method to render a partial view that doesn't merge with _Layout and doesn't fire _ViewStart.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to pass to the viewRenderer</param>
        /// <returns>String of the rendered view or null on error</returns>
        public virtual Task<TemplatedRenderResult> RenderPartialViewAsync(string viewPath, object? model = null)
            => RenderViewInternalAsync(viewPath, model, true);

        /// <summary>
        /// Renders a full MVC view to a writer. Will render with the full MVC View engine including running _ViewStart and merging into _Layout        
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to render the view with</param>
        /// <param name="writer"></param>
        /// <returns>String of the rendered view or null on error</returns>
        public virtual Task RenderViewToWriter(string viewPath, object model, TextWriter writer)
            => TryRenderViewToWriterInternalAsync(viewPath, writer, model, false);

        /// <summary>
        /// Renders a partial MVC view to given Writer. Use this method to render a partial view that doesn't merge with _Layout and doesn't fire _ViewStart.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to pass to the viewRenderer</param>
        /// <param name="writer">Writer to render the view to</param>
        public virtual Task RenderPartialViewToWriter(string viewPath, object model, TextWriter writer)
            => TryRenderViewToWriterInternalAsync(viewPath, writer, model, true);

        /// <summary>
        /// Refactored this logic out so that it is not duplicated and can be called independently allowing
        /// consuming code to use the same approach to detecting if Views exist; to support default View fallback logic
        /// when a specific view does not match!
        /// </summary>
        /// <param name="viewPath"></param>
        /// <param name="isPartial"></param>
        /// <returns></returns>
        public virtual ViewEngineResult FindView(String viewPath, bool isPartial = false)
        {
            var viewEngineResult = this.GetViewInternal(viewPath, isPartial, false);
            return viewEngineResult;
        }

        /// <summary>
        /// Support searching for Views from a specified list and returning the first valid view found.
        /// Used to provide generic fallback logic for a series of views that may or may not exist.
        /// Will raise an ArgumentException if no view is found.
        /// </summary>
        /// <param name="viewsToSearch"></param>
        /// <param name="isPartial"></param>
        /// <returns></returns>
        public virtual ViewEngineResult FindFirstValidView(List<String> viewsToSearch, bool isPartial = false)
        {
            //Search for the first valid view that we can find and return it if possible.
            var validResult = viewsToSearch.Select(v => GetViewInternal(v, isPartial, false)).FirstOrDefault(v => v.Success);
            return validResult ?? throw new ArgumentException($"No valid view could be found in the list [count={viewsToSearch.Count}] of views specified.");
        }

        /// <summary>
        /// Refactored this logic out so that it is not duplicated across multiple internal methods; keeping code dry'er for 
        /// retrieving the view and raising exception when not found.
        /// </summary>
        protected virtual ViewEngineResult GetViewInternal(String viewPath, bool isPartial = false, bool throwExceptionIfNotFound = false)
        {
            var rootPath = RazorPdfTemplating.WebAppRootPath;
            var viewName = Path.GetFileNameWithoutExtension(viewPath);

            var viewFindCommands = new Func<ViewEngineResult>[]
            {
                //Attempt to get the view using the explicit web root path and view path...
                () => ViewEngine.GetView(rootPath, viewPath, !isPartial),
                //Attempt to search for the View with built in search logic using Controller context data (e.g. /Views/{ControllerName, /View/Shared).
                () => ViewEngine.FindView(MvcController.ControllerContext, viewName, !isPartial)
            };

            var viewEngineResult = viewFindCommands.Select(c => c.Invoke()).FirstOrDefault(v => v.Success);

            if (throwExceptionIfNotFound && viewEngineResult?.View == null)
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
        /// <param name="model">Model to render the view with</param>
        /// <param name="isPartial">Determines whether to render a full or partial view</param>
        /// <returns>String of the rendered view</returns>
        protected virtual async Task<TemplatedRenderResult> RenderViewInternalAsync(string viewPath, object? model = null, bool isPartial = false)
        {
            await using var sw = new StringWriter();

            await TryRenderViewToWriterInternalAsync(viewPath, sw, model, isPartial).ConfigureAwait(false);

            var result = sw.ToString();
            
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
        /// <param name="writer">Text writer to render view to</param>
        protected virtual async Task<bool> TryRenderViewToWriterInternalAsync(string viewPath, TextWriter writer, object? model = null, bool isPartial = false)
        {
            var viewEngineResult = GetViewInternal(viewPath, isPartial);
            var view = viewEngineResult?.View;
            if (view == null) 
                return false;

            MvcController.ViewData.Model = model;

            ViewContext viewContext = new ViewContext(
                MvcController.ControllerContext,
                view,
                MvcController.ViewData,
                MvcController.TempData,
                writer,
                new HtmlHelperOptions()
            );

            await view.RenderAsync(viewContext).ConfigureAwait(false);
            return true;
        }
    }
}
