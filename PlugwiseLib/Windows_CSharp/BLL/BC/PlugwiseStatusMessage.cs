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
using plugwiseLib.BLL.BPC;
using System.Text.RegularExpressions;
using PlugwiseLib.UTIL;

namespace PlugwiseLib.BLL.BC
{
    public class PlugwiseStatusMessage
    {
   
        private string _mac;     
        public string Mac
        {
            get { return _mac; }
            set { _mac = value; }
        }
        private PlugwiseActions _type;

        public PlugwiseActions Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private byte _InternalYear;
        public byte InternalYear
        {
            get { return _InternalYear; }
            set { _InternalYear = value; }
        }

        private byte _InternalMonth;
        public byte InternalMonth
        {
            get { return _InternalMonth; }
            set { _InternalMonth = value; }
        }

        private int _InternalMinutes;
        public int InternalMinutes
        {
            get { return _InternalMinutes; }
            set { _InternalMinutes = value; }
        }

        private int _CurrentLogAdr;
        public int CurrentLogAdr
        {
            get { return _CurrentLogAdr; }
            set { _CurrentLogAdr = value; }
        }

        private bool _State;
        public bool State
        {
            get { return _State; }
            set { _State = value; }
        }

        private byte _Hertz;
        public byte Hertz
        {
            get { return _Hertz; }
            set { _Hertz = value; }
        }

        private string _HardVersion;
        public string HardVersion
        {
            get { return _HardVersion; }
            set { _HardVersion = value; }
        }

        private double _FirmVersion;
        public double FirmVersion
        {
            get { return _FirmVersion; }
            set { _FirmVersion = value; }
        }
      

        public PlugwiseStatusMessage(PlugwiseMessage msg)
        {
            this.Mac = msg.Owner;
            this.Type = PlugwiseActions.Status;
            string[] values = Regex.Split(msg.Message, "\\|");
            this.InternalYear = byte.Parse(values[0]);
            this.InternalMonth = byte.Parse(values[1]);
            this.InternalMinutes = int.Parse(values[2]);
            this.CurrentLogAdr = MessageHelper.ConvertPlugwiseLogHexToInt(int.Parse(values[3]));
            this.State = Convert.ToBoolean(byte.Parse(values[4]));
            this.Hertz = byte.Parse(values[5]);
            this.HardVersion = values[6];
            this.FirmVersion = double.Parse(values[7]);
           // this.On = Convert.ToBoolean(Convert.ToInt32(values[0]));
          //  this.LastLog = MessageHelper.ConvertPlugwiseLogHexToInt((int)ConversionClass.HexStringToUInt32(values[1]));
            
        }

    }
}
