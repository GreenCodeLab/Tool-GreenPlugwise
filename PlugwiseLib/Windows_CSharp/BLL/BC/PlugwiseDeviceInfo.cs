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

namespace PlugwiseLib.BLL.BC
{
    public class PlugwiseDeviceInfo
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

        private float _GainA;

        public float GainA
        {
            get { return _GainA; }
            set { _GainA = value; }
        }
        private float _GainB;

        public float GainB
        {
            get { return _GainB; }
            set { _GainB = value; }
        }
        private float _offTot;

        public float OffTot
        {
            get { return _offTot; }
            set { _offTot = value; }
        }
        private float _offRuis;

        public float OffRuis
        {
            get { return _offRuis; }
            set { _offRuis = value; }
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
      
      
        public PlugwiseDeviceInfo(PlugwiseCalibrationMessage cal, PlugwiseStatusMessage Stat)
        {
            this.Mac = cal.Mac;
            //this.Type = PlugwiseActions.Calibration;
            
            this.GainA = cal.GainA;
            this.GainB = cal.GainB;

            this.OffRuis = cal.OffRuis;
            this.OffTot = cal.OffTot;
            this.InternalYear = Stat.InternalYear;
            this.InternalMonth = Stat.InternalMonth;
            this.InternalMinutes = Stat.InternalMinutes;
            this.CurrentLogAdr = Stat.CurrentLogAdr;
            this.State = Stat.State;
            this.Hertz = Stat.Hertz;
            this.HardVersion = Stat.HardVersion;
            this.FirmVersion = Stat.FirmVersion;

        }
    }
}
