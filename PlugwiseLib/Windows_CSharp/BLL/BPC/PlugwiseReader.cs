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

using PlugwiseLib.BLL.BC;
using System.Collections.Generic;
using PlugwiseLib.UTIL;

namespace plugwiseLib.BLL.BPC
{
    /*public enum PlugwiseResponseCodes
    {
        power = 1,
        calibration = 2,
        powerinfo =3,
        history =4
        
    } */
    
     
    class PlugwiseReader
    {

        /// <summary>
        /// Constructor for the Plugwise Reader class. This class reads all the received plugwise messages and returns the appropriate messages
        /// </summary>
        public PlugwiseReader()
        {
        }

        /// <summary>
        /// This method reads the serial data and performs a conversion to a message object
        /// </summary>
        /// <param name="serialData">A string array containing the data received by the serial port</param>
        /// <returns>A list of PlugwiseMessages that represent the data that was received by the serial port</returns>
        public List<PlugwiseMessage> Read(string[] serialData)
        {
            List<PlugwiseMessage> output = new List<PlugwiseMessage>();
          
            string response = "";
            string mac = "";
            string sequence_nb = "";
            string type = "";
            

            foreach (string raw_msg in serialData)
            {
                PlugwiseMessage msg = new PlugwiseMessage();
            if (raw_msg.Length > 40)
            {
                //Length of serial =7
                type = raw_msg.Substring(0, 4);
                switch (type)
                {
                    //Power information request (current)
                    case "0013":
                        response = raw_msg.Substring(0, 56);
                        sequence_nb = response.Substring(4, 4);
                        mac = response.Substring(8, 16);
                        int oneSec = ConversionClass.HexStringToUInt16(response.Substring(24, 4));
                        int eightSec = ConversionClass.HexStringToUInt16(response.Substring(28, 4));
                        double allSec = ConversionClass.HexStringToUInt32(response.Substring(32, 8));
                        msg.Message = "" + eightSec + "|" + oneSec;
                        msg.Owner = mac;
                        msg.Type = Convert.ToInt16(PlugwiseActions.Status);
                        output.Add(msg);
                        break;

                    case "0024":
                        response = raw_msg.Substring(0, 70);
                        sequence_nb = response.Substring(4, 4);
                        mac = response.Substring(8, 16);
                        byte InternalYear = ConversionClass.HexToByte(response.Substring(24, 2));
                        byte InternalMonth = ConversionClass.HexToByte(response.Substring(26, 2));
                        int InternalMinutes = ConversionClass.HexStringToUInt16(response.Substring(28, 4));
                        uint CurrentLogAdr = ConversionClass.HexStringToUInt32(response.Substring(32, 8));
                        byte State = ConversionClass.HexToByte(response.Substring(40, 2));
                        byte Hertz = ConversionClass.HexToByte(response.Substring(42, 2));
                        string HardVersion = response.Substring(44, 16);
                        double FirmVersion = ConversionClass.HexStringToUInt32(response.Substring(60, 10));

                        msg.Message = "" + InternalYear + "|" + InternalMonth + "|" + InternalMinutes + "|" + CurrentLogAdr + "|" + State + "|" + Hertz + "|" + HardVersion + "|" + FirmVersion;
                        msg.Owner = mac;
                        msg.Type = Convert.ToInt16(PlugwiseActions.powerinfo);
                        output.Add(msg);
                        break;

                    case "0027":
                        response = raw_msg.Substring(0, 60);
                        sequence_nb = response.Substring(4, 4);
                        mac = response.Substring(8, 16);
                        float gaina = ConversionClass.HexStringToFloat(response.Substring(24, 8));
                        float gainb = ConversionClass.HexStringToFloat(response.Substring(32, 8));
                        float offTot = ConversionClass.HexStringToFloat(response.Substring(40, 8));
                        float offRuis = ConversionClass.HexStringToFloat(response.Substring(48, 8));
                        msg.Message = "" + gaina + "|" + gainb + "|" + offTot + "|" + offRuis;
                        msg.Owner = mac;
                        msg.Type = Convert.ToInt16(PlugwiseActions.Calibration);
                        output.Add(msg);
                        break;
                    
                    //Power buffer info
                    case "0049":
                        response = raw_msg.Substring(0, 100);
                        sequence_nb = response.Substring(4, 4);
                        mac = response.Substring(8, 16);
                        string h1 = response.Substring(24, 8);
                        uint v1 = ConversionClass.HexStringToUInt32(response.Substring(32, 8));
                        string h2 = response.Substring(40, 8);
                        uint v2 = ConversionClass.HexStringToUInt32(response.Substring(48, 8));
                        string h3 = response.Substring(56, 8);
                        uint v3 = ConversionClass.HexStringToUInt32(response.Substring(64, 8));
                        string h4 = response.Substring(72, 8);
                        uint v4 = ConversionClass.HexStringToUInt32(response.Substring(80, 8));
                        int logAddres = MessageHelper.ConvertPlugwiseLogHexToInt(int.Parse(response.Substring(88, 8), System.Globalization.NumberStyles.HexNumber));
                        msg.Message = "" + h1 + "|" + v1 + "|" + h2 + "|" + v2 + "|" + h3 + "|" + v3 + "|" + h4 + "|" + v4 + "|" + logAddres + "|" + ConversionClass.HexStringToUInt32(sequence_nb);
                        msg.Type = Convert.ToInt16(PlugwiseActions.history);
                        output.Add(msg);
                        break;
                }
            }                           
                  
            }
            return output;
        }
    }
}