using System;

namespace campus_app.models
{
    public class transaksi
    {
        public int TransaksiID { get; set; }
        public string Tipe { get; set; } // "Masuk" atau "Keluar"
        public int Nominal { get; set; }
        public string Keterangan { get; set; }
        public DateTime Tanggal { get; set; }
    }
}