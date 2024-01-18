using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Globalization;
using System.Data.SqlClient;

namespace Bromo_Airlines
{
    public partial class Detail_Penumpang : Form
    {
        int orang;
        int ID { get; set; }
        int harga_tl;
        string Db;
        string[] Db_nm =
        {
            "PersentaseDiskon",
            "MaksimumDiskon",
            "BerlakuSampai"
        };
        Dictionary<string, ComboBox> Combo = new Dictionary<string, ComboBox>();
        Dictionary<string, TextBox> dynamicTextBoxes = new Dictionary<string, TextBox>();
        public Detail_Penumpang(string berangkat, string tujuan, string maskapai, string tagl, string waktu_v, int penumpang, int harga)
        {
            InitializeComponent();
            Conect open = new Conect();
            Db = open.Database;
            berangkat_l.Text = berangkat;
            tujuan_l.Text = tujuan;
            maskapai_l.Text = maskapai;
            tgl.Text = tagl;
            waktu.Text = waktu_v;
            orang = penumpang;
            penumpang_l.Text = penumpang.ToString() + " Penumpang";
            harga_tl = harga;
            uang = harga;

            diskon(harga);


            int a = 1;
            Penumpang_.Name += a;
            Penumpang_.Text += a;
            CreateDuplicateGroupBox(panel3, Penumpang_, a, label_title, label_nama, label_info, Penumpang_1C, Penumpang_p1, tableLayoutPanel_p1);

        }
        
        public void diskon(int total)
        {
            CultureInfo culture = CultureInfo.GetCultureInfo("id-ID"); // Misalnya, Indonesia
            // Ubah nilai uang menjadi string dengan format mata uang dan tanda titik
            string formattedAmount = string.Format(culture, "IDR {0:N0}", total);
            Harga_l.Text = formattedAmount;
        }

        private void CreateDuplicateGroupBox(Panel panel,GroupBox originalGroupBox,int no,Label l1,Label l2,Label l3, ComboBox C,TextBox Text,TableLayoutPanel tabel)
        {
            int a = 1;
            string nm = "Penumpang";
            for (int i = 1; orang > no; no++)
            {
                a++;
                MessageBox.Show(no.ToString());

                // salinan GroupBox.
                GroupBox duplicatedGroupBox = new GroupBox();
                duplicatedGroupBox.Text = $"{nm} #{a}";
                // Sesuaikan nilai Y dengan jumlah salinan yang telah dibuat
                duplicatedGroupBox.Location = new System.Drawing.Point(originalGroupBox.Location.X, originalGroupBox.Location.Y + (originalGroupBox.Height + 10) * i++);
                duplicatedGroupBox.Size = originalGroupBox.Size;
                duplicatedGroupBox.Font = originalGroupBox.Font;
                duplicatedGroupBox.Anchor = originalGroupBox.Anchor;
                duplicatedGroupBox.TabIndex = originalGroupBox.TabIndex + 1;

                // Salinan tabel
                TableLayoutPanel Ntabel = new TableLayoutPanel();
                Ntabel.Anchor = tabel.Anchor;
                Ntabel.Size = Ntabel.Size;
                Ntabel.Location = tabel.Location;
                Ntabel.RowCount = tabel.RowCount;
                Ntabel.ColumnCount = tabel.ColumnCount;
                //Ntabel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
                //Salinan label1
                Label Nlabel1 = new Label();
                Nlabel1.Text = l1.Text;
                Nlabel1.Font = l1.Font;
                Ntabel.Size = tabel.Size;
                Nlabel1.Anchor = l1.Anchor;

                //Salinan label2
                Label Nlabel2 = new Label();
                Nlabel2.Text = l2.Text;
                Nlabel2.Font = l2.Font;
                Nlabel2.Anchor = l2.Anchor;

                //Salinan combobox
                ComboBox Ncombo = new ComboBox();
                Ncombo.Text = C.Text;
                Ncombo.Name = $"{nm}_{a}C";
                Ncombo.Font = C.Font;
                Ncombo.Anchor = C.Anchor;
                Ncombo.MaximumSize = C.MaximumSize;
                Ncombo.Items.AddRange(C.Items.Cast<object>().ToArray());

                Ncombo.Click += DuplicatedComboBox_Click;
                Combo.Add(Ncombo.Name, Ncombo);

                //Salinan textbox
                TextBox Textb = new TextBox();
                Textb.Name = $"{nm}_p{a}";
                Textb.Font = Text.Font;
                Textb.Anchor = Text.Anchor;
                dynamicTextBoxes.Add(Textb.Name, Textb);

                Panel nul = new Panel();
                nul.Enabled = false;
                nul.Size = Nlabel2.Size;
                //Salinan label3
                Label Nlabel3 = new Label();
                Nlabel3.Text = l3.Text;
                Nlabel3.Font = l3.Font;
                Nlabel3.Anchor = l3.Anchor;
                Nlabel3.ForeColor = l3.ForeColor;

                // Tambahkan salinan ke form.
                panel3.Controls.Add(duplicatedGroupBox);
                duplicatedGroupBox.Controls.Add(Ntabel);
                Ntabel.Controls.Add(Nlabel1);
                Ntabel.Controls.Add(Ncombo);
                Ntabel.Controls.Add(Nlabel2);
                Ntabel.Controls.Add(Textb);
                Ntabel.Controls.Add(nul);
                Ntabel.Controls.Add(Nlabel3);
            }
            //Ntabel.Controls.Add(Nlabel1);
        }

