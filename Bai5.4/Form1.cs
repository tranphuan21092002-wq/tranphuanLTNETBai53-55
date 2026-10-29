using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using HotelManagement.BUS;
using MySql.Data.MySqlClient;
using HotelManagement.DAL;

namespace HotelManagement
{
    public class Form1 : Form
    {
        private readonly HotelBUS bus = new HotelBUS();
        private DataGridView gridRooms, gridBookings;
        private ComboBox cboRoom, cboService, cboCleanRoom;
        private TextBox txtName, txtCCCD, txtPhone;
        private NumericUpDown numQuantity;
        private NumericUpDown numBookingId;
        private Label lblStatus;

        public Form1()
        {
            Text = "He thong quan ly khach san"; StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1120, 760); MinimumSize = new Size(980, 650); Font = new Font("Segoe UI", 9F);
            BuildUI(); Load += (s, e) => RefreshAll();
        }
        private void BuildUI()
        {
            var title = new Label { Text = "QUAN LY KHACH SAN", Dock = DockStyle.Top, Height = 54, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 18, FontStyle.Bold), BackColor = Color.FromArgb(31, 78, 121), ForeColor = Color.White };
            Controls.Add(title);
            var tabs = new TabControl { Dock = DockStyle.Fill, Padding = new Point(14, 5) }; Controls.Add(tabs); tabs.BringToFront();
            var tabRoom = new TabPage("Phong"); var tabCheckIn = new TabPage("Nhan phong"); var tabBookings = new TabPage("Phieu dang thue"); var tabServices = new TabPage("Dich vu & don dep");
            tabs.TabPages.AddRange(new[] { tabRoom, tabCheckIn, tabBookings, tabServices });
            gridRooms = MakeGrid(); gridRooms.Dock = DockStyle.Fill;
            var roomButtons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(8) };
            var btnRefresh = MakeButton("Lam moi danh sach phong"); btnRefresh.Click += (s,e)=>RefreshRooms(); roomButtons.Controls.Add(btnRefresh);
            roomButtons.Controls.Add(new Label { Text = "Trang thai: Trong / Dang o / Dang don dep", AutoSize = true, Padding = new Padding(10,8,0,0) });
            tabRoom.Controls.Add(gridRooms); tabRoom.Controls.Add(roomButtons);

            var checkPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(28) }; tabCheckIn.Controls.Add(checkPanel);
            int y = 24; AddField(checkPanel, "Phong trong", ref y, out cboRoom, 280); cboRoom.DropDownStyle = ComboBoxStyle.DropDownList;
            AddTextField(checkPanel, "Ho ten khach", ref y, out txtName); AddTextField(checkPanel, "CCCD", ref y, out txtCCCD); AddTextField(checkPanel, "So dien thoai", ref y, out txtPhone);
            var btnCheckIn = MakeButton("XAC NHAN NHAN PHONG"); btnCheckIn.Location = new Point(150, y + 12); btnCheckIn.Size = new Size(240, 42); btnCheckIn.Click += CheckInClick; checkPanel.Controls.Add(btnCheckIn);
            var hint = new Label { Text = "Nhap CCCD de nhan dien khach; neu chua co trong he thong, khach se duoc tao moi.", AutoSize = true, Location = new Point(28, y + 70), ForeColor = Color.DimGray }; checkPanel.Controls.Add(hint);

            var bookPanel = new Panel { Dock = DockStyle.Fill }; tabBookings.Controls.Add(bookPanel);
            gridBookings = MakeGrid(); gridBookings.Dock = DockStyle.Fill; bookPanel.Controls.Add(gridBookings);
            var bookTop = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 54, Padding = new Padding(8), WrapContents = false };
            bookTop.Controls.Add(new Label { Text = "Ma phieu:", AutoSize = true, Padding = new Padding(0,8,0,0) }); numBookingId = new NumericUpDown { Minimum = 1, Maximum = 10000000, Width = 100 }; bookTop.Controls.Add(numBookingId);
            var btnCheckout = MakeButton("Tra phong / tinh tien"); btnCheckout.Click += CheckoutClick; bookTop.Controls.Add(btnCheckout);
            var btnAddService = MakeButton("Them dich vu cho ma phieu"); btnAddService.Click += (s,e)=>{ tabs.SelectedTab = tabServices; }; bookTop.Controls.Add(btnAddService);
            var btnBookRefresh = MakeButton("Lam moi"); btnBookRefresh.Click += (s,e)=>RefreshBookings(); bookTop.Controls.Add(btnBookRefresh); bookPanel.Controls.Add(bookTop);

            var servicePanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24) }; tabServices.Controls.Add(servicePanel);
            int sy = 26; AddField(servicePanel, "Dich vu", ref sy, out cboService, 300); cboService.DropDownStyle = ComboBoxStyle.DropDownList;
            servicePanel.Controls.Add(new Label { Text = "Ma phieu dang thue", Location = new Point(28, sy), AutoSize = true });
            var serviceBookingId = new NumericUpDown { Location = new Point(220, sy - 4), Width = 150, Minimum = 1, Maximum = 10000000 }; servicePanel.Controls.Add(serviceBookingId); sy += 42;
            servicePanel.Controls.Add(new Label { Text = "So luong", Location = new Point(28, sy), AutoSize = true }); numQuantity = new NumericUpDown { Location = new Point(220, sy - 4), Width = 150, Minimum = 1, Maximum = 999, Value = 1 }; servicePanel.Controls.Add(numQuantity); sy += 42;
            var btnAdd = MakeButton("Ghi nhan dich vu"); btnAdd.Location = new Point(220, sy); btnAdd.Click += (s,e)=>{ try { if(cboService.SelectedValue==null) throw new Exception("Hay chon dich vu."); bus.AddService((int)serviceBookingId.Value, Convert.ToInt32(cboService.SelectedValue), (int)numQuantity.Value); MessageBox.Show("Da ghi nhan dich vu."); } catch(Exception ex){ ShowError(ex); } }; servicePanel.Controls.Add(btnAdd); sy += 65;
            servicePanel.Controls.Add(new Label { Text = "Danh dau phong da don dep", Location = new Point(28, sy), AutoSize = true }); sy += 28;
            cboCleanRoom = new ComboBox { Location = new Point(28, sy), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList }; servicePanel.Controls.Add(cboCleanRoom);
            var btnClean = MakeButton("Phong da sach"); btnClean.Location = new Point(265, sy - 1); btnClean.Click += (s,e)=>{ try { if(cboCleanRoom.SelectedValue==null) throw new Exception("Khong co phong can don."); bus.MarkClean(cboCleanRoom.SelectedValue.ToString()); RefreshAll(); MessageBox.Show("Da cap nhat phong ve trang thai Trong."); } catch(Exception ex){ ShowError(ex); } }; servicePanel.Controls.Add(btnClean);
            lblStatus = new Label { Dock = DockStyle.Bottom, Height = 28, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(8), BackColor = Color.FromArgb(235, 242, 248), Text = "San sang" }; Controls.Add(lblStatus); lblStatus.BringToFront();
        }
        private DataGridView MakeGrid() { return new DataGridView { ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, BackgroundColor = Color.White, Dock = DockStyle.Fill }; }
        private Button MakeButton(string text) { return new Button { Text = text, AutoSize = true, Height = 32, Padding = new Padding(8,3,8,3), BackColor = Color.FromArgb(221,235,247), FlatStyle = FlatStyle.Flat }; }
        private void AddTextField(Control parent, string label, ref int y, out TextBox box) { parent.Controls.Add(new Label { Text = label, Location = new Point(28,y+5), Width = 150 }); box = new TextBox { Location = new Point(190,y), Width = 280 }; parent.Controls.Add(box); y += 44; }
        private void AddField(Control parent, string label, ref int y, out ComboBox combo, int width) { parent.Controls.Add(new Label { Text = label, Location = new Point(28,y+5), Width = 150 }); combo = new ComboBox { Location = new Point(190,y), Width = width, DropDownStyle = ComboBoxStyle.DropDownList }; parent.Controls.Add(combo); y += 44; }
        private void RefreshAll() { try { RefreshRooms(); RefreshBookings(); RefreshDropdowns(); lblStatus.Text = "Da tai du lieu tu HotelDB"; } catch(Exception ex) { lblStatus.Text = "Khong tai duoc du lieu"; ShowError(ex); } }
        private void RefreshRooms() { gridRooms.DataSource = bus.GetRooms(); }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void RefreshBookings() { if(gridBookings != null) gridBookings.DataSource = bus.GetCurrentBookings(); }
        private void RefreshDropdowns()
        {
            cboRoom.DataSource = bus.GetAvailableRooms(); cboRoom.DisplayMember = "SoPhong"; cboRoom.ValueMember = "SoPhong";
            cboService.DataSource = bus.GetServices(); cboService.DisplayMember = "TenDV"; cboService.ValueMember = "MaDV";
            DataTable clean = bus.GetRooms(); DataView view = new DataView(clean); view.RowFilter = "[Trang thai] = 'Dang don dep'"; cboCleanRoom.DataSource = view; cboCleanRoom.DisplayMember = "So phong"; cboCleanRoom.ValueMember = "So phong";
        }
        private void CheckInClick(object sender, EventArgs e)
        {
            try { if(cboRoom.SelectedValue==null) throw new Exception("Khong co phong trong de nhan."); if(string.IsNullOrWhiteSpace(txtName.Text)||string.IsNullOrWhiteSpace(txtCCCD.Text)) throw new Exception("Hay nhap ho ten va CCCD."); bus.CheckIn(cboRoom.SelectedValue.ToString(), txtName.Text.Trim(), txtCCCD.Text.Trim(), txtPhone.Text.Trim()); MessageBox.Show("Nhan phong thanh cong."); txtName.Clear(); txtCCCD.Clear(); txtPhone.Clear(); RefreshAll(); }
            catch(Exception ex) { ShowError(ex); }
        }
        private void CheckoutClick(object sender, EventArgs e)
        {
            try { if(gridBookings.CurrentRow != null && gridBookings.Columns.Contains("Ma phieu")) numBookingId.Value = Convert.ToDecimal(gridBookings.CurrentRow.Cells["Ma phieu"].Value); if(numBookingId.Value < 1) throw new Exception("Chon phieu dang thue hoac nhap ma phieu."); decimal total = bus.CheckOut((int)numBookingId.Value); MessageBox.Show("Tra phong thanh cong.\nTong tien: " + total.ToString("N0") + " VND", "Hoa don"); RefreshAll(); }
            catch(Exception ex) { ShowError(ex); }
        }
        private void ShowError(Exception ex) { MessageBox.Show("Co loi xay ra:\n" + ex.Message, "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
}
