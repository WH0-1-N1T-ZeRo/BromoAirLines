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
    public partial class Cari_Penerbangan : Form
    {
        string Db;
        string[] awal = new string[3];
        private DataTable dataTable;
        int orang { get; set; }
        // Membuat alias untuk kolom Bandara.ID agar bisa diakses sebagai BandaraNama
        string query = $@"SELECT 
                            JadwalPenerbangan.KodePenerbangan, 
                            Maskapai.Nama AS Maskapai, 
                            BandaraKeberangkatan.Nama AS BandaraKeberangkatan, 
                            BandaraTujuan.Nama AS BandaraTujuan, 
                            JadwalPenerbangan.HargaPerTiket, 
                            JadwalPenerbangan.TanggalWaktuKeberangkatan AS TanggalKeberangkatan, 
                            JadwalPenerbangan.TanggalWaktuKeberangkatan AS WaktuPenerbangan, 
                            JadwalPenerbangan.DurasiPenerbangan 
                        FROM JadwalPenerbangan 
                        JOIN Bandara AS BandaraKeberangkatan ON JadwalPenerbangan.BandaraKeberangkatanID = BandaraKeberangkatan.ID 
                        JOIN Bandara AS BandaraTujuan ON JadwalPenerbangan.BandaraTujuanID = BandaraTujuan.ID 
                        JOIN Maskapai ON JadwalPenerbangan.MaskapaiID = Maskapai.ID 
                        WHERE JadwalPenerbangan.BandaraKeberangkatanID = @Berangkat AND JadwalPenerbangan.BandaraTujuanID = @Tujuan
                        ORDER BY HargaPerTiket ASC";
        public Cari_Penerbangan(int nib,int nit, string berangkat,string tujuan, string tanggal,int penumpang)
        {
            InitializeComponent();

            berangkat_l.Text = berangkat;
            tujuan_l.Text = tujuan;
            hari.Text = tanggal.ToString();
            penumpang_l.Text = penumpang.ToString()+" Penumpang";
            orang = penumpang;
            awal[0] = nib.ToString();
            awal[1] = nit.ToString();

            combobox_value();
            Conect open = new Conect();
            Db=open.Database;
            output_db();
            dataGridView1.Columns["DurasiPenerbangan"].Visible = false;
            dataTable = new DataTable();
        }

        public void combobox_value()
        {
            string[] value =
            {
                "Harga terendah",
                "Keberangkatan paling awal",
                "Keberangkatan paling akhir",
                "Kedatangan paling awal",
                "Kedatangan paling akhir",
                "Durasi tercepat"
            };
            // Pastikan comboBox1 adalah objek ComboBox yang telah dideklarasikan sebelumnya
            foreach (string item in value)
            {
                comboBox1.Items.Add(item);
            }
        }

        private void output_db()
        {
            using (SqlConnection connection = new SqlConnection(Db))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Tambahkan parameter untuk menghindari SQL injection
                    command.Parameters.AddWithValue("@Berangkat", awal[0]);
                    command.Parameters.AddWithValue("@Tujuan", awal[1]);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Tampilkan data dalam DataGridView
                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            Bromo_Airlines_Customer open = new Bromo_Airlines_Customer();
            open.Show();
            this.Hide();
        }

        private void Cari_Penerbangan_Load(object sender, EventArgs e)
        {
            // Membuat kolom tombol "Ubah"
            DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
            buttonColumn.Name = "BeliTiket";
            buttonColumn.HeaderText = "";
            buttonColumn.Text = "Beli Tiket";
            buttonColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(buttonColumn);
            
        }

        private void ApplyTimeFilter(string startTime, string endTime)
        {
            dataGridView1.DataSource = dataTable;

            // Pastikan kolom 'WaktuPenerbangan' ada dalam DataTable
            if (dataTable.Columns.Contains("WaktuPenerbangan"))
            {
                // Membuat DataView dari DataTable
                DataView dv = new DataView(dataTable);

                // Menentukan filter berdasarkan waktu
                dv.RowFilter = $"WaktuPenerbangan >= '#{startTime}#' AND WaktuPenerbangan <= '#{endTime}#'";

                // Menyaring DataView dengan kriteria waktu
                // dv.RowFilter = filterExpression;

                // Menetapkan DataView yang disaring sebagai sumber data DataGridView
                dataGridView1.DataSource = dv;
            }
        }


        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
            // Pastikan kita hanya memproses kolom tertentu, misalnya kolom ke-6
            if (column.Name == "TanggalKeberangkatan" && e.RowIndex >= 0)
            {
                // Ambil nilai dari sel
                object cellValue = e.Value;

                // Pastikan nilai tidak null
                if (cellValue != null)
                {
                    // Coba mengonversi nilai sel ke string tanggal
                    if (DateTime.TryParse(cellValue.ToString(), out DateTime originalDate))
                    {
                        // Mengonversi dan memformat tanggal
                        string formattedDate = FormatDate(originalDate);

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
                    MessageBox.Show("Mohon maaf atas ketidaknyamanan anda,\nterjadi masalah dalam database","Database Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }

            if (column.Name == "WaktuPenerbangan" && e.RowIndex >= 0)
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
                            string formattedDate = Ftime(originalDate,totalMinutes);

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

        private string FormatDate(DateTime originalDate)
        {
            // Mengonversi tanggal ke string dengan format yang diinginkan
            string formattedDate = originalDate.ToString("dd-MM-yyyy HH:mm");

            // Memisahkan elemen-elemen dengan spasi sebagai pemisah
            string[] dateElements = formattedDate.Split(' ');

            // dateElements[0] akan berisi bagian pertama (tanggal), dan dateElements[1] akan berisi bagian kedua (bulan dan hari)
            formattedDate = dateElements[0];
            return formattedDate;
        }

        private string Ftime(DateTime ori,int nilai)
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

        private void Button1_Click(object sender, EventArgs e)
        {
            cek_FilTime();
          //  if (radioButton1.Checked == true || radioButton2.Checked == true || radioButton3.Checked == true || radioButton4.Checked == true)
           // {
          //      string[] cut = awal[2].Split('-');
         //   MessageBox.Show(cut[0], cut[1]);
             //   ApplyTimeFilter(cut[0], cut[1]);
           // }
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    dataGridView1.Sort(dataGridView1.Columns["HargaPerTiket"], System.ComponentModel.ListSortDirection.Ascending);
                    break;
                case 1:
                    dataGridView1.Sort(dataGridView1.Columns["TanggalKeberangkatan"], System.ComponentModel.ListSortDirection.Ascending);
                    break;
                case 2:
                    dataGridView1.Sort(dataGridView1.Columns["TanggalKeberangkatan"], System.ComponentModel.ListSortDirection.Descending);
                    break;
                case 3:
                    dataGridView1.Sort(dataGridView1.Columns["WaktuPenerbangan"], System.ComponentModel.ListSortDirection.Ascending);
                    break;
                case 4:
                    dataGridView1.Sort(dataGridView1.Columns["WaktuPenerbangan"],System.ComponentModel.ListSortDirection.Descending);
                    break;
                case 5:
                    dataGridView1.Sort(dataGridView1.Columns["DurasiPenerbangan"],System.ComponentModel.ListSortDirection.Ascending);
                    break;
            }
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
                if (column.Name == "BeliTiket")
                {
                    // Ambil nilai tanggal dari cell
                    object tanggalKeberangkatanValue = row.Cells["TanggalKeberangkatan"].Value;
                    DateTime tanggalKeberangkatan = (DateTime)tanggalKeberangkatanValue;
                    string formattedDate = tanggalKeberangkatan.ToString("dddd, dd MMMM yyyy");
                        string[] data = {
                        berangkat_l.Text,
                        tujuan_l.Text,
                        row.Cells["Maskapai"].Value.ToString(),
                        formattedDate,
                        row.Cells["WaktuPenerbangan"].FormattedValue.ToString()
                    };
                    int harga = Convert.ToInt32(row.Cells["HargaPerTiket"].Value)*orang;
                    Detail_Penumpang open = new Detail_Penumpang(data[0],data[1], data[2], data[3], data[4], orang,harga);
                    open.Show();
                }
            }
        }

        public void cek_FilTime()
        {
            if (radioButton1.Checked == true) awal[2] = radioButton1.Text;
            else if (radioButton2.Checked == true) awal[2] = radioButton2.Text;
            else if (radioButton3.Checked == true) awal[2] = radioButton3.Text;
            else if (radioButton4.Checked == true) awal[2] = radioButton4.Text;
        }
    }
}