        private void DuplicatedComboBox_Click(object sender, EventArgs e)
        {
            // Kode yang akan dijalankan saat salah satu GroupBox diklik
            ComboBox clickedComboBox = sender as ComboBox;
            if (clickedComboBox != null)
            {
                // Lakukan sesuatu dengan GroupBox yang diklik
                //MessageBox.Show($"GroupBox {clickedComboBox.Name} diklik!");
                clickedComboBox.DroppedDown = true;
                clickedComboBox.Cursor= default;
            }
        }


        private void Detail_Penumpang_Load(object sender, EventArgs e)
        {
            
            // Membuat salinan GroupBox dan elemen-elemen di dalamnya sebanyak jumlah penumpang

        }

        int uang;
        private void Button1_Click(object sender, EventArgs e)
        {
            string set = string.Join(",",Db_nm);
            using (SqlConnection connection = new SqlConnection(Db))
            {
                connection.Open();
                string query = $"SELECT {set},ID FROM KodePromo WHERE Kode = @kode";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Validasi input sebelum mengeksekusi kueri
                    if (!string.IsNullOrEmpty(textBox1.Text))
                    {
                        command.Parameters.AddWithValue("@kode", textBox1.Text);
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            // Baca persentase diskon dan maksimum diskon dari database
                            decimal persentaseDiskon = Convert.ToDecimal(reader[Db_nm[0]]);
                            decimal maksimumDiskon = Convert.ToDecimal(reader[Db_nm[1]]);
                            DateTime tanggalKedaluwarsa = Convert.ToDateTime(reader[Db_nm[2]]);
                            ID = Convert.ToInt32(reader["ID"]);
                            // Validasi apakah kode promo masih berlaku
                            if (tanggalKedaluwarsa < DateTime.Now)
                            {
                                MessageBox.Show("Tanggal Expired dari" + tanggalKedaluwarsa);
                            }
                            else
                            {
                                // Hitung diskon
                                decimal calculatedDiscount = uang * persentaseDiskon / 100;

                                if (uang - calculatedDiscount < maksimumDiskon || harga_tl-uang > maksimumDiskon)
                                {
                                    MessageBox.Show("kon");
                                    uang -= Convert.ToInt32(maksimumDiskon);
                                }
                                else uang -= Convert.ToInt32(calculatedDiscount);
                                MessageBox.Show(uang.ToString());
                                diskon(uang);
                            }
                        }
                        connection.Close();
                    }
                    else
                    {
                        MessageBox.Show("TextBox kosong. Silakan masukkan nilai.");
                    }
                }
            }

        }

        // Method untuk mendapatkan nilai dari TextBox berdasarkan nama
        private string GetDynamicTextBoxValue(string textBoxName)
        {
            if (dynamicTextBoxes.ContainsKey(textBoxName))
            {
                return dynamicTextBoxes[textBoxName].Text;
            }
            return string.Empty;
        }

        private string GetDynamicComboBoxValue(string comboBoxName)
        {
            if (Combo.ContainsKey(comboBoxName))
            {
                return Combo[comboBoxName].Text;
            }
            return string.Empty;
        }


        private void Button2_Click(object sender, EventArgs e)
        {
            string query;
            if (textBox1.Text == "" || uang == harga_tl)
            {
                query = "INSERT INTO TransaksiHeader (AkunID,TanggalTransaksi,JadwalPenerbanganID,JumlahPenumpang,TotalHarga) OUTPUT INSERTED.ID VALUES (@val1,@val2,@val3,@val4,@val5)";
            }
            else
            {
                query = "INSERT INTO TransaksiHeader (AkunID,TanggalTransaksi,JadwalPenerbanganID,JumlahPenumpang,TotalHarga,KodePromoID) OUTPUT INSERTED.ID VALUES (@val1,@val2,@val3,@val4,@val5,@val6)";
            }
            try
            {
                ReadMiniData open = new ReadMiniData();
                open.On();
                using (SqlConnection connection = new SqlConnection(Db))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@val1", open.ID);
                        command.Parameters.AddWithValue("@val2", DateTime.Now);
                        command.Parameters.AddWithValue("@val3", 3);
                        command.Parameters.AddWithValue("@val4", orang);
                        command.Parameters.AddWithValue("@val5", uang);
                        command.Parameters.AddWithValue("@val6", ID);


                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            int MyID = Convert.ToInt32(reader["ID"]);
                            reader.Close();

                            string combobox;
                            string textBoxValue;
                            for (int i = 1; orang >= i; i++)
                            {
                                if (i == 1)
                                {
                                    combobox = Penumpang_1C.Text;
                                    textBoxValue = Penumpang_p1.Text;
                                }
                                else
                                {
                                    combobox = GetDynamicComboBoxValue($"Penumpang_{i}C");
                                    textBoxValue = GetDynamicTextBoxValue($"Penumpang_p" + i);
                                }
                                query = "INSERT INTO TransaksiDetail (TransaksiHeaderID, TitelPenumpang, NamaLengkapPenumpang) VALUES (@data1,@data2,@data3)";
                                using (SqlCommand cmd = new SqlCommand(query, connection))
                                {
                                    cmd.Parameters.AddWithValue("@data1", MyID);
                                    cmd.Parameters.AddWithValue("@data2", combobox);
                                    cmd.Parameters.AddWithValue("@data3", textBoxValue);

                                    int cek = cmd.ExecuteNonQuery();
                                    if (cek > 0)
                                    {
                                        Console.WriteLine("SUKSES");
                                    }
                                }
                            }
                        }
                    }
                    connection.Close();
                }
                Tiket_saya Go_to = new Tiket_saya();
                Go_to.Show();
                this.Close();
            }
            catch (Exception ek)
            {
                MessageBox.Show(ek.ToString());
            }
        }
    }
}
