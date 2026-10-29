using System.Data;
using MySql.Data.MySqlClient;

namespace HotelManagement.DAL
{
    public class DichVuDAL
    {
        public DataTable GetAll()
        {
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand("SELECT MaDV, TenDV, DonGia FROM DichVu ORDER BY TenDV", conn))
            using (var da = new MySqlDataAdapter(cmd))
            { var dt = new DataTable(); da.Fill(dt); return dt; }
        }
    }
}
