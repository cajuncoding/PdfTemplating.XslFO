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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.CustomExtensions;
using System.IO;
using PdfTemplating.SystemCustomExtensions;
using PdfTemplating.XslFO.Fonet.CustomExtensions;
using System.Diagnostics;
using PdfTemplating.XslFO;

namespace XslFO.ViewerApplication
{
    public partial class XslFOViewerForm : Form
    {
        LoadFromXsltForm frmLoadFromXslt = new LoadFromXsltForm();
        LoadFromXslFOForm frmLoadFromXslFO = new LoadFromXslFOForm();
        LoadFromPdfFile frmLoadFromPdfFile = new LoadFromPdfFile();

        public enum XslFOLoadType
        {
            Undefined,
            LoadedFromXslt,
            LoadedFromXslFO
        }

        private XslFOLoadType _enumLoadType = XslFOLoadType.Undefined;
        public XslFOLoadType LoadType 
        { 
            get 
            {
                return _enumLoadType;
            }
            set 
            {
                _enumLoadType = value;
            }
        }

        XslFOPdfOptions _pdfOptions = new XslFOPdfOptions()
        {
            Author = "BBernard",
            Title = "Xsl-FO Test Application",
            Subject = "Dynamically Xslt Generated Xsl-FO Pdf Document [{0}]".FormatArgs(DateTime.Now),
            EnableAdd = false,
            EnableCopy = true,
            EnableModify = false,
            EnablePrinting = true
        };

        public XslFOViewerForm()
        {
            InitializeComponent();
        }

        private void XslFOViewerForm_Load(object sender, EventArgs e)
        {
            if (ctlXslFOViewer.LoadStatus == ControlLibrary.XslFOViewerControlState.Unavailable)
            {
                fileToolStripMenuItem.Enabled = false;
            }
        }

        #region XSL FO Viewer Event Handlers

        private void XslFOViewer_Event(object obj, ControlLibrary.XslFOViewerEventArgs e) 
        {
            ConsoleWriteLine(e.Message);
            //if (args.RenderOutput != null)
            //{
            //    ConsoleWriteLine(args.RenderOutput);
            //}
        }

        private void XslFOViewer_OnLoadCompleted(object obj, ControlLibrary.XslFOViewerEventArgs e)
        {
            ConsoleWriteLines(this.ctlXslFOViewer.LogItems.ToArray());

            if (e.RenderOutput != null)
            {
                this.rtxtSource.Clear();
                this.rtxtSource.Text = e.RenderSource;
            }
        }

        private void ctlXslFOViewer_ViewerError(object sender, ControlLibrary.XslFOViewerEventArgs e)
        {
            ConsoleWriteLine(e.Message);
            rtxtSource.Text = e.RenderSource;
        }

        #endregion

        private void ClearConsole()
        {
            //rtxtErrorConsole.Text = String.Empty;
            rtxtSource.Clear();
            rtxtErrorConsole.Clear();
        }

        private void ConsoleWriteLine(string formatText, params object[] args)
        {
            ConsoleWriteLine(String.Format(formatText, args));
        }

        private void ConsoleWriteLine(string text)
        {
            ConsoleWriteLines(new string[] { text });
        }

        private void ConsoleWriteLines(string[] lines)
        {
            rtxtErrorConsole.Lines = rtxtErrorConsole.Lines.Push(lines).SliceBottom(50000);
            //System.Diagnostics.Debug.Write(String.Join(Environment.NewLine, lines));
        }

        private void doLoadFromXslFO()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                FileInfo xslFOSourceFileInfo = frmLoadFromXslFO.XslFOSourceFile;

                //Update the BaseDirectory for processing relative paths to Resources
                _pdfOptions.BaseDirectory = xslFOSourceFileInfo.Directory;

                ConsoleWriteLine("Loading Document: {0}", xslFOSourceFileInfo.FullName);

                ctlXslFOViewer.LoadXslFO(xslFOSourceFileInfo, _pdfOptions);

