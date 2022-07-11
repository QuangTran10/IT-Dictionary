using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IT_Dic
{
    public partial class Setting : Form
    {
        private int borderSize = 2;
        private Color borderColor = Color.FromArgb(128, 128, 255);

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        public event EventHandler Button_Clicked;

        public event EventHandler Button2_Clicked;
        public Setting()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(borderSize);
            this.panel2.BackColor = borderColor;
            this.BackColor = borderColor;
        }
        private void Setting_Load(object sender, EventArgs e)
        {
            if (File.Exists("dic.txt"))
            {
                loadConnectionString();
            }
            loadHotKey();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        /// <summary>
        /// Tab database
        /// </summary>
        private void resetText()
        {
            this.txtIP.Text = "";
            this.txtPort.Text = "";
            this.txtUser.Text = "";
            this.txtPass.Text = "";
            this.txtDatabase.Text = "";
        }

        private void loadConnectionString()
        {
            string[] a = File.ReadAllLines("dic.txt");
            this.txtIP.Text = a[0];
            this.txtPort.Text = a[1];
            this.txtUser.Text = a[2];
            this.txtPass.Text = a[3];
            this.txtDatabase.Text = a[4];
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            string ip = txtIP.Text;
            string port = txtPort.Text;
            string user = txtUser.Text;
            string pass = txtPass.Text;
            string database = txtDatabase.Text;

            string noidung = ip + "\r\n" + port + "\r\n" + user + "\r\n" + pass + "\r\n" + database;

            File.WriteAllText("dic.txt", noidung);
            resetText();
            if (this.Button_Clicked != null)
                this.Button_Clicked(sender, e);

            MessageBox.Show("Cập nhật thành công");

            loadConnectionString();
        }

        /// <summary>
        /// Tab hot key
        /// </summary>
        private void loadHotKey()
        {
            if (File.Exists("hotkey.txt"))
            {
                string[] b = File.ReadAllLines("hotkey.txt");

                this.txtKey.Text = b[0];
            }
            else
            {
                string noidung = "Shift+F";

                File.WriteAllText("hotkey.txt", noidung);
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string key;

            if (txtKey.Text.Equals(""))
            {
                key = "Shift+F";
            }
            else
            {
                key = txtKey.Text.Trim();
                key = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(key.ToLower());
                key = key.Replace(" ", "");
            }

            string noidung = key;
            try
            {
                var testhotkey = Combination.FromString(key);
            }
            catch(Exception)
            {
                MessageBox.Show(key + " bị lỗi \r\n Vui lòng nhập phím tắt hợp lệ");
                return;
            }


            File.WriteAllText("hotkey.txt", noidung);
            loadHotKey();
            MessageBox.Show("Cập nhật thành công");

            if (this.Button2_Clicked != null)
                this.Button2_Clicked(sender, e);
        }
    }
}
