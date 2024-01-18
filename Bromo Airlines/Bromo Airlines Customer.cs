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
using System.Text.RegularExpressions;

namespace Bromo_Airlines
{
    public partial class Bromo_Airlines_Customer : Form
    {
        string Db;
        public string name { get; set; }

        public Bromo_Airlines_Customer()
        {
            InitializeComponent();
            Conect open = new Conect();
            Db = open.Database;
            value_combobox();
        }

        private void Bromo_Airlines_Customer_Load(object sender, EventArgs e)
        {
            ReadMiniData open = new ReadMiniData();
            open.On();
            Nama_Akun.Text= open.Nama+"?";
            dateTimePicker1.CustomFormat = "dddd,dd MMMM yyyy";
        }

        private void value_combobox()
        {
            using (SqlConnection connection = new SqlConnection(Db))
            {
                connection.Open();
                string query = $"SELECT Nama, Kota, KodeIATA FROM Bandara"; // Sesuaikan dengan nama tabel produk Anda
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                int a = 1;
                while (reader.Read())
                {
                    a++;
                    // Mendapatkan nilai dari setiap kolom
                    string nama = reader["Nama"].ToString();
                    string kota = reader["Kota"].ToString();
                    string kodeIATA = reader["KodeIATA"].ToString();

                    // Menggabungkan informasi dari setiap kolom dan menambahkannya ke ComboBox
                    comboBox1.Items.Add($"{nama}, {kota} ({kodeIATA})");
                    comboBox2.Items.Add($"{nama}, {kota} ({kodeIATA})");
                }
                comboBox1.DropDownHeight = a;
                comboBox2.DropDownHeight = a;
                connection.Close();
            }
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == comboBox2.SelectedIndex)
            {
                MessageBox.Show("Mohon untuk menetapkan tujuan yang sesuai");
                comboBox2.SelectedIndex = -1;
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == comboBox2.SelectedIndex)
            {
                MessageBox.Show("Mohon untuk menetapkan tujuan yang sesuai");
                comboBox2.SelectedIndex = -1;
            }
        }

        private void Bromo_Airlines_Customer_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void PictureBox3_Click(object sender, EventArgs e)
        {
            Login open = new Login();
            open.Show();
            this.Hide();
        }

        private void PictureBox2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Tiket_saya form2 = new Tiket_saya();
            form2.Show();
        }
        string[] full_v = new string[2];
        private void Button1_Click(object sender, EventArgs e)
        {
            dateTimePicker1.CustomFormat = "ddd,dd MMM yyyy";
            string v1 = comboBox1.Text;
            string v2 = comboBox2.Text;

            // Mencocokkan kalimat awal dan akhir yang ada dalam kurung
            Match match = Regex.Match(v1, @"^(.+)\((.+)\)$");
            Match match2 = Regex.Match(v2, @"^(.+)\((.+)\)$");

            if (match.Success && match2.Success)
            {
                // Mencetak hasil
                string[] awal = { match.Groups[1].Value.Trim() ,match2.Groups[1].Value.Trim()};
                // Temukan indeks tanda koma
                int commaIndex = awal[0].IndexOf(',');
                int commaIndex1 = awal[1].IndexOf(',');

                if (commaIndex != -1 && commaIndex1 != -1)
                {
                    // Bagi string menjadi dua bagian
                    string bagianPertama = awal[0].Substring(0, commaIndex).Trim();
                    string bagiankedua = awal[1].Substring(0, commaIndex1).Trim();

                    string akhir = match.Groups[2].Value.Trim();
                    string akhir2 = match2.Groups[2].Value.Trim();
                    full_v[0] = $"{bagianPertama} ({akhir})";
                    full_v[1] = $"{bagiankedua} ({akhir2})";
                }
            }


            Cari_Penerbangan open = new Cari_Penerbangan(comboBox1.SelectedIndex+1,comboBox2.SelectedIndex+1,full_v[0],full_v[1],dateTimePicker1.Text, (int)numericUpDown1.Value);
            open.Show();
            this.Hide();
        }

        private void ComboBox1_Click(object sender, EventArgs e)
        {
            comboBox1.DroppedDown = true;
            comboBox1.Cursor = default;
        }

        private void ComboBox2_Click(object sender, EventArgs e)
        {
            comboBox2.DroppedDown = true;
            comboBox2.Cursor = default;
        }

        private void ComboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift && e.KeyCode == Keys.Enter) button1.PerformClick();
        }
    }
}
