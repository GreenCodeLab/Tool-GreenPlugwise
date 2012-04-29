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
using System.IO.Ports;
using PlugwiseLib.BLL.BC;
using plugwiseLib.BLL.BPC;
using System.Text.RegularExpressions;
using System.Diagnostics;

using System.Threading;
using System.IO;
using PlugwiseLib.BLL.BPC;
using PlugwiseLib.UTIL;

namespace PlugwiseLib
{
    public class plugwiseControl
    {
        private PlugwiseMessageConverter msgconv;
        private SerialPort port;
        private PlugwiseActions currentAction;
        public delegate void PlugwiseDataReceivedEvent(object sender, System.EventArgs e, List<PlugwiseMessage> data);
        public event PlugwiseDataReceivedEvent DataReceived;

        private int LastLogAdr;
        private PlugwiseReader reader;
        private List<PlugwisePowerUsageMessage> PowerUsage;
        public PlugwiseCalibrationMessage Calibration;
        private PlugwiseStatusMessage StatusMessage;
        private PlugwiseDeviceInfo DeviceInfoMessage;
        private DeviceHelper DeviceInfo;
        private PlugwiseHistoryPowerMessage HistoryMessage;
        private double CurrentConsumptionWatt;
        

        /// <summary>
        /// Constructor for the Plugwise Control class
        /// </summary>
        /// <param name="serialPort">The serial port name that the plugwise stick takes</param>
        public plugwiseControl(string serialPort)
        {
            try
            {
                port = new SerialPort(serialPort);
                port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                DataReceived += DataReceivedHandler;
                port.BaudRate = 115200;
                port.ReadBufferSize = 12288;
                currentAction = PlugwiseActions.None;
                reader = new PlugwiseReader();
                msgconv = new PlugwiseMessageConverter();
                DeviceInfo = new DeviceHelper();
                //PowerUsage = null;
                Calibration = null;
                StatusMessage = null;
                HistoryMessage = null;
                DeviceInfoMessage = null;
                LastLogAdr = 0;
                CurrentConsumptionWatt = 0;
                PowerUsage = new List<PlugwisePowerUsageMessage>();
                
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to plug.");
            }
        }

  

        public PlugwiseDeviceInfo InitPlug(string mac,string serialPort)
        {
            try
            {
                plugwiseControl control = new plugwiseControl(serialPort);
               // port.ReceivedBytesThreshold = 1;
                control.port.ReceivedBytesThreshold = 1;
                control.Open();
                Thread.Sleep(10);
                control.Action(mac, PlugwiseLib.BLL.BC.PlugwiseActions.Calibration);
                Thread.Sleep(800);
                control.Action(mac, PlugwiseLib.BLL.BC.PlugwiseActions.Status);
                Thread.Sleep(800);
                control.Close();
                
                return control.DeviceInfoMessage;
            }
            catch (Exception e)
            {
                throw new Exception("Could not initialize plug.");
            }
            finally
            {
                Console.WriteLine("Initialisation OK");
            }
        }

        public void DataReceivedHandler(object sender, System.EventArgs e, List<PlugwiseMessage> msg)
        {
            try
            {
                PowerUsage = new List<PlugwisePowerUsageMessage>();
                foreach (PlugwiseMessage received_msg in msg)
                {
                    
                    switch (received_msg.Type)
                    {
                        case 4: 
                            //Current Consumption
                            PowerUsage.Add(msgconv.ConvertToPowerUsage(received_msg));
                            break;
                        case 5: 
                            //Calibration
                            Calibration = msgconv.ConvertToCalibrationMessage(received_msg);
                            break;

                        case 6: 
                            //powerinfo
                            StatusMessage = msgconv.ConvertToStatusMessage(received_msg);
                            DeviceInfoMessage = msgconv.ConvertToDeviceInfoMessage(Calibration, StatusMessage);
                            break;

                        case 7 : 
                            //Calibration
                            HistoryMessage = msgconv.ConvertToHistoryPowerMessage(received_msg);
                            break;
                    }
   
                    
              }
            }
            catch (NullReferenceException nre)
            {
                Console.WriteLine(
                    "Cannot read Data received (NullReferenceException)" + nre.Message + ".\n");
            }

            catch (InvalidOperationException ioe)
            {
                Console.WriteLine(
                     "Cannot read Data received (InvalidOperationException)" + ioe.Message + ".\n");
            }

            catch (Exception er)
            {
                Console.WriteLine("Cannot read Data received (Other execption)" + er.Message + ".\n");
            }
            
        }

