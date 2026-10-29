using System.Data;
using HotelManagement.DAL;

namespace HotelManagement.BUS
{
    public class HotelBUS
    {
        private readonly PhongDAL phongDAL = new PhongDAL();
        private readonly DichVuDAL dichVuDAL = new DichVuDAL();
        private readonly DatPhongDAL datPhongDAL = new DatPhongDAL();
        public DataTable GetRooms() { return phongDAL.GetAll(); }
        public DataTable GetAvailableRooms() { return phongDAL.GetAvailable(); }
        public DataTable GetServices() { return dichVuDAL.GetAll(); }
        public DataTable GetCurrentBookings() { return datPhongDAL.GetCurrentBookings(); }
        public void CheckIn(string room, string name, string cccd, string phone) { datPhongDAL.CheckIn(room, name, cccd, phone); }
        public decimal CheckOut(int bookingId) { return datPhongDAL.CheckOut(bookingId); }
        public void AddService(int bookingId, int serviceId, int quantity) { datPhongDAL.AddService(bookingId, serviceId, quantity); }
        public void MarkClean(string room) { datPhongDAL.MarkClean(room); }
    }
}
