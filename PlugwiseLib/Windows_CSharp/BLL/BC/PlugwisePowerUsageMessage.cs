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

namespace PlugwiseLib.BLL.BC
{
    public class PlugwisePowerUsageMessage
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

        private float _eightSec;

        public float EightSec
        {
            get { return _eightSec; }
            set { _eightSec = value; }
        }

        private float _oneSec;

        public float OneSec
        {
            get { return _oneSec; }
            set { _oneSec = value; }
        }

        public PlugwisePowerUsageMessage(PlugwiseMessage msg)
        {
            this.Mac = msg.Owner;
            this.Type = PlugwiseActions.powerinfo;
            string[] values = Regex.Split(msg.Message, "\\|");
            this.EightSec = int.Parse(values[0]);
            this.OneSec = int.Parse(values[1]);
        }
    }
}
