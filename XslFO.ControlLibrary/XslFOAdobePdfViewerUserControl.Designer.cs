namespace PdfTemplating.ControlLibrary
{
    partial class XslFOAdobePdfViewerUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XslFOAdobePdfViewerUserControl));
            this.tmrAcrobatLoadDelay = new System.Windows.Forms.Timer(this.components);
            this.panelPreLoader = new System.Windows.Forms.Panel();
            this.lblPreLoaderTimer = new System.Windows.Forms.Label();
            this.picPreLoader = new System.Windows.Forms.PictureBox();
            this.preLoaderTimer = new System.Windows.Forms.Timer(this.components);
            this.animLoaderTimerText = new Animations.ControlForeColorAnimator(this.components);
            this.panelInitInstructions = new System.Windows.Forms.Panel();
            this.picInitIcon = new System.Windows.Forms.PictureBox();
            this.lblInitInstructions = new System.Windows.Forms.Label();
            this.panelPreLoader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPreLoader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.animLoaderTimerText)).BeginInit();
            this.panelInitInstructions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picInitIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // tmrAcrobatLoadDelay
            // 
            this.tmrAcrobatLoadDelay.Interval = 3000;
            this.tmrAcrobatLoadDelay.Tick += new System.EventHandler(this.tmrAcrobatLoadDelay_Tick);
            // 
            // panelPreLoader
            // 
            this.panelPreLoader.BackColor = System.Drawing.Color.Transparent;
            this.panelPreLoader.Controls.Add(this.lblPreLoaderTimer);
            this.panelPreLoader.Controls.Add(this.picPreLoader);
            this.panelPreLoader.Location = new System.Drawing.Point(186, 25);
            this.panelPreLoader.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelPreLoader.Name = "panelPreLoader";
            this.panelPreLoader.Size = new System.Drawing.Size(167, 154);
            this.panelPreLoader.TabIndex = 2;
            // 
            // lblPreLoaderTimer
            // 
            this.lblPreLoaderTimer.AutoSize = true;
            this.lblPreLoaderTimer.BackColor = System.Drawing.Color.Transparent;
            this.lblPreLoaderTimer.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPreLoaderTimer.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblPreLoaderTimer.Location = new System.Drawing.Point(67, 67);
            this.lblPreLoaderTimer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPreLoaderTimer.Name = "lblPreLoaderTimer";
            this.lblPreLoaderTimer.Size = new System.Drawing.Size(36, 20);
            this.lblPreLoaderTimer.TabIndex = 2;
            this.lblPreLoaderTimer.Text = "00s";
            this.lblPreLoaderTimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picPreLoader
            // 
            this.picPreLoader.BackColor = System.Drawing.Color.Transparent;
            this.picPreLoader.Image = ((System.Drawing.Image)(resources.GetObject("picPreLoader.Image")));
            this.picPreLoader.Location = new System.Drawing.Point(33, 31);
            this.picPreLoader.Margin = new System.Windows.Forms.Padding(0);
            this.picPreLoader.Name = "picPreLoader";
            this.picPreLoader.Size = new System.Drawing.Size(100, 92);
            this.picPreLoader.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picPreLoader.TabIndex = 1;
            this.picPreLoader.TabStop = false;
            // 
            // preLoaderTimer
            // 
            this.preLoaderTimer.Tick += new System.EventHandler(this.preLoaderTimer_Tick);
            // 
            // animLoaderTimerText
            // 
            this.animLoaderTimerText.Control = this.lblPreLoaderTimer;
            this.animLoaderTimerText.EndColor = System.Drawing.Color.LimeGreen;
            this.animLoaderTimerText.Intervall = 20;
            this.animLoaderTimerText.StartColor = System.Drawing.SystemColors.Control;
            // 
            // panelInitInstructions
            // 
            this.panelInitInstructions.BackColor = System.Drawing.Color.Transparent;
            this.panelInitInstructions.Controls.Add(this.picInitIcon);
            this.panelInitInstructions.Controls.Add(this.lblInitInstructions);
            this.panelInitInstructions.Location = new System.Drawing.Point(36, 205);
            this.panelInitInstructions.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelInitInstructions.Name = "panelInitInstructions";
            this.panelInitInstructions.Size = new System.Drawing.Size(665, 263);
            this.panelInitInstructions.TabIndex = 15;
            this.panelInitInstructions.Visible = false;
            this.panelInitInstructions.DoubleClick += new System.EventHandler(this.lblInitInstructions_DoubleClick);
            // 
            // picInitIcon
            // 
            this.picInitIcon.Image = ((System.Drawing.Image)(resources.GetObject("picInitIcon.Image")));
            this.picInitIcon.Location = new System.Drawing.Point(261, 121);
            this.picInitIcon.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.picInitIcon.Name = "picInitIcon";
            this.picInitIcon.Size = new System.Drawing.Size(133, 139);
            this.picInitIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picInitIcon.TabIndex = 14;
            this.picInitIcon.TabStop = false;
            this.picInitIcon.DoubleClick += new System.EventHandler(this.lblInitInstructions_DoubleClick);
            // 
            // lblInitInstructions
            // 
            this.lblInitInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInitInstructions.BackColor = System.Drawing.Color.Transparent;
            this.lblInitInstructions.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInitInstructions.ForeColor = System.Drawing.Color.DarkGray;
            this.lblInitInstructions.Location = new System.Drawing.Point(25, 18);
            this.lblInitInstructions.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInitInstructions.Name = "lblInitInstructions";
            this.lblInitInstructions.Size = new System.Drawing.Size(619, 98);
            this.lblInitInstructions.TabIndex = 13;
            this.lblInitInstructions.Text = "Adobe Acrobat Reader (Pdf Viewer) could not be initialized.  Please install the l" +
    "atest version of Adobe Acrobat Reader.";
            this.lblInitInstructions.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblInitInstructions.DoubleClick += new System.EventHandler(this.lblInitInstructions_DoubleClick);
            // 
            // XslFOAdobePdfViewerUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.panelInitInstructions);
            this.Controls.Add(this.panelPreLoader);
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "XslFOAdobePdfViewerUserControl";
            this.Size = new System.Drawing.Size(752, 492);
            this.Load += new System.EventHandler(this.XslFOAdobePdfViewerUserControl_Load);
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.XslFOAdobePdfViewerUserControl_Layout);
            this.Resize += new System.EventHandler(this.XslFOAdobePdfViewerUserControl_Resize);
            this.panelPreLoader.ResumeLayout(false);
            this.panelPreLoader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPreLoader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.animLoaderTimerText)).EndInit();
            this.panelInitInstructions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picInitIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer tmrAcrobatLoadDelay;
        private System.Windows.Forms.Panel panelPreLoader;
        private System.Windows.Forms.Label lblPreLoaderTimer;
        private System.Windows.Forms.PictureBox picPreLoader;
        private System.Windows.Forms.Timer preLoaderTimer;
        private Animations.ControlForeColorAnimator animLoaderTimerText;
        internal System.Windows.Forms.Panel panelInitInstructions;
        internal System.Windows.Forms.PictureBox picInitIcon;
        internal System.Windows.Forms.Label lblInitInstructions;
        //private AxAcroPDFLib.AxAcroPDF ctlAcrobatPdfViewer;
    }
}
