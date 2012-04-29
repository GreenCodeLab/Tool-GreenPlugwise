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


namespace PlugwiseLib.UTIL
{
    class DeviceHelper
    {
        public DeviceHelper()
        {
        }

        public double PulseCorrection(float offRuis, float GainA, float GainB, float OffTot, float pulses, int timeMeasure)
        {

            //calculation is like this: 3600 * (((pow(value + offruis, 2.0) * gain_b) + ((value + offruis) * gain_a)) + offtot)
            double output = 0;
            
            double value = pulses /timeMeasure;
            

            //do: pow(value + offruis, 2.0) * gain_b
            double first = (Math.Pow(value + offRuis,2) * GainB);
            //do:((value + offruis) * gain_a)
            double second = ((value + offRuis) * GainA) ;
            //calculate the total and add  * 
            double total = (first + second) + OffTot;
            //multiply total by the timemeasure 3600 in the original formula
            output = timeMeasure * total;
            //go to watt
          
            return output;
        }

        public double ConverPulsesToWatt(int timespan,double pulses)
        {
            double output = ConvertPulsesToKwh(timespan, pulses);
            output = output * 1000;
            return output;
        }

        public double ConvertPulsesToKwh(int timespan, double pulses)
        {
            double output = (pulses / timespan) / 468.9385193;

            return output;
        }
    }
}
