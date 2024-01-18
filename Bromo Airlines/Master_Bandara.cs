using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Bromo_Airlines
{
    public partial class Master_Bandara : Form
    {

        string Db;

        private int selectedRowIndex = -1;
        bool on_of=false;
        string YourSelectedID;

        string[] db_name = { "Nama", "KodeIATA", "Kota", "NegaraID", "JumlahTerminal", "Alamat" };
        string db_n { get; set; }
        public Master_Bandara()
        {
            InitializeComponent();
            Conect open = new Conect();
            db_n = string.Join(",", db_name);
            Db = open.Database;
            value_combobox();
            menu_on_of();
        }
        private void value_combobox()
        {
            using (SqlConnection connection = new SqlConnection(Db))
            {
                connection.Open();
                string query = $"SELECT {db_name[0]} FROM Negara"; // Sesuaikan dengan nama tabel produk Anda
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    negara.Items.Add(reader["Nama"].ToString());
                }
                connection.Close();
            }
        }
        public void output_db()
        {
            using (SqlConnection connection = new SqlConnection(Db))
            {
                connection.Open();
                string query = $"SELECT Bandara.ID, Bandara.Nama AS Nama, Bandara.KodeIATA, Bandara.Kota, Negara.Nama AS Negara, Bandara.JumlahTerminal, Bandara.Alamat FROM Bandara JOIN Negara ON Bandara.NegaraID = Negara.ID ORDER BY {db_name[0]} ASC"; // Ganti your_table dengan nama tabel yang benar.

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
        public void load_m()
        {
            nama.KeyPress += new KeyPressEventHandler(TextBoxes_KeyPress);
            kode_iata.KeyPress += new KeyPressEventHandler(TextBoxes_KeyPress);
            kota.KeyPress += new KeyPressEventHandler(TextBoxes_KeyPress);
            negara.KeyPress += new KeyPressEventHandler(TextBoxes_KeyPress);
            jumlah_terminal.KeyPress += new KeyPressEventHandler(TextBoxes_KeyPress);
            alamat.KeyPress += new KeyPressEventHandler(TextBoxes_KeyPress);
        }
        private void Master_Bandara_Load(object sender, EventArgs e)
        {
            output_db();
            // Membuat kolom tombol "Ubah"
            DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
            buttonColumn.Name = "UbahButtonColumn";
            buttonColumn.HeaderText = "";
            buttonColumn.Text = "Ubah";
            buttonColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(buttonColumn);

            // Membuat kolom tombol "Hapus"
            DataGridViewButtonColumn buttonColumn2 = new DataGridViewButtonColumn();
            buttonColumn2.Name = "HapusButtonColumn";
            buttonColumn2.HeaderText = "";
            buttonColumn2.Text = "Hapus";
            buttonColumn2.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(buttonColumn2);

            load_m();
        }


        public void menu_on_of()
        {
            on_of = !on_of;
            if (on_of==true)
            {
                Menu_system.Size = new Size(260, 0);
            }
            else
            {
                Menu_system.Size = new Size(69, 0);
            }

        }


        private void TextBoxes_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                // Menemukan kontrol berikutnya dalam urutan tab
                Control nextControl = GetNextControl((Control)sender, true);

                while (nextControl != null && !nextControl.TabStop)
                {
                    nextControl = GetNextControl(nextControl, true);
                }

                if (nextControl != null)
                {
                    nextControl.Focus();

                    if (nextControl is TextBox)
                    {
                        // Jika kontrol berikutnya adalah TextBox, pilih semua teks di dalamnya
                        ((TextBox)nextControl).SelectAll();
                    }
                    else
                    {
                        nextControl.Focus();
                    }
                }

                e.Handled = true; // Mencegah karakter Enter ditampilkan di dalam kontrol
            }
        }

        private void Button_batal_Click(object sender, EventArgs e)
        {
            nama.Clear();
            kode_iata.Clear();
            kota.Clear();
            negara.Text = "";
            jumlah_terminal.Value = 1;
            alamat.Clear();
            selectedRowIndex = 0;
        }
        private void DataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            // Periksa apakah tombol di kolom yang diinginkan diklik
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
                YourSelectedID = row.Cells["ID"].Value.ToString();
                string del = row.Cells[db_name[1]].Value.ToString();

                if (column.Name == "UbahButtonColumn")
                {
                    // Blok untuk operasi ubah
                    selectedRowIndex = e.RowIndex;

                    nama.Text = row.Cells[db_name[0]].Value.ToString();
                    kode_iata.Text = row.Cells[db_name[1]].Value.ToString();
                    kota.Text = row.Cells[db_name[2]].Value.ToString();
                    negara.SelectedIndex = Convert.ToInt32(row.Cells[db_name[3]].Value) - 1;
                    jumlah_terminal.Value = Convert.ToInt32(row.Cells[db_name[4]].Value);
                    alamat.Text = row.Cells[db_name[5]].Value.ToString();
                }
                if (column.Name == "HapusButtonColumn")
                {
                    // Blok untuk operasi hapus
                    using (SqlConnection connection = new SqlConnection(Db))
                    {
                        connection.Open();

                        DialogResult pesan;

                        // Operasi Hapus
                        pesan = MessageBox.Show($"ID dengan {db_name[0]} {del} akan dihapus", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (pesan == DialogResult.Yes)
                        {
                            string deleteQuery = $"DELETE FROM Bandara WHERE ID = @ID";

                            using (SqlCommand cmd = new SqlCommand(deleteQuery, connection))
                            {
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
                            MessageBox.Show($"Data dengan {db_name[0]} {del} Batal dihapus.");
                        }


                        connection.Close();
                    }
                }

                MessageBox.Show(selectedRowIndex.ToString());
            }
        }

        private void Button_simpan_Click(object sender, EventArgs e)
        {
            if (nama.Text == "" || kode_iata.Text == "" || kota.Text == "" || negara.Text == "" || jumlah_terminal.Value == 0 || alamat.Text == "")
            {
                string errorMessage = "";

                if (string.IsNullOrEmpty(nama.Text)) errorMessage = "Nama";
                else if (string.IsNullOrEmpty(kode_iata.Text)) errorMessage = "Kode IATA";
                else if (string.IsNullOrEmpty(kota.Text)) errorMessage = "Kota";
                else if (string.IsNullOrEmpty(negara.Text)) errorMessage = "Negara";
                else if (jumlah_terminal.Value == 0) errorMessage = "Jumlah Terminal";
                else if (string.IsNullOrEmpty(alamat.Text)) errorMessage = "Alamat";

                MessageBox.Show($"{errorMessage} belum diisi, mohon untuk dilengkapi.", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else
            {
                //Untuk save update data
                if (selectedRowIndex >= 0)
                {

                    using (SqlConnection connection = new SqlConnection(Db))
                    {
                        connection.Open();

                        // Buat perintah SQL untuk menyimpan data ke database
                        string query = $"UPDATE Bandara SET {db_name[0]} = @NilaiKolom1, {db_name[1]} = @NilaiKolom2 , {db_name[2]} = @NilaiKolom3 , {db_name[3]} = @NilaiKolom4 , {db_name[4]} = @NilaiKolom5, {db_name[5]} = @NilaiKolom6 WHERE ID = @ID ";
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            // Ganti parameter dan nilai sesuai dengan data yang akan disimpan
                            cmd.Parameters.AddWithValue("@NilaiKolom1", nama.Text);
                            cmd.Parameters.AddWithValue("@NilaiKolom2", kode_iata.Text);
                            cmd.Parameters.AddWithValue("@NilaiKolom3", kota.Text);
                            cmd.Parameters.AddWithValue("@NilaiKolom4", negara.SelectedIndex + 1);
                            cmd.Parameters.AddWithValue("@NilaiKolom5", jumlah_terminal.Value);
                            cmd.Parameters.AddWithValue("@NilaiKolom6", alamat.Text);
                            cmd.Parameters.AddWithValue("@ID", YourSelectedID);


                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Data berhasil diperbarui.");
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
                //Untuk saving data
                else
                {
                    using (SqlConnection connection = new SqlConnection(Db))
                    {
                        connection.Open();
                        string query = "SELECT COUNT(*) FROM Bandara WHERE Nama = @input1 AND KodeIATA = @input2";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@input1", nama.Text);
                            command.Parameters.AddWithValue("@input2", kode_iata.Text);
                            int count = (int)command.ExecuteScalar();

                            if (count > 0)
                            {
                                // Data sudah ada, tampilkan pesan kesalahan
                                MessageBox.Show("Data sudah ada di dalam database!");
                            }
                            else
                            {
                                // Data unik, lanjutkan penyimpanan
                                // Buat perintah SQL untuk menyimpan data ke database
                                query = $"INSERT INTO Bandara ({db_n}) VALUES (@NilaiKolom1, @NilaiKolom2, @NilaiKolom3, @NilaiKolom4, @NilaiKolom5, @NilaiKolom6)";
                                using (SqlCommand cmd = new SqlCommand(query, connection))
                                {
                                    // Ganti parameter dan nilai sesuai dengan data yang akan disimpan
                                    cmd.Parameters.AddWithValue("@NilaiKolom1", nama.Text);
                                    cmd.Parameters.AddWithValue("@NilaiKolom2", kode_iata.Text);
                                    cmd.Parameters.AddWithValue("@NilaiKolom3", kota.Text);
                                    cmd.Parameters.AddWithValue("@NilaiKolom4", negara.SelectedIndex + 1);
                                    cmd.Parameters.AddWithValue("@NilaiKolom5", jumlah_terminal.Value);
                                    cmd.Parameters.AddWithValue("@NilaiKolom6", alamat.Text);

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

            }
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
