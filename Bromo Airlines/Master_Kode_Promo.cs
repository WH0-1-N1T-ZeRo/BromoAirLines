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
    public partial class Master_Kode_Promo : Form
    {
        string Db;
        private int selectedRowIndex = -1;
        bool on_of = false;
        string YourSelectedID;

        string[] db_name = { "Kode", "PersentaseDiskon", "MaksimumDiskon","BerlakuSampai","Deskripsi" };

        public Master_Kode_Promo()
        {
            InitializeComponent();

            Conect open = new Conect();
            Db=open.Database;
        }

        public void menu_on_of()
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

        private void Master_Kode_Promo_Load(object sender, EventArgs e)
        {
            output_db();

            // Membuat kolom tombol dan menambahkannya ke DataGridView
            DataGridViewButtonColumn buttonColumn1 = new DataGridViewButtonColumn();
            buttonColumn1.HeaderText = "";
            buttonColumn1.Name = "ButtonUbah";
            buttonColumn1.Text = "Ubah";
            buttonColumn1.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(buttonColumn1);

            // Tambahkan kolom tombol kedua
            DataGridViewButtonColumn buttonColumn2 = new DataGridViewButtonColumn();
            buttonColumn2.Text = "Hapus";
            buttonColumn2.HeaderText = "";
            buttonColumn2.Name = "ButtonHapus";
            buttonColumn2.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(buttonColumn2);
        }

        public void output_db()
        {
            using (SqlConnection connection = new SqlConnection(Db))
            {
                connection.Open();
                string query = "SELECT * FROM KodePromo"; // Ganti your_table dengan nama tabel yang benar.

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
                YourSelectedID = row.Cells["ID"].Value.ToString();
                DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
                if (column.Name == "ButtonUbah")
                {
                    selectedRowIndex = e.RowIndex;
                    kode_promo.Text = dataGridView1.Rows[e.RowIndex].Cells[db_name[0]].Value.ToString();
                    diskon.Value = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[db_name[1]].Value);
                    maximum_diskom.Value = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[db_name[2]].Value);
                    tgl_berlaku.Text = ((DateTime)dataGridView1.Rows[e.RowIndex].Cells[db_name[3]].Value).ToString("yyyy-MM-dd");
                    deskripsi.Text = dataGridView1.Rows[e.RowIndex].Cells[db_name[4]].Value.ToString();
                }
                else if (column.Name == "ButtonHapus")
                {
                    DialogResult pesan;

                    pesan = MessageBox.Show($"ID ke ${YourSelectedID} akan di hapus", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (pesan == DialogResult.Yes)
                    {

                        using (SqlConnection connection = new SqlConnection(Db))
                        {
                            connection.Open();

                            // Gantilah "YourTableName" dengan nama tabel Anda dan "ID" dengan nama kolom yang sesuai
                            string deleteQuery = "DELETE FROM KodePromo WHERE ID = @ID";

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
                    }
                }
            }
        }

        private void Button_simpan_Click(object sender, EventArgs e)
        {
            if (kode_promo.Text == ""|| deskripsi.Text == "")
            {
                string pesan="";
                if (kode_promo.Text == "") pesan = "Kode Promo";
                else pesan = "Deskripsi";
                MessageBox.Show($"{pesan} Belum diisi , mohon untuk dilengkapi", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        string query = $"UPDATE KodePromo SET {db_name[0]} = @NilaiKolom1, {db_name[1]} = @NilaiKolom2 , {db_name[2]} = @NilaiKolom3 , {db_name[3]} = @NilaiKolom4, {db_name[4]} = @NilaiKolom5 WHERE ID = @ID ";
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            // Ganti parameter dan nilai sesuai dengan data yang akan disimpan
                            cmd.Parameters.AddWithValue("@NilaiKolom1", kode_promo.Text);
                            cmd.Parameters.AddWithValue("@NilaiKolom2", diskon.Value);
                            cmd.Parameters.AddWithValue("@NilaiKolom3", maximum_diskom.Value);
                            cmd.Parameters.AddWithValue("@NilaiKolom4", tgl_berlaku.Value);
                            cmd.Parameters.AddWithValue("@NilaiKolom5", deskripsi.Text);
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
                        string query = $"INSERT INTO KodePromo ({db_name[0]}, {db_name[1]}, {db_name[2]}, {db_name[3]}, {db_name[4]}) VALUES (@NilaiKolom1, @NilaiKolom2, @NilaiKolom3, @NilaiKolom4, @NilaiKolom5)";
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            // Ganti parameter dan nilai sesuai dengan data yang akan disimpan
                            cmd.Parameters.AddWithValue("@NilaiKolom1", kode_promo.Text);
                            cmd.Parameters.AddWithValue("@NilaiKolom2", diskon.Value);
                            cmd.Parameters.AddWithValue("@NilaiKolom3", maximum_diskom.Value);
                            cmd.Parameters.AddWithValue("@NilaiKolom4", tgl_berlaku.Value);
                            cmd.Parameters.AddWithValue("@NilaiKolom5", deskripsi.Text);

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
                button_batal.PerformClick();

            }
        }

        private void Button_batal_Click(object sender, EventArgs e)
        {
            kode_promo.Clear();
            diskon.Value = 1;
            maximum_diskom.Value = 1;
            deskripsi.Clear();
            selectedRowIndex = -1;
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            menu_on_of();
        }

        private void PictureBox2_Click(object sender, EventArgs e)
        {
            Menu_set.Bandara();
            this.Close();
        }

        private void PictureBox3_Click(object sender, EventArgs e)
        {
            Menu_set.Maskapai();
            this.Close();
        }

        private void PictureBox4_Click(object sender, EventArgs e)
        {
            Menu_set.Jadwal();
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
    }
}
