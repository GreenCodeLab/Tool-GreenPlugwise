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

namespace PlugwiseLib.BLL.BPC
{
    public enum PlugwiseSerialPortRequest
    {
        on,
        off,
        status,
        calibration,
        powerinfo,
        history
    }
    public class PlugwiseSerialPortFactory
    {
        /// <summary>
        /// This factory returns the strings that will be sent to the serial port when a certain command needs the be activated
        /// </summary>
        /// <param name="req">The type of request that has to be sent to the plugs</param>
        /// <param name="mac">The mac address of the receiver</param>
        /// <returns></returns>
        public static string Create(PlugwiseSerialPortRequest req,string mac)
        {
            string output = "";
            switch (req)
            {
                case PlugwiseSerialPortRequest.on:
                    output = PlugwiseSerialPortFactory.GetOnMessage(mac);
                    break;
                case PlugwiseSerialPortRequest.off:
                    output = PlugwiseSerialPortFactory.getOffMessage(mac);
                    break;
                case PlugwiseSerialPortRequest.status:
                    output = PlugwiseSerialPortFactory.getStatusMessage(mac);
                    break;
                case PlugwiseSerialPortRequest.calibration:
                    output = PlugwiseSerialPortFactory.getCalibrationMessage(mac);
                    break;
                case PlugwiseSerialPortRequest.powerinfo:
                    output = PlugwiseSerialPortFactory.getPowerinfoMessage(mac);
                    break;
               
            }
            return output;
        }

        public static string Create(PlugwiseSerialPortRequest req,string logId,string mac)
        {
            string output = "";
            switch (req)
            {
                case PlugwiseSerialPortRequest.history:

                    output = PlugwiseSerialPortFactory.getHistoryMessage(mac,logId);
                    break;
            }
           
            return output;
        }

        private static string getPowerinfoMessage(string mac)
        {
            string output = "";
            Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.Zeros);
            string crcValue = crc.ComputeChecksumString("0012" + mac);
            output = PlugwiseSerialPortFactory.GetStart() + "0012" + mac + crcValue + PlugwiseSerialPortFactory.GetEnd();
            return output;
        }

        private static string GetOnMessage(string mac)
        {
            string output = "";
            Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.Zeros);
            string crcValue = crc.ComputeChecksumString("0017" + mac + "01");
            output =PlugwiseSerialPortFactory.GetStart() + "0017" + mac + "01" + crcValue + PlugwiseSerialPortFactory.GetEnd();
        
            return output;
        }

        private static string getOffMessage(string mac)
        {
            string output = "";
            Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.Zeros);
            string crcValue = crc.ComputeChecksumString("0017" + mac + "00");
            output = PlugwiseSerialPortFactory.GetStart() + "0017" + mac + "00" + crcValue + PlugwiseSerialPortFactory.GetEnd();
            return output;
 
        }

        private static string getStatusMessage(string mac)
        {
            string output = "";
            Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.Zeros);
            string crcValue = crc.ComputeChecksumString("0023" + mac);
            output = PlugwiseSerialPortFactory.GetStart() + "0023" + mac + crcValue + PlugwiseSerialPortFactory.GetEnd();
            return output;
        }

        private static string getCalibrationMessage(string mac)
        {
            string output = "";
            Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.Zeros);
            string crcValue = crc.ComputeChecksumString("0026" + mac);
            output = PlugwiseSerialPortFactory.GetStart() + "0026" + mac + crcValue + PlugwiseSerialPortFactory.GetEnd();
            return output;
        }

        private static string getHistoryMessage(string mac, string logId)
        {
            string output = "";
            Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.Zeros);
            string crcValue = crc.ComputeChecksumString("0048" + mac + logId);
            output = PlugwiseSerialPortFactory.GetStart() + "0048" + mac + logId + crcValue + PlugwiseSerialPortFactory.GetEnd();
            return output;
        }



        public static string GetStart()
        {
            return "" + (char)5 + (char)5 + (char)3 + (char)3;
        }

        public static string GetEnd()
        {
           
           return "" + (char)13 + (char)10;
        }
    }
}
