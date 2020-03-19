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

using PdfTemplating.XslFO;
using PdfTemplating.XslFO.Fonet.CustomExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.CustomExtensions;
using System.Diagnostics;
using System.IO;
using System.IO.CustomExtensions;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Linq.CustomExtensions;
using System.Xml.Linq.Xslt.CustomExtensions;
using TE.Library;

namespace XslFO.ControlLibrary
{
    /// <summary>
    /// NOTE:  This is designed to be an abstract class however, as a UserControl, 
    /// it cannot be flagged as an 'abstract' class or the Visual Studio designer breaks.
    /// This is why we flag it as NOT a valid ToolboxItem via the ToolboxItem attribute.
    /// </summary>
    [ToolboxItem(false)]
    public partial class XslFOFormViewerAbstractControl : UserControl, IXslFOViewerControl
    {

        #region Constructors & Dispose -- Designer Customized Functions for Intialization and Cleanup
        public XslFOFormViewerAbstractControl()
        {
            InitializeComponent();
            this.LoadStatus = XslFOViewerControlState.Unloaded;

            //Initialize the Current Executing Assembly if possible
            Assembly myAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            if (myAssembly != null)
            {
                _executingAssemblyName = myAssembly.GetName().Name;
            }

            //NOTE!!! DUE TO THE FACT THAT THE FORM TITLE IS NEVER KONWN BY THE CONTROL UNTIL AFTER IT HAS COMPLETELY LOADED
            //      WE ARE UNABLE TO ACTUALLY PERFORM THE CLEANUP... SO FOR NOW OUR WORK AROUND is to provide a custom public
            //      property that lets us set the design time unique name for the ReportRenderer to use.  And in that SETTER, we
            //      will KNOW the correct value and can process initializaion cleanup activities easily.
            //NOTE: THE FOLLOWING CODE NEVER WORKED AS EXPECTED!
            //this.Load += (sender, e) => {
            //    //NOTE:  THIS IS CUSTOM Cleanup Code to ensure any temp files that exist are cleaned up appropriately
            //    //NOTE:  We ALSO do this on construction (not int he Dispose block above) in case there are continuing 
            //    //       issues preventing the containing application from closing properly!
            //    var parent = this.GetParentForm();
            //    this.CleanupOrphanedTempFilesHelper();
            //};
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.DesignMode)
            {
                //NOTE:  THIS IS CUSTOM Cleanup Code to ensure any temp files that exist are cleaned up appropriately
                //NOTE:  The Component.Disposed Event is not firing as expected therefore we must put our code here
                //       in the actual Dispose event created by the Designer!
                CleanupOrphanedTempFilesHelper();
            }

            //Default Dispose Iteration code from VS Codegen (Moved from *.Designer file)
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Protected Events for Loading Xml, Loading Xslt, Parsing, Executing, Errors, etc. for use by Inheriting Controls
        //*********************************************************************************************************************
        //* NOTE:  We must provide 'protected' methods for inherited classes to Raise Errors correctly.
        //*********************************************************************************************************************
        private List<XslFOViewerEventArgs> _errorEventList = new List<XslFOViewerEventArgs>();
        public event EventHandler<XslFOViewerEventArgs> ViewerError;
        protected virtual void RaiseViewerError(XslFOViewerEventArgs eventArgs) 
        {
            _loadTimer.Stop();
            LogItem("Load Stopped due to Error [{0}s].", _loadTimer.Elapsed.TotalSeconds);
            LogItem("Error: {0}", eventArgs.Message);

            //Track the Error Events so we can also pass them along at LoadCompleted
            //for consumers that don't want to trap every event thrown!
            _errorEventList.Add(eventArgs);

            //Finally update the Tracking Flag
            _loadingInProgress = false;

            //Now raise the Error for all listeners
            ViewerError.Raise(this, eventArgs);
        }

        public event EventHandler<XslFOViewerEventArgs> LoadStarting;
        protected virtual void RaiseLoadStarting(XslFOViewerEventArgs eventArgs) { LoadStarting.Raise(this, eventArgs); }

        public event EventHandler<XslFOViewerEventArgs> LoadCompleted;
        protected virtual void RaiseLoadCompleted(XslFOViewerEventArgs eventArgs) 
        {
            eventArgs.ErrorEvents = new List<XslFOViewerEventArgs>();
            eventArgs.ErrorEvents.AddRange(_errorEventList);

            LoadCompleted.Raise(this, eventArgs); 
        }

