
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PdfTemplating.XslFO.Razor.AspNetMvc
{
    /// <summary>
    /// BBernard - 08/02/2016
    /// NOTE: Using ViewRenderer class from Rick Strahl's Westwind Toolkit located at:
    ///         https://github.com/RickStrahl/WestwindToolkit/blob/master/Westwind.Web.Mvc/Utils/ViewRenderer.cs
    ///       Based on Rick Strah's Blog on how he created the ViewRenderer here:
    ///         https://weblog.west-wind.com/posts/2012/may/30/rendering-aspnet-mvc-views-to-string
    ///       BUT MANY ENHANCEMENTS and elimination of duplicate code updates have been made . . .        
    /// 
    /// NOTE:  A NUMBER of enhancements have been added to this class to eliminate duplicate code (make dry'er),
    ///         increase flexibility, and provide for greater functionality!
    /// NOTE: Static methods have been removed to encourage proper instantiation with Constructor DI, and
    ///         better support for future implementations (e.g AspNET, .NetCore, etc.)
    /// 
    /// Class that renders MVC views to a string using the
    /// standard MVC View Engine to render the view. 
    /// 
    /// NOTE: Requires that ASP.NET HttpContext is present to work, but works outside of the context of MVC
    ///         Therefore this project must be a .Net Framework Library as .Net Core uses a very different paradigm, and
    ///         requires a different implementation.
    /// </summary>
    public class AspNetMvcRazorViewRenderer
    {
        /// <summary>
        /// Required Controller Context
        /// </summary>
        protected ControllerContext Context { get; set; }

        /// <summary>
        /// Initializes the ViewRenderer with a Context.
        /// </summary>
        /// <param name="controllerContext">
        /// If you are running within the context of an ASP.NET MVC request pass in
        /// the controller's context. 
        /// Only leave out the context if no context is otherwise available.
        /// </param>
        public AspNetMvcRazorViewRenderer(ControllerContext controllerContext = null)
        {
            //We MUST ensure that HttpContext is valid for the internal methods that depend on it.
            //NOTE: This is required because AspNet requires use of HttpContext for mapping virtual paths,
            //      and creating default controller contexts, etc.
            if (HttpContext.Current == null) throw new InvalidOperationException(
                "ViewRenderer must run in the context of an ASP.NET (.Net Framework) Application and requires HttpContext.Current to be present;"
                + " cannot create ControllerContext if no active HttpContext instance is available."
            );

            this.Context = controllerContext ?? CreateController<EmptyController>().ControllerContext;
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
        public virtual TemplatedRenderResult RenderView(string viewPath, object model = null)
        {
            // BBernard - 08/02/2016
            // Updated to take in ViewEngineResult reference and re-use GetView() internal method for greater flexibility and code re-use.
            return RenderViewInternal(viewPath, model, false);
        }

        /// <summary>
        /// Renders a partial MVC view to string. Use this method to render a partial view that doesn't merge with _Layout and doesn't fire
        /// _ViewStart.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to pass to the viewRenderer</param>
        /// <returns>String of the rendered view or null on error</returns>
        public virtual TemplatedRenderResult RenderPartialView(string viewPath, object model = null)
        {
            // BBernard - 08/02/2016
            // Updated to take in ViewEngineResult reference and re-use GetView() internal method for greater flexibility and code re-use.
            return RenderViewInternal(viewPath, model, true);
        }

        /// <summary>
        /// Renders a full MVC view to a writer. Will render with the full MVC View engine including running _ViewStart and merging into _Layout        
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to render the view with</param>
        /// <returns>String of the rendered view or null on error</returns>
        public virtual void RenderViewToWriter(string viewPath, object model, TextWriter writer)
        {
            // BBernard - 08/02/2016
            // Updated to take in ViewEngineResult reference and re-use GetView() internal method for greater flexibility and code re-use.
            RenderViewToWriterInternal(viewPath, writer, model, false);
        }

        /// <summary>
        /// Renders a partial MVC view to given Writer. Use this method to render a partial view that doesn't merge with _Layout and doesn't fire
        /// _ViewStart.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to pass to the viewRenderer</param>
        /// <param name="writer">Writer to render the view to</param>
        public virtual void RenderPartialViewToWriter(string viewPath, object model, TextWriter writer)
        {
            // BBernard - 08/02/2016
            // Updated to take in ViewEngineResult reference and re-use GetView() internal method for greater flexibility and code re-use.
            RenderViewToWriterInternal(viewPath, writer, model, true);
        }

        /// <summary>
        /// BBernard - 08/02/2016
        /// Refactored this logic out so that it is not duplicated and can be called independently allowing
        /// consuming code to use the same approach to detecting if Views exist; to support default View fallback logic
        /// when a specific view does not match!
        /// </summary>
        /// <param name="viewPath"></param>
        /// <param name="partial"></param>
        /// <returns></returns>
        public virtual ViewEngineResult FindView(String viewPath, bool partial = false)
        {
            // first find the ViewEngine for this view
            var viewEngineResult = this.GetViewInternal(viewPath, partial);
            return viewEngineResult;
        }

        /// <summary>
        /// BBernard - 08/02/2016
        /// Support searching for Views from a specified list and returning the first valid view found.
        /// Used to provide generic fallback logic for a series of views that may or may not exist.
        /// </summary>
        /// <param name="viewsToSearch"></param>
        /// <param name="partial"></param>
        /// <returns></returns>
        public virtual ViewEngineResult FindFirstValidView(List<String> viewsToSearch, bool partial = false)
        {
            //Search for the first valid view that we can find and return it if possible.
            var validResult = viewsToSearch.Select(v => this.FindView(v, partial)).FirstOrDefault();

            //Raise an exception if no valid view can be found.
            return validResult ?? throw new ArgumentNullException($"No valid view could be found in the list [count={viewsToSearch.Count}] of views specified.");
        }

        /// <summary>
        /// BBernard - 08/02/2016
        /// Refactored this logic out so that it is not duplicated across multiple internal methods; keeping code dry'er for 
        /// retrieving the view and raising exception when not found.
        /// </summary>
        protected virtual ViewEngineResult GetViewInternal(String viewPath, bool partial = false)
        {
            //BBernard - 08/02/2016
            //NOTE:  Updated to use the new FindView method factored out for re-use.
            // get the view and attach the model to view data
            var viewEngineResult = partial
                ? ViewEngines.Engines.FindPartialView(Context, viewPath)
                : ViewEngines.Engines.FindView(Context, viewPath, null);

            //BBernard - 08/02/2016
            //NOTE:  Updated to remove dependency on project resource for this simple error message 
            //       and added additional details in the error message.
            if (viewEngineResult?.View == null)
                throw new FileNotFoundException($"View could not be found for view path [{viewPath}].");

            return viewEngineResult;
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
        /// <param name="partial">Determines whether to render a full or partial view</param>
        /// <returns>String of the rendered view</returns>
        protected virtual TemplatedRenderResult RenderViewInternal(string viewPath, object model, bool partial = false)
        {
            ////Get the physical mapped path for the Virtual Path...
            ////NOTE: This is needed to dynamically determine the parent Directory of the Physical View file to use
            ////      as context for relative path searches when rendering he Xsl-FO (e.g. Images, etc.)
            //var razorViewFilePath = HttpContext.Current.Server.MapPath(viewPath);
            //var razorViewFileInfo = new FileInfo(razorViewFilePath);

            //BBernard - 08/02/2016
            //Re-factored this code to remove duplicate logic and call the existing RenderViewToWriterInternal method which
            //already used a TextWriter for which our StringWriter inherits from; eliminating duplicated code logic.
            string result = null;
            using (var sw = new StringWriter())
            {
                RenderViewToWriterInternal(viewPath, sw, model, partial);
                result = sw.ToString();
            }

            //Return a RenderResult response object...
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
        /// <param name="partial">Determines whether to render a full or partial view</param>
        /// <param name="writer">Text writer to render view to</param>
        protected virtual void RenderViewToWriterInternal(string viewPath, TextWriter writer, object model = null, bool partial = false)
        {
            var viewEngineResult = GetViewInternal(viewPath, partial);
            
            //BBernard - 08/02/2016
            //NOTE:  Updated to remove dependency on project resource for this simple error message 
            //       and added additional details in the error message.
            //Get the view and attach the model to view data
            var view = viewEngineResult?.View
                ?? throw new ArgumentNullException("Rendering cannot completed because the ViewEngineResult is null; a valid ViewEngineResult must be specified.");

            Context.Controller.ViewData.Model = model;
            var controller = Context.Controller;

            var ctx = new ViewContext(
                Context, 
                view,
                controller.ViewData,
                controller.TempData,
                writer
            );
            
            view.Render(ctx, writer);
        }

        /// <summary>
        /// Creates an instance of an MVC controller from scratch when no existing ControllerContext is present       
        /// </summary>
        /// <typeparam name="T">Type of the controller to create</typeparam>
        /// <returns>Controller for T</returns>
        /// <exception cref="InvalidOperationException">thrown if HttpContext not available</exception>
        protected T CreateController<T>(RouteData routeData = null, params object[] parameters) where T : Controller, new()
        {
            // create a disconnected controller instance
            var controller = (T)Activator.CreateInstance(typeof(T), parameters);

            var httpContextWrapper = new HttpContextWrapper(HttpContext.Current);
            var validRouteData = routeData ?? new RouteData();

            // add the controller routing if not existing
            if (!validRouteData.Values.ContainsKey("controller") && !validRouteData.Values.ContainsKey("Controller"))
            {
                validRouteData.Values.Add("controller", controller.GetType().Name.ToLower().Replace("controller", ""));
            }

            controller.ControllerContext = new ControllerContext(httpContextWrapper, validRouteData, controller);
            return controller;
        }
    }

    /// <summary>
    /// Empty MVC Controller instance used to instantiate and provide a new ControllerContext for the ViewRenderer
    /// </summary>
    public class EmptyController : Controller
    {
    }
}
