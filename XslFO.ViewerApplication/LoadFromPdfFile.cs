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
    public partial class LoadFromPdfFile : Form
    {
        public FileInfo PdfBinaryFile 
        { 
            get
            {
                return new FileInfo(txtPdfBinaryFile.Text);
            }
        }

        public LoadFromPdfFile()
        {
            InitializeComponent();
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

        private void LoadFromXslFOForm_Load(object sender, EventArgs e)
        {

        }

        private void btnOpenPdfBinaryFile_Click(object sender, EventArgs e)
        {
            if (dlgOpenPdfFile.ShowDialog(this) == DialogResult.OK)
            {
                txtPdfBinaryFile.Text = dlgOpenPdfFile.FileName;
            }
        }

    }
}