                ConsoleWriteLine("Finished Loading Document: {0}", xslFOSourceFileInfo.FullName);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                this.LoadType = XslFOLoadType.LoadedFromXslFO;
            }
        }

        private void mnuLoadXslFOFromXml_Click(object sender, EventArgs e)
        {
            try
            {
                if (frmLoadFromXslFO.ShowDialog(this) == DialogResult.OK)
                {
                    ClearConsole();
                    doLoadFromXslFO();
                }
            }
            catch (Exception)
            {
                this.Cursor = Cursors.Default;
                this.LoadType = XslFOLoadType.LoadedFromXslt;

                //MessageBox.Show(String.Format("Error occurred loading Xml-FO from Xml FO File:  {0}{1}", Environment.NewLine, exc.GetMessagesRecursively()));
            }
        }

        private void doLoadFromXslt()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                FileInfo xmlSourceFileInfo = frmLoadFromXslt.XmlSourceFile;
                FileInfo xsltSourceFileInfo = frmLoadFromXslt.XsltSourceFile;

                //Update the BaseDirectory for processing relative paths to Resources
                _pdfOptions.BaseDirectory = xsltSourceFileInfo.Directory;

                ConsoleWriteLine("Loading Document Xml: {0}", xmlSourceFileInfo.FullName);
                ConsoleWriteLine("Loading Document Xslt: {0}", xsltSourceFileInfo.FullName);

                ctlXslFOViewer.LoadXslt(xmlSourceFileInfo, xsltSourceFileInfo, _pdfOptions);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                this.LoadType = XslFOLoadType.LoadedFromXslt;
            }

        }

        private void mnuLoadXslFOFromXslt_Click(object sender, EventArgs e)
        {
            try
            {
                if (frmLoadFromXslt.ShowDialog(this) == DialogResult.OK)
                {
                    ClearConsole();
                    doLoadFromXslt();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(String.Format("Error occurred loading Xml-FO from Xslt:  {0}{1}", Environment.NewLine, exc.GetMessagesRecursively()));
            }
        }

        private void doLoadFromPdfFile()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                FileInfo pdfFileInfo = frmLoadFromPdfFile.PdfBinaryFile;

                ConsoleWriteLine("Loading Pdf Binary File: {0}", pdfFileInfo.FullName);

                ctlXslFOViewer.LoadPdf(pdfFileInfo);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                this.LoadType = XslFOLoadType.LoadedFromXslt;
            }

        }

        private void mnuLoadPdfFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (frmLoadFromPdfFile.ShowDialog(this) == DialogResult.OK)
                {
                    ClearConsole();
                    doLoadFromPdfFile();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(String.Format("Error occurred loading Xml-FO from Xslt:  {0}{1}", Environment.NewLine, exc.GetMessagesRecursively()));
            }
        }

        private void doReload()
        {
            try
            {
                ConsoleWriteLine("Reloading document [{0}]", DateTime.Now);
                switch (this.LoadType)
                {
                    case XslFOLoadType.LoadedFromXslt:
                        doLoadFromXslt();
                        break;
                    case XslFOLoadType.LoadedFromXslFO:
                        doLoadFromXslFO();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exc)
            {
                ConsoleWriteLine("Error occurred re-loading [{0}]:  {1}{2}", this.LoadType.ToString(), Environment.NewLine, exc.GetMessagesRecursively());
                //MessageBox.Show(String.Format());
            }
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
                doReload();
        }

        private void viewErrorsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;

            //Update UI
            splitContainerMain.Panel2Collapsed = !menuItem.Checked;
        }

        private void XslFOViewerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (frmLoadFromXslt != null)
            {
                frmLoadFromXslt.Close();
            }

            if (frmLoadFromXslFO != null)
            {
                frmLoadFromXslFO.Close();
            }

        }

        private void rtxtErrorConsole_TextChanged(object sender, EventArgs e)
        {
            if (taleToolStripMenuItem.Checked)
            {
                rtxtErrorConsole.Select(rtxtErrorConsole.TextLength, 0);
                rtxtErrorConsole.ScrollToCaret();
            }
        }

        private void doRefreshTimerInterval()
        {
            Timer tmr = refreshTimer as Timer;
            bool bEnabled = tmr.Enabled;

            tmr.Stop();

            int interval = 5;
            tmr.Interval = (int.TryParse(txtRefreshTimerInterval.Text, out interval) ? interval : 5) * 1000;

            if(bEnabled) tmr.Start();
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            doReload();
        }

        private void autoReloadEnabledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mnu = sender as ToolStripMenuItem;
            mnu.Checked = !mnu.Checked;
        }

        private void autoReloadEnabledToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem mnu = sender as ToolStripMenuItem;
            doRefreshTimerInterval();
            refreshTimer.Enabled = mnu.Checked;
        }

        private void txtRefreshTimerInterval_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !e.KeyChar.ToString().IsNumeric();
        }

        private void txtRefreshTimerInterval_TextChanged(object sender, EventArgs e)
        {
            doRefreshTimerInterval();
        }

        private void clearConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearConsole();
        }


    }
}
