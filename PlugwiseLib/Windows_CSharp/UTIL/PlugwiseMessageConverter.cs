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
using plugwiseLib.BLL.BPC;

namespace PlugwiseLib.UTIL
{
    class PlugwiseMessageConverter
    {
        public PlugwiseMessageConverter()
        { }

        public PlugwisePowerUsageMessage ConvertToPowerUsage(PlugwiseMessage msg)
        {
            PlugwisePowerUsageMessage output = null;
            if (msg.Type == Convert.ToInt32(PlugwiseActions.Status))
            {
                output = new PlugwisePowerUsageMessage(msg);
            }
            return output;
        }

        public PlugwiseCalibrationMessage ConvertToCalibrationMessage(PlugwiseMessage msg)
        {
            PlugwiseCalibrationMessage output = null;
            if (msg.Type == Convert.ToInt32(PlugwiseActions.Calibration))
            {
                output = new PlugwiseCalibrationMessage(msg);
            }
            return output;
        }

        public PlugwiseStatusMessage ConvertToStatusMessage(PlugwiseMessage msg)
        {
            PlugwiseStatusMessage output = null;
            if (msg.Type == Convert.ToInt32(PlugwiseActions.powerinfo))
            {
                output = new PlugwiseStatusMessage(msg);
            }
            return output;
        }

        public PlugwiseHistoryPowerMessage ConvertToHistoryPowerMessage(PlugwiseMessage msg)
        {
            PlugwiseHistoryPowerMessage output = null;
            if (msg.Type == Convert.ToInt32(PlugwiseActions.history))
            {
                output = new PlugwiseHistoryPowerMessage(msg);
            }
            return output;
        }

        public PlugwiseDeviceInfo ConvertToDeviceInfoMessage(PlugwiseCalibrationMessage Cal,PlugwiseStatusMessage Stat)
        {
            PlugwiseDeviceInfo output = null;
            if ((Cal!= null)&(Stat!=null))
            {
                output = new PlugwiseDeviceInfo(Cal,Stat);
            }
            return output;
        }
    }
}
