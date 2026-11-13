using System;
using campus_app.models;

namespace campus_app
{
    public partial class Login : System.Web.UI.Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Memanggil class OOP Auth yang
            auth authManager = new auth();

            if (authManager.Login(txtUsername.Text, txtPassword.Text))
            {
                // Jika sukses, simpan ke Session
                Session["Username"] = txtUsername.Text;
                Response.Redirect("Default.aspx");
            }
            else
            {
                lblError.Text = "[!] Username atau Password salah!";
                lblError.Visible = true;
            }
        }
    }
}