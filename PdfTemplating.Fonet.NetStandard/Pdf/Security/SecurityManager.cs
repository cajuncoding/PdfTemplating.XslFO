namespace Fonet.Pdf.Security
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Implements Adobe's standard security handler.  A security handler is 
    /// a software module that implements various aspects of the encryption 
    /// process.
    /// </summary>
    public class SecurityManager
    {
        private static readonly byte[] Padding = {
            0x28, 0xbf, 0x4e, 0x5e, 0x4e, 0x75, 0x8a, 0x41,
            0x64, 0x00, 0x4e, 0x56, 0xff, 0xfa, 0x01, 0x08,
            0x2e, 0x2e, 0x00, 0xb6, 0xd0, 0x68, 0x3e, 0x80,
            0x2f, 0x0c, 0xa9, 0xfe, 0x64, 0x53, 0x69, 0x7a
        };

        private byte[] ownerEntry;

        private byte[] userEntry;

        private byte[] masterKey;

        private int permissions;

        /// <summary>
        ///     Constructs a new standard security manager.
        /// </summary>
        /// <param name="options">
        ///     The user supplied PDF options that provides access to the passwords and 
        ///     the access permissions.
        /// </param>
        /// <param name="fileId">
        ///     The PDF document's file identifier (see section 8.3 of PDF specification).
        /// </param>
        public SecurityManager(SecurityOptions options, FileIdentifier fileId)
        {
            // note: The order that these keys are created is important.
            CreateOwnerEntry(options);
            CreateMasterKey(options, fileId); // requires the owner entry
            CreateUserEntry(options); // requires the master key
            this.permissions = options.Permissions;
        }

        public PdfDictionary GetEncrypt(PdfObjectId objectId)
        {
            PdfDictionary encrypt = new PdfDictionary(objectId);
            encrypt[PdfName.Names.Filter] = PdfName.Names.Standard;
            encrypt[PdfName.Names.V] = new PdfNumeric(1);
            encrypt[PdfName.Names.Length] = new PdfNumeric(40);
            encrypt[PdfName.Names.R] = new PdfNumeric(2);
            PdfString o = new PdfString(ownerEntry);
            o.NeverEncrypt = true;
            encrypt[PdfName.Names.O] = o;
            PdfString u = new PdfString(userEntry);
            u.NeverEncrypt = true;
            encrypt[PdfName.Names.U] = u;
            encrypt[PdfName.Names.P] = new PdfNumeric(permissions);
            return encrypt;
        }

        /// <summary>
        ///     Computes the master key that is used to encrypt string and stream data 
        ///     in the PDF document.
        /// </summary>
        /// <param name="options">
        ///     The user supplied PDF options that provides access to the passwords and
        ///     the access permissions.
        /// </param>
        /// <param name="fileId">
        ///     The PDF document's file identifier (see section 8.3 of PDF specification).
        /// </param>
        private void CreateMasterKey(SecurityOptions options, FileIdentifier fileId)
        {
            masterKey = ComputeEncryptionKey32(
                PadPassword(options.UserPassword),
                ownerEntry,
                options.Permissions,
                fileId.CreatedPart);
        }

        /// <summary>
        ///     Computes the O(owner) value in the encryption dictionary.
        /// </summary>
        /// <remarks>
        ///     Corresponds to algorithm 3.3 on page 69 of the PDF specficiation.
        /// </remarks>
        /// <param name="options">
        ///     The user supplied PDF options that provides access to the passwords.
        /// </param>
        private void CreateOwnerEntry(SecurityOptions options)
        {
            // Pad or truncate the owner password string.
            // If there is no owner password use the user password instead.
            string password = options.OwnerPassword;
            if (password == null)
            {
                password = options.UserPassword;
            }
            byte[] paddedPassword = PadPassword(password);

            // Create an MD5 hash from the padded password.
            // The first 5 bytes of this hash will be used as an ARC4 key.
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(paddedPassword);

            // Pad or truncate the user password string.
            byte[] paddedUserPassword = PadPassword(options.UserPassword);

            // Encrypt the padded user password using the key generated above.
            Arc4 arc4 = new Arc4(hash, 0, 5);
            ownerEntry = new byte[32];
            arc4.Encrypt(paddedUserPassword, ownerEntry);
        }

        /// <summary>
        ///     Computes the U(user) value in the encryption dictionary.
        /// </summary>
        /// <remarks>
        ///     Corresponds to algorithm 3.4 on page 70 of the PDF specficiation.
        /// </remarks>
        /// <param name="options">
        ///     The user supplied PDF options that provides access to the passwords.
        /// </param>
        private void CreateUserEntry(SecurityOptions options)
        {
            // Encrypt the 32-byte padding string using the master key.
            Arc4 arc4 = new Arc4(masterKey);
            userEntry = new byte[32];
            arc4.Encrypt(Padding, userEntry);
        }

        /// <summary>
        ///     Encrypts the passed byte array using the ARC4 cipher.
        /// </summary>
        public byte[] Encrypt(byte[] data, PdfObjectId objectId)
        {
            Arc4 arc4 = new Arc4(ComputeEncryptionKey31(masterKey, objectId));
            arc4.Encrypt(data, data);
            return data;
        }

        /// <summary>
        ///     Access to the raw user entry byte array.
        /// </summary>
        /// <remarks>
        ///     Required for testing purposes;
        /// </remarks>
        internal byte[] UserEntry
        {
            get
            {
                return userEntry;
            }
            set
            {
                userEntry = value;
            }
        }

        /// <summary>
        ///     Access to the raw owner entry byte array.
        /// </summary>
        /// <remarks>
        ///     Required for testing purposes;
        /// </remarks>
        internal byte[] OwnerEntry
        {
            get
            {
                return ownerEntry;
            }
            set
            {
                ownerEntry = value;
            }
        }

        /// <summary>
        ///     Computes an encryption key that is used to encrypt string and stream data 
        ///     in the PDF document.
        /// </summary>
        /// <remarks>
        ///     Corresponds to algorithm 3.1 in section 3.5 of the PDF specficiation.
        /// </remarks>
        private static byte[] ComputeEncryptionKey31(
            byte[] masterKey, PdfObjectId objectId)
        {
            byte[] key = new byte[masterKey.Length + 5];

            // Compute a hash based on:
            // a) The master key
            Array.Copy(masterKey, 0, key, 0, masterKey.Length);
            int pos = masterKey.Length;
            // b) The low order 3 bytes of the object number.
            key[pos++] = (byte)(objectId.ObjectNumber & 0xff);
            key[pos++] = (byte)((objectId.ObjectNumber >> 8) & 0xff);
            key[pos++] = (byte)((objectId.ObjectNumber >> 16) & 0xff);
            // b) The low order 2 bytes of the generation number.
            key[pos++] = (byte)(objectId.GenerationNumber & 0xff);
            key[pos++] = (byte)((objectId.GenerationNumber >> 8) & 0xff);

            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(key);

            // The first n+5 bytes of the computed hash consitute the encryption key.
            Array.Copy(hash, 0, key, 0, masterKey.Length + 5);
            return key;
        }

        /// <summary>
        ///     Computes an encryption key that is used to encrypt string and stream data 
        ///     in the PDF document.
        /// </summary>
        /// <remarks>
        ///     Corresponds to algorithm 3.2 in section 3.5 of the PDF specficiation.
        /// </remarks>
        private static byte[] ComputeEncryptionKey32(
            byte[] paddedPassword, byte[] ownerEntry, int permissions, byte[] fileId)
        {
            MemoryStream ms = new MemoryStream();

            // Compute a hash based on:
            // a) The padded user password
            ms.Write(paddedPassword, 0, 32);
            // b) The O value in the encryption dictionary
            ms.Write(ownerEntry, 0, 32);
            // c) The P value in the encryption dictionary
            ms.Write(BitConverter.GetBytes(permissions), 0, 4);
            // d) The first element of the file identifier
            ms.Write(fileId, 0, fileId.Length);

            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(ms.ToArray());

            // The first 5 bytes of the computed hash consitute the encryption key.
            byte[] key = new byte[5];
            Array.Copy(hash, 0, key, 0, 5);
            return key;
        }

        /// <summary>
        ///     Pads or truncates a password string to exactly 32-bytes.
        /// </summary>
        /// <remarks>
        ///     Corresponds to step 1 of algorithm 3.2 on page 69 of the PDF 1.3 specficiation.
        /// </remarks>
        /// <param name="password">The password to pad or truncate.</param>
        /// <returns>
        ///     A byte array of length 32 bytes containing the padded or truncated password.
        /// </returns>
        private static byte[] PadPassword(string password)
        {
            byte[] result = new byte[32];
            if (password != null)
            {
                // Copy the first 32 characters to the result array.
                // TODO: what encoding should be used for non-ascii 
                // characters in the password?
                int pl = password.Length < 32 ? password.Length : 32;
                Encoding.ASCII.GetBytes(password, 0, pl, result, 0);

                // Pad the result using the standard padding characters defined 
                // in the PDF specification up to a length of 32 bytes.
                for (int x = pl, y = 0; x < 32; x++, y++)
                {
                    result[x] = Padding[y];
                }

            }
            else
            {
                // If the password is omitted, treat it as an empty string and
                // substitute the entire padding string in it's place.
                Padding.CopyTo(result, 0);
            }

            return result;
        }

        /// <summary>
        ///     Determines if the passed password matches the user password
        ///     used to initialise this security manager.
        /// </summary>
        /// <remarks>
        ///     Used for testing purposes only.  Corresponds to algorithm 3.5 in the
        ///     PDF 1.3 specification.
        /// </remarks>
        /// <returns>True if the password is correct.</returns>
        internal static bool CheckUserPassword(
            string password, byte[] userEntry, byte[] ownerEntry, int permissions, byte[] fileId)
        {
            return CheckUserPassword(
                PadPassword(password),
                userEntry,
                ownerEntry,
                permissions,
                fileId);
        }

        /// <summary>
        ///     Performs the actual checking of the user password.
        /// </summary>
        private static bool CheckUserPassword(
            byte[] paddedPassword, byte[] userEntry, byte[] ownerEntry, int permissions, byte[] fileId)
        {
            // Compute an encryption key from the supplied information.
            byte[] key = ComputeEncryptionKey32(
                paddedPassword, ownerEntry, permissions, fileId);

            // Decrpt the User entry using the key.
            Arc4 arc4 = new Arc4(key);
            byte[] decryptedUserEntry = new byte[32];
            arc4.Encrypt(userEntry, decryptedUserEntry);

            // Compare the two arrays, if they match then the 
            // supplied password is correct.
            return CompareArray(Padding, decryptedUserEntry);
        }

        /// <summary>
        ///     Checks the owner password.
        /// </summary>
        internal static bool CheckOwnerPassword(
            string password, byte[] userEntry, byte[] ownerEntry, int permissions, byte[] fileId)
        {
            // Compute an encryption key from the supplied information.
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(PadPassword(password));

            // Decrpt the Owner entry using the key.
            Arc4 arc4 = new Arc4(hash, 0, 5);
            byte[] decryptedOwnerEntry = new byte[32];
            arc4.Encrypt(ownerEntry, decryptedOwnerEntry);

            // The decryped owner entry is the correct user password,
            // then the owner password is also correct.
            return CheckUserPassword(decryptedOwnerEntry, userEntry, ownerEntry, permissions, fileId);
        }

        /// <summary>
        ///     Compares two byte arrays and returns true if they are equal.
        /// </summary>
        private static bool CompareArray(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
            {
                return false;
            }
            for (int x = 0; x < a1.Length; x++)
            {
                if (a1[x] != a2[x])
                {
                    return false;
                }
            }
            return true;
        }

    }
}