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
using PlugwiseLib.BLL.BC;

namespace PlugwiseLib.UTIL
{
    public class MessageHelper
    {
        public static string ConvertIntToPlugwiseLogHex(int logId)
        {
            string output ="";
            int newlogId = 278528 + (32 * logId);
            output = newlogId.ToString("X8");
            return output;

        }

        public static int ConvertPlugwiseLogHexToInt(int logId)
        {
            int output = 0;
            int newlogId = (logId - 278528) / 32;
            output = newlogId;
            return output;

        }
        
        /// <summary>
        /// This method checks if a plugwise hour number deliveres a correct date.
        /// </summary>
        /// <param name="hours">THe number of hours to add to the start date</param>
        /// <returns>true when the date is correct, false when the date is not correct</returns>
        public static bool TryCalculatePlugwiseDate(string logdate)
        {

            bool output = true;
            try
            {
                byte logYear = ConversionClass.HexToByte(logdate.Substring(0, 2));
                byte logMonth = ConversionClass.HexToByte(logdate.Substring(2, 2));
                int logMinutes = ConversionClass.HexStringToUInt16(logdate.Substring(4, 4));
                int hours = logMinutes / 60;
                int days = hours / 24;
                hours = hours - (days * 24);
                logMinutes = logMinutes - ((days * 24 * 60) + (hours * 60));
                DateTime date = new DateTime(2000 + logYear, logMonth, days+1, hours, logMinutes, 0, DateTimeKind.Utc);
                output = true;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                output = false;
            }

            return output;
        }


        public static DateTime CalculatePlugwiseDate(PlugwiseHistoryMessage Hmsg, PlugwiseDeviceInfo devInfo)
        {
            try
            {
                DateTime today = System.DateTime.Now;
                DateTime CurrentDate = MessageHelper.CalculateDate(devInfo.InternalYear, devInfo.InternalMonth, devInfo.InternalMinutes);
                

                byte logYear = ConversionClass.HexToByte(Hmsg.RawHourvalue.Substring(0, 2));
                byte logMonth = ConversionClass.HexToByte(Hmsg.RawHourvalue.Substring(2, 2));
                int logMinutes = ConversionClass.HexStringToUInt16(Hmsg.RawHourvalue.Substring(4, 4));
                DateTime logDate = MessageHelper.CalculateDate(logYear, logMonth, logMinutes);
                TimeSpan logDatediff = CurrentDate.Subtract(logDate);
                logDate = today.Subtract(logDatediff);
                return logDate;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new Exception("Error on calculate Date");
            }
        }

         public static DateTime CalculateDate(byte Year, byte Month, int Minute)
        {
            try
            {
                int CurrentMinutes = 0;
                int CurrentMonth = 0;
                int Currenthours =0;
                int Currentyears = (int)(Year+2000);
                if (Month != 0)
                {
                    CurrentMonth = (int)(Month);
                }
                else
                {
                    CurrentMonth = 1;
                }
                int Currentdays = (Minute / 1440) + 1;
                if (Minute > (Currentdays * 24 * 60))
                {
                    Currenthours = (Minute / 60) - (Currentdays * 24);
                }
                else
                {
                    Currenthours = Minute / 60 - ((Currentdays - 1) * 24 );
                }
               
                DateTime Date = new DateTime(Currentyears, CurrentMonth, Currentdays, Currenthours, 0, 0, DateTimeKind.Utc);

                return Date;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new Exception("Error on calculate Date");
            }
        }

    }
}
