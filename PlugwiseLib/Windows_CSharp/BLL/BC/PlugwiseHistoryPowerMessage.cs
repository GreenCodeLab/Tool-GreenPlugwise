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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using plugwiseLib.BLL.BPC;
using System.Text.RegularExpressions;
using PlugwiseLib.UTIL;

namespace PlugwiseLib.BLL.BC
{
    public class PlugwiseHistoryPowerMessage
    {
        DateTime nullDate = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc); 

        private string _Mac;

        public string Mac
        {
            get { return _Mac; }
            set { _Mac = value; }
        }

        private PlugwiseActions _type;

        public PlugwiseActions Type
        {
            get { return _type; }
            set { _type = value; }
        }
        private List<PlugwiseHistoryMessage> _messages;

        public List<PlugwiseHistoryMessage> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        private int _LogAddress;

        public int LogAddress
        {
            get { return _LogAddress; }
            set { _LogAddress = value; }
        }

        private int _nb_Sequence;

        public int nb_Sequence
        {
            get { return _nb_Sequence; }
            set { _nb_Sequence = value; }
        }

        private DateTime prevHour;

        public PlugwiseHistoryPowerMessage(PlugwiseMessage msg)
        {

            List<PlugwiseMessage> output = new List<PlugwiseMessage>();

            prevHour = nullDate;
            
            Messages = new List<PlugwiseHistoryMessage>();

            this.Mac = msg.Owner;
            string[] values = Regex.Split(msg.Message, "\\|");
            for (int i = 0; i < 9; i=i+2)
            {
           //     PlugwiseHistoryPowerMessage 
                PlugwiseHistoryMessage Messag = new PlugwiseHistoryMessage();
                Messag.MeasurementValue = int.Parse(values[i + 1]);
                Messag.RawHourvalue = values[i];
                this.Messages.Add(Messag);
            }
            this.LogAddress = Convert.ToInt32(values[8]);
            this.Type = PlugwiseActions.history;
            this.nb_Sequence = Convert.ToInt32(values[9]);
        }

       

        private bool CheckHours(DateTime hours)
        {
            bool output = false;
            if (prevHour == null)
            {
                prevHour = hours;
                output = true;
                
            }
            else
            {
                if (hours.CompareTo(prevHour) == 1)
                {
                    output = true;
                }
            }
            return output;
        }
    }


    public class PlugwiseHistoryMessage
    {
        private DateTime _Hourvalue;

        public DateTime Hourvalue
        {
            get { return _Hourvalue; }
            set { _Hourvalue = value; }
        }

        private string _RawHourvalue;

        public string RawHourvalue
        {
            get { return _RawHourvalue; }
            set { _RawHourvalue = value; }
        }

        private int _MeasurementValue;

        public int MeasurementValue
        {
            get { return _MeasurementValue; }
            set { _MeasurementValue = value; }
        }

        private double _Watt;

        public double Watt
        {
            get { return _Watt; }
            set { _Watt = value; }
        }

        public PlugwiseHistoryMessage()
        {
        }
    }
}