        #endregion

        #region Public Properties

        List<String> _logItems = new List<String>();
        public List<String> LogItems
        {
            get
            {
                lock (_threadLock)
                {
                    return _logItems;
                }
            }
        }

        protected XslFORenderFileOutput RenderedOutput { get; private set; }

        public XslFOViewerControlState LoadStatus { get; protected set; }

        [Description("The Unique base file name to use for rendering Pdf Temp files required for AcroX Pdf Viewer. "
                    + "This file name should be unique within the application context to prevent possible interference with other XslFO Viewer control instances.")]
        public String UniqueTempFileBaseNameForAppContext {
            get { return _uniqueBaseFileNameForApplicationContext; }
            set {
                //NOTE:  We do this on Property Set because we otherwise are unable to KNOW when the parent form is loaded, or when
                //       we (this UserControl) has been added to the form collection, etc. thereby making it quite hard to keeep
                //       our temp file folder clean.  This will minimize any impact and allow us to clean it up pro-actively!
                //NOTE:  THIS MIGHT cause potential issues if thie control is placed on a form that has multiple instances, resuting
                //       in a identical (non unique) Base Temp File name pattern, but that will have to be fixed later....
                //First Cleanup any files from OLD Name just to keep tidy!
                this.CleanupOrphanedTempFilesHelper(_uniqueBaseFileNameForApplicationContext);
                //Second intitailzie and pro-actively clean-up anything for the new base name, JUST IN CASE!
                this.CleanupOrphanedTempFilesHelper(value);
                //Finally allow the property to be udpated
                _uniqueBaseFileNameForApplicationContext = value;
            }
        }
        private String _uniqueBaseFileNameForApplicationContext = "NO_UNIQUE_NAME_SPECIFIED";

        #endregion

        #region Private Properties

        private Guid _instanceGuid = Guid.NewGuid();
        private TaskScheduler _uiThreadScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        private Object _threadLock = new Object();
        private Stopwatch _loadTimer = new Stopwatch();
        private String _executingAssemblyName = "XslFOControlLibrary"; //Initialize to Default Assembly Name
        private String _parentFormName = String.Empty;

        #endregion

        #region Private LOG Helper Methods

        private void LogItem(String message)
        {
            lock (_threadLock)
            {
                _logItems.Add(message);
            }
        }

        private void LogItem(String messageFormat, params object[] args)
        {
            var message = String.Format(messageFormat, args);
            lock (_threadLock)
            {
                _logItems.Add(message);
            }

            System.Diagnostics.Debug.WriteLine(message);
        }

        private void ClearLog()
        {
            lock (_threadLock)
            {
                _logItems.Clear();
            }
        }

        #endregion

        #region Private TEMP FILE Helper Methods

        /// <summary>
        /// Generate a unique base file name for temp files rendered by this User Control.
        /// NOTE:  THIS MIGHT cause potential issues if thie control is placed on a form that has multiple instances, resuting
        //          in a identical (non unique) Base Temp File name pattern, but that will have to be fixed later....
        /// </summary>
        /// <param name="uniqueBaseName"></param>
        /// <returns></returns>
        private String GetDerivedUniqueBaseTempFileName(String uniqueBaseName)
        {
            //NOTE:  THIS MIGHT cause potential issues if thie control is placed on a form that has multiple instances, resuting
            //       in a identical (non unique) Base Temp File name pattern, but that will have to be fixed later....
            return "{0}_{1}_{2}".FormatArgs(_executingAssemblyName, uniqueBaseName, base.Name);
        }

        private DirectoryInfo _tempDirectory = null;
        private DirectoryInfo GetTempDirectoryHelper()
        {
            if (_tempDirectory == null) {
                var tempDir = new WindowsApplicationSpecialFolderHelper(TE_ConfigFolderType.MachineTempFolder, "XslFO.PdfRenderer", "PdfRender.Temp");
                _tempDirectory = tempDir.ToDirectoryInfo().GetOrCreateSubdirectory(_executingAssemblyName);
                //var tempDir = new DirectoryInfo(Path.GetTempPath());
                //_tempDirectory = tempDir.GetOrCreateSubdirectory(_executingAssemblyName).GetOrCreateSubdirectory("PdfRender.Temp");
            }
            return _tempDirectory;
        }

