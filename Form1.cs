using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;

// ColorSynth v1.2.0

namespace ColorSynth
{

    //ColorSynth done by OSformula (http://osformula.com/)
    //.svp files belong to Dreamtronics (https://dreamtonics.com/)
    //
    //Note: This software has been done in an evening so it could be done way faster
    //if I knew how Windows Forms works but this will do the trick for now.
    //If any of the copyright holders wants me to take it down, I will without any problem
    //
    //---------------------------------LICENSES-------------------------
    //Licensed under the Apache License, Version 2.0 (the "License");
    //you may not use this file except in compliance with the License.
    //You may obtain a copy of the License at
    //
    //    http://www.apache.org/licenses/LICENSE-2.0
    //
    //Unless required by applicable law or agreed to in writing, software
    //distributed under the License is distributed on an "AS IS" BASIS,
    //WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    //See the License for the specific language governing permissions and
    //limitations under the License.

    public partial class Form1 : Form
    {
        public class SynthFile
        {
            public List<SynthTracks> tracks;

            public void Clear()
            {
                this.tracks.Clear();
            }
        }
        public class SynthTracks
        {
            public string name;
            public string dispColor;
            public string dispColor_old;
            public int dispOrder;
        }

        private string json;        //opened file as a string
        private SynthFile parsedFile;       //parsed file as an object
        private int i = 0;    //dispOrder "not ordered" fix

        //Read file as Json
        public void LoadJson(string s)
        {
            //if already open, clear first
            if (parsedFile != null)
            {
                parsedFile.Clear();
                i = 0;
                listView1.Clear();
            }

            using (StreamReader r = new StreamReader(s))
            {
                json = r.ReadToEnd();
                parsedFile = JsonConvert.DeserializeObject<SynthFile>(json);

                foreach (SynthTracks track in parsedFile.tracks)
                {
                    track.dispColor_old = track.dispColor;
                    listView1.Items.Add(track.name);
                    listView1.Items[i].BackColor = System.Drawing.ColorTranslator.FromHtml(String.Concat("#" + parsedFile.tracks[i].dispColor));
                    i++;
                }

                label1.Text = Path.GetFileName(s);
            }
        }

        //Save file as Json
        public void SaveJson(string s)
        {
            i = 0;      //dispOrder "not ordered" fix

            foreach (SynthTracks track in parsedFile.tracks)
                {
                    json = json.Replace("\"dispColor\": \"" + track.dispColor_old + "\", \"dispOrder\": " + track.dispOrder, "\"dispColor\": \"" + track.dispColor + "\", \"dispOrder\": " + i);
                    i++;
                }

                File.WriteAllText(s, json);
        }

        //Compulsory main call
        public Form1()
        {
            InitializeComponent();
        }

        //Open button action
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadJson(openFileDialog1.FileName);
            }
        }


        //windows pls why you don't let me delete this
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        //Action when a track is chosen
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                var firstSelectedItem = listView1.SelectedItems[0];
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    listView1.Items[firstSelectedItem.Index].BackColor = colorDialog1.Color;
                    String code = (colorDialog1.Color.ToArgb() & 0x00FFFFFF).ToString("X6");
                    parsedFile.tracks[firstSelectedItem.Index].dispColor = "FF" + code;
                }
            }
        }
        
        //Save button action
        private void button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SaveJson(saveFileDialog1.FileName);
            }
        }

        //Online websites
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://osformula.com/");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://dreamtonics.com/");
        
        }
    }
}
