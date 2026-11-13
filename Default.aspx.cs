using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using campus_app.data;
using campus_app.models;

namespace campus_app
{
    public partial class Default : System.Web.UI.Page
    {
        // ============================================================
        // PAGE LOAD & PROTEKSI SESSION (Syarat UTS)
        // ============================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            // Cek apakah user sudah login
            if (Session["Username"] == null)
            {
                Response.Redirect("Login.aspx");
                return; 
            }

            if (!IsPostBack)
            {
                MultiView1.ActiveViewIndex = 0;

                // Load semua data dari Database
                LoadRuanganToDropdown();
                RefreshBookingList();
                RefreshTugasList();
                UpdateSaldoTampilanDariDatabase();
                LoadRiwayatTransaksi(); // Panggil GridView

                // Sembunyikan pesan
                lblBookingMsg.Visible = false;
                lblBatalMsg.Visible = false;
                lblTugasMsg.Visible = false;
                lblHapusTugasMsg.Visible = false;
                lblKeuanganMsg.Visible = false;
            }
        }

        // ============================================================
        // NAVIGASI & LOGOUT
        // ============================================================
        protected void Menu_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            MultiView1.ActiveViewIndex = Convert.ToInt32(btn.CommandArgument);
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        // ============================================================
        // FITUR 1: BOOKING RUANGAN (Database)
        // ============================================================
        private void LoadRuanganToDropdown()
        {
            DataTable dt = db_helper.ExecuteDataTable("SELECT RuanganID, NamaRuangan FROM ruangan");
            ddlRuangan.DataSource = dt;
            ddlRuangan.DataTextField = "NamaRuangan";
            ddlRuangan.DataValueField = "RuanganID";
            ddlRuangan.DataBind();
        }

        private void RefreshBookingList()
        {
            string query = @"SELECT b.BookingID, b.RuanganID, r.NamaRuangan, b.Tanggal 
                             FROM booking b 
                             JOIN ruangan r ON b.RuanganID = r.RuanganID 
                             ORDER BY b.Tanggal ASC";
            
            DataTable dt = db_helper.ExecuteDataTable(query);

            if (dt.Rows.Count == 0)
            {
                lblDaftarBooking.Text = "Belum ada data booking.";
                ddlDaftarBooking.Items.Clear();
                btnBatalBooking.Enabled = false;
                return;
            }

            var sb = new StringBuilder("<ul>");
            ddlDaftarBooking.Items.Clear();

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["BookingID"]);
                string ruangan = row["NamaRuangan"].ToString();
                DateTime tgl = Convert.ToDateTime(row["Tanggal"]);
                
                bool isPast = tgl.Date < DateTime.Today;
                string cssClass = isPast ? " style='color:#999; text-decoration:line-through;'" : "";
                string status = isPast ? " (Sudah Lewat)" : "";
                
                sb.AppendFormat("<li{0}><b>{1}</b> - {2:dd MMMM yyyy}{3}</li>", cssClass, ruangan, tgl, status);

