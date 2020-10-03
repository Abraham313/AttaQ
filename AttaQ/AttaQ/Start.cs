using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AttaQ
{
    public partial class Start : Form
    {
        public Start()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Update UP = new Update();
            UP.Show();
        }

        private void Start_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            // Form Size 고정
        }

        private void Bt_exit_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("정말로 보안패치를 종료하시겠습니까 ?", "경고", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}
