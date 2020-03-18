namespace Fonet.Pdf.Security
{
    /// <summary>
    /// ARC4 is a fast, simple stream encryption algorithm that is
    /// compatible with RSA Security's RC4 algorithm.
    /// </summary>
    internal class Arc4
    {
        private byte[] state = new byte[256];
        private int x;
        private int y;

        internal Arc4()
        {
        }

        internal Arc4(byte[] key)
        {
            Initialise(key);
        }

        internal Arc4(byte[] key, int offset, int length)
        {
            // Extract the key from the passed array and call initialise.
            byte[] k2 = new byte[length];
            for (int x = 0; x < length; x++)
            {
                k2[x] = key[offset + x];
            }
            Initialise(k2);
        }

        /// <summary>
        ///     Initialises internal state from the passed key.
        /// </summary>
        /// <remarks>
        ///     Can be called again with a new key to reuse an Arc4 instance.
        /// </remarks>
        /// <param name="key">The encryption key.</param>
        internal void Initialise(byte[] key)
        {
            for (int i = 0; i < 256; i++)
            {
                state[i] = (byte)i;
            }
            for (int i = 0, j = 0; i < 256; i++)
            {
                j = (j + state[i] + key[i % key.Length]) % 256;
                byte t = state[i];
                state[i] = state[j];
                state[j] = t;
            }
            x = 0;
            y = 0;
        }

        /// <summary>
        ///     Encrypts or decrypts the passed byte array.
        /// </summary>
        /// <param name="dataIn">
        ///     The data to be encrypted or decrypted.
        /// </param>
        /// <param name="dataOut">
        ///     The location that the encrypted or decrypted data is to be placed.
        ///     The passed array should be at least the same size as dataIn.
        ///     It is permissible for the same array to be passed for both dataIn
        ///     and dataOut.
        /// </param>
        internal void Encrypt(byte[] dataIn, byte[] dataOut)
        {
            for (int x = 0; x < dataIn.Length; x++)
            {
                dataOut[x] = (byte)(dataIn[x] ^ Arc4Byte());
            }
        }

        /// <summary>
        ///     Generates a pseudorandom byte used to encrypt or decrypt.
        /// </summary>
        private byte Arc4Byte()
        {
            x = (x + 1) % 256;
            y = (y + state[x]) % 256;
            byte temp = state[x];
            state[x] = state[y];
            state[y] = temp;
            return state[(state[x] + state[y]) % 256];
        }

    }
}