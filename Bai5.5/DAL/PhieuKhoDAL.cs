using System;
using System.Data;
using MySql.Data.MySqlClient;
using InventoryManagement.DTO;
namespace InventoryManagement.DAL
{
    public class PhieuKhoDAL
    {
        public int Save(PhieuKhoDTO p)
        {
            if (p == null || p.ChiTiet == null || p.ChiTiet.Count == 0) throw new Exception("Phieu kho phai co it nhat mot vat tu.");
            if (p.LoaiPhieu != "NHAP" && p.LoaiPhieu != "XUAT") throw new Exception("Loai phieu khong hop le.");
            using (var c = Database.GetConnection())
            {
                c.Open(); using (MySqlTransaction trans = c.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        decimal tong = 0; foreach (var ct in p.ChiTiet) { if (ct.SoLuong <= 0 || ct.DonGia < 0) throw new Exception("So luong phai > 0 va don gia khong am."); tong += ct.SoLuong * ct.DonGia; }
                        int maPhieu;
                        using (var cmd = new MySqlCommand("INSERT INTO PhieuKho(LoaiPhieu,MaNCC,TongTien) VALUES(@loai,@ncc,@tong); SELECT LAST_INSERT_ID();", c, trans))
                        { cmd.Parameters.Add("@loai",MySqlDbType.VarChar,10).Value=p.LoaiPhieu; cmd.Parameters.Add("@ncc",MySqlDbType.Int32).Value=p.MaNCC.HasValue?(object)p.MaNCC.Value:DBNull.Value; var t=cmd.Parameters.Add("@tong",MySqlDbType.Decimal);t.Precision=18;t.Scale=2;t.Value=tong; maPhieu=(int)cmd.ExecuteScalar(); }
                        foreach (var ct in p.ChiTiet)
                        {
                            using (var cmd = new MySqlCommand("INSERT INTO ChiTietPhieuKho(MaPhieu,MaVatTu,SoLuong,DonGia) VALUES(@phieu,@vatTu,@sl,@gia)",c,trans))
                            { cmd.Parameters.Add("@phieu",MySqlDbType.Int32).Value=maPhieu;cmd.Parameters.Add("@vatTu",MySqlDbType.VarChar,20).Value=ct.MaVatTu;var sl=cmd.Parameters.Add("@sl",MySqlDbType.Decimal);sl.Precision=18;sl.Scale=2;sl.Value=ct.SoLuong;var gia=cmd.Parameters.Add("@gia",MySqlDbType.Decimal);gia.Precision=18;gia.Scale=2;gia.Value=ct.DonGia;cmd.ExecuteNonQuery(); }
                            string sql = p.LoaiPhieu=="NHAP" ? "UPDATE VatTu SET SoLuongTon=SoLuongTon+@sl WHERE MaVatTu=@ma" : "UPDATE VatTu SET SoLuongTon=SoLuongTon-@sl WHERE MaVatTu=@ma AND SoLuongTon>=@sl";
                            using (var cmd = new MySqlCommand(sql,c,trans)) { var sl=cmd.Parameters.Add("@sl",MySqlDbType.Decimal);sl.Precision=18;sl.Scale=2;sl.Value=ct.SoLuong;cmd.Parameters.Add("@ma",MySqlDbType.VarChar,20).Value=ct.MaVatTu;int rows=cmd.ExecuteNonQuery();if(rows==0){ if(p.LoaiPhieu=="XUAT") throw new Exception("Khong du hang trong kho hoac ma vat tu khong ton tai: "+ct.MaVatTu);throw new Exception("Khong tim thay vat tu: "+ct.MaVatTu); } }
                        }
                        trans.Commit(); return maPhieu;
                    }
                    catch { try { trans.Rollback(); } catch { } throw; }
                }
            }
        }
        public DataTable GetHistory() { using(var c=Database.GetConnection()) using(var cmd=new MySqlCommand("SELECT p.MaPhieu,p.LoaiPhieu,p.NgayLap,n.TenNCC,p.TongTien FROM PhieuKho p LEFT JOIN NhaCungCap n ON p.MaNCC=n.MaNCC ORDER BY p.MaPhieu DESC",c)) using(var da=new MySqlDataAdapter(cmd)){var dt=new DataTable();da.Fill(dt);return dt;} }
    }
}
