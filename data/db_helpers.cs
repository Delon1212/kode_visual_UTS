using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace campus_app.data
{
    public static class db_helper
    {
        // Mengambil connection string dari Web.config
        private static string ConnStr => ConfigurationManager.ConnectionStrings["CampusDB"].ConnectionString;

        // 1. Untuk INSERT, UPDATE, DELETE (Mengembalikan jumlah baris terpengaruh)
        public static int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnStr))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // 2. Untuk SELECT 1 nilai (Misal: Total Saldo)
        public static object ExecuteScalar(string query, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnStr))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteScalar();
                }
            }
        }

        // 3. Untuk SELECT tabel data
        public static DataTable ExecuteDataTable(string query, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnStr))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }
    }
}