namespace Fonet.Pdf.Security
{
    using System.Collections.Specialized;

    public class SecurityOptions
    {
        /// <summary>
        ///     Password that disables all security permissions
        /// </summary>
        protected string ownerPassword;

        /// <summary>
        ///     The user password 
        /// </summary>
        protected string userPassword;

        /// <summary>
        ///     Collection of flags describing permissions granted to user who opens 
        ///     a file with the user password.
        /// </summary>
        /// <remarks>
        ///     The given initial value zero's out first two bits.
        ///     The PDF specification dictates that these entries must be 0.
        /// </remarks>
        protected BitVector32 permissions = new BitVector32(-4);

        /// <summary>
        ///     Returns the owner password as a string.
        /// </summary>
        /// <value>
        ///     The default value is null
        /// </value>
        public string OwnerPassword
        {
            get
            {
                return ownerPassword;
            }
            set
            {
                ownerPassword = value;
            }
        }

        /// <summary>
        ///     Returns the user password as a string.
        /// </summary>
        /// <value>
        ///     The default value is null
        /// </value>
        public string UserPassword
        {
            get
            {
                return userPassword;
            }
            set
            {
                userPassword = value;
            }
        }

        /// <summary>
        ///     The document access privileges encoded in a 32-bit unsigned integer
        /// </summary>
        /// <value>
        ///     The default access priviliges are:
        ///     <ul>
        ///     <li>Printing disallowed</li>
        ///     <li>Modifications disallowed</li>
        ///     <li>Copy and Paste disallowed</li>
        ///     <li>Addition or modification of annotation/form fields disallowed</li>
        ///     </ul>
        ///     To override any of these priviliges see the <see cref="EnablePrinting"/>,
        ///     <see cref="EnableChanging"/>, <see cref="EnableCopying"/>, 
        ///     <see cref="EnableAdding"/> methods
        /// </value>
        public int Permissions
        {
            get
            {
                return permissions.Data;
            }
            set
            {
                permissions = new BitVector32(value);
            }
        }

        /// <summary>
        ///     Enables or disables printing.
        /// </summary>
        /// <param name="enable">If true enables printing otherwise false</param>
        public void EnablePrinting(bool enable)
        {
            permissions[4] = enable;
        }

        /// <summary>
        ///     Enable or disable changing the document other than by adding or 
        ///     changing text notes and AcroForm fields.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableChanging(bool enable)
        {
            permissions[8] = enable;
        }

        /// <summary>
        ///     Enable or disable copying of text and graphics from the document.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableCopying(bool enable)
        {
            permissions[16] = enable;
        }

        /// <summary>
        ///     Enable or disable adding and changing text notes and AcroForm fields.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableAdding(bool enable)
        {
            permissions[32] = enable;
        }

    }
}