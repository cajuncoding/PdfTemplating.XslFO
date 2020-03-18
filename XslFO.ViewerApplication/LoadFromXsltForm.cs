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
using System.IO;

namespace PdfTemplating.ViewerApplication
{
    public partial class LoadFromXsltForm : Form
    {
        public FileInfo XmlSourceFile 
        { 
            get
            {
                return new FileInfo(txtXmlSourceFile.Text);
            }
        }

        public FileInfo XsltSourceFile
        {
            get
            {
                return new FileInfo(txtXsltSourceFile.Text);
            }
        }

        public LoadFromXsltForm()
        {
            InitializeComponent();
        }

        private void btnOpenXmlSourceFile_Click(object sender, EventArgs e)
        {
            if(dlgOpenXmlFile.ShowDialog(this) == DialogResult.OK)
            {
                txtXmlSourceFile.Text = dlgOpenXmlFile.FileName;
            }
        }

        private void btnOpenXsltSourceFile_Click(object sender, EventArgs e)
        {
            if (dlgOpenXsltFile.ShowDialog(this) == DialogResult.OK)
            {
                txtXsltSourceFile.Text = dlgOpenXsltFile.FileName;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
