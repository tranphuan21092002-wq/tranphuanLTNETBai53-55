using System;
using System.Data;
using MySql.Data.MySqlClient;
using InventoryManagement.DTO;
namespace InventoryManagement.DAL
{
    public class NhaCungCapDAL
    {
        public DataTable GetAll(string keyword="") { using(var c=Database.GetConnection()) using(var cmd=new MySqlCommand("SELECT MaNCC,TenNCC,DienThoai,DiaChi FROM NhaCungCap WHERE (@kw=N'' OR TenNCC LIKE @p OR DienThoai LIKE @p) ORDER BY MaNCC",c)) { cmd.Parameters.Add("@kw",MySqlDbType.VarChar,150).Value=keyword??"";cmd.Parameters.Add("@p",MySqlDbType.VarChar,170).Value="%"+(keyword??"")+"%";using(var da=new MySqlDataAdapter(cmd)){var dt=new DataTable();da.Fill(dt);return dt;} } }
        public void Insert(NhaCungCapDTO x) { using(var c=Database.GetConnection()) using(var cmd=new MySqlCommand("INSERT INTO NhaCungCap(TenNCC,DienThoai,DiaChi) VALUES(@ten,@dt,@dc)",c)){Add(cmd,x);c.Open();cmd.ExecuteNonQuery();} }
        public void Update(NhaCungCapDTO x) { using(var c=Database.GetConnection()) using(var cmd=new MySqlCommand("UPDATE NhaCungCap SET TenNCC=@ten,DienThoai=@dt,DiaChi=@dc WHERE MaNCC=@ma",c)){Add(cmd,x);cmd.Parameters.Add("@ma",MySqlDbType.Int32).Value=x.MaNCC;c.Open();if(cmd.ExecuteNonQuery()==0)throw new Exception("Khong tim thay nha cung cap.");} }
        public void Delete(int ma) { using(var c=Database.GetConnection()) using(var cmd=new MySqlCommand("DELETE FROM NhaCungCap WHERE MaNCC=@ma",c)){cmd.Parameters.Add("@ma",MySqlDbType.Int32).Value=ma;c.Open();cmd.ExecuteNonQuery();} }
        private void Add(MySqlCommand cmd,NhaCungCapDTO x){cmd.Parameters.Add("@ten",MySqlDbType.VarChar,150).Value=x.TenNCC;cmd.Parameters.Add("@dt",MySqlDbType.VarChar,15).Value=string.IsNullOrWhiteSpace(x.DienThoai)?(object)DBNull.Value:x.DienThoai;cmd.Parameters.Add("@dc",MySqlDbType.VarChar,255).Value=(object)x.DiaChi??DBNull.Value;}
    }
}
