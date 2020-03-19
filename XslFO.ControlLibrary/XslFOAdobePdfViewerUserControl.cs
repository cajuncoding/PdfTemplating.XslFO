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
using System.CustomExtensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using AxAcroPDFLib;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.CustomExtensions;
using System.Diagnostics;
using System.Windows.Forms.CustomExtensions;

namespace XslFO.ControlLibrary
{
    [ToolboxItem(true)]
    public partial class XslFOAdobePdfViewerUserControl : XslFOFormViewerAbstractControl, IMessageFilter
    {

        #region Constructors

        public XslFOAdobePdfViewerUserControl() 
            : base()
        {
            InitializeComponent();

            //Implement the Windows Message filter to fix issues with AxAcroLib
            Application.AddMessageFilter(this);

            //Wire Up AbstractControlEvents for OUR Implementation!
            this.LoadStarting += this.AbstractControl_LoadStarted;
            this.LoadCompleted += this.AbstractControl_LoadCompleted;
            this.ViewerError += this.AbstractControl_LoadError;

            //Initialize Default Values
            this.ToolbarEnabled = false;

            //Lazy Load the AxAcroPdf Control
            //NOTE:  We lazy load the control because it has SERIOUS focus issues when initialized and causes the parent
            //       Form to mysteriously loose Focus!
            //NOTE:  WE MUST do this AFTER we know our currently Focused control or else the very first Load will loose
            //       it's focus due to the Focus issues of the AxAcroPdf control.
            this.doAxAcroPdfControlInit();
        }

        #endregion

        #region Public Properties

        public AxAcroPDF AcrobatPdfViewerControl
        {
            get
            {
                return this._ctlAcrobatPdfViewer;
            }
        }

        public bool ToolbarEnabled { get; set; }

        public int LoadDelayMillisForFocusIssues
        {
            get { return tmrAcrobatLoadDelay.Interval; }
            set { tmrAcrobatLoadDelay.Interval = value; }
        }
        
        #endregion

        #region Private Helper Methods

        //NOTE:  Since we Lazy Load this we move all initialization code out of the Designer for easier maintenance.
        private AxAcroPDFLib.AxAcroPDF _ctlAcrobatPdfViewer;
        private Exception initException;

        private bool doAxAcroPdfControlInit()
        {
            //Short circuit if the Viewer is already initialized correctly (ie. Not Null);
            if (_ctlAcrobatPdfViewer != null) return true;

            try
            {
                //TEST EXCEPTION!
                //throw new Exception("Test Initialization Failures for Adobe AxAcroPDFLib...");

                //NOTE:  Manually Call CreateControl() which is required to ensure the correct AxHost state
                //       for all ActiveX controls generated Dynamically
                this._ctlAcrobatPdfViewer = new AxAcroPDFLib.AxAcroPDF();
                this._ctlAcrobatPdfViewer.CreateControl();

                ////The following code was copied from Designer code to support Lazy Loading of the AxAcroPdf Control
                this.SuspendLayout();
                ((System.ComponentModel.ISupportInitialize)(this._ctlAcrobatPdfViewer)).BeginInit();

                //// 
                //// _ctlAcrobatPdfViewer
                //// 
                //this._ctlAcrobatPdfViewer.Dock = System.Windows.Forms.DockStyle.Fill;
                //NOTE:  Changed default state of Enabled to False due to the FOCUS control issues!
                this._ctlAcrobatPdfViewer.Enabled = false;
                this._ctlAcrobatPdfViewer.Location = new System.Drawing.Point(246, 141);
                this._ctlAcrobatPdfViewer.Name = "ctlAcrobatPdfViewer";
                //this.ctlAcrobatPdfViewer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("ctlAcrobatPdfViewer.OcxState")));
                this._ctlAcrobatPdfViewer.Size = new System.Drawing.Size(192, 192);
                this._ctlAcrobatPdfViewer.TabIndex = 3;
                this._ctlAcrobatPdfViewer.Visible = false;
                this._ctlAcrobatPdfViewer.TabStop = false;

                //Wire Up Event Handlers
                //BBernard - 02/26/2018
                //This stopped working in recent versions of Acrobat Reader (~v11).
                //this._ctlAcrobatPdfViewer.OnError += new EventHandler(this.ctlAcrobatPdfViewer_OnError);

                ////The following code was copied from Designer code to support Lazy Loading of the AxAcroPdf Control
                this.Controls.Add(this._ctlAcrobatPdfViewer);
                ((System.ComponentModel.ISupportInitialize)(this._ctlAcrobatPdfViewer)).EndInit();

                //NOTE:  We initialize the Container Properties here just in case . . . never proven if this is necessary though.
                this._ctlAcrobatPdfViewer.ContainingControl = this;
                this._ctlAcrobatPdfViewer.Parent = this;

                //Finalize UI Layout by Docking the Control correctly
                this._ctlAcrobatPdfViewer.Dock = DockStyle.Fill;

                //NOTE:  Default status is always Unloaded from the Base Control 
                //       because we assume that in general the control will initialize
                //this.LoadStatus = XslFOViewerControlState.Unloaded
            }
            catch (Exception exc)
            {
                //NOTE:  If there is an exception here we should not have to do anything as the control will
                //       continue to be lazily initialized OnLoad and that will update the UI as needed.
                //if (_ctlAcrobatPdfViewer == null)
                //{
                //    this.SetLoadingState(XslFOViewerControlState.Unloaded);
                //}

                //Dispose of and Ensure that our ActiveX Control reference is NULL if there is an exception
                //NOTE:  If for some reason another error occurs we can disgard it because we are already at an error state that
                //          we don't care to handle, but to just prevent from bubbling up to the user.  Our display will
                //          let the user know that a pre-requisite installaion of Adobe Acrobat Reader is required.
                try 
                { 
                    _ctlAcrobatPdfViewer.DisposeSafely(); 
                }
                finally
                {
                    _ctlAcrobatPdfViewer = null;
                    //NOTE:  Since this instance of the base control couldn't be initialized we set our Status as NOW Unavailable
                    this.LoadStatus = XslFOViewerControlState.Unavailable;
                }

                //For debugging (ie. Alert of error message) we store a reference to the last Exception here
                initException = exc;
            }
            finally
            {
                this.ResumeLayout();
            }

            //Finally return the iniitalization status (ie. Successful if not Null)
            return (_ctlAcrobatPdfViewer != null);
        }