        private FileInfo _tempFileInfo = null;
        private FileInfo GetTempFileHelper()
        {
            if (_tempFileInfo == null)
            {
                //Initialize Temp File Info for the first time
                //Initialize new Temp File Full Name & Path
                String tempFileName = "{0}-{1}.pdf".FormatArgs(GetDerivedUniqueBaseTempFileName(_uniqueBaseFileNameForApplicationContext), _instanceGuid).ToFileSystemSafeString(true);

                //Initlialize the Temp FileInfo reference
                var tempDir = GetTempDirectoryHelper();
                _tempFileInfo = tempDir.GetFile(tempFileName);

                //Ensure that the new FileInfo is Clean if Needed.
                CleanupTempFileHelper();
            }
            else
            {
                //Clean up the current Temp File Info if necessary
                CleanupTempFileHelper();
            }

            return _tempFileInfo;
        }

        private void CleanupTempFileHelper()
        {
            //Clean up if there is an Existing Temp File
            if (_tempFileInfo != null)
            {
                try
                {
                    _tempFileInfo.DeleteSafely();
                    LogItem("Temp File successfully cleaned up [{0}]: ", _tempFileInfo.FullName);
                }
                catch (Exception exc)
                {
                    LogItem("Temp File is Locked and cannot be cleaned up [{0}]: ", _tempFileInfo.FullName, exc.GetMessages());
                }
            }
        }

        /// <summary>
        /// Cleans up the Temp folder for files that have a Filename deriving from the Base File the current value of 
        /// the base name property of the control
        /// </summary>
        private void CleanupOrphanedTempFilesHelper()
        {
            CleanupOrphanedTempFilesHelper(_uniqueBaseFileNameForApplicationContext);
        }

        /// <summary>
        /// Cleans up the Temp folder for files that have a Filename deriving from the Base File name specified.  We allow callers to
        /// specify the base name so that this can be run before/after the BaseName property is changed; but with full logic re-use.
        /// </summary>
        /// <param name="uniqueBaseName"></param>
        private void CleanupOrphanedTempFilesHelper(String uniqueBaseName)
        {
            //NOTE:  Since we use Temp files there could Possibly be Orphans so we pro-actively try to clean up after 
            //       ourself here by looking for possible Temp Files created by This Control Instance as it is defined 
            //       in a given application (i.e. pattern of 'base.Name*.pdf').
            //NOTE:  We ALSO do this on construction (not int he Dispose block above) in case there are continuing 
            //       issues preventing the containing application from closing properly!
            //NOTE:  We CANNOT do this in the Constructor/Initializer because the base.Name reference is not valid
            //       until the Control is fully Loaded.
            var tempDir = GetTempDirectoryHelper();
            foreach (var fileInfo in tempDir.GetFiles("{0}-*.pdf".FormatArgs(GetDerivedUniqueBaseTempFileName(uniqueBaseName))))
            {
                try
                {
                    fileInfo.DeleteSafely();
                    LogItem("Orphaned Temp File successfully cleaned up [{0}]: ", fileInfo.FullName);
                }
                catch (Exception exc)
                {
                    LogItem("Orphaned Temp File is Locked and cannot be cleaned up [{0}]: ", fileInfo.FullName, exc.GetMessages());
                }
            }
        }

        #endregion

        #region Private EVENT Helper Methods / Objects

        private bool _loadingInProgress = false;
        //BBernard - 01/16/2015
        //Made this Public to enable Containing Forms to TriggerLoadStart() externally for better UI when the Form
        //  has LONG running code to prepare the Xml Document or Xslt prior to being able to call this control.
        public void TriggerLoadStart()
        {
            if (this.LoadStatus == XslFOViewerControlState.Unavailable)
            {
                //Short Circuit if our current Load Status is UNAVAILALBE because we cannot be initialized, 
                //  and therefore should not run anything.
                return;
            }

            if(!_loadingInProgress)
            {
                this.LoadStatus = XslFOViewerControlState.Loading;
                RaiseLoadStarting(new XslFOViewerEventArgs("Load Starting."));
                _loadTimer.Restart();

                //Set the tracking Flag so that we don't raise Laoding Event multiple times 
                //accidentally from Overloaded methods that cascade
                _loadingInProgress = true;
            }
        }

