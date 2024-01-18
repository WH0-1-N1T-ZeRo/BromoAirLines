using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Bromo_Airlines
{
    public partial class Ubah_Status_Penerbangan : Form
    {
        string Db;
        bool on_of;
        private DataTable dataTable;
        string YourSelectedID;
        // Membuat alias untuk kolom Bandara.ID agar bisa diakses sebagai BandaraNama

        public Ubah_Status_Penerbangan()
        {
            InitializeComponent();
            Conect open = new Conect();
            Db=open.Database;
            value_combobox();
            output_db();
            //OposisiInput();
        }

        public void menu_on_of()
        {
            on_of = !on_of;
            if (on_of)
            {
                Menu_system.Size = new Size(260, 0);
            }
            else
            {
                Menu_system.Size = new Size(69, 0);
            }
        }
        private void value_combobox()
        {
            using (SqlConnection connection = new SqlConnection(Db))
            {
                connection.Open();
                string query = $"SELECT Nama FROM StatusPenerbangan"; // Sesuaikan dengan nama tabel produk Anda
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Status.Items.Add(reader["Nama"].ToString());
                }
                connection.Close();
            }
        }

        private void Negara_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Status.SelectedItem.ToString() == "Delay")
            {
                panel_delay.Visible = true;
            }
            else if (Status.SelectedItem.ToString() == "Sesuai Jadwal") Status.SelectedIndex = 0;
            else if (Status.SelectedItem.ToString() == "Telah Berangkat") Status.SelectedIndex = 1;
            else if (Status.SelectedItem.ToString() == "Cancel") Status.SelectedIndex = 3;
            else
            {
                panel_delay.Visible = false;
            }
        }

        private void output_db()
        {
            using (SqlConnection connection =new SqlConnection(Db))
            {
                connection.Open();
                string sql = @"SELECT PerubahanStatusJadwalPenerbangan.ID,
                            JadwalPenerbanganID,
                            JadwalPenerbangan.KodePenerbangan AS KodePenerbangan,
                            Maskapai.Nama AS Maskapai,
                            BandaraKeberangkatan.Nama AS BandaraKeberangkatan,
                            BandaraTujuan.Nama AS BandaraTujuan,
                            JadwalPenerbangan.TanggalWaktuKeberangkatan AS TanggalKeberangkatan,
                            JadwalPenerbangan.TanggalWaktuKeberangkatan AS WaktuPenerbangan,
                            JadwalPenerbangan.DurasiPenerbangan,
                            StatusPenerbangan.Nama AS StatusPenerbangan,
                            PerkiraanDurasiDelay,
                            WaktuPerubahanTerjadi AS TerakhirDiubah
                            FROM PerubahanStatusJadwalPenerbangan
                            INNER JOIN JadwalPenerbangan ON JadwalPenerbangan.ID = JadwalPenerbanganID
                            JOIN Bandara AS BandaraKeberangkatan ON JadwalPenerbangan.BandaraKeberangkatanID = BandaraKeberangkatan.ID
                            JOIN Bandara AS BandaraTujuan ON JadwalPenerbangan.BandaraTujuanID = BandaraTujuan.ID
                            JOIN Maskapai ON JadwalPenerbangan.MaskapaiID = Maskapai.ID
                            JOIN StatusPenerbangan ON StatusPenerbanganID = StatusPenerbangan.ID";
                using (SqlCommand com = new SqlCommand(sql, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(com))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridView1.DataSource = dataTable;
                        dataGridView1.Columns["JadwalPenerbanganID"].Visible = false;
                        dataGridView1.Columns["PerkiraanDurasiDelay"].Visible = false;
                        dataGridView1.Columns["ID"].Visible = false;
                    }
                } 
            }
        }

        private void OposisiInput()
        {
            using (SqlConnection connection = new SqlConnection(Db))
            {
                connection.Open();
                string query = "SELECT ID, TanggalWaktuKeberangkatan FROM JadwalPenerbangan";
                using (SqlCommand TimeCek = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = TimeCek.ExecuteReader();
                    List<int> IDjadwal = new List<int>();
                    List<DateTime> Time = new List<DateTime>();
                    while (reader.Read())
                    {
                        IDjadwal.Add(Convert.ToInt32(reader["ID"]));
                        Time.Add(Convert.ToDateTime(reader["TanggalWaktuKeberangkatan"]));
                    }
                    reader.Close();
                    foreach (int a in IDjadwal)
                    {

                        using (SqlCommand Com = new SqlCommand("SELECT COUNT(*) FROM PerubahanStatusJadwalPenerbangan WHERE StatusPenerbanganID = @MyValue", connection))
                        {
                            Com.Parameters.AddWithValue("@MyValue", IDjadwal[a]);
                            //MessageBox.Show(IDjadwal[a].ToString());
                            int cek = (int)Com.ExecuteScalar();
                            if (cek < 0)
                            {
                                if (Time[a] < DateTime.Now)
                                {
                                    query = "INSERT INTO PerubahanStatusJadwalPenerbangan (JadwalPenerbanganID,StatusPenerbanganID,WaktuPerubahanTerjadi) VALUES (@ID,@Status,@Waktu)";
                                    using (SqlCommand command = new SqlCommand(query, connection))
                                    {
                                        command.Parameters.AddWithValue("@ID", IDjadwal[a]);
                                        command.Parameters.AddWithValue("@Status", 2);
                                        command.Parameters.AddWithValue("@Waktu", DateTime.Now);

                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Masok" + IDjadwal[a].ToString());
                            }
                            Com.Parameters.RemoveAt("@MyValue");
                        }
                    }
                    
                }
            }
        }
        private void Ubah_Status_Penerbangan_Load(object sender, EventArgs e)
        {
            // Membuat kolom tombol "Ubah"
            DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
            buttonColumn.Name = "UbahButtonColumn";
            buttonColumn.HeaderText = "";
            buttonColumn.Text = "Ubah";
            buttonColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(buttonColumn);
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

        private void Button_simpan_Click(object sender, EventArgs e)
        {
            using(SqlConnection connection =new SqlConnection(Db))
            {
                connection.Open();
                string sql = "";
                if(Status.SelectedItem.ToString() == "Delay") sql = $"UPDATE PerubahanStatusJadwalPenerbangan SET StatusPenerbanganID=2 , WaktuPerubahanTerjadi =@waktu , PerkiraanDurasiDelay=@Pdly WHERE ID=@id";
                else sql = $"UPDATE PerubahanStatusJadwalPenerbangan SET StatusPenerbanganID=@status, WaktuPerubahanTerjadi =@waktu WHERE ID=@id";
                using (SqlCommand command=new SqlCommand(sql, connection))
                {
                    if (Perkiraan_durasi.Text != "")
                    {
                        string inputString = Perkiraan_durasi.Text;
                        inputString = inputString.Replace(" ", "");
                        // Menghilangkan karakter non-digit dari string
                        string numericPart = new string(inputString.Where(char.IsDigit).ToArray());

                        // Konversi string menjadi integer
                        int totalMinutes = int.Parse(numericPart);
                        command.Parameters.AddWithValue("@Pdly", totalMinutes);
                    }
                    command.Parameters.AddWithValue("@status", Status.SelectedIndex+1);
                    command.Parameters.AddWithValue("@waktu", DateTime.Now);
                    command.Parameters.AddWithValue("@id", YourSelectedID);

                    int cek = command.ExecuteNonQuery();
                    if (cek > 0)
                    {
                        output_db();
                        button_batal.PerformClick();
                    }
                }
            }
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                YourSelectedID = row.Cells["ID"].Value.ToString();

                DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
                // Pastikan kita hanya memproses kolom tertentu, misalnya kolom ke-6
                if (column.Name == "TanggalKeberangkatan")
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
                        MessageBox.Show("Mohon maaf atas ketidaknyamanan anda,\nterjadi masalah dalam database", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                else if (column.Name == "WaktuPenerbangan")
                {
                    // Mengambil nilai dari kolom 6
                    object cellvalue = e.Value;

                    // Pastikan nilai tidak null
                    if (cellvalue != null)
                    {
                        // Ubah nilai tanggal ke format yang diinginkan HH:mm:ss
                        if (e.Value is DateTime)
                        {
                            e.Value = ((DateTime)e.Value).ToString("HH:MM");
                            e.FormattingApplied = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Mohon maaf atas ketidaknyamanan anda,\nterjadi masalah dalam database", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                else if (column.Name == "DurasiPenerbangan")
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
                            else if (remainingMinutes == 00) e.Value = $"{hours:D2} jam";
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
                        MessageBox.Show("Mohon maaf atas ketidaknyamanan anda,\nterjadi masalah dalam database", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                else if (column.Name == "PerkiraanDurasiDelay")
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
                            else if (remainingMinutes == 00) e.Value = $"{hours:D2} jam";
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
                        MessageBox.Show("Mohon maaf atas ketidaknyamanan anda,\nterjadi masalah dalam database", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                else if (column.Name == "StatusPenerbangan")
                {
                    object cellValue = e.Value;
                    // Ambil nilai dari kolom 7
                    object Cv2 = dataGridView1.Rows[e.RowIndex].Cells["PerkiraanDurasiDelay"].FormattedValue;

                    if (cellValue != null && cellValue.ToString() == "Delay")
                    {

                        e.Value = $"Delay (selama ±{Cv2})";
                        e.FormattingApplied = true;
                    }
                }

                else if (column.Name == "TerakhirDiubah")
                {
                    // Mengambil nilai dari kolom 6
                    object cellvalue = e.Value;

                    // Pastikan nilai tidak null
                    if (cellvalue != null)
                    {
                        // Ubah nilai tanggal ke format yang diinginkan HH:mm:ss
                        if (e.Value is DateTime)
                        {
                            e.Value = ((DateTime)e.Value).ToString("dd-MM-yyyy HH:mm:ss");
                            e.FormattingApplied = true;
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

        private string Ftime(DateTime ori)
        {

            string newFormattedDate = ori.ToString("dd-MM-yyyy HH:mm");
            string[] sp = newFormattedDate.Split(' ');

            // dateElements[0] akan berisi bagian pertama (tanggal), dan dateElements[1] akan berisi bagian kedua (bulan dan hari)

            string time = sp[1];

            return time;
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
                if(column.Name == "UbahButtonColumn")
                {
                    tableLayoutPanel1.Visible = true;
                }
            }
        }

        private void Negara_Click(object sender, EventArgs e)
        {
            Status.DroppedDown = true;
            Status.Cursor = default;
        }

        private void Button_batal_Click(object sender, EventArgs e)
        {
            Status.SelectedIndex = default(int);
            Status.Text = "";
            YourSelectedID="";
            tableLayoutPanel1.Visible = false;
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

        private void PictureBox7_Click(object sender, EventArgs e)
        {
            Menu_set.Login();
            this.Close();
        }
    }
}