        private Stopwatch _loadingStopwatch = new Stopwatch();

        protected override void SetLoadingState(XslFOViewerControlState loadingState)
        {
            this.SuspendLayout();

            try
            {
                //Due to UI Control Issues we must HIDE and Re-Show the control
                //to ensure that the UI is always updated.  Therefore we simply always
                //Hide and then only if appropriate do we re-show the control.
                if (_ctlAcrobatPdfViewer != null)
                {
                    _ctlAcrobatPdfViewer.Hide();
                }

                //Always reset the following Timers
                lblPreLoaderTimer.Hide();
                animLoaderTimerText.Stop();
                _loadingStopwatch.Stop();
                preLoaderTimer.Stop();

                //Always reset the following UI Visible elements
                panelPreLoader.Hide();
                panelInitInstructions.Hide();

                //Now handle the correct state for Pre-Loader
                if (loadingState == XslFOViewerControlState.Loading && _ctlAcrobatPdfViewer != null)
                {
                    //Loading State is IN PROGRESS
                    panelPreLoader.Show();
                    _loadingStopwatch.Restart();
                    lblPreLoaderTimer.Text = "";
                    preLoaderTimer.Start();
                    //Kick off the Pre-Load Timer Text animation
                    TaskScheduler.FromCurrentSynchronizationContext().DelayExecution(TimeSpan.FromSeconds(3), () =>
                    {
                        lblPreLoaderTimer.ForeColor = animLoaderTimerText.StartColor;
                        lblPreLoaderTimer.Show();
                        animLoaderTimerText.Start(false);
                    });
                }
                else if(loadingState == XslFOViewerControlState.LoadCompleted && _ctlAcrobatPdfViewer != null)
                {
                    //Update the UI since it is re-set after the Load
                    //Due to COM issues we need to ensure that all calls to the underlying Ctl are property Try/Catch wrapped.
                    this._ctlAcrobatPdfViewer.Show();

                    //The control will maliciously grab focus until after it has completely finished loading
                    //the Pdf file.  There is no Event for us to monitor so instead we simply implement a standard
                    //acceptable wait period.  
                    //NOTE:  The process is to re-set the controls original Enabled state  after the wait period has elapsed
                    //       so we execute a background thread to perform the wait and then trigger the update back on the UI
                    //       thread.  This ensures that our UI is not interrupted.
                    //NOTE:  Stop and Restart the delay timer so that it will run after a FULL Delay
                    tmrAcrobatLoadDelay.Stop();
                    tmrAcrobatLoadDelay.Start();
                }
                else if (loadingState == XslFOViewerControlState.Unloaded && _ctlAcrobatPdfViewer != null)
                {
                    //By default we have hidden all elements including the PreLoader and Unavailble Instructions
                    //therefore we do nothing here when the status is simply Unloaded.
                }
                else if (loadingState == XslFOViewerControlState.Unavailable || _ctlAcrobatPdfViewer == null)
                {
                    //IF the Acrobat Pdf Viewer is unable to be Initialized or is Unloaded we handle it here!
                    //NOTE:  If NOT Initialized we must show the Initialize Error Instructions.
                        panelInitInstructions.Show();
                }

            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine("Error Trapped Refreshing UI Elements -- {0}".FormatArgs(exc.GetMessages()));
            }
            finally
            {
                Thread.Yield();
                this.ResumeLayout(true);
            }
        }

