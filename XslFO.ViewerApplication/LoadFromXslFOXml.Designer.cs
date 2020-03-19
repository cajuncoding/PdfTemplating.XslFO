namespace XslFO.ViewerApplication
{
    partial class LoadFromXslFOForm
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
            this.dlgOpenXslFOFile = new System.Windows.Forms.OpenFileDialog();
            this.txtXslFOSourceFile = new System.Windows.Forms.TextBox();
            this.lblXmlSource = new System.Windows.Forms.Label();
            this.btnOpenXslFOSourceFile = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dlgOpenXslFOFile
            // 
            this.dlgOpenXslFOFile.Filter = "Xsl-FO Files|*.fo|Xml Files|*.xml";
            this.dlgOpenXslFOFile.Title = "Open Xml Source File";
            // 
            // txtXslFOSourceFile
            // 
            this.txtXslFOSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtXslFOSourceFile.BackColor = System.Drawing.SystemColors.Window;
            this.txtXslFOSourceFile.Location = new System.Drawing.Point(12, 28);
            this.txtXslFOSourceFile.Name = "txtXslFOSourceFile";
            this.txtXslFOSourceFile.ReadOnly = true;
            this.txtXslFOSourceFile.Size = new System.Drawing.Size(317, 20);
            this.txtXslFOSourceFile.TabIndex = 0;
            // 
            // lblXmlSource
            // 
            this.lblXmlSource.AutoSize = true;
            this.lblXmlSource.Location = new System.Drawing.Point(12, 12);
            this.lblXmlSource.Name = "lblXmlSource";
            this.lblXmlSource.Size = new System.Drawing.Size(97, 13);
            this.lblXmlSource.TabIndex = 2;
            this.lblXmlSource.Text = "Xsl-FO Source File:";
            // 
            // btnOpenXslFOSourceFile
            // 
            this.btnOpenXslFOSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenXslFOSourceFile.Location = new System.Drawing.Point(335, 28);
            this.btnOpenXslFOSourceFile.Name = "btnOpenXslFOSourceFile";
            this.btnOpenXslFOSourceFile.Size = new System.Drawing.Size(24, 20);
            this.btnOpenXslFOSourceFile.TabIndex = 4;
            this.btnOpenXslFOSourceFile.Text = "...";
            this.btnOpenXslFOSourceFile.UseVisualStyleBackColor = true;
            this.btnOpenXslFOSourceFile.Click += new System.EventHandler(this.btnOpenXslFOSourceFile_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(285, 54);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(204, 54);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // LoadFromXslFOForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 89);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOpenXslFOSourceFile);
            this.Controls.Add(this.lblXmlSource);
            this.Controls.Add(this.txtXslFOSourceFile);
            this.Name = "LoadFromXslFOForm";
            this.Text = "Load Xml-FO From Xml";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog dlgOpenXslFOFile;
        private System.Windows.Forms.TextBox txtXslFOSourceFile;
        private System.Windows.Forms.Label lblXmlSource;
        private System.Windows.Forms.Button btnOpenXslFOSourceFile;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}