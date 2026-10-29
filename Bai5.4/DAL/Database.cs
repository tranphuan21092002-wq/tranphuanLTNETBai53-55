using MySql.Data.MySqlClient;

namespace HotelManagement.DAL
{
    public static class Database
    {

        private static readonly string connectionString = "Server=localhost;Port=3306;Database=HotelDB;Uid=root;Pwd=;SslMode=Disabled;"; public static MySqlConnection GetConnection() { return new MySqlConnection(connectionString); }
    }
}
