using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IT_Dic
{
    public partial class Notification : Form
    {
        private int borderSize = 2;
        private Color borderColor = Color.FromArgb(128, 128, 255);
        private ArrayList dic;

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        public Notification()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(borderSize);
            this.panel2.BackColor = borderColor;
            this.BackColor = borderColor;
        }
        public string convert(string data)
        {
            var windows1252 = Encoding.GetEncoding(1252);
            var utf8Bytes = windows1252.GetBytes(data);
            var correct = Encoding.UTF8.GetString(utf8Bytes);
            return correct;
        }

        public Notification(ArrayList a, string keywords) : this()
        {
            this.lblKey.Text = keywords;
            var listItems = new ListItem[a.Count + 1];
            flowLayoutPanel1.Controls.Clear();
            int i = 0;
            dic = a;
            keywords = keywords.Trim();
            string replacekey = "<mark>" + keywords + "</mark>";
            foreach (Dictionary item in dic)
            {
                string japanese_trans = "", japan_hiragana = "", vietnamese = "", english = "", example = "";
                listItems[i] = new ListItem();
                if (item.Tm_id != 0)
                {
                    japanese_trans = convert(item.Tm_japanese_translate);
                    japan_hiragana = convert(item.Tm_japanese_hiragana);
                    vietnamese = convert(item.Tm_vietnamese_tranlate);
                    english = convert(item.Tm_english_tranlate);
                    example = convert(item.Tm_example);

                    japanese_trans = japanese_trans.Replace(keywords, replacekey);
                    japan_hiragana = japan_hiragana.Replace(keywords, replacekey);
                    vietnamese = Regex.Replace(vietnamese, keywords, replacekey, RegexOptions.IgnoreCase);
                    english = Regex.Replace(english, keywords, replacekey, RegexOptions.IgnoreCase);
                    if (example == "")
                    {
                        example = "";
                    }
                    else
                    {
                        example = example.Replace("\n", "<br>");
                        example = "<br><b>Ví dụ:</b><br>" + Regex.Replace(example, keywords, replacekey, RegexOptions.IgnoreCase); ;
                    }
                }
                else
                {
                    japanese_trans = item.Tm_japanese_translate;
                    japan_hiragana = item.Tm_japanese_hiragana;
                    vietnamese = item.Tm_vietnamese_tranlate;
                    english = item.Tm_english_tranlate;
                    example = item.Tm_example;
                }


                listItems[i].Content = "<html>" +
                "<style> mark{background-color:#CE96F8;} p{padding : 0;margin: 0;line-height:20px;font-size:12px;text-align:justify }</style>" +
                "<body>" +
                "<p style='font-size: 12px; line-height:0'>" + japan_hiragana + "</p>" +
                "<p style= 'color:red; font-size:16px'><b>" + japanese_trans + "</b></p>" +
                "<p>" + japan_hiragana + "</p>" +
                "<p>" + vietnamese + "</p>" +
                "<p>" + english + example + "</p></body></html>";

                flowLayoutPanel1.Controls.Add(listItems[i]);
                i++;
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lblKey.Text.Trim());
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
    }
}
