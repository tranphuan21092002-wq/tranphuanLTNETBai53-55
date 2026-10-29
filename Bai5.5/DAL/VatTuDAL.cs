using System;
using System.Data;
using MySql.Data.MySqlClient;
using InventoryManagement.DTO;
namespace InventoryManagement.DAL
{
    public class VatTuDAL
    {
        public DataTable GetAll(string keyword = "")
        {
            using (var c = Database.GetConnection()) using (var cmd = new MySqlCommand(@"SELECT MaVatTu, TenVatTu, DVT, SoLuongTon FROM VatTu WHERE (@kw = N'' OR MaVatTu LIKE @pattern OR TenVatTu LIKE @pattern) ORDER BY MaVatTu", c))
            { cmd.Parameters.Add("@kw", MySqlDbType.VarChar, 150).Value = keyword ?? ""; cmd.Parameters.Add("@pattern", MySqlDbType.VarChar, 172).Value = "%" + (keyword ?? "") + "%"; using (var da = new MySqlDataAdapter(cmd)) { var dt = new DataTable(); da.Fill(dt); return dt; } }
        }
        public void Insert(VatTuDTO x) { using (var c=Database.GetConnection()) using (var cmd=new MySqlCommand("INSERT INTO VatTu(MaVatTu,TenVatTu,DVT,SoLuongTon) VALUES(@ma,@ten,@dvt,@ton)",c)) { Add(cmd,x); c.Open(); cmd.ExecuteNonQuery(); } }
        public void Update(VatTuDTO x) { using (var c=Database.GetConnection()) using (var cmd=new MySqlCommand("UPDATE VatTu SET TenVatTu=@ten,DVT=@dvt,SoLuongTon=@ton WHERE MaVatTu=@ma",c)) { Add(cmd,x); c.Open(); if(cmd.ExecuteNonQuery()==0) throw new Exception("Khong tim thay ma vat tu."); } }
        public void Delete(string ma) { using (var c=Database.GetConnection()) using (var cmd=new MySqlCommand("DELETE FROM VatTu WHERE MaVatTu=@ma",c)) { cmd.Parameters.Add("@ma",MySqlDbType.VarChar,20).Value=ma; c.Open(); cmd.ExecuteNonQuery(); } }
        private void Add(MySqlCommand cmd,VatTuDTO x) { cmd.Parameters.Add("@ma",MySqlDbType.VarChar,20).Value=x.MaVatTu; cmd.Parameters.Add("@ten",MySqlDbType.VarChar,150).Value=x.TenVatTu; cmd.Parameters.Add("@dvt",MySqlDbType.VarChar,20).Value=(object)x.DVT??DBNull.Value; cmd.Parameters.Add("@ton",MySqlDbType.Decimal).Value=x.SoLuongTon; cmd.Parameters["@ton"].Precision=18; cmd.Parameters["@ton"].Scale=2; }
        public DataTable GetLowStock(decimal threshold) { using(var c=Database.GetConnection()) using(var cmd=new MySqlCommand("SELECT MaVatTu,TenVatTu,DVT,SoLuongTon FROM VatTu WHERE SoLuongTon < @muc ORDER BY SoLuongTon",c)) { cmd.Parameters.Add("@muc",MySqlDbType.Decimal).Value=threshold; cmd.Parameters["@muc"].Precision=18;cmd.Parameters["@muc"].Scale=2; using(var da=new MySqlDataAdapter(cmd)){var dt=new DataTable();da.Fill(dt);return dt;} } }
        public decimal GetInventoryValue() { using(var c=Database.GetConnection()) using(var cmd=new MySqlCommand("SELECT COALESCE(SUM(v.SoLuongTon * COALESCE((SELECT ct.DonGia FROM ChiTietPhieuKho ct INNER JOIN PhieuKho p ON p.MaPhieu=ct.MaPhieu WHERE ct.MaVatTu=v.MaVatTu ORDER BY p.NgayLap DESC,p.MaPhieu DESC LIMIT 1),0)),0) FROM VatTu v",c)) { c.Open(); return Convert.ToDecimal(cmd.ExecuteScalar()); } }
    }
}
