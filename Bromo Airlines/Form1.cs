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
    public partial class Login : Form
    {
        string Db;
        public Login()
        {
            InitializeComponent();

            Conect open = new Conect();
            Db = open.Database;
        }

        public void cek_input()
        {
            if (User_name.Text == "" || Password.Text == "")
            {
                button_Login.Enabled = false;
            }
            else
            {
                button_Login.Enabled = true;
            }
        }

        public void salah()
        {
            User_name.Clear();
            Password.Clear();
        }
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            cek_input();
        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            cek_input();
        }

        private void User_name_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Password.Focus();
            }
        }

        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) button_Login.PerformClick();
            else if (e.KeyCode == Keys.Up) User_name.Focus();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Daftar_Akun open = new Daftar_Akun();
            open.Show();
            this.Hide();
        }

        private void Button_Login_Click(object sender, EventArgs e)
        {
            InpStatus.Status();
            if (User_name.Text == "")
            {
                MessageBox.Show("Harap mengisi Username", "Error Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (Password.Text == "") MessageBox.Show("Harap mengisi Password", "Error Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                using (SqlConnection connection = new SqlConnection(Db))
                {
                    connection.Open();
                    string query = $"SELECT COUNT(1) FROM Akun WHERE Username = @Username AND Password = @Password"; // Ganti your_table dengan nama tabel yang benar.

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", User_name.Text);
                        command.Parameters.AddWithValue("@Password", Password.Text);

                        int count = Convert.ToInt32(command.ExecuteScalar());

                        if (count == 1)
                        {
                            // Gantilah 'kolom_admin' dengan nama kolom yang menyimpan informasi apakah pengguna adalah admin atau bukan.
                            string queryAdminCheck = $"SELECT MerupakanAdmin, Nama FROM Akun WHERE Username = @Username";

                            using (SqlCommand commandAdminCheck = new SqlCommand(queryAdminCheck, connection))
                            {
                                commandAdminCheck.Parameters.AddWithValue("@Username", User_name.Text);

                                bool isAdmin = Convert.ToBoolean(commandAdminCheck.ExecuteScalar());
                                string pesan = "";

                                using (SqlDataReader reader = commandAdminCheck.ExecuteReader())
                                {
                                    if (isAdmin)
                                    {
                                        reader.Read();
                                        pesan = $"Admin {reader["Nama"]}";
                                        Main_Form open = new Main_Form();
                                        open.Show();
                                        this.Hide();
                                    }
                                    else
                                    {
                                        if (reader.Read())
                                        {
                                            pesan = reader["Nama"].ToString();
                                            Update on = new Update();
                                            on.On(pesan);
                                            Bromo_Airlines_Customer open = new Bromo_Airlines_Customer();
                                            open.Show();
                                            this.Hide();
                                        }
                                    }
                                    MessageBox.Show($"Login berhasil! \nWellcome to Bromo AirLines \n {pesan}","Login",MessageBoxButtons.OK);

                                }
                            }
                        }
                        else
                        {
                            
                            MessageBox.Show("Login gagal. Periksa kembali nama pengguna dan kata sandi Anda.");
                            salah();
                        }
                        
                    }
                }
            }
        }


    }
}

