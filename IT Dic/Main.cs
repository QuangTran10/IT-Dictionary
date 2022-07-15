﻿using Gma.System.MouseKeyHook;
using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        private string textCopy;

        public Main()
        {
            InitializeComponent();

            SubscribeGlobal();

            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(borderSize);
            this.panel2.BackColor = borderColor;
            this.BackColor = borderColor;
 
            manage = new ManageDictionary();

            conn = ConnectDB.Connect();

        }
        private void SubscribeGlobal()
        {
            Unsubscribe();
            Subscribe(Hook.GlobalEvents());
        }
        private void Subscribe(IKeyboardMouseEvents events)
        {
            globalMouseHook = events;

            globalMouseHook.MouseDragStarted += MouseDragStarted;

            globalMouseHook.MouseDragFinishedExt += MouseDragFinished;
        }

        private void Unsubscribe()
        {
            if (globalMouseHook == null) return;

            globalMouseHook.MouseDragStarted -= MouseDragStarted;
            globalMouseHook.MouseDragFinishedExt -= MouseDragFinished;

            globalMouseHook.Dispose();
            globalMouseHook = null;
        }

        private void resetConnection()
        {
            conn.Close();
            conn = ConnectDB.Connect();
        }
        private void showResult()
        {
            string content = textCopy.Trim();
            Debug.Print("Call hot key completed");

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
            Logger logger = LogManager.GetLogger("fileLogger");
            try
            {
                //If the connection isn't open, it will open and set value of isOpen parameter to 1
                conn.Close();
                conn.Open();
                ArrayList re = new ArrayList();

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
            catch (MySqlException ex)
            {
                MessageBox.Show("MySQL chưa được khởi động hoặc tên CSDL bị sai\r\nVui lòng kiểm tra lại"+ ex.StackTrace);
                logger.Error(ex, ex.Message);
            }
            catch (SocketException)
            {
                MessageBox.Show("Không kết nối được với cơ sở dữ liệu");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kết nối thất bại\r\n"+ ex.Message);
                logger.Error(ex, ex.Message);
            }
            conn.Close();
        }

        private void registerHotKey()
        {
            SubscribeGlobal();
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
            WindowState = FormWindowState.Minimized;
            SubscribeGlobal();
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
            Logger logger = LogManager.GetLogger("fileLogger");
            string content = txtSearch.Text.Trim();
            if (content.Equals(""))
                return;
            try
            {
                ArrayList re = new ArrayList();
                //If the connection isn't open, it will open and set value of isOpen parameter to 1
                conn.Close();
                conn.Open();
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
            catch (MySqlException ex)
            {
                MessageBox.Show("MySQL chưa được khởi động hoặc tên CSDL bị sai\r\nVui lòng kiểm tra lại");
                logger.Error(ex, ex.Message);
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
            SubscribeGlobal();
            registerHotKey();
        }
        private void MouseDragStarted(object sender, MouseEventArgs e)
        {
            //Log("MouseDragStarted\n");
        }


        [STAThread]
        private async void MouseDragFinished(object sender, MouseEventExtArgs e)
        {
            Logger logger = LogManager.GetLogger("fileLogger");

            CancellationToken token = new CancellationToken();
            try
            {
                await Task.Delay(50);

                SendKeys.SendWait("^c");

                SendKeys.Flush();

                await Task.Delay(50);

                textCopy = Clipboard.GetText(TextDataFormat.UnicodeText);

                Thread.Sleep(50);

                txtSearch.Text = textCopy;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                logger.Error(ex, ex.Message);
            }

            //If you want to select the text and display the results at the same time, please uncomment
            //showResult();

        }

        private void hiệnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            notify.Visible = false;
            WindowState = FormWindowState.Normal;
            SubscribeGlobal();
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
