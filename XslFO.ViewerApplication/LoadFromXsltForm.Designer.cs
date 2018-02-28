namespace XslFO.ViewerApplication
{
    partial class LoadFromXsltForm
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
            this.dlgOpenXmlFile = new System.Windows.Forms.OpenFileDialog();
            this.txtXmlSourceFile = new System.Windows.Forms.TextBox();
            this.txtXsltSourceFile = new System.Windows.Forms.TextBox();
            this.lblXmlSource = new System.Windows.Forms.Label();
            this.lblXsltSource = new System.Windows.Forms.Label();
            this.btnOpenXmlSourceFile = new System.Windows.Forms.Button();
            this.btnOpenXsltSourceFile = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.dlgOpenXsltFile = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // dlgOpenXmlFile
            // 
            this.dlgOpenXmlFile.Filter = "Xml Files|*.xml";
            this.dlgOpenXmlFile.Title = "Open Xml Source File";
            // 
            // txtXmlSourceFile
            // 
            this.txtXmlSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtXmlSourceFile.BackColor = System.Drawing.SystemColors.Window;
            this.txtXmlSourceFile.Location = new System.Drawing.Point(12, 28);
            this.txtXmlSourceFile.Name = "txtXmlSourceFile";
            this.txtXmlSourceFile.ReadOnly = true;
            this.txtXmlSourceFile.Size = new System.Drawing.Size(317, 20);
            this.txtXmlSourceFile.TabIndex = 0;
            // 
            // txtXsltSourceFile
            // 
            this.txtXsltSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtXsltSourceFile.BackColor = System.Drawing.SystemColors.Window;
            this.txtXsltSourceFile.Location = new System.Drawing.Point(12, 69);
            this.txtXsltSourceFile.Name = "txtXsltSourceFile";
            this.txtXsltSourceFile.ReadOnly = true;
            this.txtXsltSourceFile.Size = new System.Drawing.Size(317, 20);
            this.txtXsltSourceFile.TabIndex = 1;
            // 
            // lblXmlSource
            // 
            this.lblXmlSource.AutoSize = true;
            this.lblXmlSource.Location = new System.Drawing.Point(12, 12);
            this.lblXmlSource.Name = "lblXmlSource";
            this.lblXmlSource.Size = new System.Drawing.Size(83, 13);
            this.lblXmlSource.TabIndex = 2;
            this.lblXmlSource.Text = "Xml Source File:";
            // 
            // lblXsltSource
            // 
            this.lblXsltSource.AutoSize = true;
            this.lblXsltSource.Location = new System.Drawing.Point(12, 53);
            this.lblXsltSource.Name = "lblXsltSource";
            this.lblXsltSource.Size = new System.Drawing.Size(83, 13);
            this.lblXsltSource.TabIndex = 3;
            this.lblXsltSource.Text = "Xslt Source File:";
            // 
            // btnOpenXmlSourceFile
            // 
            this.btnOpenXmlSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenXmlSourceFile.Location = new System.Drawing.Point(335, 28);
            this.btnOpenXmlSourceFile.Name = "btnOpenXmlSourceFile";
            this.btnOpenXmlSourceFile.Size = new System.Drawing.Size(24, 20);
            this.btnOpenXmlSourceFile.TabIndex = 4;
            this.btnOpenXmlSourceFile.Text = "...";
            this.btnOpenXmlSourceFile.UseVisualStyleBackColor = true;
            this.btnOpenXmlSourceFile.Click += new System.EventHandler(this.btnOpenXmlSourceFile_Click);
            // 
            // btnOpenXsltSourceFile
            // 
            this.btnOpenXsltSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenXsltSourceFile.Location = new System.Drawing.Point(335, 68);
            this.btnOpenXsltSourceFile.Name = "btnOpenXsltSourceFile";
            this.btnOpenXsltSourceFile.Size = new System.Drawing.Size(24, 20);
            this.btnOpenXsltSourceFile.TabIndex = 6;
            this.btnOpenXsltSourceFile.Text = "...";
            this.btnOpenXsltSourceFile.UseVisualStyleBackColor = true;
            this.btnOpenXsltSourceFile.Click += new System.EventHandler(this.btnOpenXsltSourceFile_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(284, 95);
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
            this.btnOK.Location = new System.Drawing.Point(203, 95);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // dlgOpenXsltFile
            // 
            this.dlgOpenXsltFile.Filter = "Xslt Files|*.xsl?|Xsl|*.xsl";
            this.dlgOpenXsltFile.Title = "Open Xslt Source File";
            // 
            // LoadFromXsltForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 127);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOpenXmlSourceFile);
            this.Controls.Add(this.btnOpenXsltSourceFile);
            this.Controls.Add(this.lblXsltSource);
            this.Controls.Add(this.lblXmlSource);
            this.Controls.Add(this.txtXmlSourceFile);
            this.Controls.Add(this.txtXsltSourceFile);
            this.Name = "LoadFromXsltForm";
            this.Text = "Load Xml-FO From Xslt";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog dlgOpenXmlFile;
        private System.Windows.Forms.TextBox txtXmlSourceFile;
        private System.Windows.Forms.TextBox txtXsltSourceFile;
        private System.Windows.Forms.Label lblXmlSource;
        private System.Windows.Forms.Label lblXsltSource;
        private System.Windows.Forms.Button btnOpenXmlSourceFile;
        private System.Windows.Forms.Button btnOpenXsltSourceFile;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.OpenFileDialog dlgOpenXsltFile;
    }
}