        //Private helper mehod to manage events for Loading
        private void TriggerLoadFile(XslFORenderFileOutput renderedOutput)
        {
            try
            {
                if (this.LoadStatus == XslFOViewerControlState.Unavailable)
                {
                    //Process ERROR / UNAVAILABLE State

                    //Raise Error because the Viewer Control is not fully initialized.
                    var eventArg = new XslFOViewerEventArgs("Viewer Error [{0}s]: Pdf Viewer Control not initialized [Status={1}].".FormatArgs(_loadTimer.Elapsed.TotalSeconds, this.LoadStatus.ToString()));
                    LogItem(eventArg.Message);
                    RaiseViewerError(eventArg);
                    //RaiseLoadCompleted(eventArg);

                    //Short Circuit if our current Load Status is UNAVAILALBE because we cannot be initialized, 
                    //  and therefore should not run anything.
                    return;
                }
                else
                {
                    //Process NORMALLY

                    //Update our reference to the rendered output
                    this.RenderedOutput = renderedOutput;

                    //Process the new rendered output
                    if (renderedOutput != null)
                    {
                        FileInfo pdfBinaryFileInfo = renderedOutput.PdfFileInfo;
                        pdfBinaryFileInfo.Refresh();
                        if (pdfBinaryFileInfo != null && pdfBinaryFileInfo.Exists)
                        {
                            //Call the Abstract Method (implemented by Inheritec Controls) to load the File!
                            LoadFile(pdfBinaryFileInfo);
                            //ONLY update to Load Completed if the LoadFile abstract method executed successfully!
                            this.LoadStatus = XslFOViewerControlState.LoadCompleted;
                        }
                        else
                        {
                            throw new ArgumentNullException("Pdf Binary file is null or does not exist; a valid file must be specified [{0}].".FormatArgs(pdfBinaryFileInfo.FullName));
                        }

                        var eventArg = new XslFOViewerEventArgs("Load Completed [{0}s].\nPdf File Loaded: [{1}]".FormatArgs(_loadTimer.Elapsed.TotalSeconds, pdfBinaryFileInfo.FullName), renderedOutput);
                        LogItem(eventArg.Message);
                        RaiseLoadCompleted(eventArg);
                    }
                    else
                    {
                        this.LoadStatus = XslFOViewerControlState.Unloaded;
                        MessageBox.Show(this.ParentForm, "Rendered Output is null or empty!");
                    }
                }
            }
            catch (Exception exc)
            {
                this.LoadStatus = XslFOViewerControlState.Unloaded;
                RaiseViewerError(new XslFOViewerEventArgs("Error occurred Loading the Rendered Output Pdf File; {0}; Pdf Output File Path [{1}]".FormatArgs(exc.GetMessages(), renderedOutput.PdfFileInfo.FullName), renderedOutput.XslFODocument.ToString()));
            }
            finally
            {
                _loadTimer.Stop();

                //Finally update the Tracking Flag
                _loadingInProgress = false;
            }
        }

        #endregion

        #region IXslFOViewerControl Interface Members - Public exposed methods for Rendering Xml to Pdf

        FileInfo _currentLoadedFileInfo = null;
        public FileInfo LoadedFile
        {
            get
            {
                return _currentLoadedFileInfo;
            }
        }

        public void LoadXslt(FileInfo xmlSourceFile, FileInfo xslFOXsltSourceFile, XslFOPdfOptions pdfOptions)
        {
            XDocument xXmlDoc = null;
            XDocument xXsltDoc = null;

            TriggerLoadStart();

            try
            {
                LogItem("Xml Source Load Starting.");
                var timer = Stopwatch.StartNew();
                xXmlDoc = xmlSourceFile.OpenXDocument();
                LogItem("Xml Source Document Load Completed [{0}s].", timer.Elapsed.TotalSeconds);
            }
            catch (Exception exc)
            {
                throw new ArgumentException("The Xml source file specified could not be read; ensure that the file exists and that you have read access", exc);
            }

            try
            {
                LogItem("Xslt Source Load Starting.");
                var timer = Stopwatch.StartNew();
                xXsltDoc = xslFOXsltSourceFile.OpenXDocument();
                LogItem("Xslt Source Document Load Completed [{0}s].", timer.Elapsed.TotalSeconds);
            }
            catch (Exception exc)
            {
                throw new ArgumentException("The Xslt source file (to create Xml FO) specified could not be read; ensure that the file exists and that you have read access", exc);
            }

            LoadXslt(xXmlDoc, xXsltDoc, pdfOptions);
        }

