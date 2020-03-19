namespace XslFO.ViewerApplication
{
    partial class LoadFromPdfFile
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
            this.dlgOpenPdfFile = new System.Windows.Forms.OpenFileDialog();
            this.txtPdfBinaryFile = new System.Windows.Forms.TextBox();
            this.lblXmlSource = new System.Windows.Forms.Label();
            this.btnOpenPdfBinaryFile = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dlgOpenPdfFile
            // 
            this.dlgOpenPdfFile.Filter = "Pdf Files|*.pdf|All Files|*.*";
            this.dlgOpenPdfFile.Title = "Open Xml Source File";
            // 
            // txtPdfBinaryFile
            // 
            this.txtPdfBinaryFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPdfBinaryFile.BackColor = System.Drawing.SystemColors.Window;
            this.txtPdfBinaryFile.Location = new System.Drawing.Point(12, 28);
            this.txtPdfBinaryFile.Name = "txtPdfBinaryFile";
            this.txtPdfBinaryFile.ReadOnly = true;
            this.txtPdfBinaryFile.Size = new System.Drawing.Size(317, 20);
            this.txtPdfBinaryFile.TabIndex = 0;
            // 
            // lblXmlSource
            // 
            this.lblXmlSource.AutoSize = true;
            this.lblXmlSource.Location = new System.Drawing.Point(12, 12);
            this.lblXmlSource.Name = "lblXmlSource";
            this.lblXmlSource.Size = new System.Drawing.Size(77, 13);
            this.lblXmlSource.TabIndex = 2;
            this.lblXmlSource.Text = "Pdf Binary File:";
            // 
            // btnOpenPdfBinaryFile
            // 
            this.btnOpenPdfBinaryFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenPdfBinaryFile.Location = new System.Drawing.Point(335, 28);
            this.btnOpenPdfBinaryFile.Name = "btnOpenPdfBinaryFile";
            this.btnOpenPdfBinaryFile.Size = new System.Drawing.Size(24, 20);
            this.btnOpenPdfBinaryFile.TabIndex = 4;
            this.btnOpenPdfBinaryFile.Text = "...";
            this.btnOpenPdfBinaryFile.UseVisualStyleBackColor = true;
            this.btnOpenPdfBinaryFile.Click += new System.EventHandler(this.btnOpenPdfBinaryFile_Click);
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
            // LoadFromPdfFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 89);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOpenPdfBinaryFile);
            this.Controls.Add(this.lblXmlSource);
            this.Controls.Add(this.txtPdfBinaryFile);
            this.Name = "LoadFromPdfFile";
            this.Text = "Load Pdf File Directly";
            this.Load += new System.EventHandler(this.LoadFromXslFOForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog dlgOpenPdfFile;
        private System.Windows.Forms.TextBox txtPdfBinaryFile;
        private System.Windows.Forms.Label lblXmlSource;
        private System.Windows.Forms.Button btnOpenPdfBinaryFile;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}