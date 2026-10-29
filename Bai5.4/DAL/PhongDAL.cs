
using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace HotelManagement.DAL
{
    public class PhongDAL
    {
        public DataTable GetAll()
        {
            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    string sql =
                        "SELECT SoPhong AS 'So phong', " +
                        "LoaiPhong AS 'Loai phong', " +
                        "GiaTheoGio AS 'Gia theo gio', " +
                        "TrangThai AS 'Trang thai' " +
                        "FROM Phong ORDER BY SoPhong";

                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var da = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Loi tai danh sach phong:\n" + ex.Message,
                    "Loi MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return new DataTable();
            }
        }

        public DataTable GetAvailable()
        {
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(
                "SELECT SoPhong, LoaiPhong, GiaTheoGio " +
                "FROM Phong WHERE TrangThai='Trong' ORDER BY SoPhong",
                conn))
            using (var da = new MySqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public void SetStatus(
            MySqlConnection conn,
            MySqlTransaction tx,
            string soPhong,
            string status)
        {
            using (var cmd = new MySqlCommand(
                "UPDATE Phong SET TrangThai=@status " +
                "WHERE SoPhong=@room", conn, tx))
            {
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@room", soPhong);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
