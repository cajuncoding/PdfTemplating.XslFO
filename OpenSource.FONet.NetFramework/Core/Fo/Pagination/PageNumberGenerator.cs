namespace Fonet.Fo.Pagination
{
    using System;
    using System.Text;

    internal class PageNumberGenerator
    {
        private string format;
        private char groupingSeparator;
        private int groupingSize;
        private int letterValue;

        private const int DECIMAL = 1; // '0*1'
        private const int LOWERALPHA = 2; // 'a'
        private const int UPPERALPHA = 3; // 'A'
        private const int LOWERROMAN = 4; // 'i'
        private const int UPPERROMAN = 5; // 'I'

        private int formatType = DECIMAL;
        private int minPadding = 0;

        private string[] zeros = {
            "", "0", "00", "000", "0000", "00000"
        };

        public PageNumberGenerator(string format, char groupingSeparator,
                                   int groupingSize, int letterValue)
        {
            this.format = format;
            this.groupingSeparator = groupingSeparator;
            this.groupingSize = groupingSize;
            this.letterValue = letterValue;

            int fmtLen = format.Length;
            if (fmtLen == 1)
            {
                if (format.Equals("1"))
                {
                    formatType = DECIMAL;
                    minPadding = 0;
                }
                else if (format.Equals("a"))
                {
                    formatType = LOWERALPHA;
                }
                else if (format.Equals("A"))
                {
                    formatType = UPPERALPHA;
                }
                else if (format.Equals("i"))
                {
                    formatType = LOWERROMAN;
                }
                else if (format.Equals("I"))
                {
                    formatType = UPPERROMAN;
                }
                else
                {
                    formatType = DECIMAL;
                    minPadding = 0;
                }
            }
            else
            {
                for (int i = 0; i < fmtLen - 1; i++)
                {
                    if (format[i] != '0')
                    {
                        formatType = DECIMAL;
                        minPadding = 0;
                    }
                    else
                    {
                        minPadding = fmtLen - 1;
                    }
                }
            }
        }

        public string makeFormattedPageNumber(int number)
        {
            string pn = null;
            if (formatType == DECIMAL)
            {
                pn = number.ToString();
                if (minPadding >= pn.Length)
                {
                    int nz = minPadding - pn.Length + 1;
                    pn = zeros[nz] + pn;
                }
            }
            else if ((formatType == LOWERROMAN) || (formatType == UPPERROMAN))
            {
                pn = makeRoman(number);
                if (formatType == UPPERROMAN)
                {
                    pn = pn.ToUpper();
                }
            }
            else
            {
                pn = makeAlpha(number);
                if (formatType == UPPERALPHA)
                {
                    pn = pn.ToUpper();
                }
            }
            return pn;
        }

        private string makeRoman(int num)
        {
            int[] arabic = {
                1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1
            };
            string[] roman = {
                "m", "cm", "d", "cd", "c", "xc", "l", "xl", "x", "ix", "v", "iv",
                "i"
            };

            int i = 0;
            StringBuilder romanNumber = new StringBuilder();

            while (num > 0)
            {
                while (num >= arabic[i])
                {
                    num = num - arabic[i];
                    romanNumber.Append(roman[i]);
                }
                i = i + 1;
            }
            return romanNumber.ToString();
        }

        private string makeAlpha(int num)
        {
            string letters = "abcdefghijklmnopqrstuvwxyz";
            StringBuilder alphaNumber = new StringBuilder();

            int nbase = 26;
            int rem = 0;
            num--;
            if (num < nbase)
            {
                alphaNumber.Append(letters[num]);
            }
            else
            {
                while (num >= nbase)
                {
                    rem = num % nbase;
                    alphaNumber.Append(letters[rem]);
                    num = num / nbase;
                }
                alphaNumber.Append(letters[num - 1]);
            }
            char[] strArray = alphaNumber.ToString().ToCharArray();
            Array.Reverse(strArray);
            return new string(strArray).ToString();
        }
    }
}