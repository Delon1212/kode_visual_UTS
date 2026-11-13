<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="campus_app.Default" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Campus App - Admin Dashboard</title>
    <%-- Panggil file CSS terpisah --%>
    <link href="~/assets/css/style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">

            <!-- HEADER -->
            <div class="header">
                <div class="logout">
                    <asp:LinkButton ID="lnkLogout" runat="server" Text="Logout" OnClick="lnkLogout_Click" />
                </div>
                <h1>Campus App</h1>
                <p>Sistem Informasi Terpadu Mahasiswa</p>
            </div>

            <!-- NAVIGASI MENU -->
            <div class="nav">
                <asp:Button runat="server" Text="Booking Ruangan" CommandArgument="0" OnClick="Menu_Click" CssClass="btn-nav" CausesValidation="false" />
                <asp:Button runat="server" Text="Agenda Kampus" CommandArgument="1" OnClick="Menu_Click" CssClass="btn-nav" CausesValidation="false" />
                <asp:Button runat="server" Text="Manajemen Tugas" CommandArgument="2" OnClick="Menu_Click" CssClass="btn-nav" CausesValidation="false" />
                <asp:Button runat="server" Text="Keuangan" CommandArgument="3" OnClick="Menu_Click" CssClass="btn-nav" CausesValidation="false" />
            </div>

            <!-- KONTEN MULTIVIEW -->
            <div class="content">
                <asp:MultiView ID="MultiView1" runat="server">

                    <!-- ================= VIEW 0: BOOKING ================= -->
                    <asp:View ID="ViewBooking" runat="server">
                        <div class="card">
                            <h3>Formulir Booking Ruangan</h3>

                            <div class="form-group">
                                <label>Pilih Ruangan:</label>
                                <!-- Dropdown diisi dari database di code-behind -->
                                <asp:DropDownList ID="ddlRuangan" runat="server"></asp:DropDownList>
                            </div>

                            <div class="form-group">
                                <label>Pilih Tanggal:</label>
                                <asp:Calendar ID="calBooking" runat="server" BackColor="White" BorderColor="#ccc"></asp:Calendar>
                            </div>

                            <asp:Button ID="btnBooking" runat="server" Text="Simpan Booking" CssClass="btn-primary" OnClick="btnBooking_Click" />
                            <asp:Label ID="lblBookingMsg" runat="server" CssClass="message" Visible="false" />

                            <hr />

                            <h4>Daftar Booking Aktif:</h4>
                            <div class="data-list">
                                <asp:Label ID="lblDaftarBooking" runat="server" Text="Belum ada data booking." />
                            </div>

                            <hr />

                            <h4>Pembatalan Booking:</h4>
                            <div class="form-group">
                                <label>Pilih Booking yang akan dibatalkan:</label>
                                <asp:DropDownList ID="ddlDaftarBooking" runat="server"></asp:DropDownList>
                            </div>
                            <asp:Button ID="btnBatalBooking" runat="server" Text="Batalkan Booking" CssClass="btn-danger" 
                                        OnClick="btnBatalBooking_Click" OnClientClick="return confirm('Yakin ingin membatalkan booking ini?');" />
                            <asp:Label ID="lblBatalMsg" runat="server" CssClass="message" Visible="false" />
                        </div>
                    </asp:View>

                    <!-- ================= VIEW 1: AGENDA ================= -->
                    <asp:View ID="ViewAgenda" runat="server">
                        <div class="card">
                            <h3>Agenda & Pengumuman Kampus</h3>
                            <p style="margin-bottom: 15px; color: #555;">Banner informasi di bawah ini dimuat secara dinamis dari file XML:</p>
                            
                            <div class="banner-area">
                                <asp:AdRotator ID="AdRotator1" runat="server" AdvertisementFile="~/data_banner.xml" Width="600px" />
                            </div>
                            
                            <p style="margin-top: 15px; font-size: 13px; color: #888; font-style: italic;">
                                *Silakan refresh halaman (F5) untuk melihat banner berganti secara acak.
                            </p>
                        </div>
                    </asp:View>

                    <!-- ================= VIEW 2: TUGAS ================= -->
                    <asp:View ID="ViewTugas" runat="server">
                        <div class="card">
                            <h3>Manajemen Tugas</h3>

                            <div class="form-group">
                                <label>Nama Tugas:</label>
                                <asp:TextBox ID="txtNamaTugas" runat="server" placeholder="Contoh: Mengumpulkan Laporan"></asp:TextBox>
                            </div>

                            <div class="form-group">
                                <label>Tanggal Deadline:</label>
                                <asp:Calendar ID="calTugas" runat="server" BackColor="White"></asp:Calendar>
                            </div>

                            <asp:Button ID="btnTambahTugas" runat="server" Text="Tambah Tugas" CssClass="btn-primary" OnClick="btnTambahTugas_Click" />
                            <asp:Label ID="lblTugasMsg" runat="server" CssClass="message" Visible="false" />

                            <hr />

                            <h4>Daftar Tugas Anda:</h4>
                            <div class="data-list">
                                <asp:Label ID="lblDaftarTugas" runat="server" Text="Belum ada tugas." />
                            </div>

                            <hr />

                            <h4>Hapus Tugas:</h4>
                            <div class="form-group">
                                <label>Pilih Tugas yang akan dihapus:</label>
                                <asp:DropDownList ID="ddlDaftarTugas" runat="server"></asp:DropDownList>
                            </div>
                            <asp:Button ID="btnHapusTugas" runat="server" Text="Hapus Tugas" CssClass="btn-danger" 
                                        OnClick="btnHapusTugas_Click" OnClientClick="return confirm('Yakin ingin menghapus tugas ini?');" />
                            <asp:Label ID="lblHapusTugasMsg" runat="server" CssClass="message" Visible="false" />
                        </div>
                    </asp:View>

                    <!-- ================= VIEW 3: KEUANGAN ================= -->
                    <asp:View ID="ViewKeuangan" runat="server">
                        <div class="card">
                            <h3>Pencatatan Keuangan Pribadi</h3>

                            <div class="form-group">
                                <label>Jenis Transaksi:</label>
                                <asp:DropDownList ID="ddlTipeUang" runat="server">
                                    <asp:ListItem Value="Masuk">Pemasukan (Uang Masuk)</asp:ListItem>
                                    <asp:ListItem Value="Keluar">Pengeluaran (Uang Keluar)</asp:ListItem>
                                </asp:DropDownList>
                            </div>

                            <div class="form-group">
                                <label>Nominal (Rp):</label>
                                <asp:TextBox ID="txtNominal" runat="server" TextMode="Number" placeholder="Contoh: 50000"></asp:TextBox>
                            </div>

                            <asp:Button ID="btnSimpanUang" runat="server" Text="Catat Transaksi" CssClass="btn-primary" OnClick="btnSimpanUang_Click" />
                            <asp:Label ID="lblKeuanganMsg" runat="server" CssClass="message" Visible="false" />

                            <!-- Box Saldo -->
                            <div id="saldoBox" runat="server" class="saldo-box">
                                <h2>Total Saldo Saat Ini</h2>
                                <div class="amount">
                                    <asp:Label ID="lblSaldo" runat="server" Text="Rp 0" />
                                </div>
                            </div>

                            <hr />
                            
                            <!-- Kontrol Penampil Data (Syarat UTS) -->
                            <h4>Riwayat Transaksi (Data dari Database)</h4>
                            <asp:GridView ID="gvRiwayat" runat="server" AutoGenerateColumns="True" CssClass="grid-view" GridLines="None">
                            </asp:GridView>

                        </div>
                    </asp:View>

                </asp:MultiView>
            </div>
        </div>
    </form>
</body>
</html>