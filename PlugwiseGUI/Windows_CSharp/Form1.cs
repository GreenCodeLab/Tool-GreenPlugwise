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
using PlugwiseGUI;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
            configureUI();
        }

        private void configureUI()
        {
            //Au lancement du programme on ne peut lancer une mesure
            button1.Enabled = false;
            button2.Enabled = false;

            List<string> confs = new Configurater().getConf();

            if (!confs.Equals(null))
            {
                try
                {
                    this.checkBox2.Checked = confs[0].Equals("True");
                    this.checkBox3.Checked = confs[1].Equals("True");
                    this.checkBox4.Checked = confs[2].Equals("True");
                    this.checkBox5.Checked = confs[3].Equals("True");
                    this.checkBox6.Checked = confs[4].Equals("True");
                    this.checkBox7.Checked = confs[5].Equals("True");
                    this.checkBox8.Checked = confs[6].Equals("True");
                    this.checkBox9.Checked = confs[7].Equals("True");
                    this.checkBox10.Checked = confs[8].Equals("True");

                    this.textBox2.Text = confs[9];
                    TimerMesure.Plug_mac[0] = this.textBox2.Text;
                    this.textBox3.Text = confs[10];
                    TimerMesure.Plug_mac[1] = this.textBox3.Text;
                    this.textBox4.Text = confs[11];
                    TimerMesure.Plug_mac[2] = this.textBox4.Text;
                    this.textBox7.Text = confs[12];
                    TimerMesure.Plug_mac[3] = this.textBox7.Text;
                    this.textBox6.Text = confs[13];
                    TimerMesure.Plug_mac[4] = this.textBox6.Text;
                    this.textBox5.Text = confs[14];
                    TimerMesure.Plug_mac[5] = this.textBox5.Text;
                    this.textBox10.Text = confs[15];
                    TimerMesure.Plug_mac[6] = this.textBox10.Text;
                    this.textBox9.Text = confs[16];
                    TimerMesure.Plug_mac[7] = this.textBox9.Text;
                    this.textBox8.Text = confs[17];
                    TimerMesure.Plug_mac[8] = this.textBox8.Text;

                    this.textBox12.Text = confs[18];
                    TimerMesure.freq = int.Parse(this.textBox12.Text);
                    this.textBox1.Text = confs[19];
                    this.textBox11.Text = confs[20];
                    TimerMesure.serialPort = this.textBox11.Text;

                    this.checkBox12.Checked = confs[21].Equals("True");
                    TimerMesure.bufferisation = this.checkBox12.Checked;
                    this.checkBox1.Checked = confs[22].Equals("True");
                    TimerMesure.mesurecpu = this.checkBox1.Checked;
                }
                catch
                {
                    Messager.show("Fichier de fonfiguration incomplet, les données manquantes n'ont pas été importées.");
                }
                
            }
            else
            {
                Messager.show("Fichier de configuration introuvable.");
            }
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

                    this.button1.Text = "Arrêter";
                    checkBox1.Enabled = false;
                    textBox11.Enabled = false;
                    textBox12.Enabled = false;
                    checkBox12.Enabled = false;
                    button5.Enabled = false;
                    button2.Enabled = false;
                    button4.Enabled = false;
                    button3.Enabled = false;
                    InfoLabel.Text = "Acquisition des données en cours";

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
                    TimerMesure.mesure_init_2(getListNames());
                    Thread.Sleep(1000);
                    uptade_Plug_state();

                    TimerMesure.timermesure();

                }
                else
                {
                    this.button1.Text = "Démarrer la mesure";
                    checkBox1.Enabled = true;
                    textBox11.Enabled = true;
                    textBox12.Enabled = true;
                    checkBox12.Enabled = true;
                    button5.Enabled = true;
                    button2.Enabled = true;
                    button4.Enabled = true;
                    button3.Enabled = true;
                    InfoLabel.Text = "";
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

            button1.Enabled = true;
            InfoLabel.Text = "Consommation initialisée";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process myInfo = new Process();
            myInfo.StartInfo.FileName = "kst2.exe";
            myInfo.StartInfo.WorkingDirectory = @"C:\Program Files\Kst 2.0.4\bin";
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
            TimerMesure.mesure_init_2(getListNames());
            uptade_Plug_state();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            alertWorkInitializeInProgress();

            //lancement de l'événement de travail de fond
            initialisePlugBackgroundWorker.RunWorkerAsync();

            TimerMesure.serialPort = textBox11.Text;
            TimerMesure.mesure_init();
            uptade_Plug_state();

            alertWorkInitializeFinished();
        }

        // méthode de travail de fond threadé permettant de ne pas geler l'UI
        private void initialisePlugBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            saveConf();
        }

        private void alertWorkInitializeInProgress()
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button5.Enabled = false;

            InfoLabel.Text = "Initialisation en cours";
        }

        private void alertWorkInitializeFinished()
        {
            button2.Enabled = true;
            button5.Enabled = true;

            InfoLabel.Text = "Plugs initialisés";
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            saveConf();
        }

        private void saveConf()
        {
            List<string> confs = new List<string>();

            confs.Add(this.checkBox2.Checked.ToString());
            confs.Add(this.checkBox3.Checked.ToString());
            confs.Add(this.checkBox4.Checked.ToString());
            confs.Add(this.checkBox5.Checked.ToString());
            confs.Add(this.checkBox6.Checked.ToString());
            confs.Add(this.checkBox7.Checked.ToString());
            confs.Add(this.checkBox8.Checked.ToString());
            confs.Add(this.checkBox9.Checked.ToString());
            confs.Add(this.checkBox10.Checked.ToString());
            confs.Add(this.textBox2.Text);
            confs.Add(this.textBox3.Text);
            confs.Add(this.textBox4.Text);
            confs.Add(this.textBox7.Text);
            confs.Add(this.textBox6.Text);
            confs.Add(this.textBox5.Text);
            confs.Add(this.textBox10.Text);
            confs.Add(this.textBox9.Text);
            confs.Add(this.textBox8.Text);

            confs.Add(this.textBox12.Text);
            confs.Add(this.textBox1.Text);
            confs.Add(this.textBox11.Text);

            confs.Add(this.checkBox12.Checked.ToString());
            confs.Add(this.checkBox1.Checked.ToString());

            new Configurater().saveConf(confs);
        }

        public List<String> getListNames()
        {
            List<String> listNames = new List<String>();

            listNames.Add(textBox22.Text.Trim());
            listNames.Add(textBox23.Text.Trim());
            listNames.Add(textBox24.Text.Trim());
            listNames.Add(textBox25.Text.Trim());
            listNames.Add(textBox26.Text.Trim());
            listNames.Add(textBox27.Text.Trim());
            listNames.Add(textBox28.Text.Trim());
            listNames.Add(textBox29.Text.Trim());
            listNames.Add(textBox30.Text.Trim());

            for(int i=0;i<listNames.Count;i++)
            {
                if(listNames[i] == "")
                    listNames[i] = "ID" + (i+1);
            }

            return listNames;
        }

    }

}
