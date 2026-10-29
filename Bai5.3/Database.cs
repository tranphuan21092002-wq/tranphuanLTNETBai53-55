
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace CafePOS
{
    public static class Database
    {
        private static string connectionString =
            "Server=localhost;Port=3306;Database=CafePOSDB;Uid=root;Pwd=;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public static bool TestConnection()
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Khong ket noi duoc MySQL:\n" + ex.Message,
                    "Loi ket noi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return false;
            }
        }

        public static DataTable GetData(string sql)
        {
            using (MySqlConnection conn = GetConnection())
            using (MySqlDataAdapter adapter =
                new MySqlDataAdapter(sql, conn))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
    }
}
