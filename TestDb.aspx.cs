using System;
using System.Configuration;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace CampusApp
{
    public partial class TestDb : System.Web.UI.Page
    {
        // ============================================================
        // TEST 1: Diagnostik lengkap step-by-step
        // ============================================================
        protected void btnTest_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            // ---- LANGKAH 1: Cek Assembly MySql.Data ----
            sb.Append("<h3>LANGKAH 1: Assembly MySql.Data</h3>");
            try
            {
                var asm = typeof(MySqlConnection).Assembly;
                sb.Append($"<div class='box ok'>✅ Assembly ter-load<br/>");
                sb.Append($"<span class='label'>Nama:</span> <span class='value'>{asm.FullName}</span><br/>");
                sb.Append($"<span class='label'>Lokasi:</span> <span class='value'>{asm.Location}</span>");
                sb.Append("</div>");
            }
            catch (Exception ex)
            {
                sb.Append($"<div class='box fail'>❌ GAGAL load MySql.Data: {ex.Message}</div>");
                pnlHasil.Controls.Add(new System.Web.UI.LiteralControl(sb.ToString()));
                return;
            }

            // ---- LANGKAH 2: Cek Connection String di Web.config ----
            sb.Append("<h3>LANGKAH 2: Connection String (Web.config)</h3>");
            var setting = ConfigurationManager.ConnectionStrings["CampusDB"];
            if (setting == null)
            {
                sb.Append("<div class='box fail'>❌ Connection string 'CampusDB' TIDAK ditemukan di Web.config!</div>");
                pnlHasil.Controls.Add(new System.Web.UI.LiteralControl(sb.ToString()));
                return;
            }
            sb.Append($"<div class='box ok'>✅ Connection string ditemukan<br/>");
            sb.Append($"<span class='label'>providerName:</span> <span class='value'>{setting.ProviderName}</span><br/>");
            sb.Append($"<span class='label'>connectionString:</span> <span class='value'>{MaskPassword(setting.ConnectionString)}</span>");
            sb.Append("</div>");

            // ---- LANGKAH 3: Test Buka Koneksi ----
            sb.Append("<h3>LANGKAH 3: Test Buka Koneksi</h3>");
            try
            {
                using (MySqlConnection conn = new MySqlConnection(setting.ConnectionString))
                {
                    conn.Open();
                    sb.Append($"<div class='box ok'>✅ Koneksi BERHASIL dibuka!<br/>");
                    sb.Append($"<span class='label'>ServerVersion:</span> <span class='value'>{conn.ServerVersion}</span><br/>");
                    sb.Append($"<span class='label'>DataSource:</span> <span class='value'>{conn.DataSource}</span><br/>");
                    sb.Append($"<span class='label'>Database:</span> <span class='value'>{conn.Database}</span><br/>");
                    sb.Append($"<span class='label'>State:</span> <span class='value'>{conn.State}</span>");
                    sb.Append("</div>");

                    // ---- LANGKAH 4: Cek Database visual_ii ----
                    sb.Append("<h3>LANGKAH 4: Cek Database 'visual_ii'</h3>");
                    using (MySqlCommand cmd = new MySqlCommand(
                        "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA " +
                        "WHERE SCHEMA_NAME = 'visual_ii'", conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result == null)
                            sb.Append("<div class='box fail'>❌ Database 'visual_ii' TIDAK ADA di server!</div>");
                        else
                            sb.Append("<div class='box ok'>✅ Database 'visual_ii' ditemukan</div>");
                    }

                    // ---- LANGKAH 5: Daftar Tabel ----
                    sb.Append("<h3>LANGKAH 5: Daftar Tabel di 'visual_ii'</h3>");
                    using (MySqlCommand cmd = new MySqlCommand(
                        "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES " +
                        "WHERE TABLE_SCHEMA = 'visual_ii' ORDER BY TABLE_NAME", conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        var tables = new System.Collections.Generic.List<string>();
                        while (reader.Read()) tables.Add(reader.GetString(0));

                        if (tables.Count == 0)
                            sb.Append("<div class='box warn'>⚠️ Tidak ada tabel (SQL script belum dijalankan?)</div>");
                        else
                        {
                            sb.Append($"<div class='box ok'>✅ {tables.Count} tabel ditemukan:<br/>");
                            foreach (var t in tables)
                                sb.Append($"&nbsp;&nbsp;• <span class='value'>{t}</span><br/>");
                            sb.Append("</div>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sb.Append("<div class='box fail'>");
                sb.Append($"❌ <b>KONEKSI GAGAL</b><br/><br/>");
                sb.Append($"<span class='label'>Tipe error:</span> <span class='value'>{ex.GetType().Name}</span><br/>");
                sb.Append($"<span class='label'>Pesan:</span> <span class='value'>{ex.Message}</span><br/><br/>");
                sb.Append("<b>Detail lengkap (kirim ini ke saya):</b>");
                sb.Append($"<pre>{System.Web.HttpUtility.HtmlEncode(ex.ToString())}</pre>");
                sb.Append("</div>");

                // Saran berdasarkan jenis error
                sb.Append("<h3>💡 Saran Berdasarkan Error</h3>");
                sb.Append($"<div class='box warn'>{GetSaran(ex)}</div>");
            }

            pnlHasil.Controls.Add(new System.Web.UI.LiteralControl(sb.ToString()));
        }

        // ============================================================
        // TEST 2: Koneksi hardcoded — untuk memastikan bukan Web.config
        // ============================================================
        protected void btnTestRaw_Click(object sender, EventArgs e)
        {
            // Coba 3 variasi connection string umum
            string[] variasi = new string[]
            {
                // Variasi A: paling lengkap (MySQL 8.x)
                "Server=localhost;Port=3306;Database=visual_ii;Uid=root;Pwd=;SslMode=none;AllowPublicKeyRetrieval=True;Charset=utf8mb4;",
                
                // Variasi B: tanpa SSL (MySQL 5.7)
                "Server=localhost;Database=visual_ii;Uid=root;Pwd=;",
                
                // Variasi C: pakai 127.0.0.1 (hindari masalah DNS/IPv6)
                "Server=127.0.0.1;Port=3306;Database=visual_ii;Uid=root;Pwd=;SslMode=none;AllowPublicKeyRetrieval=True;"
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("<h3>🔧 Test Koneksi Hardcoded (bypass Web.config)</h3>");
            sb.Append("<p>Mencoba 3 variasi connection string umum:</p>");

            foreach (var connStr in variasi)
            {
                sb.Append($"<div class='box'>");
                sb.Append($"<span class='label'>Try:</span> <span class='value'>{MaskPassword(connStr)}</span><br/>");
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connStr))
                    {
                        conn.Open();
                        sb.Append($"<b style='color:#4ec9b0'>✅ BERHASIL</b> — Server: {conn.ServerVersion}, State: {conn.State}");
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    sb.Append($"<b style='color:#f48771'>❌ GAGAL</b> — {ex.Message}");
                }
                sb.Append("</div>");
            }

            sb.Append("<div class='box warn'>");
            sb.Append("<b>📋 Cara pakai:</b><br/>");
            sb.Append("Kalau salah satu variasi BERHASIL tapi Web.config gagal → masalahnya di connection string Web.config.<br/>");
            sb.Append("Kalau SEMUA variasi GAGAL → masalahnya di MySQL server / user / password.<br/>");
            sb.Append("Salin variasi yang BERHASIL ke Web.config.");
            sb.Append("</div>");

            pnlHasil.Controls.Add(new System.Web.UI.LiteralControl(sb.ToString()));
        }

        // ============================================================
        // Helper: sembunyikan password
        // ============================================================
        private string MaskPassword(string connStr)
        {
            if (string.IsNullOrEmpty(connStr)) return "";
            return System.Text.RegularExpressions.Regex.Replace(
                connStr, @"(Pwd|Password)=([^;]*)", "$1=***");
        }

        // ============================================================
        // Helper: saran per tipe error
        // ============================================================
        private string GetSaran(Exception ex)
        {
            string msg = ex.Message.ToLower();
            string type = ex.GetType().Name;

            if (msg.Contains("access denied") || msg.Contains("authentication"))
                return "<b>Penyebab: Username/password MySQL salah.</b><br/>" +
                       "👉 Buka phpMyAdmin, coba login dengan <code>root</code> tanpa password.<br/>" +
                       "👉 Kalau berhasil, connection string Anda sudah benar (Uid=root;Pwd=;).<br/>" +
                       "👉 Kalau login pakai password, tambahkan <code>Pwd=passwordAnda;</code> di Web.config.";

            if (msg.Contains("unable to connect") || msg.Contains("host") || msg.Contains("timeout"))
                return "<b>Penyebab: MySQL server tidak running, atau port salah.</b><br/>" +
                       "👉 Buka XAMPP Control Panel, pastikan MySQL <b>Running</b> (hijau).<br/>" +
                       "👉 Cek port MySQL di XAMPP (default 3306, kadang 3307).<br/>" +
                       "👉 Kalau port 3307, ganti <code>Port=3307;</code> di connection string.<br/>" +
                       "👉 Coba buka <code>http://localhost/phpmyadmin</code> — kalau tidak bisa, MySQL memang mati.";

            if (msg.Contains("ssl") || msg.Contains("public key"))
                return "<b>Penyebab: MySQL 8.x butuh SSL setting khusus.</b><br/>" +
                       "👉 Pastikan connection string punya <code>SslMode=none;AllowPublicKeyRetrieval=True;</code><br/>" +
                       "👉 Untuk MySQL 8.x, ini WAJIB. Tanpa ini, error SSL muncul.";

            if (msg.Contains("unknown database") || msg.Contains("database") && msg.Contains("does not exist"))
                return "<b>Penyebab: Database 'visual_ii' tidak ada.</b><br/>" +
                       "👉 Buka phpMyAdmin, cek apakah ada database <code>visual_ii</code>.<br/>" +
                       "👉 Kalau belum, buat: tab <b>Databases</b> → nama <code>visual_ii</code> → Create.<br/>" +
                       "👉 Lalu jalankan SQL script yang saya berikan sebelumnya.";

            if (msg.Contains("could not load file") || msg.Contains("assembly") || type.Contains("FileNotFound"))
                return "<b>Penyebab: Assembly MySql.Data tidak ditemukan.</b><br/>" +
                       "👉 Klik kanan project → <b>Manage NuGet Packages</b> → Browse → cari <code>MySql.Data</code> → Install.<br/>" +
                       "👉 Cek di <b>References</b> project, harus ada <code>MySql.Data</code>.<br/>" +
                       "👉 Setelah install, <b>Rebuild Solution</b> (Build → Rebuild).";

            if (msg.Contains("port") || msg.Contains("3306"))
                return "<b>Penyebab: Port MySQL tidak sesuai.</b><br/>" +
                       "👉 Buka XAMPP Control Panel → lihat angka di samping MySQL (misal 3306 atau 3307).<br/>" +
                       "👉 Sesuaikan <code>Port=XXXX;</code> di connection string.";

            return "<b>Error tidak dikenali.</b><br/>" +
                   "👉 Copy-paste <b>detail lengkap</b> dari kotak merah di atas ke chat, saya bantu deteksi.<br/>" +
                   "👉 Sertakan juga screenshot XAMPP Control Panel (status MySQL).";
        }
    }
}