using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;

namespace Bromo_Airlines
{
    public partial class Main_Form : Form
    {
        bool on_of = false;
        public Main_Form()
        {
            InitializeComponent();
            menu_on_of();
        }


        public void menu_on_of()
        {
            on_of = !on_of;
            if (on_of == true)
            {
                Menu.Size =new Size(260,0);
            }
            else
            {
                Menu.Size = new Size(69, 0);
            }
        }

        private void PictureBox2_Click_1(object sender, EventArgs e)
        {
            Menu_set.Bandara();
            this.Hide();
        }

        private void PictureBox3_Click(object sender, EventArgs e)
        {
            Menu_set.Maskapai();
            this.Hide();
        }

        private void PictureBox4_Click(object sender, EventArgs e)
        {
            Menu_set.Jadwal();
            this.Hide();
        }

        private void PictureBox5_Click(object sender, EventArgs e)
        {
            Menu_set.KodePromo();
            this.Hide();
        }

        private void PictureBox6_Click(object sender, EventArgs e)
        {
            Menu_set.UbahJadwal();
            this.Hide();
        }

        private void PictureBox7_Click(object sender, EventArgs e)
        {
            Menu_set.Login();
            this.Hide();
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            menu_on_of();
        }

        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
