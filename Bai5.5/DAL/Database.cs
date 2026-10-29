
using MySql.Data.MySqlClient;

namespace InventoryManagement.DAL
{
    public static class Database
    {
        private static readonly string connectionString =
            "Server=localhost;Port=3306;Database=InventoryDB;Uid=root;Pwd=;SslMode=Disabled;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
