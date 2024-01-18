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
    public partial class Master_Maskapai : Form
    {
        string Db;
        private int selectedRowIndex = -1;
        bool on_of = false;
        string YourSelectedID;

        string[] db_name = { "Nama"
                , "Perusahaan"
                , "JumlahKru"
                , "Deskripsi"};
        string db_n;
        string id;

        public Master_Maskapai()
        {
            InitializeComponent();
            menu_on_of();

            Conect open = new Conect();
            Db=open.Database;

             db_n = string.Join(",", db_name);
        }
        private void menu_on_of()
        {
            on_of = !on_of;
            if (on_of == true)
            {
                Menu.Size = new Size(260, 0);
            }
            else
            {
                Menu.Size = new Size(69, 0);
            }
        }

        private void Master_Jadwal_Penerbangan_Load(object sender, EventArgs e)
        {
            output_db();

            // Membuat kolom tombol dan menambahkannya ke DataGridView
            DataGridViewButtonColumn buttonColumn1 = new DataGridViewButtonColumn();
            buttonColumn1.HeaderText = "";
            buttonColumn1.Name = "btn_ubah";
            buttonColumn1.Text = "Ubah";
            buttonColumn1.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(buttonColumn1);

            // Tambahkan kolom tombol kedua
            DataGridViewButtonColumn buttonColumn2 = new DataGridViewButtonColumn();
            buttonColumn2.HeaderText = "";
            buttonColumn2.Name = "btn_hapus";
            buttonColumn2.Text = "Hapus";
            buttonColumn2.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(buttonColumn2);
        }

        private void output_db()
        {
            using (SqlConnection connection = new SqlConnection(Db))
            {
                connection.Open();
                string query = $"SELECT ID,{db_n} FROM Maskapai ORDER BY {db_name[0]} ASC"; // Ganti your_table dengan nama tabel yang benar.

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Tampilkan data dalam DataGridView
                        dataGridView1.DataSource = dataTable;
                        dataGridView1.Columns["ID"].Visible = false;
                    }

                }
            }
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
                YourSelectedID = row.Cells["ID"].Value.ToString();
                string del = row.Cells[db_name[0]].Value.ToString();

                using (SqlConnection connection = new SqlConnection(Db))
                {
                    connection.Open();

                    DialogResult pesan;
                    if (column.Name=="btn_ubah")
                    {
                        selectedRowIndex = e.RowIndex;
                        nama_maskapai.Text = dataGridView1.Rows[e.RowIndex].Cells[db_name[0]].Value.ToString();
                        perusahaan.Text = dataGridView1.Rows[e.RowIndex].Cells[db_name[1]].Value.ToString();
                        jumlah_kru.Value = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[db_name[2]].Value);
                        deskripsi.Text = dataGridView1.Rows[e.RowIndex].Cells[db_name[3]].Value.ToString();
                    }
                    else if (column.Name == "btn_hapus")
                    {
                        pesan = MessageBox.Show($"ID dengan {db_name[0]} {del} akan di hapus", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (pesan == DialogResult.Yes)
                        {
                            string deleteQuery = $"DELETE FROM Maskapai WHERE {db_name[0]} = @ID";

                            using (SqlCommand cmd = new SqlCommand(deleteQuery, connection))
                            {
                                // Gantilah "YourSelectedID" dengan ID data yang akan dihapus
                                cmd.Parameters.AddWithValue("@ID", YourSelectedID);

                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Data berhasil dihapus.");
                                }
                                else
                                {
                                    MessageBox.Show("Gagal menghapus data.");
                                }
                                output_db();
                            }

                        }
                        else
                        {
                            MessageBox.Show($"Data dengan {db_name[0]} {del} batal di hapus");
                        }
                    }
                }
            }
        }

        private void Button_batal_Click(object sender, EventArgs e)
        {
            nama_maskapai.Clear();
            perusahaan.Clear();
            jumlah_kru.Value = 1;
            deskripsi.Clear();
        }

        private void Button_simpan_Click(object sender, EventArgs e)
        {
            if (nama_maskapai.Text == "" || perusahaan.Text == "" || deskripsi.Text == "")
            {
                string sms = "";

                if (string.IsNullOrEmpty(nama_maskapai.Text)) sms = "Nama maskapai";
                else if (string.IsNullOrEmpty(perusahaan.Text)) sms = "Nama perusahaan";
                else if (string.IsNullOrEmpty(deskripsi.Text)) sms = "Deskripsi";

                MessageBox.Show($"{sms} belum diisi, mohon untuk dilengkapi.", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else
            {
                if (selectedRowIndex >= 0)
                {

                    // Ambil nilai ID dari kolom pertama (indeks 0)
                    using (SqlConnection connection = new SqlConnection(Db))
                    {
                        connection.Open();

                        // Buat perintah SQL untuk menyimpan data ke database
                        string query = $"UPDATE Maskapai SET {db_name[0]} = @NilaiKolom1, {db_name[1]} = @NilaiKolom2 , {db_name[2]} = @NilaiKolom3 , {db_name[3]} = @NilaiKolom4 WHERE ID = @ID ";
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            // Ganti parameter dan nilai sesuai dengan data yang akan disimpan
                            cmd.Parameters.AddWithValue("@NilaiKolom1", nama_maskapai.Text);
                            cmd.Parameters.AddWithValue("@NilaiKolom2", perusahaan.Text);
                            cmd.Parameters.AddWithValue("@NilaiKolom3", jumlah_kru.Value);
                            cmd.Parameters.AddWithValue("@NilaiKolom4", deskripsi.Text);
                            cmd.Parameters.AddWithValue("@ID", YourSelectedID);


                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Data berhasil diperbarui.");
                                button_batal.PerformClick();
                            }
                            else
                            {
                                MessageBox.Show("Gagal memperbarui data.");
                            }
                        }
                        connection.Close();
                        output_db();
                    }


                    // Reset input dan tombol "Simpan"
                    button_batal.PerformClick();
                }
                else
                {

                    using (SqlConnection connection = new SqlConnection(Db))
                    {
                        connection.Open();

                        // Buat perintah SQL untuk menyimpan data ke database
                        string query = $"INSERT INTO Maskapai ({db_name[0]},{db_name[1]},{db_name[2]},{db_name[3]}) VALUES (@NilaiKolom1, @NilaiKolom2, @NilaiKolom3, @NilaiKolom4)";
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            // Ganti parameter dan nilai sesuai dengan data yang akan disimpan
                            cmd.Parameters.AddWithValue("@NilaiKolom1", nama_maskapai.Text);
                            cmd.Parameters.AddWithValue("@NilaiKolom2", perusahaan.Text);
                            cmd.Parameters.AddWithValue("@NilaiKolom3", jumlah_kru.Value);
                            cmd.Parameters.AddWithValue("@NilaiKolom4", deskripsi.Text);

                            int affectedRows = cmd.ExecuteNonQuery();

                            if (affectedRows > 0)
                            {
                                MessageBox.Show("Data berhasil disimpan ke database.");
                            }
                            else
                            {
                                MessageBox.Show("Gagal menyimpan data ke database.");
                            }
                        }
                        connection.Close();
                        output_db();
                    }
                }

            }
        }

        private void PictureBox2_Click(object sender, EventArgs e)
        {
            Menu_set.Bandara();
            this.Close();
        }

        private void PictureBox4_Click(object sender, EventArgs e)
        {
            Menu_set.Jadwal();
            this.Close();
        }

        private void PictureBox5_Click(object sender, EventArgs e)
        {
            Menu_set.KodePromo();
            this.Close();
        }

        private void PictureBox6_Click(object sender, EventArgs e)
        {
            Menu_set.UbahJadwal();
            this.Close();
        }

        private void PictureBox7_Click(object sender, EventArgs e)
        {
            Menu_set.Login();
            this.Close();
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            menu_on_of();
        }
    }
}
