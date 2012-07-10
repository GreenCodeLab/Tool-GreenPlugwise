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
using System.Windows.Forms;
using PlugwiseLib;
using System.IO;
using System.Diagnostics;
using System.Threading;
using WindowsFormsApplication1;


public class TimerMesure
{
    static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
    static int alarmCounter = 1;
    public static bool exitFlag = false;
    public static bool mesureinprogress = false;
    public static bool mesurecpu = false;
    public static int freq = 1;
    static plugwiseControl control = new plugwiseControl("Com5");
    public static PerformanceCounter PCounter;
    public static PerformanceCounter PCounter2;
    public static PerformanceCounter PCounter3;
    public static double[] consommation = new double[9];
    public static double[] idle = new double[9] ;
    public static double[] delta = new double[9];
    public static string[] Plug_mac = new string[9];// { "000D6F0000729127", "000D6F000076AF5A", "000D6F00007691DD", "000D6F000076B6E8", "000D6F0000768D19", "000D6F0000768EA9", "000D6F000072AE3D", "000D6F0000768A4B", "000D6F000076D54C"};
    public static bool[] Plug_Ok = new bool[9];
    public static PlugwiseLib.BLL.BC.PlugwiseDeviceInfo[] Plug_Calib = new PlugwiseLib.BLL.BC.PlugwiseDeviceInfo[9];
    public static bool[] Plug_act = new bool[9];
    public static int[] Plug_num;
    public static bool bufferisation = false;
    public static int[] erreur = new int[9] {0,0,0,0,0,0,0,0,0};
    public static string serialPort;

    // This is the method to run when the timer is raised.
    public static void TimerEventProcessor(Object myObject,
                                            EventArgs myEventArgs)
    {

        // Displays a message box asking whether to continue running the timer.
        if (TimerMesure.mesureinprogress == true)
        {
            // Restarts the timer and increments the counter.
            TextWriter writer;
            string fileName = @"C:\GreenCodeLab_Plugwyse\log.txt";
            

            string mes = "";
            double[] mesure = new double[9];
            if (bufferisation == false)
            {
                for (int i = 0; i < Plug_num.Length; i++)
                {
                    if (Plug_act[Plug_num[i]] == true)
                    {

                        mes = mes + TimerMesure.mesure(Plug_mac[Plug_num[i]], Plug_Calib[Plug_num[i]], Plug_num[i]) + ";";
                    }
                }
              
            }
            else
            {
                mesure = control.MeasureAllCurrentConsumption(Plug_mac, Plug_Calib, Plug_num, serialPort);
                
                for (int i = 0; i < Plug_num.Length; i++)
                {
                    if (mesure[Plug_num[i]]==0.0)
                    {
                        erreur[Plug_num[i]]++;
                        mes = mes + "0;";
                      
                    }
                    else
                    {
                    mes = mes + mesure[Plug_num[i]].ToString() + ";" ;
                    }
                }
                
                
                
     
            }
            writer = File.AppendText(fileName);
            writer.WriteLine(mes);
            writer.Close();
            myTimer.Stop();
            alarmCounter += 1;
            myTimer.Enabled = true;
        }
        else
        {
            myTimer.Stop();
            // Stops the timer.
            exitFlag = true;
        }
        
    }

    public static int timermesure()
    {
        /* Adds the event and the event handler for the method that will 
           process the timer event to the timer. */
        myTimer.Tick += new EventHandler(TimerEventProcessor);

        // Sets the timer interval to 5 seconds.
        myTimer.Interval = 1000 * freq;
       

        myTimer.Start();

        // Runs the timer, and raises the event.
        while (exitFlag == false)
        {
            // Processes all the events in the queue.
           Application.DoEvents();
            
        }
        return 0;
    }

    public static void mesure_init()
    {
     
        PlugwiseLib.BLL.BC.PlugwiseDeviceInfo calib;
        int num = 0;
        for (int i=0; i<9; i++)
        {
            calib = control.InitPlug(TimerMesure.Plug_mac[i], serialPort);
            if (calib != null)
            {
                Plug_Ok[i] = true;
                Array.Resize(ref Plug_num, num+1);
                Plug_num[num] = i;
                num++;
            }
            else
            {
                Plug_Ok[i] = false;
            }
        }
        if (!Directory.Exists(@"C:\GreenCodeLab_Plugwyse"))
        {
            Directory.CreateDirectory(@"C:\GreenCodeLab_Plugwyse");
        }
        
            }

