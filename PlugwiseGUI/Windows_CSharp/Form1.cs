using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PlugwiseLib;
using System.Threading;
using System.Diagnostics;
using System.IO;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
         //   TimerMesure.mesure_init();
            InitializeComponent();
        //    uptade_Plug_state();     
            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\GreenCodeLab_Plugwyse\conf.txt");
            //file.ReadLine();
            this.checkBox2.Checked = file.ReadLine().Equals("True");
            this.checkBox3.Checked = file.ReadLine().Equals("True");
            this.checkBox4.Checked = file.ReadLine().Equals("True");
            this.checkBox5.Checked = file.ReadLine().Equals("True");
            this.checkBox6.Checked = file.ReadLine().Equals("True");
            this.checkBox7.Checked = file.ReadLine().Equals("True");
            this.checkBox8.Checked = file.ReadLine().Equals("True");
            this.checkBox9.Checked = file.ReadLine().Equals("True");
            this.checkBox10.Checked = file.ReadLine().Equals("True");
            this.textBox2.Text = file.ReadLine();
            TimerMesure.Plug_mac[0] = this.textBox2.Text;
            this.textBox3.Text = file.ReadLine();
            TimerMesure.Plug_mac[1] = this.textBox3.Text;
            this.textBox4.Text = file.ReadLine();
            TimerMesure.Plug_mac[2] = this.textBox4.Text;
            this.textBox7.Text = file.ReadLine();
            TimerMesure.Plug_mac[3] = this.textBox7.Text;
            this.textBox6.Text = file.ReadLine();
            TimerMesure.Plug_mac[4] = this.textBox6.Text;
            this.textBox5.Text = file.ReadLine();
            TimerMesure.Plug_mac[5] = this.textBox5.Text;
            this.textBox10.Text = file.ReadLine();
            TimerMesure.Plug_mac[6] = this.textBox10.Text;
            this.textBox9.Text = file.ReadLine();
            TimerMesure.Plug_mac[7] = this.textBox9.Text;
            this.textBox8.Text = file.ReadLine();
            TimerMesure.Plug_mac[8] = this.textBox8.Text;

            this.textBox12.Text = file.ReadLine();
            TimerMesure.freq = int.Parse(this.textBox12.Text);
            this.textBox1.Text = file.ReadLine();
            this.textBox11.Text = file.ReadLine();
            TimerMesure.serialPort = this.textBox11.Text;

            this.checkBox12.Checked = file.ReadLine().Equals("True");
            TimerMesure.bufferisation = this.checkBox12.Checked;
            this.checkBox1.Checked = file.ReadLine().Equals("True");
            TimerMesure.mesurecpu = this.checkBox1.Checked;

            file.Close();
        }

        private void uptade_Plug_state()
        {
            this.checkBox2.Enabled = TimerMesure.Plug_Ok[0];
            this.checkBox3.Enabled = TimerMesure.Plug_Ok[1];
            this.checkBox4.Enabled = TimerMesure.Plug_Ok[2];
            this.checkBox5.Enabled = TimerMesure.Plug_Ok[3];
            this.checkBox6.Enabled = TimerMesure.Plug_Ok[4];
            this.checkBox7.Enabled = TimerMesure.Plug_Ok[5];
            this.checkBox8.Enabled = TimerMesure.Plug_Ok[6];
            this.checkBox9.Enabled = TimerMesure.Plug_Ok[7];
            this.checkBox10.Enabled = TimerMesure.Plug_Ok[8];

        }
        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
 
            if (TimerMesure.Plug_num == null)
            {
                MessageBox.Show("Pas de Plug initialisé");
            }
            else
            {
                if (TimerMesure.mesureinprogress == false)
                {
                    TimerMesure.mesureinprogress = true;
                    TimerMesure.Plug_act[0] = this.checkBox2.Checked;
                    TimerMesure.Plug_act[1] = this.checkBox3.Checked;
                    TimerMesure.Plug_act[2] = this.checkBox4.Checked;
                    TimerMesure.Plug_act[3] = this.checkBox5.Checked;
                    TimerMesure.Plug_act[4] = this.checkBox6.Checked;
                    TimerMesure.Plug_act[5] = this.checkBox7.Checked;
                    TimerMesure.Plug_act[6] = this.checkBox8.Checked;
                    TimerMesure.Plug_act[7] = this.checkBox9.Checked;
                    TimerMesure.Plug_act[8] = this.checkBox10.Checked;

                    TimerMesure.serialPort = textBox11.Text;
                    TimerMesure.mesure_init_2();
                    Thread.Sleep(1000);
                    uptade_Plug_state();

                    this.button1.Text = "Arrêter";
                    checkBox1.Enabled = false;
                    textBox11.Enabled = false;
                    textBox12.Enabled = false;
                    checkBox12.Enabled = false;
                    TimerMesure.timermesure();

                }
                else
                {
                   

                    this.button1.Text = "Lancer";
                    checkBox1.Enabled = true;
                    textBox11.Enabled = true;
                    textBox12.Enabled = true;
                    checkBox12.Enabled = true;
                    TimerMesure.mesureinprogress = false;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked == false)
            {
                TimerMesure.mesurecpu = false;
            }
            else
            {
                TimerMesure.mesurecpu = true;
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < 9; i++)
            {
                if (TimerMesure.Plug_act[i] == true)
                {
                    TimerMesure.consommation[i] = 0;
                    TimerMesure.delta[i] = 0;
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process myInfo = new Process();
            myInfo.StartInfo.FileName = "kst2.exe";
            myInfo.StartInfo.WorkingDirectory = @"C:\Program Files (x86)\Kst 2.0.4\bin";
            myInfo.Start(); 
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           // CultureInfo culture = CultureInfo.CreateSpecificCulture("en-GB");
            TimerMesure.idle[0] = double.Parse(textBox1.Text);
       
        }


        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            TimerMesure.freq = int.Parse(textBox12.Text);
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            TimerMesure.bufferisation = this.checkBox12.Checked;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox13.Text = TimerMesure.erreur[0].ToString();
            textBox14.Text = TimerMesure.erreur[1].ToString();
            textBox15.Text = TimerMesure.erreur[2].ToString();
            textBox16.Text = TimerMesure.erreur[3].ToString();
            textBox17.Text = TimerMesure.erreur[4].ToString();
            textBox18.Text = TimerMesure.erreur[5].ToString();
            textBox19.Text = TimerMesure.erreur[6].ToString();
            textBox20.Text = TimerMesure.erreur[7].ToString();
            textBox21.Text = TimerMesure.erreur[8].ToString();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            TimerMesure.Plug_mac[0] = this.textBox2.Text;
            TimerMesure.Plug_mac[1] = this.textBox3.Text;
            TimerMesure.Plug_mac[2] = this.textBox4.Text;
            TimerMesure.Plug_mac[3] = this.textBox7.Text;
            TimerMesure.Plug_mac[4] = this.textBox6.Text;
            TimerMesure.Plug_mac[5] = this.textBox5.Text;
            TimerMesure.Plug_mac[6] = this.textBox10.Text;
            TimerMesure.Plug_mac[7] = this.textBox9.Text;
            TimerMesure.Plug_mac[8] = this.textBox8.Text;

            TimerMesure.Plug_mac[0] = this.textBox2.Text;
            TimerMesure.Plug_act[0] = this.checkBox2.Checked;
            TimerMesure.Plug_act[1] = this.checkBox3.Checked;
            TimerMesure.Plug_act[2] = this.checkBox4.Checked;
            TimerMesure.Plug_act[3] = this.checkBox5.Checked;
            TimerMesure.Plug_act[4] = this.checkBox6.Checked;
            TimerMesure.Plug_act[5] = this.checkBox7.Checked;
            TimerMesure.Plug_act[6] = this.checkBox8.Checked;
            TimerMesure.Plug_act[7] = this.checkBox9.Checked;
            TimerMesure.Plug_act[8] = this.checkBox10.Checked;

            TimerMesure.serialPort = textBox11.Text;
            TimerMesure.mesure_init_2();
            uptade_Plug_state();
            

        }

        private void button5_Click(object sender, EventArgs e)
        {
            TimerMesure.serialPort = textBox11.Text;
            TimerMesure.mesure_init();
            uptade_Plug_state();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            System.IO.StreamWriter writer = new System.IO.StreamWriter(@"C:\GreenCodeLab_Plugwyse\conf.txt");
          //  writer = File.Create(@"C:\GreenCodeLab_Plugwyse\conf.txt");
        

            writer.WriteLine(this.checkBox2.Checked.ToString());
            writer.WriteLine(this.checkBox3.Checked.ToString());
            writer.WriteLine(this.checkBox4.Checked.ToString());
            writer.WriteLine(this.checkBox5.Checked.ToString());
            writer.WriteLine(this.checkBox6.Checked.ToString());
            writer.WriteLine(this.checkBox7.Checked.ToString());
            writer.WriteLine(this.checkBox8.Checked.ToString());
            writer.WriteLine(this.checkBox9.Checked.ToString());
            writer.WriteLine(this.checkBox10.Checked.ToString());
            writer.WriteLine(this.textBox2.Text);
            writer.WriteLine(this.textBox3.Text);
            writer.WriteLine(this.textBox4.Text);
            writer.WriteLine(this.textBox7.Text);
            writer.WriteLine(this.textBox6.Text);
            writer.WriteLine(this.textBox5.Text);
            writer.WriteLine(this.textBox10.Text);
            writer.WriteLine(this.textBox9.Text);
            writer.WriteLine(this.textBox8.Text);

            writer.WriteLine(this.textBox12.Text);
            writer.WriteLine(this.textBox1.Text);
            writer.WriteLine(this.textBox11.Text);

            writer.WriteLine(this.checkBox12.Checked.ToString());
            writer.WriteLine(this.checkBox1.Checked.ToString());
            writer.Close();
        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

      

  

   

    }
}
