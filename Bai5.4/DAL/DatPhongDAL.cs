using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace HotelManagement.DAL
{
    public class DatPhongDAL
    {
        public DataTable GetCurrentBookings()
        {
            string sql = "SELECT p.MaPhieu AS 'Ma phieu', p.SoPhong AS 'So phong', k.HoTen AS 'Khach hang', k.CCCD, p.NgayNhanPhong AS 'Nhan phong' FROM PhieuDatPhong p JOIN KhachHang k ON p.MaKH=k.MaKH WHERE p.NgayTraPhong IS NULL ORDER BY p.NgayNhanPhong";
            using (var conn = Database.GetConnection()) using (var cmd = new MySqlCommand(sql, conn)) using (var da = new MySqlDataAdapter(cmd))
            { var dt = new DataTable(); da.Fill(dt); return dt; }
        }
        public void CheckIn(string room, string name, string cccd, string phone)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open(); using (var tx = conn.BeginTransaction())
                try
                {
                    int customerId;
                    using (var check = new MySqlCommand("SELECT MaKH FROM KhachHang WHERE CCCD=@cccd LIMIT 1", conn, tx))
                    { check.Parameters.AddWithValue("@cccd", cccd); object existing = check.ExecuteScalar();
                      if (existing == null) { using (var add = new MySqlCommand("INSERT INTO KhachHang(HoTen,CCCD,SoDienThoai) VALUES(@name,@cccd,@phone); SELECT LAST_INSERT_ID();", conn, tx)) { add.Parameters.AddWithValue("@name", name); add.Parameters.AddWithValue("@cccd", cccd); add.Parameters.AddWithValue("@phone", phone); customerId = Convert.ToInt32(add.ExecuteScalar()); } }
                      else { customerId = Convert.ToInt32(existing); using (var upd = new MySqlCommand("UPDATE KhachHang SET HoTen=@name, SoDienThoai=@phone WHERE MaKH=@id", conn, tx)) { upd.Parameters.AddWithValue("@name", name); upd.Parameters.AddWithValue("@phone", phone); upd.Parameters.AddWithValue("@id", customerId); upd.ExecuteNonQuery(); } }
                    }
                    using (var roomCheck = new MySqlCommand("SELECT TrangThai FROM Phong WHERE SoPhong=@room FOR UPDATE", conn, tx))
                    { roomCheck.Parameters.AddWithValue("@room", room); object state = roomCheck.ExecuteScalar(); if (state == null || state.ToString() != "Trong") throw new Exception("Phong khong ton tai hoac khong o trang thai Trong."); }
                    using (var cmd = new MySqlCommand("INSERT INTO PhieuDatPhong(SoPhong,MaKH,NgayNhanPhong) VALUES(@room,@customer,NOW())", conn, tx))
                    { cmd.Parameters.AddWithValue("@room", room); cmd.Parameters.AddWithValue("@customer", customerId); cmd.ExecuteNonQuery(); }
                    using (var cmd = new MySqlCommand("UPDATE Phong SET TrangThai='Dang o' WHERE SoPhong=@room", conn, tx)) { cmd.Parameters.AddWithValue("@room", room); cmd.ExecuteNonQuery(); }
                    tx.Commit();
                }
                catch { tx.Rollback(); throw; }
            }
        }
        public decimal CheckOut(int maPhieu)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open(); using (var tx = conn.BeginTransaction())
                try
                {
                    string room; decimal rate; DateTime checkIn; int bookingId;
                    using (var cmd = new MySqlCommand("SELECT p.MaPhieu,p.SoPhong,p.NgayNhanPhong,ph.GiaTheoGio FROM PhieuDatPhong p JOIN Phong ph ON p.SoPhong=ph.SoPhong WHERE p.MaPhieu=@id AND p.NgayTraPhong IS NULL FOR UPDATE", conn, tx))
                    { cmd.Parameters.AddWithValue("@id", maPhieu); using (var r = cmd.ExecuteReader()) { if (!r.Read()) throw new Exception("Khong tim thay phieu dang thue."); bookingId = r.GetInt32("MaPhieu"); room = r.GetString("SoPhong"); checkIn = r.GetDateTime("NgayNhanPhong"); rate = r.GetDecimal("GiaTheoGio"); } }
                    decimal roomCost = Math.Max(1, (decimal)Math.Ceiling((DateTime.Now - checkIn).TotalHours)) * rate;
                    decimal serviceCost;
                    using (var cmd = new MySqlCommand("SELECT IFNULL(SUM(sd.SoLuong*d.DonGia),0) FROM SuDungDichVu sd JOIN DichVu d ON sd.MaDV=d.MaDV WHERE sd.MaPhieu=@id", conn, tx)) { cmd.Parameters.AddWithValue("@id", bookingId); serviceCost = Convert.ToDecimal(cmd.ExecuteScalar()); }
                    decimal total = roomCost + serviceCost;
                    using (var cmd = new MySqlCommand("UPDATE PhieuDatPhong SET NgayTraPhong=NOW(),TongTienThanhToan=@total WHERE MaPhieu=@id", conn, tx)) { cmd.Parameters.AddWithValue("@total", total); cmd.Parameters.AddWithValue("@id", bookingId); cmd.ExecuteNonQuery(); }
                    using (var cmd = new MySqlCommand("UPDATE Phong SET TrangThai='Dang don dep' WHERE SoPhong=@room", conn, tx)) { cmd.Parameters.AddWithValue("@room", room); cmd.ExecuteNonQuery(); }
                    tx.Commit(); return total;
                }
                catch { tx.Rollback(); throw; }
            }
        }
        public void AddService(int maPhieu, int maDV, int soLuong)
        {
            using (var conn = Database.GetConnection())
            { conn.Open(); using (var cmd = new MySqlCommand("INSERT INTO SuDungDichVu(MaPhieu,MaDV,SoLuong) VALUES(@booking,@service,@qty)", conn)) { cmd.Parameters.AddWithValue("@booking", maPhieu); cmd.Parameters.AddWithValue("@service", maDV); cmd.Parameters.AddWithValue("@qty", soLuong); cmd.ExecuteNonQuery(); } }
        }
        public void MarkClean(string room)
        {
            using (var conn = Database.GetConnection()) { conn.Open(); using (var cmd = new MySqlCommand("UPDATE Phong SET TrangThai='Trong' WHERE SoPhong=@room AND TrangThai='Dang don dep'", conn)) { cmd.Parameters.AddWithValue("@room", room); if (cmd.ExecuteNonQuery() == 0) throw new Exception("Phong khong o trang thai Dang don dep."); } }
        }
    }
}