    public static void mesure_init_2(List<String> names)
    {

        PlugwiseLib.BLL.BC.PlugwiseDeviceInfo calib;
        string fileName = @"C:\GreenCodeLab_Plugwyse\log.txt";
        TextWriter writer;
        writer = new StreamWriter(fileName);
        string entete = "";

        int num = 0;
        for (int i = 0; i < 9; i++)
        {
            if (Plug_act[i] == true)
            {
                calib = control.InitPlug(TimerMesure.Plug_mac[i],serialPort);
                if (calib != null)
                {
                    Plug_Ok[i] = true;
                    Plug_Calib[i] = calib;
                    if (bufferisation == false)
                    {
                        entete = entete + names[i] + ";%CPU " + (i + 1).ToString() + ";Bytes/S " + (i + 1).ToString()  + ";Disk Bytes/S " + (i + 1).ToString() + ";consommation " + (i + 1).ToString() + ";Impact consommation " + (i + 1).ToString();
                    }
                    else
                    {
                        entete = entete + names[i] + ";";
                    }
                    Array.Resize(ref Plug_num, num + 1);
                    Plug_num[num] = i;
                    num++;
                }
                else
                {
                    Plug_Ok[i] = false;
                }
            }
            else
            {
                Plug_Ok[i] = false;

            }
        }


        writer.WriteLine(entete);
        writer.Close();
        if (mesurecpu == true)
        {
            PerformanceCounterCategory networkCategory;
             string instanceName = null;

            PCounter = new PerformanceCounter();
            PCounter.MachineName = Environment.MachineName; //Attribution de la propriété MachineName permettant de "dire" au Compteur de performance de compter sur la machine local
            PCounter.CategoryName = "Processor";
            PCounter.CounterName = "% Processor Time";
            PCounter.InstanceName = "_Total";

            PCounter2 = new PerformanceCounter();
            PCounter2.MachineName = Environment.MachineName; //Attribution de la propriété MachineName permettant de "dire" au Compteur de performance de compter sur la machine local
            PCounter2.CategoryName = "Network Interface";
            PCounter2.CounterName = "Bytes Total/sec";
            networkCategory = new PerformanceCounterCategory("Network Interface");
              foreach (string instance in networkCategory.GetInstanceNames())

        {
            Console.WriteLine(instance);
            if (instance == "Realtek PCIe FE Family Controller")
            {
                instanceName = instance;
                break;
            }
        }

              PCounter2.InstanceName = instanceName;

              PCounter3 = new PerformanceCounter();
              PCounter3.MachineName = Environment.MachineName; //Attribution de la propriété MachineName permettant de "dire" au Compteur de performance de compter sur la machine local
              PCounter3.CategoryName = "PhysicalDisk";
              PCounter3.CounterName = "% Disk Time";
              PCounter3.InstanceName = "_Total";
        }
    }

    public static string mesure(string mac, PlugwiseLib.BLL.BC.PlugwiseDeviceInfo Calib, int e)
    {
   
        
        float chargea = 0;
        float chargeb = 0;
        float chargec = 0;

        if (mesurecpu == true)
        {
            chargea = PCounter.NextValue();
            chargeb = PCounter2.NextValue();
            chargec = PCounter3.NextValue();
        }
        else
        {
            chargea = 0;
            chargeb = 0;
            chargec = 0;
        }
        
        double mesure = control.MeasureCurrentConsumption(mac, Calib, serialPort);
        consommation[e] = consommation[e] + mesure / 3.6;
        delta[e] = delta[e] + (mesure - idle[e]) * freq / 3.6;
        string result = mesure.ToString() + ";" + chargea.ToString() + ";" + chargeb.ToString() + ";" + chargec.ToString() + ";"  + consommation[e].ToString() + ";" + delta[e].ToString();
      return result;
        
    }
}