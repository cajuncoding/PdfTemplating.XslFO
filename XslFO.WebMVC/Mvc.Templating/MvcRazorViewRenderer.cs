
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVC.Templating
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
    /// 
    /// Class that renders MVC views to a string using the
    /// standard MVC View Engine to render the view. 
    /// 
    /// Requires that ASP.NET HttpContext is present to
    /// work, but works outside of the context of MVC
    /// </summary>
    public class MvcRazorViewRenderer
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
        public MvcRazorViewRenderer(ControllerContext controllerContext = null)
        {
            // Create a known controller from HttpContext if no context is passed
            if (controllerContext == null)
            {
                if (HttpContext.Current != null)
                    controllerContext = CreateController<EmptyController>().ControllerContext;
                else
                    throw new InvalidOperationException(
                        "ViewRenderer must run in the context of an ASP.NET " +
                        "Application and requires HttpContext.Current to be present.");
            }
            Context = controllerContext;
        }

        /// <summary>
        /// BBernard - 08/02/2016
        /// Updated to take in ViewEngineResult reference and re-use GetView() internal method for greater flexibility and code re-use.
        /// 
        /// Renders a full MVC view to a string. Will render with the full MVC
        /// View engine including running _ViewStart and merging into _Layout        
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to render the view with</param>
        /// <returns>String of the rendered view or null on error</returns>
        public string RenderViewToString(ViewEngineResult viewEngineResult, object model = null)
        {
            return RenderViewToStringInternal(viewEngineResult, model, false);
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
        public string RenderViewToString(string viewPath, object model = null)
        {
            /// BBernard - 08/02/2016
            /// Updated to take in ViewEngineResult reference and re-use GetView() internal method for greater flexibility and code re-use.
            return RenderViewToStringInternal(GetViewInternal(viewPath), model, false);
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
        public void RenderView(string viewPath, object model, TextWriter writer)
        {
            /// BBernard - 08/02/2016
            /// Updated to take in ViewEngineResult reference and re-use GetView() internal method for greater flexibility and code re-use.
            RenderViewToWriterInternal(GetViewInternal(viewPath), writer, model, false);
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
        public string RenderPartialViewToString(string viewPath, object model = null)
        {
            /// BBernard - 08/02/2016
            /// Updated to take in ViewEngineResult reference and re-use GetView() internal method for greater flexibility and code re-use.
            return RenderViewToStringInternal(GetViewInternal(viewPath), model, true);
        }

        /// <summary>
        /// BBernard - 08/02/2016
        /// Updated to take in ViewEngineResult reference and re-use GetView() internal method for greater flexibility and code re-use.
        /// 
        /// Renders a partial MVC view to string. Use this method to render
        /// a partial view that doesn't merge with _Layout and doesn't fire
        /// _ViewStart.
        /// </summary>
        /// <param name="viewEngineResult">
        /// The valid non-null ViewEngineResult to use for Rendering.
        /// </param>
        /// <param name="model">The model to pass to the viewRenderer</param>
        /// <returns>String of the rendered view or null on error</returns>
        public string RenderPartialViewToString(ViewEngineResult viewEngineResult, object model = null)
        {
            /// BBernard - 08/02/2016
            /// Updated to take in ViewEngineResult reference and re-use GetView() internal method for greater flexibility and code re-use.
            return RenderViewToStringInternal(viewEngineResult, model, true);
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
        public void RenderPartialView(string viewPath, object model, TextWriter writer)
        {
            /// BBernard - 08/02/2016
            /// Updated to take in ViewEngineResult reference and re-use GetView() internal method for greater flexibility and code re-use.
            RenderViewToWriterInternal(GetViewInternal(viewPath), writer, model, true);
        }

        /// <summary>
        /// Renders a partial MVC view to string. Use this method to render a partial view that doesn't merge with _Layout and doesn't fire _ViewStart.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to pass to the viewRenderer</param>
        /// <param name="controllerContext">Active Controller context</param>
        /// <returns>String of the rendered view or null on error</returns>
        public static string RenderView(string viewPath, object model = null, ControllerContext controllerContext = null)
        {
            MvcRazorViewRenderer renderer = new MvcRazorViewRenderer(controllerContext);
            return renderer.RenderViewToString(viewPath, model);
        }

        /// <summary>
        /// Renders a partial MVC view to the given writer. Use this method to render a partial view that doesn't merge with _Layout and doesn't fire _ViewStart.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to pass to the viewRenderer</param>
        /// <param name="writer">Writer to render the view to</param>
        /// <param name="controllerContext">Active Controller context</param>
        /// <returns>String of the rendered view or null on error</returns>
        public static void RenderView(string viewPath, TextWriter writer, object model, ControllerContext controllerContext)
        {
            MvcRazorViewRenderer renderer = new MvcRazorViewRenderer(controllerContext);
            renderer.RenderView(viewPath, model, writer);
        }

        /// <summary>
        /// Renders a partial MVC view to string. Use this method to render a partial view that doesn't merge with _Layout and doesn't fire _ViewStart.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to pass to the viewRenderer</param>
        /// <param name="controllerContext">Active Controller context</param>
        /// <param name="errorMessage">optional out parameter that captures an error message instead of throwing</param>
        /// <returns>String of the rendered view or null on error</returns>
        public static string RenderView(string viewPath, object model, ControllerContext controllerContext, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                MvcRazorViewRenderer renderer = new MvcRazorViewRenderer(controllerContext);
                return renderer.RenderViewToString(viewPath, model);
            }
            catch (Exception ex)
            {
                errorMessage = ex.GetBaseException().Message;
            }
            return null;
        }

        /// <summary>
        /// Renders a partial MVC view to the given writer. Use this method to render a partial view that doesn't merge with _Layout and doesn't fire _ViewStart.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to pass to the viewRenderer</param>
        /// <param name="controllerContext">Active Controller context</param>
        /// <param name="writer">Writer to render the view to</param>
        /// <param name="errorMessage">optional out parameter that captures an error message instead of throwing</param>
        /// <returns>String of the rendered view or null on error</returns>
        public static void RenderView(string viewPath, object model, TextWriter writer, ControllerContext controllerContext, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                MvcRazorViewRenderer renderer = new MvcRazorViewRenderer(controllerContext);
                renderer.RenderView(viewPath, model, writer);
            }
            catch (Exception ex)
            {
                errorMessage = ex.GetBaseException().Message;
            }
        }


        /// <summary>
        /// Renders a partial MVC view to string. Use this method to render a partial view that doesn't merge with _Layout and doesn't fire _ViewStart.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to pass to the viewRenderer</param>
        /// <param name="controllerContext">Active controller context</param>
        /// <returns>String of the rendered view or null on error</returns>
        public static string RenderPartialView(string viewPath, object model = null, ControllerContext controllerContext = null)
        {
            MvcRazorViewRenderer renderer = new MvcRazorViewRenderer(controllerContext);
            return renderer.RenderPartialViewToString(viewPath, model);
        }

        /// <summary>
        /// Renders a partial MVC view to string. Use this method to render a partial view that doesn't merge with _Layout and doesn't fire _ViewStart.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to pass to the viewRenderer</param>
        /// <param name="controllerContext">Active controller context</param>
        /// <param name="writer">Text writer to render view to</param>
        /// <param name="errorMessage">optional output parameter to receive an error message on failure</param>
        public static void RenderPartialView(string viewPath, TextWriter writer, object model = null, ControllerContext controllerContext = null)
        {
            MvcRazorViewRenderer renderer = new MvcRazorViewRenderer(controllerContext);
            renderer.RenderPartialView(viewPath, model, writer);
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
        public ViewEngineResult FindView(String viewPath, bool partial = false)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            if (partial)
            {
                viewEngineResult = ViewEngines.Engines.FindPartialView(Context, viewPath);
            }
            else
            {
                viewEngineResult = ViewEngines.Engines.FindView(Context, viewPath, null);
            }

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
        public ViewEngineResult FindFirstValidView(List<String> viewsToSearch, bool partial = false)
        {
            //Search for the first valid view that we can find and return it if possible.
            foreach(var viewPath in viewsToSearch)
            {
                ViewEngineResult viewEngineResult = this.FindView(viewPath, partial);
                if (viewEngineResult != null && viewEngineResult.View != null)
                {
                    return viewEngineResult;
                }
            }

            //Raise an exception if no valid view can be found.
            throw new ArgumentNullException(String.Format("No valid view could be found in the list [count={0}] of views specified.", viewsToSearch.Count));
        }

        /// <summary>
        /// BBernard - 08/02/2016
        /// Refactored this logic out so that it is not duplicated across multiple internal methods; keeping code dry'er for 
        /// retrieving the view and raising exception when not found.
        /// </summary>
        private ViewEngineResult GetViewInternal(String viewPath, bool partial = false)
        {
            //BBernard - 08/02/2016
            //NOTE:  Updated to use the new FindView method factored out for re-use.
            // get the view and attach the model to view data
            var viewEngineResult = this.FindView(viewPath, partial);

            //BBernard - 08/02/2016
            //NOTE:  Updated to remove dependency on project resource for this simple error message 
            //       and added additional details in the error message.
            if (viewEngineResult == null || viewEngineResult.View == null)
                throw new FileNotFoundException(String.Format("View could not be found for view path [{0}].", viewPath));

            return viewEngineResult;
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
        protected void RenderViewToWriterInternal(ViewEngineResult viewEngineResult, TextWriter writer, object model = null, bool partial = false)
        {
            //BBernard - 08/02/2016
            //NOTE:  Updated to remove dependency on project resource for this simple error message 
            //       and added additional details in the error message.
            if (viewEngineResult == null || viewEngineResult.View == null)
                throw new ArgumentNullException(String.Format("Rendering cannot completed because the ViewEngineResult is null; a valid ViewEngineResult must be specified."));

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            Context.Controller.ViewData.Model = model;

            var ctx = new ViewContext(Context, view,
                                        Context.Controller.ViewData,
                                        Context.Controller.TempData,
                                        writer);
            view.Render(ctx, writer);
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
        /// <param name="model">Model to render the view with</param>
        /// <param name="partial">Determines whether to render a full or partial view</param>
        /// <returns>String of the rendered view</returns>
        private string RenderViewToStringInternal(ViewEngineResult viewEngineResult, object model, bool partial = false)
        {
            //BBernard - 08/02/2016
            //Re-factored this code to remove duplicate logic and call the existing RenderViewToWriterInternal method which
            //already used a TextWriter for which our StringWriter inherits from; eliminating duplicated code logic.
            string result = null;
            using (var sw = new StringWriter())
            {
                RenderViewToWriterInternal(viewEngineResult, sw, model, partial);
                result = sw.ToString();
            }

            return result;
        }


        /// <summary>
        /// Creates an instance of an MVC controller from scratch when no existing ControllerContext is present       
        /// </summary>
        /// <typeparam name="T">Type of the controller to create</typeparam>
        /// <returns>Controller for T</returns>
        /// <exception cref="InvalidOperationException">thrown if HttpContext not available</exception>
        public static T CreateController<T>(RouteData routeData = null, params object[] parameters)
                    where T : Controller, new()
        {
            // create a disconnected controller instance
            T controller = (T)Activator.CreateInstance(typeof(T), parameters);

            // get context wrapper from HttpContext if available
            HttpContextBase wrapper = null;
            if (HttpContext.Current != null)
                wrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
            else
                throw new InvalidOperationException(
                    "Can't create Controller Context if no active HttpContext instance is available.");

            if (routeData == null)
                routeData = new RouteData();

            // add the controller routing if not existing
            if (!routeData.Values.ContainsKey("controller") && !routeData.Values.ContainsKey("Controller"))
                routeData.Values.Add("controller", controller.GetType().Name
                                                            .ToLower()
                                                            .Replace("controller", ""));

            controller.ControllerContext = new ControllerContext(wrapper, routeData, controller);
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
