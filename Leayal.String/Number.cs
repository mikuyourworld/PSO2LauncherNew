﻿using System;

namespace Leayal
{
    public static class Number
    {
        public static int Parse(string str)
        {
            return String.ToInt(str);
        }

        /// <summary>
        /// Try parse a int from a string. Return True if the string is fully parsed.
        /// </summary>
        /// <param name="str">String. The string which will be parsed.</param>
        /// <param name="result">Out int.</param>
        /// <returns>Bool. True if the string is fully parsed.</returns>
        public unsafe static bool TryParse(string str, out int result)
        {
            result = 0;
            int pow = 0;
            bool returnResult = true;
            fixed (char* c = str)
                for (int i = str.Length; i > 0; i--)
                    if (String.charint.ContainsKey(c[i]))
                    {
                        if (pow > 0)
                            result += (int)Math.Pow(10, pow) + String.charint[c[i]];
                        else
                            result += String.charint[c[i]];
                        pow += 1;
                    }
                    else
                        returnResult = false;
            return returnResult;
        }
    }
}
