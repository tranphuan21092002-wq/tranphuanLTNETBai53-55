using System;
using System.Data;
using InventoryManagement.DAL;
using InventoryManagement.DTO;
namespace InventoryManagement.BUS
{
    public class InventoryBUS
    {
        private readonly VatTuDAL vatTu = new VatTuDAL();
        private readonly NhaCungCapDAL ncc = new NhaCungCapDAL();
        private readonly PhieuKhoDAL phieu = new PhieuKhoDAL();
        public DataTable GetVatTu(string q="")=>vatTu.GetAll(q);
        public void AddVatTu(VatTuDTO x){ValidateVatTu(x);vatTu.Insert(x);}
        public void UpdateVatTu(VatTuDTO x){ValidateVatTu(x);vatTu.Update(x);}
        public void DeleteVatTu(string ma){if(string.IsNullOrWhiteSpace(ma))throw new Exception("Hay nhap ma vat tu.");vatTu.Delete(ma);}
        private void ValidateVatTu(VatTuDTO x){if(x==null||string.IsNullOrWhiteSpace(x.MaVatTu)||string.IsNullOrWhiteSpace(x.TenVatTu))throw new Exception("Ma va ten vat tu khong duoc de trong.");if(x.SoLuongTon<0)throw new Exception("Ton kho khong duoc am.");}
        public DataTable GetNCC(string q="")=>ncc.GetAll(q);
        public void AddNCC(NhaCungCapDTO x){if(string.IsNullOrWhiteSpace(x.TenNCC))throw new Exception("Ten nha cung cap khong duoc de trong.");ncc.Insert(x);}
        public void UpdateNCC(NhaCungCapDTO x){if(x.MaNCC<=0||string.IsNullOrWhiteSpace(x.TenNCC))throw new Exception("Hay chon nha cung cap va nhap ten.");ncc.Update(x);}
        public void DeleteNCC(int ma)=>ncc.Delete(ma);
        public DataTable GetLowStock(decimal m)=>vatTu.GetLowStock(m);
        public decimal GetInventoryValue()=>vatTu.GetInventoryValue();
        public DataTable GetHistory()=>phieu.GetHistory();
        public int SaveVoucher(PhieuKhoDTO p)=>phieu.Save(p);
    }
}