        public void LoadXslt(XDocument xXmlInputDoc, XDocument xXslFOXsltDoc, XslFOPdfOptions pdfOptions)
        {
            TriggerLoadStart();

            //NOTE:  We Lock this entire method because it is the Core method for the UI Control and we don't want this Control
            //       to allow primary processing by more than one Thread at a time.  Though Background threads are launched
            //       to not block the UI thread, this is not intended to process multiple items at a Time and this
            //       block contains code where critical values are being modified.
            lock (_threadLock)
            {
                ClearLog();

                var uiTaskFactory = new TaskFactory(_uiThreadScheduler);
                LogItem("XslFO Pdf Render Process  Starting.");
                var timerOverall = Stopwatch.StartNew();

                //Execute the Work Async, with a Continuation back on the UI Thread when completed.
                var workerTask = Task.Factory.StartNew<XslFORenderFileOutput>(() =>
                {
                    //***********************************************************
                    //Initialize and Compile the XsltTransformer
                    //***********************************************************
                    LogItem("Xslt Compile Starting.");
                    var timer = Stopwatch.StartNew();

                    var xmlResolver = new XmlUrlExtendedResolver(pdfOptions.BaseDirectory);
                    XslTransformEngine xslTransformer = xXslFOXsltDoc.CreateXslTransformEngine(new XslTransformEngineOptions()
                    {
                        XsltDocumentResolver = xmlResolver,
                        XsltLoadResolver = xmlResolver
                    });

                    LogItem("Xslt Compile Completed in [{0}] seconds.", timer.Elapsed.TotalSeconds);

                    //***********************************************************
                    //Execute the Xslt Transformation
                    //***********************************************************
                    LogItem("Xslt Execution Starting.");
                    timer.Restart();

                    var xXslFOResultsDoc = xslTransformer.TransformToXDocument(xXmlInputDoc);
                        
                    LogItem("Xslt Execution Completed in [{0}] seconds.", timer.Elapsed.TotalSeconds);

                    //***********************************************************
                    //Render the XslFO results into a Pdf
                    //***********************************************************
                    //return xXmlInputDoc.TransformToPdfFile(xslTransformer, tempPdfFileInfo, pdfOptions, _fnFonetEventHandler, _fnFonetEventHandler, _fnXsltEventHandler);
                    LogItem("Pdf Render Starting.");
                    timer.Restart();

                    var XslFORenderedOutput = xXslFOResultsDoc.RenderXslFOToPdfFile(GetTempFileHelper(), new XslFORenderOptions()
                    {
                        PdfOptions = pdfOptions,
                        RenderErrorHandler = GetFONetErrorHandler(),
                        RenderEventHandler = GetFONetEventHandler()
                    });

                    LogItem("Pdf Render Completed in [{0}] seconds.", timer.Elapsed.TotalSeconds);

                    //***********************************************************
                    //Finally Return the Renderd Output from the the Background
                    //Thread so that the results can be processed on the UI!
                    //***********************************************************
                    return XslFORenderedOutput;

                }).ContinueWith((workTask) =>
                {
                    //NOTE: Since this is the Followup Thread executing we must check to see if the Background Thread Faulted and handle it correctly.
                    //This allows us to process the Exception back on the UI Thread instead of loosing it on the background thread.
                    if (workTask.IsFaulted)
                    {
                        String renderSource = String.Empty;
                        var innerException = workTask.Exception.InnerException;

                        if (innerException is XslTransformEngineException)
                        {
                            renderSource = innerException.As<XslTransformEngineException>().XsltOutput;
                        }
                        else if (innerException is XslFORenderException)
                        {
                            renderSource = innerException.As<XslFORenderException>().RenderSource;
                        }

                        this.LoadStatus = XslFOViewerControlState.Unloaded;
                        RaiseViewerError(new XslFOViewerEventArgs("Exception Occurred Processing the Transformer.  Inner Exception Details: {0}".FormatArgs(workTask.Exception.GetMessages()), renderSource));
                    }
                    else
                    {
                        //Halt and Wait for the workerTask's result! 
                        //NOTE:  This DOES NOT Halt our UI, because this is only invoked within our ContinueWith continuation of the Parallel Task
                        //       which will handle the coordination and usher our results back to the UI Thread via the ThreadScheduler parameter below!
                        LogItem("XslFO Pdf Render Process Completed in [{0}] seconds.", timerOverall.Elapsed.TotalSeconds);
                        TriggerLoadFile(workTask.Result);
                    }

                    //Cleanup Temp File once loaded because Acrobat ActiveX Control no longer needs it
                    //NOTE:  Acrobot can browse the file after it is loaded, however Find/Search functionality is broken
                    //       so we cannot delete the file pre-maturely.
                    //CleanupTempFileHelper();

                }, _uiThreadScheduler);
            }
        }