        public double MeasureCurrentConsumption(string mac, PlugwiseDeviceInfo Calib, string serialPort)
        {
            try
            {
                plugwiseControl control = new plugwiseControl(serialPort);
                control.Open();
                control.Action(mac, PlugwiseLib.BLL.BC.PlugwiseActions.powerinfo);
                while (control.PowerUsage.Count == 0)
                { 
                    Thread.Sleep(1); 
                }
                control.Close();
                double pulse = DeviceInfo.PulseCorrection(Calib.OffRuis, Calib.GainA, Calib.GainB, Calib.OffTot, control.PowerUsage[0].EightSec, 8);
                CurrentConsumptionWatt = DeviceInfo.ConverPulsesToWatt(8, pulse);
                return CurrentConsumptionWatt;
            }
            catch (Exception e)
            {
                throw new Exception("Could not measure consumption .");
            }
        }

        public double[] MeasureAllCurrentConsumption(string[] mac, PlugwiseDeviceInfo[] Calib, int[] Plug_num, string serialPort)
        {
            try
            {
                double[] CurrentConsumptionWatt_a = new double[10];

                plugwiseControl control = new plugwiseControl(serialPort);
                control.port.ReceivedBytesThreshold = 25;
                control.Open();
                bool[] Plug_inv = new bool[10] {false, false, false, false, false, false, false, false, false, false};
                for (int i = 0; i < Plug_num.Length; i=i+2)
                {
                    control.Action(mac[Plug_num[i]], PlugwiseLib.BLL.BC.PlugwiseActions.powerinfo);
                   
                }
                while (control.PowerUsage.Count == 0)
                {
                    Thread.Sleep(1);
                }             
                //control.Close();
                foreach (PlugwisePowerUsageMessage received_power in control.PowerUsage)
                {
                int ind =    Array.FindIndex(mac, s => s.Equals(received_power.Mac));

                if (ind != (-1))
                {
                    double pulse = DeviceInfo.PulseCorrection(Calib[ind].OffRuis, Calib[ind].GainA, Calib[ind].GainB, Calib[ind].OffTot, received_power.EightSec, 8);

                    CurrentConsumptionWatt_a[ind] = DeviceInfo.ConverPulsesToWatt(8, pulse);
                    Plug_inv[ind] = true;
                }
                }
                control.Close();
                plugwiseControl control2 = new plugwiseControl(serialPort);
                control2.port.ReceivedBytesThreshold = 25;
                control2.Open();
                for (int i = 1; i < Plug_num.Length; i=i+2)
                {
                    control2.Action(mac[Plug_num[i]], PlugwiseLib.BLL.BC.PlugwiseActions.powerinfo);

                }
                while (control2.PowerUsage.Count == 0)
                {
                    Thread.Sleep(1);
                }
                control2.Close();
                foreach (PlugwisePowerUsageMessage received_power2 in control2.PowerUsage)
                {
                    int ind = Array.FindIndex(mac, s => s.Equals(received_power2.Mac));
                    if (ind != (-1))
                    {
                        double pulse = DeviceInfo.PulseCorrection(Calib[ind].OffRuis, Calib[ind].GainA, Calib[ind].GainB, Calib[ind].OffTot, received_power2.EightSec, 8);
                        CurrentConsumptionWatt_a[ind] = DeviceInfo.ConverPulsesToWatt(8, pulse);
                        Plug_inv[ind] = true;
                    }
                }

              //  for (int i = 0; i < Plug_num.Length; i++)
                for (int i = 0; i < 10; i++)
                {
                 //   if (Plug_inv[Plug_num[i]] == false)
                    if (Plug_inv[i] == false)
                    {
                        CurrentConsumptionWatt_a[i] = 0.0;
                    }
                }

                return CurrentConsumptionWatt_a;
            }
            catch (Exception e)
            {
                throw new Exception("Could not measure consumption .");
            }
        }
        public void DownloadLastRecord(string mac, PlugwiseDeviceInfo Info, string serialPort)
        {
            try
            {
                plugwiseControl control = new plugwiseControl(serialPort);
                control.Open();
                if (Info != null)
                {
                    int LogAdr = Info.CurrentLogAdr;

                    control.Action(mac, LogAdr, PlugwiseLib.BLL.BC.PlugwiseActions.history);
                            while (control.HistoryMessage == null)
                            {
                                Thread.Sleep(10);
                            }
                            if (LastLogAdr == control.HistoryMessage.LogAddress)
                            {
                                Console.WriteLine(HistoryMessage.Messages[0].Hourvalue + " " + HistoryMessage.Messages[0].MeasurementValue + "\n");
                                Console.WriteLine(HistoryMessage.Messages[1].Hourvalue + " " + HistoryMessage.Messages[1].MeasurementValue + "\n");
                                Console.WriteLine(HistoryMessage.Messages[2].Hourvalue + " " + HistoryMessage.Messages[2].MeasurementValue + "\n");
                                Console.WriteLine(HistoryMessage.Messages[3].Hourvalue + " " + HistoryMessage.Messages[3].MeasurementValue + "\n");
                            }
                       
                    }
                    else
                    {

                        Console.WriteLine("Calibration not Existed");
                    }
                    control.Close();
                
            }
            catch (Exception e)
            {
                throw new Exception("Could not download History .");
            }
        }

