//------------------ BEGIN LICENSE BLOCK ------------------
//
// Project : Green Code Lab Plugwyse Library
// Description :
// Author: Green Code Lab
// Website: http://greencodelab.fr
// Version: 1.0
// Supports: Windows
//
// Original project : http://plugwiselib.codeplex.com/
// Copyright (c) 2012 Green Code Lab
// Licensed under the GPL license.
// See http://www.gnu.org/licenses/gpl.html
//
//------------------- END LICENSE BLOCK -------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace PlugwiseLib.UTIL
{
    static class ConversionClass
    {
        public static float HexStringToFloat(string pvVal)
        {
            int discarded = 0;
            return BitConverter.ToSingle(ReverseBytes(GetBytes(pvVal, out discarded)), 0);
        }

        public static ushort HexStringToUInt16(string pvVal)
        {
            ushort num = 0;
            if (pvVal == null)
            {
                return num;
            }
            int discarded = 0;
            if (pvVal.Length == 2)
            {
                pvVal = "00" + pvVal;
            }
            if (pvVal.Length != 4)
            {
                return 0;
            }
            num = BitConverter.ToUInt16(GetBytes(pvVal, out discarded), 0);
            return (ushort)((num >> 8) | (num << 8));
        }

        public static uint HexStringToUInt32(string pvVal)
        {
            int discarded = 0;
            if (pvVal != null)
            {
                while (pvVal.Length < 8)
                {
                    pvVal = "00" + pvVal;
                }
                if (pvVal.Length != 8)
                {
                    return 0;
                }
                return BitConverter.ToUInt32(ReverseBytes(GetBytes(pvVal, out discarded)), 0);
            }
            return 0;
        }




        public static byte[] GetBytes(string hexString, out int discarded)
        {
            discarded = 0;
            if (hexString == null)
            {
                return new byte[0];
            }
            string str = "";
            for (int i = 0; i < hexString.Length; i++)
            {
                char c = hexString[i];
                if (IsHexDigit(c))
                {
                    str = str + c;
                }
                else
                {
                    discarded++;
                }
            }
            if ((str.Length % 2) != 0)
            {
                discarded++;
                str = str.Substring(0, str.Length - 1);
            }
            int num2 = str.Length / 2;
            byte[] buffer = new byte[num2];
            int num3 = 0;
            for (int j = 0; j < buffer.Length; j++)
            {
                string hex = new string(new char[] { str[num3], str[num3 + 1] });
                buffer[j] = HexToByte(hex);
                num3 += 2;
            }
            return buffer;
        }

        public static bool IsHexDigit(char c)
        {
            int num2 = Convert.ToInt32('A');
            int num3 = Convert.ToInt32('0');
            c = char.ToUpper(c);
            int num = Convert.ToInt32(c);
            return (((num >= num2) && (num < (num2 + 6))) || ((num >= num3) && (num < (num3 + 10))));
        }

        public static byte HexToByte(string hex)
        {
            if ((hex.Length > 2) || (hex.Length <= 0))
            {
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            }
            return byte.Parse(hex, NumberStyles.HexNumber);
        }

        private static byte[] ReverseBytes(byte[] lvBytes)
        {
            for (int i = 0; i <= (Math.Round((double)(lvBytes.Length / 2)) - 1.0); i++)
            {
                byte num = lvBytes[lvBytes.Length - (1 + i)];
                lvBytes[lvBytes.Length - (1 + i)] = lvBytes[i];
                lvBytes[i] = num;
            }
            return lvBytes;
        }
    }

}
