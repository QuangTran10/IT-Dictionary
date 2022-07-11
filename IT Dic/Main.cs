using Gma.System.MouseKeyHook;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IT_Dic
{
    public partial class Main : Form
    {
        private int borderSize = 2;
        private Color borderColor = Color.FromArgb(128, 128, 255);

        ManageDictionary manage;
        MySqlConnection conn;

        private IKeyboardMouseEvents globalMouseHook;

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4
        }
        private int isOpen;
        public Main()
        {
            InitializeComponent();
            
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(borderSize);
            this.panel2.BackColor = borderColor;
            this.BackColor = borderColor;

            this.Subscribe();
 
            manage = new ManageDictionary();

            conn = ConnectDB.Connect();

        }
        private void Subscribe()
        {
            globalMouseHook = Hook.GlobalEvents();

            globalMouseHook.MouseDragFinished += MouseDragFinished;
        }

        private void Unsubscribe()
        {
            globalMouseHook.MouseDragFinished -= MouseDragFinished;

            globalMouseHook.Dispose();
        }

        private void resetConnection()
        {
            conn.Close();
            conn = ConnectDB.Connect();
            isOpen = 0;
        } 
        
        private void showResult()
        {
            string content = this.txtSearch.Text.Trim();
            if (content.Equals(""))
            {
                MessageBox.Show("Không tìm thấy từ khoá");
                return;
            }
            else
            {
                this.displayNotification(content);
            }
            
        }

        private void displayNotification(string content)
        {
            ArrayList re = new ArrayList();
            try
            {
                //If the connection isn't open, it will open and set value of isOpen parameter to 1
                conn.Open();
                
                re = manage.findWord(conn, content);

                if (re.Count == 0)
                {
                    Dictionary a = new Dictionary(0, "Không tìm thấy kết quả", "", "", "", "");
                    re.Add(a);
                    Notification noti = new Notification(re, content);
                    noti.TopMost = true;
                    Thread.Sleep(100);
                    noti.Show();
                }
                else
                {
                    Notification noti = new Notification(re, content);
                    noti.TopMost = true;
                    Thread.Sleep(2000);
                    noti.Show();
                }
            }
            catch (MySqlException)
            {
                MessageBox.Show("MySQL chưa được khởi động hoặc tên CSDL bị sai\r\nVui lòng kiểm tra lại");
            }
            catch (SocketException)
            {
                MessageBox.Show("Không kết nối được với cơ sở dữ liệu");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kết nối thất bại\r\n"+ ex.Message);
            }
            conn.Close();
            isOpen = 0;
        }

        private void registerHotKey()
        {
            this.Unsubscribe();
            this.Subscribe();
            string[] hot;
            string key="Shift+F";
            if (File.Exists("hotkey.txt"))
            {
                hot = File.ReadAllLines("hotkey.txt");
                key = hot[0];
            }
            var undo = Combination.FromString(key);

            Action actionUndo = showResult;

            var assignment = new Dictionary<Combination, Action>
            {
                {undo, actionUndo}
            };

            Hook.GlobalEvents().OnCombination(assignment);
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc là muốn thoát không?", "IT Dictionary", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void Main_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notify.Visible = true;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                notify.Visible = false;
            }
        }
        private void notify_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            notify.Visible = false;
            WindowState = FormWindowState.Normal;
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            string content = txtSearch.Text.Trim();
            if (content.Equals(""))
                return;
            try
            {
                ArrayList re = new ArrayList();
                //If the connection isn't open, it will open and set value of isOpen parameter to 1
                if (isOpen == 0)
                {
                    conn.Open();
                    isOpen = 1;
                }
                re = manage.findWord(conn, content);

                if (re.Count == 0)
                {
                    Dictionary a = new Dictionary(0, "Không tìm thấy kết quả", "", "", "", "");
                    re.Add(a);
                    Notification noti = new Notification(re, content);
                    noti.TopMost = true;
                    noti.Show();
                }
                else
                {
                    Notification noti = new Notification(re, content);
                    noti.TopMost = true;
                    noti.Show();
                }
            }
            catch (MySqlException)
            {
                MessageBox.Show("MySQL chưa được khởi động hoặc tên CSDL bị sai\r\nVui lòng kiểm tra lại");
            }
            catch (SocketException)
            {
                MessageBox.Show("Không kết nối được với cơ sở dữ liệu");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kết nối thất bại\r\n" + ex.Message);
            }
            conn.Close();
            isOpen = 0;
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            Setting s = new Setting();
            s.Button_Clicked += new EventHandler(btnReset_Click);
            s.Button2_Clicked += new EventHandler(btnResetHotKeys_Click);
            s.Show();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            this.resetConnection();
        }
        private void btnResetHotKeys_Click(object sender, EventArgs e)
        {
            this.registerHotKey();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            registerHotKey();
        }

        private async void MouseDragFinished(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            IDataObject tmpClipboard = Clipboard.GetDataObject();

            Clipboard.Clear();

            await Task.Delay(50);

            SendKeys.SendWait("^c");

            await Task.Delay(50);

            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                this.txtSearch.Text = text;
            }
            else
            {
                Clipboard.SetDataObject(tmpClipboard);
            }
        }

        private void hiệnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            notify.Visible = false;
            WindowState = FormWindowState.Normal;
        }

        private void tắtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc là muốn thoát không?", "IT Dictionary", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void cấuHìnhDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setting s = new Setting();
            s.Button_Clicked += new EventHandler(btnReset_Click);
            s.Button2_Clicked += new EventHandler(btnResetHotKeys_Click);
            s.Show();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                btnFind_Click(sender, e);
            }
        }
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Unsubscribe();
        }
    }
}
