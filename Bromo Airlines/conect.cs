using System;
using System.IO;
using Newtonsoft.Json;

using System.Data.SqlClient;
using System.Collections.Generic;
namespace Bromo_Airlines
{
    public class Conect
    {
        public bool Saklar_menu { get; set; }

        public string FilePath { get; } = "Mini_data.json";
        public string Database { get; } = "Data Source=DESKTOP-RSQP5LM\\SQLEXPRESS;Initial Catalog=BromoAirlines;Integrated Security=True";
    }

    public class MyData
    {
        public string Nama { get; set; }
    }

    public class ReadMiniData
    {
        public string Nama { get; set; }
        public string ID { get; set; }
        public void On()
        {
            Conect on = new Conect();
            string filePath = on.FilePath;

            // Cek apakah file JSON sudah ada
            if (File.Exists(filePath))
            {
                // Jika sudah ada, baca dan deserialisasi
                string jsonContent = File.ReadAllText(filePath);
                // Membuat objek yang sesuai dengan struktur file JSON
                MyData miniData = JsonConvert.DeserializeObject<MyData>(jsonContent);
                // Mengakses properti "nama" dari objek MyData
                Nama = miniData.Nama;
                // Menampilkan nama
                Console.WriteLine($"Nama: {Nama}");
                Conect open = new Conect();
                using (SqlConnection connection=new SqlConnection(open.Database))
                {
                    connection.Open();
                    string query = "SELECT ID,Nama FROM Akun WHERE Nama = @nama";
                    using(SqlCommand command= new SqlCommand(query,connection))
                    {
                        command.Parameters.AddWithValue("@nama", Nama);
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            ID = reader["ID"].ToString();
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("File JSON belum ada.");
            }
        }
    }

    public class Update
    {
        public void On(string nama)
        {
            Conect on = new Conect();
            string filePath = on.FilePath;

            // Cek apakah file JSON sudah ada
            if (File.Exists(filePath))
            {
                // Baca dan deserialisasi file JSON
                string jsonContent = File.ReadAllText(filePath);
                MyData miniData = JsonConvert.DeserializeObject<MyData>(jsonContent);
                // Ubah nilai properti "nama"
                miniData.Nama = nama;
                // Serialisasi objek kembali ke format JSON
                string updatedJson = JsonConvert.SerializeObject(miniData);
                // Tulis kembali ke file JSON
                File.WriteAllText(filePath, updatedJson);
                Console.WriteLine("Nilai properti 'nama' telah diubah dan disimpan.");
            }
            else
            {
                Console.WriteLine("File JSON belum ada.");
            }
        }
    }

    public static class cekStatus
    {
        public static int jadwalPenerbanganID { get; set; }
        public static int statusPenerbanganID { get; set; }
        public static DateTime waktuPerubahanTerjadi { get; set; }
        public static int perkiraanDurasiDelay { get; set; }
        static string DurasiDelay { get; set; }
        public static string delay { get; set; }
        public static void ValueStatus(int id)
        {
            Conect on = new Conect();
            string query = "SELECT JadwalPenerbanganID, StatusPenerbanganID, WaktuPerubahanTerjadi, PerkiraanDurasiDelay " +
                           "FROM PerubahanStatusJadwalPenerbangan " +
                           "WHERE JadwalPenerbanganID = @id";

            using (SqlConnection connection = new SqlConnection(on.Database))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // Ubah menjadi if agar hanya membaca satu baris
                        {
                            if (perkiraanDurasiDelay != default(int)) perkiraanDurasiDelay = reader.GetInt32(3);
                            jadwalPenerbanganID = reader.GetInt32(0);
                            statusPenerbanganID = reader.GetInt32(1);
                            waktuPerubahanTerjadi = reader.GetDateTime(2);
                            // Handle nilai NULL dengan nullable int

                            Mystatus();
                        }
                    }
                }
            }
        }

        public static void Mystatus()
        {
            int hours = perkiraanDurasiDelay / 60;
            int remainingMinutes = perkiraanDurasiDelay % 60;
            if (hours == 00) DurasiDelay = $"{remainingMinutes:D2} menit";
            else if (remainingMinutes == 00) DurasiDelay = $"{hours} jam";
            else DurasiDelay = $"{hours:D2} jam {remainingMinutes:D2} menit";

            delay = $"Delay (selama ±{DurasiDelay})";
        }
    }

    public static class InpStatus
    {
        public static int Wdlay { get; set; }
        public static void Status()
        {
            Conect open = new Conect();

            string selectQuery = "SELECT ID,TanggalWaktuKeberangkatan FROM JadwalPenerbangan";
            string insertQuery = "INSERT INTO PerubahanStatusJadwalPenerbangan (JadwalPenerbanganId,StatusPenerbanganID,WaktuPerubahanTerjadi) VALUES (@JadwalPenerbanganId,@Status,@Waktu)";
            List<int> jadwalPenerbanganId = new List<int>();
            List<int> TglN = new List<int>();
            DateTime TglW;
            using (SqlConnection connection = new SqlConnection(open.Database))
            {
                connection.Open();

                // Mengambil semua ID dari tabel JadwalPenerbangan
                using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                {
                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            jadwalPenerbanganId.Add(reader.GetInt32(0));
                            TglW=reader.GetDateTime(1);
                            if (TglW < DateTime.Now)
                            {
                                TglN.Add(2);
                            }
                            else
                            {
                                TglN.Add(1);
                            }
                        }
                    }
                }
                int nulStatus = 0;
                foreach (int stat in jadwalPenerbanganId)
                {
                    // Memeriksa apakah JadwalPenerbanganID sudah ada di tabel PerubahanStatusJadwalPenerbangan
                    string checkDuplicateQuery = "SELECT COUNT(*) FROM PerubahanStatusJadwalPenerbangan WHERE JadwalPenerbanganID = @JadwalPenerbanganId";

                    using (SqlCommand checkDuplicateCommand = new SqlCommand(checkDuplicateQuery, connection))
                    {
                        checkDuplicateCommand.Parameters.AddWithValue("@JadwalPenerbanganId", stat);

                        int existingCount = (int)checkDuplicateCommand.ExecuteScalar();

                        // Jika JadwalPenerbanganID tidak ada, maka lakukan insert
                        if (existingCount == 0)
                        {

                            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@JadwalPenerbanganId", stat);
                                insertCommand.Parameters.AddWithValue("@Status", TglN[nulStatus++]);
                                insertCommand.Parameters.AddWithValue("@Waktu", DateTime.Now);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                        else 
                        {
                            // JadwalPenerbanganID sudah ada, mungkin lakukan penanganan duplikasi atau lewati
                        }
                    }
                }

                connection.Close();
            }
            
            Console.WriteLine("Proses selesai.");
        }

    }

    public static class Menu_set
    {
        public static void Bandara()
        {
            Master_Bandara open = new Master_Bandara();
            open.Show();
        }
        public static void Maskapai()
        {
            Master_Maskapai open = new Master_Maskapai();
            open.Show();
        }
        public static void Jadwal()
        {
            Master_Jadwal_Penerbangan open = new Master_Jadwal_Penerbangan();
            open.Show();
        }
        public static void KodePromo()
        {
            Master_Kode_Promo open = new Master_Kode_Promo();
            open.Show();
        }
        public static void UbahJadwal()
        {
            Ubah_Status_Penerbangan open = new Ubah_Status_Penerbangan();
            open.Show();
        }
        public static void Login()
        {
            Login open = new Login();
            open.Show();
        }
    }
}
