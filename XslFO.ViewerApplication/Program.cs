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
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.CustomExtensions;

namespace XslFO.ViewerApplication
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);
            Application.ThreadException += new ThreadExceptionEventHandler((object sender, ThreadExceptionEventArgs e) =>
            {
                MessageBox.Show(String.Format("Unhandled Application Exception [ThreadException]:{0}{0}{1}", Environment.NewLine, e.Exception.GetMessages()));
            });

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler((object sender, UnhandledExceptionEventArgs e) =>
            {
                MessageBox.Show(String.Format("Unhandled Application Exception [ThreadException]:{0}{0}{1}", Environment.NewLine, ((Exception)e.ExceptionObject).GetMessages()));
            });

            Application.Run(new XslFOViewerForm());
        }
    }
}
