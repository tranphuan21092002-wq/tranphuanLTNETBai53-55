using MySql.Data.MySqlClient;
using HotelManagement.DTO;

namespace HotelManagement.DAL
{
    public class KhachHangDAL
    {
        public KhachHangDTO FindByCCCD(MySqlConnection conn, MySqlTransaction tx, string cccd)
        {
            using (var cmd = new MySqlCommand("SELECT MaKH,HoTen,CCCD,SoDienThoai FROM KhachHang WHERE CCCD=@cccd LIMIT 1", conn, tx))
            {
                cmd.Parameters.AddWithValue("@cccd", cccd);
                using (var r = cmd.ExecuteReader())
                { if (!r.Read()) return null; return new KhachHangDTO { MaKH = r.GetInt32("MaKH"), HoTen = r.GetString("HoTen"), CCCD = r.GetString("CCCD"), SoDienThoai = r.IsDBNull(r.GetOrdinal("SoDienThoai")) ? "" : r.GetString("SoDienThoai") }; }
            }
        }
        public int Add(MySqlConnection conn, MySqlTransaction tx, string hoTen, string cccd, string sdt)
        {
            using (var cmd = new MySqlCommand("INSERT INTO KhachHang(HoTen,CCCD,SoDienThoai) VALUES(@name,@cccd,@phone); SELECT LAST_INSERT_ID();", conn, tx))
            { cmd.Parameters.AddWithValue("@name", hoTen); cmd.Parameters.AddWithValue("@cccd", cccd); cmd.Parameters.AddWithValue("@phone", sdt); return System.Convert.ToInt32(cmd.ExecuteScalar()); }
        }
    }
}