        public void DownloadFirstRecord(string mac, PlugwiseDeviceInfo Info, string serialPort)
        {
            try
            {
                plugwiseControl control = new plugwiseControl(serialPort);
                control.Open();
                if (Info != null)
                {
                    control.Action(mac, 96, PlugwiseLib.BLL.BC.PlugwiseActions.history);
                            while (control.HistoryMessage == null)
                            {
                                Thread.Sleep(10);
                            }
                            Console.WriteLine(HistoryMessage.Messages[0].Hourvalue + " " + HistoryMessage.Messages[0].MeasurementValue + "\n");
                            Console.WriteLine(HistoryMessage.Messages[1].Hourvalue + " " + HistoryMessage.Messages[1].MeasurementValue + "\n");
                            Console.WriteLine(HistoryMessage.Messages[2].Hourvalue + " " + HistoryMessage.Messages[2].MeasurementValue + "\n");
                            Console.WriteLine(HistoryMessage.Messages[3].Hourvalue + " " + HistoryMessage.Messages[3].MeasurementValue + "\n");
                        }
                    
                
                    control.Close();
                }
           
            catch (Exception e)
            {
                throw new Exception("Could not download History .");
            }
        }

        public void DownloadHistory(string mac, PlugwiseDeviceInfo Info, string serialPort)
        {
            try
            {
                TextWriter HistoryLog = new StreamWriter("log.txt");
                plugwiseControl control = new plugwiseControl(serialPort);
                control.Open();
                HistoryLog.WriteLine("Nb Sequence;logYear;logMonth;logMinutes;Watt\n");
                if (Info != null)
                {
                    double LogAdr = Info.CurrentLogAdr;
                    while (LastLogAdr < LogAdr)
                    {
                            
                            control.Action(mac, LastLogAdr, PlugwiseLib.BLL.BC.PlugwiseActions.history);
                            bool waitTimeout = false;
                            int Cpt = 0;
                            while ((control.HistoryMessage == null)&(waitTimeout == false))
                            {
                                Cpt++;
                                if (Cpt < 100)
                                {
                                    Thread.Sleep(10);
                                }
                                else
                                {
                                    waitTimeout =true;
                                }
                            }
                            HistoryBuffer(Info, control.HistoryMessage, HistoryLog);
                            LastLogAdr++;
                            control.HistoryMessage = null;
                       }
     
                    HistoryLog.Close();
                }
                control.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Could not download History .");
            }
        }

        public void HistoryBuffer(PlugwiseDeviceInfo Infoms, PlugwiseHistoryPowerMessage Hmsg, TextWriter Log)
        {
            int i;
            for (i=0;i<4;i++)
            {
     
                double pulse = DeviceInfo.PulseCorrection(Infoms.OffRuis, Infoms.GainA, Infoms.GainB, Infoms.OffTot, Hmsg.Messages[i].MeasurementValue, 3600);
                Hmsg.Messages[i].Watt = DeviceInfo.ConverPulsesToWatt(3600, pulse);
               // DateTime Date = MessageHelper.CalculatePlugwiseDate(Hmsg.Messages[i], Infoms);
                byte logYear = ConversionClass.HexToByte(Hmsg.Messages[i].RawHourvalue.Substring(0, 2));
                byte logMonth = ConversionClass.HexToByte(Hmsg.Messages[i].RawHourvalue.Substring(2, 2));
                int logMinutes = ConversionClass.HexStringToUInt16(Hmsg.Messages[i].RawHourvalue.Substring(4, 4));

                Log.WriteLine(Hmsg.nb_Sequence + ";" + Hmsg.Messages[i].RawHourvalue + ";" + logYear + ";" + logMonth + ";" + logMinutes + ";" + Hmsg.Messages[i].Watt + "\n");
    
            }
            
        }

