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
    public partial class Daftar_Akun : Form
    {
        string Db;
        public Daftar_Akun()
        {
            InitializeComponent();
            Conect open = new Conect();
            Db=open.Database;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
            {
                string pesan="";
                if (textBox1.Text == "") pesan = "User Name";
                else if (textBox2.Text == "") pesan = "Nama";
                else if (textBox3.Text == "") pesan = "Nomor Telepon";
                else if (textBox4.Text == "") pesan = "Password";
                MessageBox.Show(pesan+" Kosong, mohon Diisi", "Data pendaftaran", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (textBox3.TextLength < 10)
            {
                MessageBox.Show("Masukkan nomor telepon yang benar.\nMinimal adalah 10 digit", "Nomor Telepon", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (textBox4.TextLength < 8)
            {
                MessageBox.Show("Minimal password adalah 8 karakter", "Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                using(SqlConnection connect = new SqlConnection(Db))
                {
                    connect.Open();
                    SqlCommand con = new SqlCommand("SELECT COUNT(*) FROM Akun WHERE Username = @use", connect);

                    con.Parameters.AddWithValue("@use", textBox1.Text);

                    int count = (int)con.ExecuteScalar();
                    if (count < 0)
                    {

                        string qery = "INSERT INTO Akun (Username,Password,Nama,TanggalLahir,NomorTelepon,MerupakanAdmin) VALUES (@data1,@data2,@data3,@data4,@data5,@data6)";
                        using (SqlCommand command = new SqlCommand(qery, connect))
                        {
                            command.Parameters.AddWithValue("@data1", textBox1.Text);
                            command.Parameters.AddWithValue("@data2", textBox4.Text);
                            command.Parameters.AddWithValue("@data3", textBox2.Text);
                            command.Parameters.AddWithValue("@data4", dateTimePicker1.Value);
                            command.Parameters.AddWithValue("@data5", textBox3.Text);
                            command.Parameters.AddWithValue("@data6", "False");

                            int cekSave = command.ExecuteNonQuery();
                            if (cekSave > 0)
                            {
                                MessageBox.Show("Data berhasil disimpan ke database.");
                                Update on = new Update();
                                on.On(textBox2.Text);
                                Bromo_Airlines_Customer open = new Bromo_Airlines_Customer();
                                open.Show();
                            }
                            else
                            {
                                MessageBox.Show("Gagal menyimpan data ke database.","DataBase", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Username Telah Digunakan.","Username",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        Hapus();
                    }
                }
            }
        }

        public void Hapus()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            dateTimePicker1.Value = DateTime.Now;
        }
        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Login open = new Login();
            open.Show();
            this.Hide();
            this.Close();
        }

        private void Daftar_Akun_FormClosing(object sender, FormClosingEventArgs e)
        {
            Login open = new Login();
            open.Show();
        }

        private void TextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Pengecekan input yang dapat digunakan
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled=true;
            }
        }
    }
}