        public void LoadXslt(XDocument xXmlInputDoc, XslTransformEngine xslTransformer, XslFOPdfOptions pdfOptions)
        {
            TriggerLoadStart();

            //NOTE:  We Lock this entire method because it is the Core method for the UI Control and we don't want this Control
            //       to allow primary processing by more than one Thread at a time.  Though Background threads are launched
            //       to not block the UI thread, this is not intended to process multiple items at a Time and this
            //       block contains code where critical values are being modified.
            lock (_threadLock)
            {
                ClearLog();

                //Initialize Temp File Helper to manage temp files for Transformation into PDF
                var uiTaskFactory = new TaskFactory(_uiThreadScheduler);
                LogItem("XslFO Pdf Render Process  Starting.");
                var timerOverall = Stopwatch.StartNew();

                //Execute the Work Async, with a Continuation back on the UI Thread when completed.
                var workerTask = Task.Factory.StartNew<XslFORenderFileOutput>(() =>
                {
                    //***********************************************************
                    //XsltEngine is already passed in and Compiled for performance
                    //Execute the Xslt Transformation
                    //***********************************************************
                    LogItem("Xslt Execution Starting.");
                    var timer = Stopwatch.StartNew();

                    var xXslFOResultsDoc = xslTransformer.TransformToXDocument(xXmlInputDoc);

                    LogItem("Xslt Execution Completed in [{0}] seconds.", timer.Elapsed.TotalSeconds);

                    //***********************************************************
                    //Render the XslFO results into a Pdf
                    //***********************************************************
                    //return xXmlInputDoc.TransformToPdfFile(xslTransformer, tempPdfFileInfo, pdfOptions, _fnFonetEventHandler, _fnFonetEventHandler, _fnXsltEventHandler);
                    LogItem("Pdf Render Starting.");
                    timer.Restart();

                    var XslFORenderedOutput = xXslFOResultsDoc.RenderXslFOToPdfFile(GetTempFileHelper(), new XslFORenderOptions()
                    {
                        PdfOptions = pdfOptions,
                        RenderErrorHandler = GetFONetErrorHandler(),
                        RenderEventHandler = GetFONetEventHandler()
                    });

                    LogItem("Pdf Render Completed in [{0}] seconds.", timer.Elapsed.TotalSeconds);

                    //***********************************************************
                    //Finally Return the Renderd Output from the the Background
                    //Thread so that the results can be processed on the UI!
                    //***********************************************************
                    return XslFORenderedOutput;

                }).ContinueWith((workTask) =>
                {
                    //NOTE: Since this is the Followup Thread executing we must check to see if the Background Thread Faulted and handle it correctly.
                    //This allows us to process the Exception back on the UI Thread instead of loosing it on the background thread.
                    if (workTask.IsFaulted)
                    {
                        String renderSource = String.Empty;
                        var innerException = workTask.Exception.InnerException;

                        if (innerException is XslTransformEngineException)
                        {
                            renderSource = innerException.As<XslTransformEngineException>().XsltOutput;
                        }
                        else if (innerException is XslFORenderException)
                        {
                            renderSource = innerException.As<XslFORenderException>().RenderSource;
                        }

                        RaiseViewerError(new XslFOViewerEventArgs("Exception Occurred Processing the Transformer.  Inner Exception Details: {0}".FormatArgs(workTask.Exception.GetMessages()), renderSource));
                    }
                    else
                    {
                        //Halt and Wait for the workerTask's result! 
                        //NOTE:  This DOES NOT Halt our UI, because this is only invoked within our ContinueWith continuation of the Parallel Task
                        //       which will handle the coordination and usher our results back to the UI Thread via the ThreadScheduler parameter below!
                        LogItem("XslFO Pdf Render Process Completed in [{0}] seconds.", timerOverall.Elapsed.TotalSeconds);
                        TriggerLoadFile(workTask.Result);
                    }

                    //Cleanup Temp File once loaded because Acrobat ActiveX Control no longer needs it
                    //NOTE:  Acrobot can browse the file after it is loaded, however Find/Search functionality is broken
                    //       so we cannot delete the file pre-maturely.
                    //CleanupTempFileHelper();

                }, _uiThreadScheduler);
            }
        }