                string suffix = isPast ? " [LEWAT]" : "";
                string displayText = string.Format("{0} - {1:dd/MM/yyyy}{2}", ruangan, tgl, suffix);
                ddlDaftarBooking.Items.Add(new ListItem(displayText, id.ToString()));
            }
            
            sb.Append("</ul>");
            lblDaftarBooking.Text = sb.ToString();
            btnBatalBooking.Enabled = true;
        }

        protected void btnBooking_Click(object sender, EventArgs e)
        {
            lblBookingMsg.Visible = true;

            if (calBooking.SelectedDate == DateTime.MinValue)
            {
                lblBookingMsg.Text = "[!] Pilih tanggal di kalender dulu!";
                return;
            }

            if (calBooking.SelectedDate.Date < DateTime.Today)
            {
                lblBookingMsg.Text = "[!] Tidak bisa booking untuk tanggal yang sudah lewat!";
                return;
            }

            try
            {
                string query = "INSERT INTO booking (RuanganID, Tanggal) VALUES (@rid, @tgl)";
                db_helper.ExecuteNonQuery(query, 
                    new MySqlParameter("@rid", ddlRuangan.SelectedValue),
                    new MySqlParameter("@tgl", calBooking.SelectedDate)
                );

                RefreshBookingList();
                lblBookingMsg.Text = string.Format("[OK] Sukses! {0} dibooking untuk tanggal {1:dd MMMM yyyy}.", ddlRuangan.SelectedItem.Text, calBooking.SelectedDate);
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) 
                    lblBookingMsg.Text = "[!] Ruangan ini sudah dibooking pada tanggal tersebut! (Dicegah oleh Database)";
                else
                    lblBookingMsg.Text = "[X] Gagal: " + ex.Message;
            }
        }

        protected void btnBatalBooking_Click(object sender, EventArgs e)
        {
            lblBatalMsg.Visible = true;

            if (string.IsNullOrEmpty(ddlDaftarBooking.SelectedValue))
            {
                lblBatalMsg.Text = "[!] Tidak ada booking yang bisa dibatalkan.";
                return;
            }

            try
            {
                int bookingId = Convert.ToInt32(ddlDaftarBooking.SelectedValue);
                string query = "DELETE FROM booking WHERE BookingID = @id";
                
                db_helper.ExecuteNonQuery(query, new MySqlParameter("@id", bookingId));

                RefreshBookingList();
                lblBatalMsg.Text = "[OK] Booking berhasil dibatalkan dari Database.";
            }
            catch (Exception ex)
            {
                lblBatalMsg.Text = "[X] Gagal membatalkan: " + ex.Message;
            }
        }

        // ============================================================
        // FITUR 2: MANAJEMEN TUGAS (Database)
        // ============================================================
        private void RefreshTugasList()
        {
            string query = "SELECT TugasID, NamaTugas, Deadline FROM tugas ORDER BY Deadline ASC";
            DataTable dt = db_helper.ExecuteDataTable(query);

            if (dt.Rows.Count == 0)
            {
                lblDaftarTugas.Text = "Belum ada tugas.";
                ddlDaftarTugas.Items.Clear();
                btnHapusTugas.Enabled = false;
                return;
            }

            var sb = new StringBuilder("<ul>");
            ddlDaftarTugas.Items.Clear();

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["TugasID"]);
                string nama = row["NamaTugas"].ToString();
                DateTime dl = Convert.ToDateTime(row["Deadline"]);

                sb.AppendFormat("<li><b>{0}</b> - Deadline: {1:dd/MM/yyyy}</li>", nama, dl);

                string displayText = string.Format("{0} - {1:dd/MM/yyyy}", nama, dl);
                ddlDaftarTugas.Items.Add(new ListItem(displayText, id.ToString()));
            }
            
            sb.Append("</ul>");
            lblDaftarTugas.Text = sb.ToString();
            btnHapusTugas.Enabled = true;
        }

        protected void btnTambahTugas_Click(object sender, EventArgs e)
        {
            lblTugasMsg.Visible = true;

            if (string.IsNullOrWhiteSpace(txtNamaTugas.Text))
            {
                lblTugasMsg.Text = "[!] Nama tugas tidak boleh kosong!";
                return;
            }

            if (calTugas.SelectedDate == DateTime.MinValue)
            {
                lblTugasMsg.Text = "[!] Pilih deadline di kalender dulu!";
                return;
            }

            try
            {
                string query = "INSERT INTO tugas (NamaTugas, Deadline) VALUES (@nama, @dl)";
                db_helper.ExecuteNonQuery(query,
                    new MySqlParameter("@nama", txtNamaTugas.Text.Trim()),
                    new MySqlParameter("@dl", calTugas.SelectedDate)
                );

                RefreshTugasList();
                lblTugasMsg.Text = "[OK] Tugas berhasil ditambahkan ke Database!";
                txtNamaTugas.Text = "";
            }
            catch (Exception ex)
            {
                lblTugasMsg.Text = "[X] Gagal menyimpan: " + ex.Message;
            }
        }

        protected void btnHapusTugas_Click(object sender, EventArgs e)
        {
            lblHapusTugasMsg.Visible = true;

            if (string.IsNullOrEmpty(ddlDaftarTugas.SelectedValue))
            {
                lblHapusTugasMsg.Text = "[!] Tidak ada tugas yang bisa dihapus.";
                return;
            }

            try
            {
                int tugasId = Convert.ToInt32(ddlDaftarTugas.SelectedValue);
                string query = "DELETE FROM tugas WHERE TugasID = @id";
                
                db_helper.ExecuteNonQuery(query, new MySqlParameter("@id", tugasId));

                RefreshTugasList();
                lblHapusTugasMsg.Text = "[OK] Tugas berhasil dihapus dari Database.";
            }
            catch (Exception ex)
            {
                lblHapusTugasMsg.Text = "[X] Gagal menghapus: " + ex.Message;
            }
        }

        // ============================================================
        // FITUR 3: KEUANGAN & GRIDVIEW (ADO.NET)
        // ============================================================
        protected void btnSimpanUang_Click(object sender, EventArgs e)
        {
            lblKeuanganMsg.Visible = true;

            int nominal;
            if (!int.TryParse(txtNominal.Text, out nominal) || nominal <= 0)
            {
                lblKeuanganMsg.Text = "[!] Masukkan nominal yang valid (> 0)!";
                return;
            }

            int saldoSekarang = HitungSaldoDariDatabase();

            if (ddlTipeUang.SelectedValue == "Keluar" && nominal > saldoSekarang)
            {
                lblKeuanganMsg.Text = string.Format("[!] Saldo tidak cukup! Saldo Anda: Rp {0:N0}.", saldoSekarang);
                return;
            }

            try
            {
                string query = "INSERT INTO transaksi (Tipe, Nominal, Keterangan) VALUES (@tipe, @nominal, @ket)";
                db_helper.ExecuteNonQuery(query,
                    new MySqlParameter("@tipe", ddlTipeUang.SelectedValue),
                    new MySqlParameter("@nominal", nominal),
                    new MySqlParameter("@ket", "Input dari Web UI")
                );

                UpdateSaldoTampilanDariDatabase();
                LoadRiwayatTransaksi(); // Refresh GridView setelah insert
                lblKeuanganMsg.Text = string.Format("[OK] Transaksi {0} Rp {1:N0} berhasil disimpan ke Database!", ddlTipeUang.SelectedItem.Text, nominal);
                txtNominal.Text = "";
            }
            catch (Exception ex)
            {
                lblKeuanganMsg.Text = "[X] Gagal menyimpan: " + ex.Message;
            }
        }

        private int HitungSaldoDariDatabase()
        {
            string query = @"
                SELECT 
                    COALESCE(SUM(CASE WHEN Tipe = 'Masuk' THEN Nominal ELSE 0 END), 0) - 
                    COALESCE(SUM(CASE WHEN Tipe = 'Keluar' THEN Nominal ELSE 0 END), 0) 
                FROM transaksi";

            object result = db_helper.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        private void UpdateSaldoTampilanDariDatabase()
        {
            int saldo = HitungSaldoDariDatabase();
            lblSaldo.Text = "Rp " + saldo.ToString("N0");
            saldoBox.Attributes["class"] = saldo < 0 ? "saldo-box negatif" : "saldo-box";
        }

        // ============================================================
        // FUNGSI GRIDVIEW (Syarat UTS: Menampilkan Data ADO.NET)
        // ============================================================
        private void LoadRiwayatTransaksi()
        {
            // Ubah DESC menjadi ASC agar data ID 1 muncul paling atas
            string query = "SELECT TransaksiID as ID, Tipe, Nominal, Keterangan, Tanggal FROM transaksi ORDER BY TransaksiID ASC";

            DataTable dt = db_helper.ExecuteDataTable(query);

            gvRiwayat.DataSource = dt;
            gvRiwayat.DataBind();
        }
    }
}