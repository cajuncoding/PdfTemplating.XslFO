using PdfTemplating.ControlLibrary;

namespace PdfTemplating.ViewerApplication
{
    partial class XslFOViewerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLoadXslFOFromXml = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLoadXslFOFromXslt = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLoadPdfFile = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.autoReloadEnabledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtRefreshTimerInterval = new System.Windows.Forms.ToolStripTextBox();
            this.errorConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.viewErrorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.taleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.tabView = new System.Windows.Forms.TabControl();
            this.tabViewPdf = new System.Windows.Forms.TabPage();
            this.ctlXslFOViewer = new XslFOAdobePdfViewerUserControl();
            this.tabViewSource = new System.Windows.Forms.TabPage();
            this.rtxtSource = new System.Windows.Forms.RichTextBox();
            this.rtxtErrorConsole = new System.Windows.Forms.RichTextBox();
            this.statusBar.SuspendLayout();
            this.mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.tabView.SuspendLayout();
            this.tabViewPdf.SuspendLayout();
            this.tabViewSource.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusText,
            this.statusMessage});
            this.statusBar.Location = new System.Drawing.Point(0, 527);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(642, 22);
            this.statusBar.TabIndex = 2;
            this.statusBar.Text = "statusStrip1";
            // 
            // statusText
            // 
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(0, 17);
            // 
            // statusMessage
            // 
            this.statusMessage.Name = "statusMessage";
            this.statusMessage.Size = new System.Drawing.Size(88, 17);
            this.statusMessage.Text = "Status Message";
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.errorConsoleToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(642, 24);
            this.mainMenu.TabIndex = 4;
            this.mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLoadXslFOFromXml,
            this.mnuLoadXslFOFromXslt,
            this.mnuLoadPdfFile,
            this.reloadToolStripMenuItem,
            this.toolStripSeparator1,
            this.autoReloadEnabledToolStripMenuItem,
            this.txtRefreshTimerInterval});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // mnuLoadXslFOFromXml
            // 
            this.mnuLoadXslFOFromXml.Name = "mnuLoadXslFOFromXml";
            this.mnuLoadXslFOFromXml.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mnuLoadXslFOFromXml.Size = new System.Drawing.Size(257, 22);
            this.mnuLoadXslFOFromXml.Text = "Load Xml-FO Source";
            this.mnuLoadXslFOFromXml.Click += new System.EventHandler(this.mnuLoadXslFOFromXml_Click);
            // 
            // mnuLoadXslFOFromXslt
            // 
            this.mnuLoadXslFOFromXslt.Name = "mnuLoadXslFOFromXslt";
            this.mnuLoadXslFOFromXslt.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.mnuLoadXslFOFromXslt.Size = new System.Drawing.Size(257, 22);
            this.mnuLoadXslFOFromXslt.Text = "Load Xml-FO From Xslt";
            this.mnuLoadXslFOFromXslt.Click += new System.EventHandler(this.mnuLoadXslFOFromXslt_Click);
            // 
            // mnuLoadPdfFile
            // 
            this.mnuLoadPdfFile.Name = "mnuLoadPdfFile";
            this.mnuLoadPdfFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.mnuLoadPdfFile.Size = new System.Drawing.Size(257, 22);
            this.mnuLoadPdfFile.Text = "Load Pdf Directly From File";
            this.mnuLoadPdfFile.Click += new System.EventHandler(this.mnuLoadPdfFile_Click);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.reloadToolStripMenuItem.Text = "Reload";
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(254, 6);
            // 
            // autoReloadEnabledToolStripMenuItem
            // 
            this.autoReloadEnabledToolStripMenuItem.Name = "autoReloadEnabledToolStripMenuItem";
            this.autoReloadEnabledToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.autoReloadEnabledToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.autoReloadEnabledToolStripMenuItem.Text = "Auto-Reload Enabled";
            this.autoReloadEnabledToolStripMenuItem.CheckedChanged += new System.EventHandler(this.autoReloadEnabledToolStripMenuItem_CheckedChanged);
            this.autoReloadEnabledToolStripMenuItem.Click += new System.EventHandler(this.autoReloadEnabledToolStripMenuItem_Click);
            // 
            // txtRefreshTimerInterval
            // 
            this.txtRefreshTimerInterval.MaxLength = 4;
            this.txtRefreshTimerInterval.Name = "txtRefreshTimerInterval";
            this.txtRefreshTimerInterval.Size = new System.Drawing.Size(100, 23);
            this.txtRefreshTimerInterval.Text = "5";
            this.txtRefreshTimerInterval.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRefreshTimerInterval.ToolTipText = "Enter the interval to refresh the document when auto-reload is enabled.";
            this.txtRefreshTimerInterval.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRefreshTimerInterval_KeyPress);
            this.txtRefreshTimerInterval.TextChanged += new System.EventHandler(this.txtRefreshTimerInterval_TextChanged);
            // 
            // errorConsoleToolStripMenuItem
            // 
            this.errorConsoleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearConsoleToolStripMenuItem,
            this.toolStripSeparator2,
            this.viewErrorsToolStripMenuItem,
            this.taleToolStripMenuItem});
            this.errorConsoleToolStripMenuItem.Name = "errorConsoleToolStripMenuItem";
            this.errorConsoleToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.errorConsoleToolStripMenuItem.Text = "Console";
            // 
            // clearConsoleToolStripMenuItem
            // 
            this.clearConsoleToolStripMenuItem.Name = "clearConsoleToolStripMenuItem";
            this.clearConsoleToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.clearConsoleToolStripMenuItem.Text = "Clear Console";
            this.clearConsoleToolStripMenuItem.Click += new System.EventHandler(this.clearConsoleToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(210, 6);
            // 
            // viewErrorsToolStripMenuItem
            // 
            this.viewErrorsToolStripMenuItem.Checked = true;
            this.viewErrorsToolStripMenuItem.CheckOnClick = true;
            this.viewErrorsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewErrorsToolStripMenuItem.Name = "viewErrorsToolStripMenuItem";
            this.viewErrorsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.viewErrorsToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.viewErrorsToolStripMenuItem.Text = "View Error Console";
            this.viewErrorsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.viewErrorsToolStripMenuItem_CheckedChanged);
            // 
            // taleToolStripMenuItem
            // 
            this.taleToolStripMenuItem.Checked = true;
            this.taleToolStripMenuItem.CheckOnClick = true;
            this.taleToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.taleToolStripMenuItem.Name = "taleToolStripMenuItem";
            this.taleToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.taleToolStripMenuItem.Text = "Tale";
            // 
            // refreshTimer
            // 
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 24);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.tabView);
            this.splitContainerMain.Panel1MinSize = 50;
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.rtxtErrorConsole);
            this.splitContainerMain.Panel2MinSize = 50;
            this.splitContainerMain.Size = new System.Drawing.Size(642, 503);
            this.splitContainerMain.SplitterDistance = 366;
            this.splitContainerMain.TabIndex = 5;
            // 
            // tabView
            // 
            this.tabView.Controls.Add(this.tabViewPdf);
            this.tabView.Controls.Add(this.tabViewSource);
            this.tabView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabView.Location = new System.Drawing.Point(0, 0);
            this.tabView.Name = "tabView";
            this.tabView.SelectedIndex = 0;
            this.tabView.Size = new System.Drawing.Size(642, 366);
            this.tabView.TabIndex = 5;
            // 
            // tabViewPdf
            // 
            this.tabViewPdf.Controls.Add(this.ctlXslFOViewer);
            this.tabViewPdf.Location = new System.Drawing.Point(4, 22);
            this.tabViewPdf.Margin = new System.Windows.Forms.Padding(0);
            this.tabViewPdf.Name = "tabViewPdf";
            this.tabViewPdf.Size = new System.Drawing.Size(634, 340);
            this.tabViewPdf.TabIndex = 0;
            this.tabViewPdf.Text = "PDF";
            this.tabViewPdf.UseVisualStyleBackColor = true;
            // 
            // ctlXslFOViewer
            // 
            this.ctlXslFOViewer.BackColor = System.Drawing.Color.White;
            this.ctlXslFOViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ctlXslFOViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctlXslFOViewer.LoadDelayMillisForFocusIssues = 3000;
            this.ctlXslFOViewer.Location = new System.Drawing.Point(0, 0);
            this.ctlXslFOViewer.Name = "ctlXslFOViewer";
            this.ctlXslFOViewer.Size = new System.Drawing.Size(634, 340);
            this.ctlXslFOViewer.TabIndex = 5;
            this.ctlXslFOViewer.ToolbarEnabled = false;
            this.ctlXslFOViewer.ViewerError += new System.EventHandler<XslFOViewerEventArgs>(this.ctlXslFOViewer_ViewerError);
            this.ctlXslFOViewer.LoadCompleted += new System.EventHandler<XslFOViewerEventArgs>(this.XslFOViewer_OnLoadCompleted);
            // 
            // tabViewSource
            // 
            this.tabViewSource.Controls.Add(this.rtxtSource);
            this.tabViewSource.Location = new System.Drawing.Point(4, 22);
            this.tabViewSource.Margin = new System.Windows.Forms.Padding(0);
            this.tabViewSource.Name = "tabViewSource";
            this.tabViewSource.Size = new System.Drawing.Size(634, 340);
            this.tabViewSource.TabIndex = 1;
            this.tabViewSource.Text = "Xml Source";
            this.tabViewSource.UseVisualStyleBackColor = true;
            // 
            // rtxtSource
            // 
            this.rtxtSource.BackColor = System.Drawing.SystemColors.ControlLight;
            this.rtxtSource.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtSource.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxtSource.ForeColor = System.Drawing.SystemColors.WindowText;
            this.rtxtSource.Location = new System.Drawing.Point(0, 0);
            this.rtxtSource.Name = "rtxtSource";
            this.rtxtSource.ReadOnly = true;
            this.rtxtSource.Size = new System.Drawing.Size(634, 340);
            this.rtxtSource.TabIndex = 0;
            this.rtxtSource.Text = "";
            // 
            // rtxtErrorConsole
            // 
            this.rtxtErrorConsole.BackColor = System.Drawing.Color.Black;
            this.rtxtErrorConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtErrorConsole.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxtErrorConsole.ForeColor = System.Drawing.Color.Ivory;
            this.rtxtErrorConsole.Location = new System.Drawing.Point(0, 0);
            this.rtxtErrorConsole.Name = "rtxtErrorConsole";
            this.rtxtErrorConsole.ReadOnly = true;
            this.rtxtErrorConsole.Size = new System.Drawing.Size(642, 133);
            this.rtxtErrorConsole.TabIndex = 1;
            this.rtxtErrorConsole.Text = "";
            this.rtxtErrorConsole.WordWrap = false;
            this.rtxtErrorConsole.TextChanged += new System.EventHandler(this.rtxtErrorConsole_TextChanged);
            // 
            // XslFOViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 549);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.Name = "XslFOViewerForm";
            this.Text = "Xsl-FO Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.XslFOViewerForm_FormClosing);
            this.Load += new System.EventHandler(this.XslFOViewerForm_Load);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.tabView.ResumeLayout(false);
            this.tabViewPdf.ResumeLayout(false);
            this.tabViewSource.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel statusText;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuLoadXslFOFromXml;
        private System.Windows.Forms.ToolStripMenuItem mnuLoadXslFOFromXslt;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.RichTextBox rtxtErrorConsole;
        private System.Windows.Forms.ToolStripMenuItem errorConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewErrorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem taleToolStripMenuItem;
        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.ToolStripMenuItem autoReloadEnabledToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox txtRefreshTimerInterval;
        private System.Windows.Forms.ToolStripStatusLabel statusMessage;
        private System.Windows.Forms.ToolStripMenuItem clearConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.TabControl tabView;
        private System.Windows.Forms.TabPage tabViewPdf;
        private ControlLibrary.XslFOAdobePdfViewerUserControl ctlXslFOViewer;
        private System.Windows.Forms.TabPage tabViewSource;
        private System.Windows.Forms.RichTextBox rtxtSource;
        private System.Windows.Forms.ToolStripMenuItem mnuLoadPdfFile;
    }
}