        #endregion

        #region Abstract Method Required Implementations
        /// <summary>
        /// Override the Base class to provide implementation specific loading of the FileInfo specified.
        /// </summary>
        /// <param name="pdfBinaryFileInfo"></param>
        public override void LoadFile(FileInfo pdfBinaryFileInfo)
        {
            ////The following code was copied from Designer code to support Lazy Loading of the AxAcroPdf Control

            try
            {
                //NOTE:  We can't load anything if the PDF Viewer Control is never initialized.
                if (_ctlAcrobatPdfViewer != null)
                {
                    //Validate File Info parameter
                    pdfBinaryFileInfo.Refresh();
                    if (!pdfBinaryFileInfo.Exists)
                    {
                        throw new ArgumentException(String.Format("The specified file [{0}] does not exist; a valid file must be specified.", pdfBinaryFileInfo.FullName));
                    }

                    //Handle AxAcroPdf Focus Issues...
                    //Note:  AxAcroPdf has bad Focus control problems so we have to manually overcome them
                    //       when a new file is being loaded!
                    Control currentFocusedCtl = this.ParentForm.FocusedDescendant();

                    if (currentFocusedCtl != null)
                    {
                        //If there is a valid control to focus then we first disable the Acrobat ActiveX Control
                        //to prevent it from being able to recieve the focus when it makes an attempt to grab it!
                        _ctlAcrobatPdfViewer.Enabled = false;
                        _ctlAcrobatPdfViewer.SuspendLayout();
                    }

                    //Load the File into the Control
                    _ctlAcrobatPdfViewer.LoadFile(pdfBinaryFileInfo.FullName);

                    if (currentFocusedCtl != null)
                    {
                        //Now we ensure that the Focus is stil maintained so that the User is not aware of any
                        //issues and is able to continue cursoring around.
                        //currentFocusedCtl.FindForm().Activate();
                        currentFocusedCtl.Select();
                        currentFocusedCtl.Focus();

                        _ctlAcrobatPdfViewer.ResumeLayout(true);
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error occurred Loading Xsl FO Report: " + exc.GetMessages());
            }
        }
       
        public override void ClearReport()
        {
            //Load the File into the Control
            
        }

        #endregion

        #region Internal Event Handlers
        
        private void ctlAcrobatPdfViewer_OnError(object sender, EventArgs e)
        {
            Control ctl = sender as Control;
            if (ctl != null)
            {
                this.RaiseViewerError(new XslFOViewerEventArgs(String.Format("Error occurred in the Acrobat Pdf ActiveX Viewer control [Type={0}] [Name={1}]:  {2}", ctl.GetType().ToString(), ctl.Name, e.ToString())));
                //MessageBox.Show(String.Format("Error occurred in the Acrobat Pdf Viewer control:  {0}", e.ToString()));
            }
            ctl = null;
            _ctlAcrobatPdfViewer = null;
        }

        private void tmrAcrobatLoadDelay_Tick(object sender, EventArgs e)
        {
            //Disable the Timer so that it no longer runs
            sender.As<System.Windows.Forms.Timer>().Stop();

            if (_ctlAcrobatPdfViewer != null)
            {
                //Update the Display to re-enable the Control if appropriate
                //NOTE:  Since this is a nested control if the Wrapper User Control is Disabled then the PDF viewer will
                //       inherently be disabled, so we control the flag explicity here and setting to true will work fine.
                //ctlAcrobatPdfViewer.SetPropertyThreadSafe((ctl) => ctl.Enabled, (this.Enabled && true));
                _ctlAcrobatPdfViewer.Enabled = this.Enabled;

                _ctlAcrobatPdfViewer.setShowToolbar(this.ToolbarEnabled);
            }
        }

        private void AbstractControl_LoadStarted(object sencer, XslFOViewerEventArgs e)
        {
            //Force UI update of the current LoadStatus
            SetLoadingState(this.LoadStatus);
        }

        private void AbstractControl_LoadCompleted(object sender, XslFOViewerEventArgs e)
        {
            //Force UI update of the current LoadStatus
            SetLoadingState(this.LoadStatus);
        }

        private void AbstractControl_LoadError(object sender, XslFOViewerEventArgs e)
        {
            //Force UI update of the current LoadStatus
            SetLoadingState(this.LoadStatus);
        }

        //LOAD event handler for Control's Load within the Designer
        private void XslFOAdobePdfViewerUserControl_Load(object sender, EventArgs e)
        {
            //Initialize the UI and trigger Code to re-format UI!
            //NOTE:  We INITIALIZE HERE so that the Loading UI is visible in the Designer, but NOT at RunTime!
            //       Otherwise all elements would be hidden in the Designer because that is their Default state
            //       and would be harder to work with for Development.
            this.SetLoadingState(this.LoadStatus);
        }

        private void XslFOAdobePdfViewerUserControl_Resize(object sender, EventArgs e)
        {
        }

        private void XslFOAdobePdfViewerUserControl_Layout(object sender, LayoutEventArgs e)
        {
            this.SuspendLayout();
            
            //Center Init Instructions
            panelInitInstructions.CenterInParent();

            //Center PreLoader elements
            panelPreLoader.CenterInParent();
            picPreLoader.CenterInParent();
            //Note:  Because the 's' is smaller the value looks off-center so we adjust by 2px to improve the look.
            lblPreLoaderTimer.CenterInParent(horizontalOffset: 2);
            
            this.ResumeLayout();
        }

        #endregion

        #region IMessageFilter Interface Method Implementation
        /// <summary>
        /// Implements a Message Filter to eliminate TAB keypresses from Reaching the Adobe ActiveX Control and causing Fatal Exceptions.
        /// For More Info see:  http://forums.adobe.com/thread/530591?tstart=0
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        //WindowsMessageValueIndex _messagesIndex = new WindowsMessageValueIndex();
        public bool PreFilterMessage(ref Message m)
        {
            // Block the internal Adobe's Application Fatal AccessViolationException when pressing the TAB key.
            bool bFiltered = false;

            // Adobe's Application Message = 0x1450 (5200L)
            const int WM_ACROBAT_1450 = 0x1450;

            //Debug Messages being intercepted
            //if (_messagesIndex.ContainsKey(m.Msg))
            //{
            //    if (ctlAcrobatPdfViewer.Handle == m.HWnd)
            //    {
            //        Console.WriteLine("Adobe AxAcroPdf Message: [Name={5}] [HWnd={0}] [Msg={1}] [Result={2}] [LParam={3}] [WParam={4}]", m.HWnd, m.Msg, m.Result, m.LParam, m.WParam, _messagesIndex[m.Msg].MsgName);
            //    }
            //    else
            //    {
            //        //Console.WriteLine("Generic Form Message: [Name={5}] [HWnd={0}] [Msg={1}] [Result={2}] [LParam={3}] [WParam={4}]", m.HWnd, m.Msg, m.Result, m.LParam, m.WParam, _messagesIndex[m.Msg].MsgName);
            //    }
            //}

            // Apply the filter
            switch(m.Msg)
            {
                case WM_ACROBAT_1450:
                    // Block this message or it will trigger a fatal AccessViolationException when the TAB key is pressed.
                    bFiltered = true;
                    String errorMessage = "Adobe ActiveX Application Fatal AccessViolationException prevented after TAB Key Press";
                    this.RaiseViewerError(new XslFOViewerEventArgs(errorMessage));
                    break;
                default:
                    // Other messages can pass through the filter
                    bFiltered = false;
                    break;
            }

            return bFiltered;
        }
        #endregion

        #region UI Composite Control Events

        private void preLoaderTimer_Tick(object sender, EventArgs e)
        {
            var elapsedTime = _loadingStopwatch.Elapsed.TotalSeconds;
            lblPreLoaderTimer.Text = "{0}s".FormatArgs(Convert.ToInt32(elapsedTime));
            XslFOAdobePdfViewerUserControl_Layout(this, null); 
        }

        #endregion

        private void lblInitInstructions_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("Initialization Exception Details:{0}{0}{1}".FormatArgs(
                Environment.NewLine, 
                initException.GetMessages()), 
                "Pdf Viewer Init Error", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Exclamation
            );
        }


    }
}
