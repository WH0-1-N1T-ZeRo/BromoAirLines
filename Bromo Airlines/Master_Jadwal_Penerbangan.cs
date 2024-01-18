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
using System.Globalization;
using System.Text.RegularExpressions;

namespace Bromo_Airlines
{
    public partial class Master_Jadwal_Penerbangan : Form
    {
        string Db;
        string[] db_name = { "KodePenerbangan"
                ,"BandaraKeberangkatan"
                ,"BandaraTujuan"
                ,"Maskapai"
                ,"TanggalKeberangkatan"
                ,"DurasiPenerbangan"
                ,"HargaPerTiket"};
        string db_n;
        string YourSelectedID;
        bool on_of;
        private int selectedRowIndex = -1;
        public Master_Jadwal_Penerbangan()
        {
            InitializeComponent();
            Conect open = new Conect();
            Db = open.Database;
            dataGridView1.CellFormatting += DataGridView1_CellFormatting;
            db_n = string.Join(",", db_name);
            value_combobox();
        }

        private void cek_KodePenerbangan()
        {
            // Gunakan ekspresi reguler untuk memeriksa format
            string pattern = "^[A-Za-z]{2}-\\d{4}$";
            if (Regex.IsMatch(kode_penerbangan.Text, pattern))
            {
                using (SqlConnection connection = new SqlConnection(Db))
                {
                    connection.Open();
                    string query = $"SELECT COUNT(*) FROM JadwalPenerbangan WHERE {db_name[0]} = @input1";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@input1", kode_penerbangan.Text);
                        int count = (int)command.ExecuteScalar();
                        if (count > 0) {
                            MessageBox.Show($"Data {db_name[0]} : {kode_penerbangan.Text} sudah ada di dalam database!", "Duplikasi", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            button_batal.PerformClick();
                        }
                    }
                }

            }
            else button_batal.PerformClick();
        }

        //Membuat combobox punya value dari database
        private void value_combobox()
        {
            using (SqlConnection connection = new SqlConnection(Db))
            {
                connection.Open();

                // Query untuk mendapatkan data dari tabel Bandara
                string bandaraQuery = "SELECT Nama AS Bandara FROM Bandara";
                SqlCommand bandaraCommand = new SqlCommand(bandaraQuery, connection);
                SqlDataReader bandaraReader = bandaraCommand.ExecuteReader();
                int a = 1;
                while (bandaraReader.Read())
                {
                    a++;
                    // Mendapatkan nilai dari kolom "Bandara"
                    string namaBandara = bandaraReader["Bandara"].ToString();
                    // Menambahkan nilai ke ComboBox
                    dari.Items.Add(namaBandara);
                    ke.Items.Add(namaBandara);
                }
                dari.DropDownHeight = a;
                ke.DropDownHeight = a;
                bandaraReader.Close(); // Tutup pembaca setelah selesai membaca data dari tabel Bandara

                // Query untuk mendapatkan data dari tabel Maskapai
                string maskapaiQuery = "SELECT nama AS Maskapai FROM Maskapai";
                SqlCommand maskapaiCommand = new SqlCommand(maskapaiQuery, connection);
                SqlDataReader maskapaiReader = maskapaiCommand.ExecuteReader();
                a = 1; // Reset variabel a untuk pembaca Maskapai
                while (maskapaiReader.Read())
                {
                    a++;
                    // Mendapatkan nilai dari kolom "Maskapai"
                    string nmMaskapai = maskapaiReader["Maskapai"].ToString();
                    // Menambahkan nilai ke ComboBox
                    maskapai.Items.Add(nmMaskapai);
                }
                maskapai.DropDownHeight = a;
                maskapaiReader.Close(); // Tutup pembaca setelah selesai membaca data dari tabel Maskapai

                connection.Close();
            }

        }