        public void LoadXslFO(FileInfo xslFOSourceFile, XslFOPdfOptions pdfOptions)
        {
            XDocument xXslFODoc = null;
            try
            {
                TriggerLoadStart();

                xXslFODoc = xslFOSourceFile.OpenXDocument();
            }
            catch (Exception exc)
            {
                throw new ArgumentException("The Xml source file specified could not be read; ensure that the file exists and that you have read access", exc);
            }

            LoadXslFO(xXslFODoc, pdfOptions);
        }

        public void LoadXslFO(XDocument xXslFODoc, XslFOPdfOptions pdfOptions)
        {
            TriggerLoadStart();

            FileInfo pdfBinaryFileInfo = GetTempFileHelper();

            var xslFORenderedOutput = xXslFODoc.RenderXslFOToPdfFile(pdfBinaryFileInfo, new XslFORenderOptions()
            {
                PdfOptions = pdfOptions,
                RenderErrorHandler = GetFONetErrorHandler(),
                RenderEventHandler = GetFONetEventHandler()
            });

            TriggerLoadFile(xslFORenderedOutput);

            //Cleanup Temp File once loaded because Acrobat ActiveX Control no longer needs it
            //NOTE:  Acrobot can browse the file after it is loaded, however Find/Search functionality is broken
            //       so we cannot delete the file pre-maturely.
            //CleanupTempFileHelper();
        }

        public void LoadPdf(FileInfo pdfFileInfo)
        {
            TriggerLoadStart();

            TriggerLoadFile(new XslFORenderFileOutput(null, pdfFileInfo));

            //Cleanup Temp File once loaded because Acrobat ActiveX Control no longer needs it
            //NOTE:  Acrobot can browse the file after it is loaded, however Find/Search functionality is broken
            //       so we cannot delete the file pre-maturely.
            //CleanupTempFileHelper();
        }

        public virtual void RefreshReport()
        {
            TriggerLoadStart();
            TriggerLoadFile(this.RenderedOutput);
        }


        #endregion

        #region Abstract/Virtual Methods to be implemented by Inheriting Controls -- Virtual is used to Simulate Abstact methods from UI Control.

        public virtual void LoadFile(FileInfo pdfBinaryFileInfo) {
            throw new NotImplementedException("Inheriting Control Must Implement this Method!  It is not marked Abstract due to Visual Studio Designer issues.");
        }

        public virtual void ClearReport()
        {
            throw new NotImplementedException("Inheriting Control Must Implement this Method!  It is not marked Abstract due to Visual Studio Designer issues.");
        }

        protected virtual void SetLoadingState(XslFOViewerControlState loadingState)
        {
            throw new NotImplementedException("Inheriting Control Must Implement this Method!  It is not marked Abstract due to Visual Studio Designer issues.");
        }

        #endregion
     
        #region Internal Event Handlers

        private EventHandler<XsltExtensionEventArgs> GetXsltExtensionEventHandler()
        {
            return new EventHandler<XsltExtensionEventArgs>((sender, e) =>
            {
                var messageExc = e as XsltMessageEventArgs;
                if (messageExc != null)
                {
                    LogItem("{0}: {1}", "XsltCustomExtension", messageExc.GetMessage());
                }
            });
        }

        private EventHandler<XslFOEventArg> GetFONetEventHandler()
        {
            return new EventHandler<XslFOEventArg>((sender, e) =>
            {
                LogItem("[{0}] {1}", "XslFORenderer", e.Message);
            });
        }

        private EventHandler<XslFOErrorEventArg> GetFONetErrorHandler()
        {
            return new EventHandler<XslFOErrorEventArg>((sender, e) =>
            {
                LogItem("[{0}] {1}", "XslFORenderer", e.Message);
            });
        }

        private void axAcroPDF1_OnError(object sender, EventArgs e)
        {
            MessageBox.Show(String.Format("ActiveX Acrobat Pdf Control Error:  {0}", e.ToString()));
        }

        #endregion

    }

}
