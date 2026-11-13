using System;
using MySql.Data.MySqlClient;
using campus_app.data;

namespace campus_app.models
{
    public class auth
    {
        // Method OOP untuk memvalidasi user ke database
        public bool Login(string username, string password)
        {
            string query = "SELECT COUNT(*) FROM users WHERE Username = @user AND PasswordHash = @pass";

            object result = db_helper.ExecuteScalar(query,
                new MySqlParameter("@user", username),
                new MySqlParameter("@pass", password)
            );

            // Jika hasil > 0, berarti user ditemukan
            return Convert.ToInt32(result) > 0;
        }
    }
}