        private void Master_Jadwal_Penerbangan_Load(object sender, EventArgs e)
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
        }
        private void output_db()
        {
            using (SqlConnection connection = new SqlConnection(Db))
            {
                connection.Open();

                // Menentukan kolom yang akan dipilih
                string selectColumns = string.Join(", ", db_name.Select(col => $"JadwalPenerbangan.{col}"));

                // Membuat alias untuk kolom Bandara.ID agar bisa diakses sebagai BandaraNama
                string query = "SELECT JadwalPenerbangan.ID," +
                    $"JadwalPenerbangan.KodePenerbangan, " +
                    $"BandaraKeberangkatan.Nama AS BandaraKeberangkatan, " +
                    $"BandaraTujuan.Nama AS BandaraTujuan, " +
                    $"Maskapai.Nama AS Maskapai, " +
                    $"JadwalPenerbangan.TanggalWaktuKeberangkatan AS TanggalKeberangkatan, " +
                    $"JadwalPenerbangan.TanggalWaktuKeberangkatan AS WaktuKeberangkatan, " +
                    $"JadwalPenerbangan.DurasiPenerbangan, " +
                    $"JadwalPenerbangan.HargaPerTiket FROM JadwalPenerbangan " +
                    $"JOIN Bandara AS BandaraKeberangkatan ON JadwalPenerbangan.BandaraKeberangkatanID = BandaraKeberangkatan.ID " +
                    $"JOIN Bandara AS BandaraTujuan ON JadwalPenerbangan.BandaraTujuanID = BandaraTujuan.ID " +
                    $"JOIN Maskapai ON JadwalPenerbangan.MaskapaiID = Maskapai.ID";

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

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            // Periksa apakah tombol di kolom yang diinginkan diklik
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
                YourSelectedID = row.Cells["ID"].Value.ToString();

                if (column.Name == "UbahButtonColumn")
                {
                    // Blok untuk operasi ubah
                    row = dataGridView1.Rows[e.RowIndex];
                    selectedRowIndex = e.RowIndex;

                    kode_penerbangan.Text = row.Cells[db_name[0]].Value.ToString();
                    dari.Text = row.Cells[db_name[1]].Value.ToString();
                    ke.Text = row.Cells[db_name[2]].Value.ToString();
                    maskapai.Text = row.Cells[db_name[3]].Value.ToString();
                    harga.Value = Convert.ToInt32(row.Cells[db_name[6]].Value);
                    waktu_keb.Text = row.Cells["WaktuKeberangkatan"].FormattedValue.ToString();
                    durasi.Text = row.Cells[db_name[5]].FormattedValue.ToString();
                    tanggal.Value = Convert.ToDateTime(row.Cells[db_name[4]].Value);
                }
                if (column.Name == "HapusButtonColumn")
                {
                    // Blok untuk operasi hapus
                    using (SqlConnection connection = new SqlConnection(Db))
                    {
                        connection.Open();

                        row = dataGridView1.Rows[e.RowIndex];
                        string del = row.Cells[db_name[0]].Value.ToString();
                        DialogResult pesan;

                        // Operasi Hapus
                        pesan = MessageBox.Show($"ID dengan {db_name[0]} {del} akan dihapus", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (pesan == DialogResult.Yes)
                        {
                            string deleteQuery = $"DELETE FROM JadwalPenerbangan WHERE ID = @ID";

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
            }
        }

        private void Button_simpan_Click(object sender, EventArgs e)
        {
            if (kode_penerbangan.Text == "" || ke.Text == "" || dari.Text == "" || maskapai.Text == "" || harga.Value == 0 || tanggal.Text == "" || waktu_keb.Text == "" || durasi.Text == "")
            {
                string errorMessage = "";

                if (string.IsNullOrEmpty(kode_penerbangan.Text)) errorMessage = "Kode Penerbangan";
                else if (string.IsNullOrEmpty(dari.Text)) errorMessage = "Keberangkatan";
                else if (string.IsNullOrEmpty(ke.Text)) errorMessage = "Tujuan";
                else if (string.IsNullOrEmpty(maskapai.Text)) errorMessage = "Maskapai";
                else if (harga.Value == 0) errorMessage = "Harga";
                else if (string.IsNullOrEmpty(tanggal.Text)) errorMessage = "Tanggal";
                else if (string.IsNullOrEmpty(waktu_keb.Text)) errorMessage = "Waktu Keberangkatan";
                else if (string.IsNullOrEmpty(durasi.Text)) errorMessage = "Durasi Penerbangan";

                MessageBox.Show($"{errorMessage} belum diisi, mohon untuk dilengkapi.", errorMessage, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else
            {
                //Untuk save update data
                if (selectedRowIndex >= 0)
                {
                    cek_KodePenerbangan();
                    using (SqlConnection connection = new SqlConnection(Db))
                    {
                        connection.Open();

                        // Buat perintah SQL untuk menyimpan data ke database
                        string query = $"UPDATE JadwalPenerbangan SET {db_name[0]} = @NilaiKolom1, BandaraKeberangkatanID = @NilaiKolom2 , BandaraTujuanID = @NilaiKolom3 , MaskapaiID = @NilaiKolom4 , TanggalWaktuKeberangkatan = @NilaiKolom5, {db_name[5]} = @NilaiKolom6, {db_name[6]} = @NilaiKolom7 WHERE ID = @ID ";
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            string waktu = waktu_keb.Text;
                            waktu = waktu.Replace(".", ":");
                            // Gabungkan tanggal dan waktu menjadi satu nilai DateTime
                            string combinedDateTimeString = $"{tanggal.Value.Date.ToString("yyyy-MM-dd")} {waktu}:00.000";

                            // Ubah nilai gabungan ke tipe data DateTime
                            if (DateTime.TryParseExact(combinedDateTimeString, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime combinedDateTime))
                            {
                                cmd.Parameters.AddWithValue("@NilaiKolom5", combinedDateTime);
                            }
                            else
                            {
                                // Penanganan kesalahan jika parsing gagal
                                MessageBox.Show("Format waktu tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }


                            string inputString = durasi.Text;
                            inputString = inputString.Replace(" ", "");
                            // Menghilangkan karakter non-digit dari string
                            string numericPart = new string(inputString.Where(char.IsDigit).ToArray());

                            // Konversi string menjadi integer
                            int totalMinutes = int.Parse(numericPart);
                            int[] muasal = { dari.SelectedIndex + 1, ke.SelectedIndex + 1, maskapai.SelectedIndex + 1 };
                            // Ganti parameter dan nilai sesuai dengan data yang akan disimpan
                            cmd.Parameters.AddWithValue("@NilaiKolom1", kode_penerbangan.Text);
                            cmd.Parameters.AddWithValue("@NilaiKolom2", muasal[0]);
                            cmd.Parameters.AddWithValue("@NilaiKolom3", muasal[1]);
                            cmd.Parameters.AddWithValue("@NilaiKolom4", muasal[2]);
                            cmd.Parameters.AddWithValue("@NilaiKolom6", totalMinutes);
                            cmd.Parameters.AddWithValue("@NilaiKolom7", harga.Value);
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
                    cek_KodePenerbangan();
                    using (SqlConnection connection = new SqlConnection(Db))
                    {
                        connection.Open();

                        // Buat perintah SQL untuk menyimpan data ke database
                        string query = $"INSERT INTO JadwalPenerbangan ({db_n}) VALUES (@NilaiKolom1, @NilaiKolom2, @NilaiKolom3, @NilaiKolom4, @NilaiKolom5, @NilaiKolom6, @NilaiKolom7)";
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            string waktu = waktu_keb.Text;
                            waktu = waktu.Replace(".", ":");
                            // Gabungkan tanggal dan waktu menjadi satu nilai DateTime
                            string combinedDateTimeString = $"{tanggal.Value.Date.ToString("yyyy-MM-dd")} {waktu}:00.000";

                            // Ubah nilai gabungan ke tipe data DateTime
                            if (DateTime.TryParseExact(combinedDateTimeString, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime combinedDateTime))
                            {
                                cmd.Parameters.AddWithValue("@NilaiKolom5", combinedDateTime);
                            }
                            else
                            {
                                // Penanganan kesalahan jika parsing gagal
                                MessageBox.Show("Format waktu tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }


                            string inputString = durasi.Text;
                            inputString = inputString.Replace(" ", "");
                            // Menghilangkan karakter non-digit dari string
                            string numericPart = new string(inputString.Where(char.IsDigit).ToArray());

                            // Konversi string menjadi integer
                            int totalMinutes = int.Parse(numericPart);
                            int[] muasal = { dari.SelectedIndex + 1, ke.SelectedIndex + 1, maskapai.SelectedIndex + 1 };
                            // Ganti parameter dan nilai sesuai dengan data yang akan disimpan
                            cmd.Parameters.AddWithValue("@NilaiKolom1", kode_penerbangan.Text);
                            cmd.Parameters.AddWithValue("@NilaiKolom2", muasal[0]);
                            cmd.Parameters.AddWithValue("@NilaiKolom3", muasal[1]);
                            cmd.Parameters.AddWithValue("@NilaiKolom4", muasal[2]);
                            cmd.Parameters.AddWithValue("@NilaiKolom6", totalMinutes);
                            cmd.Parameters.AddWithValue("@NilaiKolom7", harga.Value);

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

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dari.SelectedIndex == ke.SelectedIndex)
            {
                ke.SelectedIndex = -1;
            }
        }

        private void Label1_Click(object sender, EventArgs e)
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

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
            // Pastikan kita hanya memproses kolom tertentu, misalnya kolom ke-6
            if (column.Name == "DurasiPenerbangan" && e.RowIndex >= 0)
            {
                // Ambil nilai dari sel
                object cellValue = e.Value;

                // Pastikan nilai tidak null
                if (cellValue != null)
                {
                    // Coba mengonversi nilai sel ke integer
                    if (int.TryParse(cellValue.ToString(), out int totalMinutes))
                    {
                        // Menghitung jam dan menit
                        int hours = totalMinutes / 60;
                        int remainingMinutes = totalMinutes % 60;
                        if (hours == 00) e.Value = $"{remainingMinutes:D2} menit";
                        else if (remainingMinutes == 00) e.Value = $"{hours} jam";
                        else e.Value = $"{hours:D2} jam {remainingMinutes:D2} menit";
                        // Setelah mengatur nilai, hindari pemformatan lanjutan
                        e.FormattingApplied = true;
                    }
                    else
                    {
                        // Handle jika nilai tidak dapat diubah menjadi integer
                        // Misalnya, memberikan pesan kesalahan atau melakukan tindakan yang sesuai
                    }
                }
                else
                {
                    // Handle jika nilai null
                    // Misalnya, memberikan pesan kesalahan atau melakukan tindakan yang sesuai
                }
            }
            else if (column.Name == "TanggalKeberangkatan" && e.RowIndex >= 0)
            {
                // Ubah nilai tanggal ke format yang diinginkan HH:mm:ss
                if (e.Value is DateTime)
                {
                    e.Value = ((DateTime)e.Value).ToString("dd-MM-yyyy");
                    e.FormattingApplied = true;
                }
            }
            else if (column.Name == "WaktuKeberangkatan" && e.RowIndex >= 0)
            {
                // Ubah nilai tanggal ke format yang diinginkan HH:mm:ss
                if (e.Value is DateTime)
                {
                    e.Value = ((DateTime)e.Value).ToString("HH:MM");
                    e.FormattingApplied = true;
                }
            }
        }

        private void Button_batal_Click(object sender, EventArgs e)
        {
            kode_penerbangan.Clear();
            dari.Text = "";
            ke.Text = "";
            maskapai.Text = "";
            waktu_keb.Clear();
            durasi.Clear();
            harga.Value = 1;
            DateTime HariIni = DateTime.Now;
            tanggal.Value = HariIni;
            selectedRowIndex = 0;
        }
    }
}
