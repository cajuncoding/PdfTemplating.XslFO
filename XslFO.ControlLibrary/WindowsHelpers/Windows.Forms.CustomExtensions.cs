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

using System.Collections.Generic;
using PdfTemplating.SystemCustomExtensions;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Windows.Forms.CustomExtensions
{

    public static class WindowsFormCustomExtensions
    {
        //public static void CenterOnScreen(this Form form)
        //{
        //    //ENSURE THE FORM IS CENTERED ON SCREEN
        //    var screen = Screen.FromControl(form.ParentForm ?? form);
        //    var intHeight = screen.WorkingArea.Height;
        //    var intWidth = screen.WorkingArea.Width;
        //    var intTop = screen.WorkingArea.Top;
        //    var intLeft = screen.WorkingArea.Left;

        //    ////Re-Center the Form to handle resizing Events -- CENTER ON SCREEN
        //    form.Top = (intTop + (intHeight / 2)) - (form.Height / 2);
        //    form.Left = (intLeft + (intWidth / 2)) - (form.Width / 2);
        //}

        public static void ShowCenteredInParent(this Form form, Form parentForm)
        {
            if (parentForm == null) throw new ArgumentNullException("parentForm");

            form.StartPosition = FormStartPosition.Manual;
            //The following math will work if the child form is smaller or larger than the Parent form
            //while also accounting for the fact that we don't want to move the form off screen.
            var xPos = Math.Max(parentForm.Location.X + ((parentForm.Width - form.Width) / 2), 0);
            var yPos = Math.Max(parentForm.Location.Y + ((parentForm.Height - form.Height) / 2), 0);

            form.Location = new Point(xPos, yPos);
            form.Show(parentForm);
        }

        /// <summary>
        /// Shows the Dialog and initializes it so that it will appear centered on the Parent Form. An exception is thrown 
        /// if the parentForm is null because a Dialog must be modal and owned by a parent Form.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="parentForm"></param>
        /// <returns></returns>
        public static DialogResult ShowDialogCenteredInParent(this Form form, Form parentForm) {
            if (parentForm == null) {
                throw new ArgumentNullException("parentForm", "The Dialog [{0}] cannot be displayed because the parentForm ".FormatArgs(form.Text)
                                                 + "paremeter is null -- Modal Dialogs must have a parent/owning form.");
            }
    
            form.StartPosition = FormStartPosition.CenterParent;
            return form.ShowDialog(parentForm);
        }
    }

    public static class MessageBoxCustomExtensions
    {

    }

    public static class ControlThreadSafeExtensions
    {
        delegate void SetPropertyThreadSafeDelegate<TResult>(Control souce, Expression<Func<Control, TResult>> selector, TResult value);

        public static void SetPropertyThreadSafe<TResult>(this Control source, Expression<Func<Control, TResult>> selector, TResult value)
        {
            //Implement Elegant recursive self-referencing call to ensure that all manipulation of the control occurs on the UI Thread; the
            //same thread that created the control.
            if (source.InvokeRequired)
            {
                //Note:  Here we recursively instantiate a delegate with a signature that exactly matches this method; this wrapper will allow
                //       this method to call itself via the Invoke command, but on the same thread as the Control.  Therefore the Control's InvokeRequired
                //       flag will not be True and our recursion will stop.
                var safeDelegate = new SetPropertyThreadSafeDelegate<TResult>(SetPropertyThreadSafe);
                source.Invoke(safeDelegate, new object[] { source, selector, value });
            }
            else
            {
                //Note:  When we are already on the same thread as the control the InvokeRequired flag will not be True and therefore we
                //       can directly manipulate the control since we are on the UI thread!  This also stops any recursion when called from
                //       other threads.
                var propInfo = ((MemberExpression)selector.Body).Member as PropertyInfo;
                propInfo.SetValue(source, value, null);
            }
        }

        delegate TResult GetPropertyThreadSafeDelegate<TResult>(Control souce, Expression<Func<Control, TResult>> selector);

        public static TResult GetPropertyThreadSafe<TResult>(this Control source, Expression<Func<Control, TResult>> selector)
        {
            //Implement Elegant recursive self-referencing call to ensure that all manipulation of the control occurs on the UI Thread; the
            //same thread that created the control.
            if (source.InvokeRequired)
            {
                //Note:  Here we recursively instantiate a delegate with a signature that exactly matches this method; this wrapper will allow
                //       this method to call itself via the Invoke command, but on the same thread as the Control.  Therefore the Control's InvokeRequired
                //       flag will not be True and our recursion will stop.
                var safeDelegate = new GetPropertyThreadSafeDelegate<TResult>(GetPropertyThreadSafe);
                return (TResult)source.Invoke(safeDelegate, new object[] { source, selector });
            }
            else
            {
                //Note:  When we are already on the same thread as teh control the InvokeRequired flag will not be True and therefore we
                //       can directly manipulate the control since we are on the UI thread!  This also stops any recursion when called from
                //       other threads.
                var propInfo = ((MemberExpression)selector.Body).Member as PropertyInfo;
                return (TResult)propInfo.GetValue(source, null);
            }
        }

        //private delegate void SetPropertyThreadSafeDelegate<TResult>(Control objThis, Expression<Func<TResult>> property, TResult value);

        //public static void SetPropertyThreadSafe<TResult>(this Control objThis, Expression<Func<TResult>> property, TResult value)
        //{
        //    var propertyInfo = (property.Body as MemberExpression).Member as PropertyInfo;

        //    Type thisType = objThis.GetType();
        //    if (propertyInfo == null
        //        || !thisType.IsSubclassOf(propertyInfo.ReflectedType)
        //        || thisType.GetProperty(propertyInfo.Name, propertyInfo.PropertyType) == null)
        //    {
        //        throw new ArgumentException(String.Format("The lambda expression 'property' must reference a valid property on this Control [Type={0}] [Name={1}].", thisType.DeclaringType, objThis.Name));
        //    }

        //    //Implement Elegant recursive self-referencing call to ensure that all manipulation of the control occurs on the UI Thread; the
        //    //same thread that created the control.
        //    if (objThis.InvokeRequired)
        //    {
        //        //Note:  Here we recursively instantiate a delegate with a signature that exactly matches this method; this wrapper will allow
        //        //       this method to call itslef via the Invoke command, but on the same thread as the Control.  Therefore teh Control's InvokeRequired
        //        //       flag will not be True and our recursion will stop.
        //        objThis.Invoke(new SetPropertyThreadSafeDelegate<TResult>(SetPropertyThreadSafe), new object[] { objThis, property, value });
        //    }
        //    else
        //    {
        //        //Note:  When we are already on the same thread as teh control the InvokeRequired flag will not be True and therefore we
        //        //       can directly manipulate the control since we are on the UI thread!  This also stops any recursion when called from
        //        //       other threads.
        //        objThis.GetType().InvokeMember(propertyInfo.Name, BindingFlags.SetProperty, null, objThis, new object[] { value });
        //    }
        //}
    }

    public static class ControlsExtensions
    {
        public static Control FocusedDescendant(this Control objThis)
        {
            Control focusedControl = objThis.Descendants().Where(ctl => ctl.Focused == true).FirstOrDefault();
            return focusedControl;
        }

        public static Control FocusedChild(this Control objThis)
        {
            Control focusedControl = objThis.Children().Where(ctl => ctl.Focused == true).FirstOrDefault();
            return focusedControl;
        }

        public static IEnumerable<Control> Children(this Control objThis)
        {
            return objThis.Controls.Children();
        }

        public static IEnumerable<Control> Descendants(this Control objThis)
        {
            return objThis.Controls.Descendants();
        }

        /// <summary>
        /// Converts the Controls Collection to an IEnumerable&lt;Control&gt; collection that supports Linq and contains only Children Controls
        /// </summary>
        public static IEnumerable<Control> Children(this Control.ControlCollection controls)
        {
            return controls.ToIEnumerable(false);
        }

        /// <summary>
        /// Converts the Controls Collection to an IEnumerable&lt;Control&gt; collection that supports Linq and contains all Descendant Controls
        /// Specific logic can be made because all controls have references to their Parent.
        /// </summary>
        public static IEnumerable<Control> Descendants(this Control.ControlCollection controls)
        {
            return controls.ToIEnumerable(true);
        }

        /// <summary>
        /// Converts the Controls Collection to an IEnumerable&lt;Control&gt; collection that supports Linq and contains all Descendant Controls.
        /// Specific logic can be made because all controls have references to their Parent.
        /// </summary>
        public static IEnumerable<Control> ToIEnumerable(this Control.ControlCollection controls)
        {
            return controls.ToIEnumerable(true);
        }
        
        /// <summary>
        /// Converts the Controls Collection to an IEnumerable&lt;Control&gt; collection that supports Linq
        /// </summary>
        public static IEnumerable<Control> ToIEnumerable(this Control.ControlCollection controls, bool bTraverseAllDescendants)
        {
            foreach (Control control in controls)
            {
                if (bTraverseAllDescendants)
                {
                    //Note:  This will result in a Recursive call (to ToIEnumerable) for all children/grandchildren/etc.
                    //       but will stop when it reaches the bottom and the outer foreach loop has no nested controls!
                    foreach (Control grandChild in control.Controls.ToIEnumerable())
                        yield return grandChild;
                }

                yield return control;
            }
        }

        /// <summary>
        /// Climb the Control Hierarchy in search of the specified Control Type.  
        /// E.g. Find the closest TabControl above the current control in the Hieararchy.
        /// </summary>
        public static T FindParent<T>(this Control control) where T : Control
        {
            if (control.Parent == null)
            {
                return null;
            }

            //If the parent can be cast to the Expected type then we return it, otherwise
            //we recursively keep looking.
            var parent = control.Parent as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return control.Parent.FindParent<T>();
            }
        }

        /// <summary>
        /// Retrieves all Parent Controls for the specified control as an IEnumerable&lt;Control&gt; collection that supports Linq
        /// </summary>
        public static IEnumerable<Control> Parents(this Control control)
        {
            //while(control.Parent != null)
            //{
            //    foreach (Control parent in control.Controls.ToIEnumerable())
            //        yield return grandChild;
            //}

            //foreach (Control control in control.)
            //{
            //    foreach (Control grandChild in control.Controls.ToIEnumerable())
            //        yield return grandChild;

            //    yield return control;
            //}

            Stack<Control> hierarchyStack = new Stack<Control>();
            Control parentControl = control.Parent;
            while (parentControl != null)
            {
                hierarchyStack.Push(parentControl);
                parentControl = parentControl.Parent;
            }

            return hierarchyStack.ToList();
        }

        public static Form GetParentForm(this Control control)
        {
            //Handle cases where we call GetParentForm() on a Form Control itself!
            Form form = control as Form;
            if (form != null)
            {
                return form;
            }
            else
            {
                return control.FindParent<Form>();
            }
        }

        public static void CenterInParent(this Control control, int verticalOffset = 0, int horizontalOffset = 0)
        {
            control.CenterInParent(true, verticalOffset, true, horizontalOffset);
        }

        public static void CenterInParent(this Control control, bool centerVertically, bool centerHorizontally)
        {
            control.CenterInParent(centerVertically, 0, centerHorizontally, 0);
        }

        public static void CenterInParent(this Control control, bool centerVertically, int verticalOffset, bool centerHorizontally, int horizontalOffset)
        {
            Control parent = control.Parent;
            if (parent != null)
            {
                if (centerVertically)
                {
                    control.Top = Math.Max(Convert.ToInt32((parent.Height / 2) - (control.Height / 2)) + verticalOffset, 0);
                }
                if (centerHorizontally)
                {
                    control.Left = Math.Max(Convert.ToInt32((parent.Width / 2) - (control.Width / 2)) + horizontalOffset, 0);
                }
            }
        }

    }

    public static class ToolStripCustomExtensions {

        /// <summary>
        /// Custom method for setting the Rounded Edges (if Possible) for ToolStrip Menu's... 
        /// makeing iniitializing them much easier than inheriting, overriding, or duplicating this logig on Load events.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="bEnabled"></param>
        public static void SetRoundedEdges(this ToolStripRenderer renderer, bool bEnabled) {
            var proRenderer = renderer as ToolStripProfessionalRenderer;
            if (proRenderer != null) {
                proRenderer.RoundedEdges = bEnabled;
            }
            else {
                //Rounded Edges to not apply to SystemRenderer's so we are passive here so that calling code is always safe!
            }
        }


    }


}
