namespace Fonet.Util
{
    using System;
    using System.Collections;

    internal class StringTokenizer : IEnumerator
    {
        private int currentPosition;
        private int newPosition;
        private int maxPosition;
        private String str;
        private String delimiters;
        private bool retDelims;

        /// <summary>
        ///     maxDelimChar stores the value of the delimiter character with 
        ///     the highest value. It is used to optimize the detection of 
        ///     delimiter characters.
        /// </summary>
        private char maxDelimChar;

        /// <summary>
        ///     Set maxDelimChar to the highest char in the delimiter set.
        /// </summary>
        private void SetMaxDelimChar()
        {
            if (delimiters == null)
            {
                maxDelimChar = (char)(0);
                return;
            }

            char m = (char)(0);
            for (int i = 0; i < delimiters.Length; i++)
            {
                char c = delimiters[i];
                if (m < c)
                {
                    m = c;
                }
            }
            maxDelimChar = m;
        }

        /// <summary>
        ///     Constructs a string tokenizer for the specified string. All 
        ///     characters in the <i>delim</i> argument are the delimiters 
        ///     for separating tokens.<br/>
        ///     If the <i>returnDelims</i> flag is <i>true</i>, then 
        ///     the delimiter characters are also returned as tokens. Each delimiter 
        ///     is returned as a string of length one. If the flag is 
        ///     <i>false</i>, the delimiter characters are skipped and only 
        ///     serve as separators between tokens. 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="delim"></param>
        /// <param name="returnDelims"></param>
        public StringTokenizer(String str, String delim, bool returnDelims)
        {
            currentPosition = 0;
            newPosition = -1;
            this.str = str;
            maxPosition = str.Length;
            delimiters = delim;
            retDelims = returnDelims;
            SetMaxDelimChar();
        }

        /// <summary>
        ///     Constructs a string tokenizer for the specified string. The 
        ///     characters in the <code>delim</code> argument are the delimiters 
        ///     for separating tokens. Delimiter characters themselves will not 
        ///     be treated as tokens.
        /// </summary>
        /// <param name="str">a string to be parsed.</param>
        /// <param name="delim">the delimiters.</param>
        public StringTokenizer(String str, String delim) : this(str, delim, false)
        {
        }

        /// <summary>
        ///     Constructs a string tokenizer for the specified string. The 
        ///     tokenizer uses the default delimiter set, which is the space 
        ///     character, the tab character, the newline character, the 
        ///     carriage-return character, and the form-feed character. 
        ///     Delimiter characters themselves will not be treated as tokens.
        /// </summary>
        /// <param name="str">a string to be parsed</param>
        public StringTokenizer(String str) : this(str, " \t\n\r\f", false)
        {
        }

        /// <summary>
        ///     Skips delimiters starting from the specified position. If 
        ///     retDelims is false, returns the index of the first non-delimiter 
        ///     character at or after startPos. If retDelims is true, startPos 
        ///     is returned.
        /// </summary>
        /// <param name="startPos"></param>
        /// <returns></returns>
        private int SkipDelimiters(int startPos)
        {
            if (delimiters == null)
            {
                throw new NullReferenceException();
            }

            int position = startPos;
            while (!retDelims && position < maxPosition)
            {
                char c = str[position];
                if ((c > maxDelimChar) || (delimiters.IndexOf(c) < 0))
                {
                    break;
                }
                position++;
            }
            return position;
        }

        /// <summary>
        ///     Skips ahead from startPos and returns the index of the next 
        ///     delimiter character encountered, or maxPosition if no such 
        ///     delimiter is found.
        /// </summary>
        /// <param name="startPos"></param>
        /// <returns></returns>
        private int ScanToken(int startPos)
        {
            int position = startPos;
            while (position < maxPosition)
            {
                char c = str[position];
                if ((c <= maxDelimChar) && (delimiters.IndexOf(c) >= 0))
                {
                    break;
                }
                position++;
            }
            if (retDelims && (startPos == position))
            {
                char c = str[position];
                if ((c <= maxDelimChar) && (delimiters.IndexOf(c) >= 0))
                {
                    position++;
                }
            }
            return position;
        }

        /// <summary>
        ///     Returns the next token from this string tokenizer.
        /// </summary>
        /// <returns>the next token from this string tokenizer.</returns>
        public virtual String NextToken()
        {
            /*
             * If next position already computed in hasMoreElements() and
             * delimiters have changed between the computation and this invocation,
             * then use the computed value.
             */
            currentPosition = (newPosition >= 0) ?
                newPosition : SkipDelimiters(currentPosition);

            newPosition = -1;

            if (currentPosition >= maxPosition)
            {
                throw new InvalidOperationException();
            }
            int start = currentPosition;
            currentPosition = ScanToken(currentPosition);
            return str.Substring(start, currentPosition - start);
        }

        /// <summary>
        ///     Returns the same value as the <code>hasMoreTokens</code> method. 
        ///     It exists so that this class can implement the 
        ///     <i>Enumeration</i> interface. 
        /// </summary>
        /// <returns>
        /// <i>true</i> if there are more tokens; <i>false</i> 
        /// otherwise.</returns>
        public bool MoveNext()
        {
            /*
             * Temporary store this position and use it in the following
             * nextToken() method only if the delimiters have'nt been changed in
             * that nextToken() invocation.
             */
            newPosition = SkipDelimiters(currentPosition);
            return (newPosition < maxPosition);
        }

        /// <summary>
        /// Tests if there are more tokens available from this tokenizer's 
        /// string.  If this method returns <tt>true</tt>, then a subsequent 
        /// call to <tt>nextToken</tt> with no argument will successfully 
        /// return a token.
        /// </summary>
        /// <returns>
        /// <code>true</code> if and only if there is at least one token in 
        /// the string after the current position; <code>false</code> otherwise.
        /// </returns>
        public bool HasMoreTokens()
        {
            /*
             * Temporary store this position and use it in the following
             * NextToken() method only if the delimiters have'nt been changed in
             * that NextToken() invocation.
             */
            newPosition = SkipDelimiters(currentPosition);
            return (newPosition < maxPosition);
        }

        /// <summary>
        /// Returns the same value as the <code>nextToken</code> method, except 
        /// that its declared return value is <code>Object</code> rather than 
        /// <code>String</code>. It exists so that this class can implement the
        /// <code>Enumeration</code> interface. 
        /// </summary>
        public Object Current
        {
            get
            {
                return NextToken();
            }
        }

        public void Reset()
        {
        }

        /// <summary>
        /// Calculates the number of times that this tokenizer's 
        /// <code>nextToken</code> method can be called before it generates an 
        /// exception. The current position is not advanced.
        /// </summary>
        /// <returns>
        /// the number of tokens remaining in the string using the current 
        /// delimiter set.</returns>
        public virtual int CountTokens()
        {
            int count = 0;
            int currpos = currentPosition;
            while (currpos < maxPosition)
            {
                currpos = SkipDelimiters(currpos);
                if (currpos >= maxPosition)
                {
                    break;
                }
                currpos = ScanToken(currpos);
                count++;
            }
            return count;
        }
    }
}