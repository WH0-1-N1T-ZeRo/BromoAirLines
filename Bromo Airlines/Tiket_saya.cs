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
    public partial class Tiket_saya : Form
    {
        string Db;
        public Tiket_saya()
        {
            InitializeComponent();
            Conect open = new Conect();
            Db = open.Database;
            DatabaseOutput();
        }


        public void DatabaseOutput()
        {
            //  try
            //{
            using (SqlConnection connection = new SqlConnection(Db))
            {
                connection.Open();

                string selectIdQuery = "SELECT JadwalPenerbanganID FROM TransaksiHeader WHERE AkunID = @akun";

                using (SqlCommand idCommand = new SqlCommand(selectIdQuery, connection))
                {
                    ReadMiniData read = new ReadMiniData();
                    read.On();
                    int ID = Convert.ToInt32(read.ID);
                    idCommand.Parameters.AddWithValue("@akun", ID);
                    int IdJadwal;
                    List<int> jadwalPenerbanganIds = new List<int>();

                    using (SqlDataReader idReader = idCommand.ExecuteReader())
                    {
                        while (idReader.Read())
                        {
                            IdJadwal = Convert.ToInt32(idReader["JadwalPenerbanganID"]);
                            jadwalPenerbanganIds.Add(IdJadwal);
                        }
                    }
                    string iDValues = string.Join(",", jadwalPenerbanganIds);
                    MessageBox.Show(iDValues);
                    string selectDetailsQuery = @"SELECT PerubahanStatusJadwalPenerbangan.ID,
                            JadwalPenerbangan.KodePenerbangan AS KodePenerbangan,
                            Maskapai.Nama AS Maskapai,
                            BandaraKeberangkatan.Nama AS BandaraKeberangkatan,
                            BandaraTujuan.Nama AS BandaraTujuan,
                            JadwalPenerbangan.TanggalWaktuKeberangkatan AS TanggalKeberangkatan,
                            JadwalPenerbangan.TanggalWaktuKeberangkatan AS WaktuPenerbangan,
                            JadwalPenerbangan.DurasiPenerbangan,
                            StatusPenerbangan.Nama AS StatusPenerbangan,
                            PerkiraanDurasiDelay
                            FROM PerubahanStatusJadwalPenerbangan
                            INNER JOIN JadwalPenerbangan ON JadwalPenerbangan.ID = JadwalPenerbanganID
                            JOIN Bandara AS BandaraKeberangkatan ON JadwalPenerbangan.BandaraKeberangkatanID = BandaraKeberangkatan.ID
                            JOIN Bandara AS BandaraTujuan ON JadwalPenerbangan.BandaraTujuanID = BandaraTujuan.ID
                            JOIN Maskapai ON JadwalPenerbangan.MaskapaiID = Maskapai.ID
                            JOIN StatusPenerbangan ON StatusPenerbanganID = StatusPenerbangan.ID " +
        $"WHERE JadwalPenerbanganID IN ({iDValues})";
                    // ... (your details query)

                    using (SqlCommand detailsCommand = new SqlCommand(selectDetailsQuery, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(detailsCommand))
                        {
                            DataTable data = new DataTable();
                            adapter.Fill(data);

                            dataGridView1.DataSource = data;

                            // dataGridView1.Columns["DurasiPenerbangan"].Visible = false;
                        }
                    }
                }

            }
        }



        private void Tiket_saya_Load(object sender, EventArgs e)
        {
            //dataGridView1.Columns.Add("StatusPenerbangan", "StatusPenerbangan");
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
            if (column.Name == "TanggalKeberangkatan" && e.RowIndex >= 0)
            {
                // Ubah nilai tanggal ke format yang diinginkan HH:mm:ss
                if (e.Value is DateTime)
                {
                    e.Value = ((DateTime)e.Value).ToString("dd-MM-yyyy");
                    e.FormattingApplied = true;
                }
            }
            else if (column.Name == "WaktuPenerbangan" && e.RowIndex >= 0)
            {
                // Ambil nilai dari kolom 7
                object cellValue = dataGridView1.Rows[e.RowIndex].Cells["DurasiPenerbangan"].Value;
                // Periksa apakah nilai dapat diubah menjadi integer
                if (int.TryParse(cellValue.ToString(), out int totalMinutes))
                {
                    // Mengambil nilai dari kolom 6
                    object cellValueColumn6 = e.Value;

                    // Pastikan nilai tidak null
                    if (cellValueColumn6 != null)
                    {
                        // Coba mengonversi nilai sel ke string tanggal
                        if (DateTime.TryParse(cellValueColumn6.ToString(), out DateTime originalDate))
                        {

                            // Mengonversi dan memformat tanggal
                            string formattedDate = Ftime(originalDate, totalMinutes);

                            // Setel nilai yang diformat ke sel
                            e.Value = formattedDate;

                            // Setelah mengatur nilai, hindari pemformatan lanjutan
                            e.FormattingApplied = true;
                        }
                        else
                        {
                            // Handle jika nilai tidak dapat diubah menjadi DateTime
                            // Misalnya, memberikan pesan kesalahan atau melakukan tindakan yang sesuai
                        }
                    }
                    else
                    {
                        MessageBox.Show("Mohon maaf atas ketidaknyamanan anda,\nterjadi masalah dalam database", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Handle jika nilai dari kolom 7 tidak dapat diubah menjadi integer
                    // Misalnya, memberikan pesan kesalahan atau melakukan tindakan yang sesuai
                }
            }

        }
        private string Ftime(DateTime ori, int nilai)
        {
            // Tambahkan nilai waktu ke originalDate
            DateTime newDate = ori.AddMinutes(nilai);

            string newFormattedDate = ori.ToString("dd-MM-yyyy HH:mm");
            string[] sp = newFormattedDate.Split(' ');

            // Format tanggal kembali dengan tambahan waktu
            string old = newDate.ToString("dd-MM-yyyy HH:mm");
            string[] spl = old.Split(' ');

            // dateElements[0] akan berisi bagian pertama (tanggal), dan dateElements[1] akan berisi bagian kedua (bulan dan hari)

            string time = $"{sp[1]} - {spl[1]}";

            return time;
        }

        private void PictureBox5_Click(object sender, EventArgs e)
        {
            Bromo_Airlines_Customer open = new Bromo_Airlines_Customer();
            open.Show();
            this.Hide();
            this.Close();
        }
    }
}