        public void ClosePlug(string mac, string serialPort)
        {
            try
            {
                plugwiseControl control = new plugwiseControl(serialPort);
                control.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Could not close plug.");
            }
        }

        /// <summary>
        /// This is the method that sends a command to the plugwise plugs.
        /// </summary>
        /// <param name="mac">The mac adress of the plug that needs to perform the action</param>
        /// <param name="action">The action that has to be performed</param>
        public void Action(string mac,PlugwiseActions action)
        {
            try
            {
                
                string message = "";
                switch (action)
                {
                    case PlugwiseActions.On:
                        currentAction = PlugwiseActions.On;
                        message = PlugwiseSerialPortFactory.Create(PlugwiseSerialPortRequest.on,mac);
                       
                        
                        break;
                    case PlugwiseActions.Off:
                        currentAction = PlugwiseActions.Off;
                        message = PlugwiseSerialPortFactory.Create(PlugwiseSerialPortRequest.off, mac);
                        
                       
                        break;
                    case PlugwiseActions.Status:

                        currentAction = PlugwiseActions.Status;
                        message = PlugwiseSerialPortFactory.Create(PlugwiseSerialPortRequest.status, mac);
                      
                        break;
                    case PlugwiseActions.Calibration:
                        currentAction = PlugwiseActions.Calibration;
                        message = PlugwiseSerialPortFactory.Create(PlugwiseSerialPortRequest.calibration, mac);
                      
                      
                        break;
                    case PlugwiseActions.powerinfo:
                        currentAction = PlugwiseActions.powerinfo;
                        message = PlugwiseSerialPortFactory.Create(PlugwiseSerialPortRequest.powerinfo,mac);
        
                        break;

                    case PlugwiseActions.history:
                        message = "";
                        break;
                }
                if (message.Length > 0)
                {
                    port.WriteLine(message);
                    Thread.Sleep(10);
                }

            }
            catch (Exception)
            {
              
               /// throw e;
                throw new Exception("Action error");
            }
        }
        /// <summary>
        /// This is the method that sends a command to the plugwise plugs that retrieves the history power information
        /// </summary>
        /// <param name="mac">The mac adress of the plug that needs to perform the action</param>
        /// <param name="logId">The id of the history message that has to be retrieved</param>
        /// <param name="action">The action that has to be performed this MUST be history</param>
        public void Action(string mac,int logId,PlugwiseActions action)
        {
            string message = "";
            switch(action)
            {
                case PlugwiseActions.history:
                 currentAction = PlugwiseActions.history;
                 message = PlugwiseSerialPortFactory.Create(PlugwiseSerialPortRequest.history, MessageHelper.ConvertIntToPlugwiseLogHex(logId), mac);
             break;
            }

            if (message.Length > 0)
            {
                port.WriteLine(message);
                Thread.Sleep(10);
            }
        }

      
        public void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {// Event for receiving data

            try
            {

                Thread.Sleep(300);
                string txt = port.ReadExisting();
                string pattern = @"|\r\n";
                List<PlugwiseMessage> msg = reader.Read(Regex.Split(txt, pattern));
                if (msg != null)
                {
                  DataReceived(sender, new System.EventArgs(), msg);
                }
             
            }
            catch (NullReferenceException nre)
            {
                Console.WriteLine(
                    "Cannot read Data received (NullReferenceException).\n" +
                    nre.Message);
            }

            catch (InvalidOperationException ioe)
            {
                Console.WriteLine(
                     "Cannot read Data received (InvalidOperationException).\n" +
                    ioe.Message);
            }

            catch (Exception er)
            {
                Console.WriteLine("Cannot read Data received (Other execption).\n" + er.Message);
            }
            
        }

       
        /// <summary>
        /// This method Opens the connection to the serial port
        /// </summary>
        public void Open()
        {
            try
            {
                if (!port.IsOpen)
                {
                    port.Open();
                }
            }
            catch (System.IO.IOException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method closes the connection to the serial port
        /// </summary>
        public void Close()
        {
            try
            {
                if (port.IsOpen)
                {
                    port.Close();
                }
                Thread.Sleep(5);
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }
    }
}
