using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IT_Dic
{
    public partial class ListItem : UserControl
    {
        private string content;
        public ListItem()
        {
            InitializeComponent();
        }
        public string Content
        {
            get => content;
            set
            {
                content = value;
                this.lblContent.Text = content;
            }
        }

        private void ListItem_Load(object sender, EventArgs e)
        {
            this.lblContent.AutoSize = true;
            this.lblContent.MaximumSize = new Size(560, 0);
            this.lblContent.MinimumSize = new Size(560, 0);
            this.Height = lblContent.Height;
        }
    }
}
