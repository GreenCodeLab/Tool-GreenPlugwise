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
using System.Text;
public enum InitialCrcValue { Zeros, NonZero1 = 0xffff, NonZero2 = 0x1D0F }

public class Crc16Ccitt
{
    const ushort poly = 4129;
    ushort[] table = new ushort[256];
    ushort initialValue = 0;

    public ushort ComputeChecksum(byte[] bytes)
    {
        ushort crc = this.initialValue;
        for (int i = 0; i < bytes.Length; i++)
        {
            crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & bytes[i]))]);
        }
        return crc;
    }

    public byte[] ComputeChecksumBytes(byte[] bytes)
    {
        ushort crc = ComputeChecksum(bytes);
        return new byte[] { (byte)(crc >> 8), (byte)(crc & 0x00ff) };
    }

    public string ComputeChecksumString(string value)
    {
        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
        byte[] ba = this.ComputeChecksumBytes(encoding.GetBytes(value));
        StringBuilder sb = new StringBuilder(ba.Length * 2);

        foreach (byte b in ba)
        {
            sb.AppendFormat("{0:x2}", b);
        }
        return sb.ToString();
    }

    public Crc16Ccitt(InitialCrcValue initialValue)
    {
        this.initialValue = (ushort)initialValue;
        ushort temp, a;
        for (int i = 0; i < table.Length; i++)
        {
            temp = 0;
            a = (ushort)(i << 8);
            for (int j = 0; j < 8; j++)
            {
                if (((temp ^ a) & 0x8000) != 0)
                {
                    temp = (ushort)((temp << 1) ^ poly);
                }
                else
                {
                    temp <<= 1;
                }
                a <<= 1;
            }
            table[i] = temp;
        }
    }
}
