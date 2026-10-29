
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using InventoryManagement.BUS;
using InventoryManagement.DTO;

namespace InventoryManagement.GUI
{
    public class MainForm : Form
    {
        private readonly InventoryBUS bus = new InventoryBUS();

        private DataGridView gvVatTu, gvNcc, gvChiTiet, gvCanhBao, gvLichSu;
        private TextBox txtMaVT, txtTenVT, txtDVT, txtTimVT;
        private TextBox txtTenNCC, txtDienThoai, txtDiaChi, txtTimNCC;
        private NumericUpDown numTon, numNguong, numSoLuong, numDonGia;
        private ComboBox cboLoaiPhieu, cboNCC, cboVatTu;
        private Label lblTongGiaTri, lblStatus;

        private int selectedNcc = 0;
        private DataTable chiTiet;

        public MainForm()
        {
            Text = "He thong quan ly nhap - xuat kho";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1180, 760);
            MinimumSize = new Size(1000, 650);
            Font = new Font("Segoe UI", 9F);

            BuildUI();
            Load += (s, e) => RunSafe(RefreshAll);
        }

        private void BuildUI()
        {
            var title = new Label
            {
                Text = "QUAN LY QUY TRINH NHAP - XUAT KHO",
                Dock = DockStyle.Top,
                Height = 48,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                BackColor = Color.FromArgb(31, 78, 121),
                ForeColor = Color.White
            };
            Controls.Add(title);

            var tabs = new TabControl { Dock = DockStyle.Fill };
            Controls.Add(tabs);
            tabs.BringToFront();

            var tVatTu = new TabPage("Vat tu");
            var tNcc = new TabPage("Nha cung cap");
            var tPhieu = new TabPage("Lap phieu kho");
            var tBaoCao = new TabPage("Canh bao & bao cao");
            var tLichSu = new TabPage("Lich su phieu");

            tabs.TabPages.AddRange(new[]
            {
                tVatTu, tNcc, tPhieu, tBaoCao, tLichSu
            });

            // TAB VAT TU
            var pvt = new Panel
            {
                Dock = DockStyle.Top,
                Height = 142,
                Padding = new Padding(10)
            };
            tVatTu.Controls.Add(pvt);

            txtMaVT = AddText(pvt, "Ma vat tu", 10, 12, 140);
            txtTenVT = AddText(pvt, "Ten vat tu", 260, 12, 200);
            txtDVT = AddText(pvt, "Don vi tinh", 590, 12, 130);
            numTon = AddNum(pvt, "So luong ton", 820, 12, 130, 0, 100000000);

            txtTimVT = AddText(pvt, "Tim ma/ten", 10, 62, 180);

            var bTim = Btn("Tim kiem", 200, 60);
            bTim.Click += (s, e) => RunSafe(() =>
                gvVatTu.DataSource = bus.GetVatTu(txtTimVT.Text.Trim()));
            pvt.Controls.Add(bTim);

            var bThem = Btn("Them", 310, 60);
            bThem.Click += (s, e) => RunSafe(() =>
            {
                bus.AddVatTu(ReadVatTu());
                RefreshAll();
                MessageBox.Show("Da them vat tu.");
            });
            pvt.Controls.Add(bThem);

            var bSua = Btn("Sua", 390, 60);
            bSua.Click += (s, e) => RunSafe(() =>
            {
                bus.UpdateVatTu(ReadVatTu());
                RefreshAll();
                MessageBox.Show("Da cap nhat vat tu.");
            });
            pvt.Controls.Add(bSua);

            var bXoa = Btn("Xoa", 460, 60);
            bXoa.Click += (s, e) => RunSafe(() =>
            {
                if (MessageBox.Show("Xoa vat tu nay?", "Xac nhan",
                    MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;

                bus.DeleteVatTu(txtMaVT.Text.Trim());
                RefreshAll();
            });
            pvt.Controls.Add(bXoa);

            var bMoi = Btn("Lam moi", 530, 60);
            bMoi.Click += (s, e) =>
            {
                txtMaVT.Clear();
                txtTenVT.Clear();
                txtDVT.Clear();
                numTon.Value = 0;
            };
            pvt.Controls.Add(bMoi);

            gvVatTu = Grid();
            gvVatTu.Dock = DockStyle.Fill;
            tVatTu.Controls.Add(gvVatTu);

            gvVatTu.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;

                var r = gvVatTu.Rows[e.RowIndex];
                txtMaVT.Text = Val(r, "MaVatTu");
                txtTenVT.Text = Val(r, "TenVatTu");
                txtDVT.Text = Val(r, "DVT");
                numTon.Value = SafeDecimal(Val(r, "SoLuongTon"));
            };

            // TAB NHA CUNG CAP
            var pn = new Panel
            {
                Dock = DockStyle.Top,
                Height = 145,
                Padding = new Padding(10)
            };
            tNcc.Controls.Add(pn);

            txtTenNCC = AddText(pn, "Ten nha cung cap", 10, 12, 210);
            txtDienThoai = AddText(pn, "Dien thoai", 330, 12, 150);
            txtDiaChi = AddText(pn, "Dia chi", 560, 12, 230);
            txtTimNCC = AddText(pn, "Tim kiem", 10, 62, 180);

            var bnTim = Btn("Tim", 200, 60);
            bnTim.Click += (s, e) => RunSafe(() =>
                gvNcc.DataSource = bus.GetNCC(txtTimNCC.Text.Trim()));
            pn.Controls.Add(bnTim);

            var bnThem = Btn("Them", 270, 60);
            bnThem.Click += (s, e) => RunSafe(() =>
            {
                bus.AddNCC(ReadNcc());
                RefreshAll();
                MessageBox.Show("Da them nha cung cap.");
            });
            pn.Controls.Add(bnThem);

            var bnSua = Btn("Sua", 350, 60);
            bnSua.Click += (s, e) => RunSafe(() =>
            {
                var x = ReadNcc();
                x.MaNCC = selectedNcc;
                bus.UpdateNCC(x);
                RefreshAll();
                MessageBox.Show("Da cap nhat nha cung cap.");
            });
            pn.Controls.Add(bnSua);

            var bnXoa = Btn("Xoa", 420, 60);
            bnXoa.Click += (s, e) => RunSafe(() =>
            {
                if (selectedNcc <= 0)
                    throw new Exception("Hay chon nha cung cap trong bang.");

                if (MessageBox.Show("Xoa nha cung cap nay?", "Xac nhan",
                    MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;

                bus.DeleteNCC(selectedNcc);
                selectedNcc = 0;
                RefreshAll();
            });
            pn.Controls.Add(bnXoa);

            gvNcc = Grid();
            gvNcc.Dock = DockStyle.Fill;
            tNcc.Controls.Add(gvNcc);

            gvNcc.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;

                var r = gvNcc.Rows[e.RowIndex];
                selectedNcc = Convert.ToInt32(r.Cells["MaNCC"].Value);
                txtTenNCC.Text = Val(r, "TenNCC");
                txtDienThoai.Text = Val(r, "DienThoai");
                txtDiaChi.Text = Val(r, "DiaChi");
            };

            // TAB LAP PHIEU KHO
            var pp = new Panel
            {
                Dock = DockStyle.Top,
                Height = 205,
                Padding = new Padding(10)
            };
            tPhieu.Controls.Add(pp);

            cboLoaiPhieu = new ComboBox
            {
                Location = new Point(130, 15),
                Width = 130,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboLoaiPhieu.Items.AddRange(new object[] { "NHAP", "XUAT" });
            cboLoaiPhieu.SelectedIndex = 0;

            pp.Controls.Add(new Label
            {
                Text = "Loai phieu",
                Location = new Point(10, 18),
                AutoSize = true
            });
            pp.Controls.Add(cboLoaiPhieu);

            cboNCC = new ComboBox
            {
                Location = new Point(390, 15),
                Width = 270,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            pp.Controls.Add(new Label
            {
                Text = "Nha cung cap (phieu nhap)",
                Location = new Point(260, 18),
                AutoSize = true
            });
            pp.Controls.Add(cboNCC);

            cboVatTu = new ComboBox
            {
                Location = new Point(130, 58),
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            pp.Controls.Add(new Label
            {
                Text = "Vat tu",
                Location = new Point(10, 62),
                AutoSize = true
            });
            pp.Controls.Add(cboVatTu);

            numSoLuong = AddNum(pp, "So luong", 370, 55, 110, 0.01m, 100000000);
            numSoLuong.DecimalPlaces = 2;
            numSoLuong.Value = 1;

            numDonGia = AddNum(pp, "Don gia", 550, 55, 120, 0, 1000000000);
            numDonGia.DecimalPlaces = 2;
            numDonGia.Increment = 1000;

            var bThemCT = Btn("Them vao phieu", 710, 55);
            bThemCT.Click += (s, e) => RunSafe(AddDetail);
            pp.Controls.Add(bThemCT);

            var bXoaCT = Btn("Xoa dong", 830, 55);
            bXoaCT.Click += (s, e) =>
            {
                if (gvChiTiet.CurrentRow != null &&
                    gvChiTiet.CurrentRow.Index >= 0)
                    chiTiet.Rows.RemoveAt(gvChiTiet.CurrentRow.Index);
            };
            pp.Controls.Add(bXoaCT);

            var bLuu = Btn("LUU PHIEU KHO", 710, 100);
            bLuu.Size = new Size(180, 38);
            bLuu.Click += (s, e) => RunSafe(SaveVoucher);
            pp.Controls.Add(bLuu);

            pp.Controls.Add(new Label
            {
                Text = "Nhap: cong ton kho | Xuat: tru ton kho, khong cho phep ton am",
                Location = new Point(10, 150),
                AutoSize = true,
                ForeColor = Color.DarkSlateGray
            });

            chiTiet = new DataTable();
            chiTiet.Columns.Add("MaVatTu", typeof(string));
            chiTiet.Columns.Add("TenVatTu", typeof(string));
            chiTiet.Columns.Add("SoLuong", typeof(decimal));
            chiTiet.Columns.Add("DonGia", typeof(decimal));
            chiTiet.Columns.Add("ThanhTien", typeof(decimal));

            gvChiTiet = Grid();
            gvChiTiet.Dock = DockStyle.Fill;
            gvChiTiet.DataSource = chiTiet;
            tPhieu.Controls.Add(gvChiTiet);

            cboLoaiPhieu.SelectedIndexChanged += (s, e) =>
            {
                cboNCC.Enabled = cboLoaiPhieu.SelectedItem != null &&
                    cboLoaiPhieu.SelectedItem.ToString() == "NHAP";
            };

            // TAB BAO CAO
            var pb = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                Padding = new Padding(10)
            };
            tBaoCao.Controls.Add(pb);

            numNguong = AddNum(pb, "Nguong ton an toan", 10, 10, 120, 0, 100000000);
            numNguong.DecimalPlaces = 2;
            numNguong.Value = 20;

            var bLoc = Btn("Loc vat tu ton thap", 200, 10);
            bLoc.Click += (s, e) => RunSafe(() =>
                gvCanhBao.DataSource = bus.GetLowStock(numNguong.Value));
            pb.Controls.Add(bLoc);

            var bGiaTri = Btn("Tinh tong gia tri ton kho", 370, 10);
            bGiaTri.Click += (s, e) => RunSafe(() =>
                lblTongGiaTri.Text = "Tong gia tri ton kho: " +
                bus.GetInventoryValue().ToString("N2") + " VND");
            pb.Controls.Add(bGiaTri);

            lblTongGiaTri = new Label
            {
                Text = "Tong gia tri ton kho: chua tinh",
                Location = new Point(620, 16),
                AutoSize = true
            };
            pb.Controls.Add(lblTongGiaTri);

            gvCanhBao = Grid();
            gvCanhBao.Dock = DockStyle.Fill;
            tBaoCao.Controls.Add(gvCanhBao);

            // TAB LICH SU
            gvLichSu = Grid();
            gvLichSu.Dock = DockStyle.Fill;
            tLichSu.Controls.Add(gvLichSu);

            var pl = new Panel { Dock = DockStyle.Top, Height = 45 };
            var bl = Btn("Lam moi lich su", 10, 5);
            bl.Click += (s, e) => RunSafe(() =>
                gvLichSu.DataSource = bus.GetHistory());
            pl.Controls.Add(bl);
            tLichSu.Controls.Add(pl);
            pl.BringToFront();

            lblStatus = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 26,
                Text = "San sang",
                Padding = new Padding(8),
                BackColor = Color.FromArgb(235, 242, 248)
            };
            Controls.Add(lblStatus);
            lblStatus.BringToFront();
        }

        private TextBox AddText(Control p, string label, int x, int y, int w)
        {
            p.Controls.Add(new Label
            {
                Text = label,
                Location = new Point(x, y + 4),
                AutoSize = true
            });

            var t = new TextBox
            {
                Location = new Point(x, y + 23),
                Width = w
            };

            p.Controls.Add(t);
            return t;
        }

        private NumericUpDown AddNum(
            Control p, string label, int x, int y,
            int w, decimal min, decimal max)
        {
            p.Controls.Add(new Label
            {
                Text = label,
                Location = new Point(x, y + 2),
                AutoSize = true
            });

            var n = new NumericUpDown
            {
                Location = new Point(x, y + 22),
                Width = w,
                Minimum = min,
                Maximum = max,
                ThousandsSeparator = true
            };

            p.Controls.Add(n);
            return n;
        }

        private Button Btn(string text, int x, int y)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Height = 30,
                BackColor = Color.FromArgb(221, 235, 247),
                FlatStyle = FlatStyle.Flat
            };
        }

        private DataGridView Grid()
        {
            return new DataGridView
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White
            };
        }

        private string Val(DataGridViewRow r, string col)
        {
            return r.Cells[col].Value == null
                ? ""
                : Convert.ToString(r.Cells[col].Value);
        }

        private decimal SafeDecimal(string s)
        {
            decimal x;
            return decimal.TryParse(s, out x)
                ? Math.Max(0, Math.Min(100000000, x))
                : 0;
        }

        private VatTuDTO ReadVatTu()
        {
            return new VatTuDTO
            {
                MaVatTu = txtMaVT.Text.Trim(),
                TenVatTu = txtTenVT.Text.Trim(),
                DVT = txtDVT.Text.Trim(),
                SoLuongTon = numTon.Value
            };
        }

        private NhaCungCapDTO ReadNcc()
        {
            return new NhaCungCapDTO
            {
                TenNCC = txtTenNCC.Text.Trim(),
                DienThoai = txtDienThoai.Text.Trim(),
                DiaChi = txtDiaChi.Text.Trim()
            };
        }

        // Tai du lieu tung phan de mot loi khong chan cac bang khac.
        private void RefreshAll()
        {
            string loi = "";

            try
            {
                gvVatTu.DataSource = bus.GetVatTu();
            }
            catch (Exception ex)
            {
                loi += "Vat tu: " + ex.Message + "\n";
            }

            try
            {
                gvNcc.DataSource = bus.GetNCC();
            }
            catch (Exception ex)
            {
                loi += "Nha cung cap: " + ex.Message + "\n";
            }

            try
            {
                cboVatTu.DataSource = bus.GetVatTu();
                cboVatTu.DisplayMember = "TenVatTu";
                cboVatTu.ValueMember = "MaVatTu";
            }
            catch (Exception ex)
            {
                loi += "Combo vat tu: " + ex.Message + "\n";
            }

            try
            {
                cboNCC.DataSource = bus.GetNCC();
                cboNCC.DisplayMember = "TenNCC";
                cboNCC.ValueMember = "MaNCC";
            }
            catch (Exception ex)
            {
                loi += "Combo nha cung cap: " + ex.Message + "\n";
            }

            try
            {
                gvLichSu.DataSource = bus.GetHistory();
            }
            catch (Exception ex)
            {
                loi += "Lich su phieu: " + ex.Message + "\n";
            }

            lblStatus.Text = string.IsNullOrEmpty(loi)
                ? "Da tai du lieu InventoryDB"
                : "Co loi khi tai du lieu";

            if (!string.IsNullOrEmpty(loi))
            {
                MessageBox.Show(loi, "Chi tiet loi tai du lieu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AddDetail()
        {
            if (cboVatTu.SelectedValue == null)
                throw new Exception("Hay chon vat tu.");

            string ma = Convert.ToString(cboVatTu.SelectedValue);
            string ten = Convert.ToString(
                ((DataRowView)cboVatTu.SelectedItem)["TenVatTu"]);

            if (numSoLuong.Value <= 0)
                throw new Exception("So luong phai lon hon 0.");

            foreach (DataRow r in chiTiet.Rows)
            {
                if (Convert.ToString(r["MaVatTu"]) == ma)
                    throw new Exception(
                        "Moi vat tu chi duoc them mot dong trong mot phieu.");
            }

            decimal sl = numSoLuong.Value;
            decimal gia = numDonGia.Value;

            chiTiet.Rows.Add(ma, ten, sl, gia, sl * gia);
        }

        private void SaveVoucher()
        {
            if (chiTiet.Rows.Count == 0)
                throw new Exception("Hay them it nhat mot vat tu vao phieu.");

            var p = new PhieuKhoDTO
            {
                LoaiPhieu = cboLoaiPhieu.SelectedItem.ToString(),
                MaNCC = GetNcc()
            };

            foreach (DataRow r in chiTiet.Rows)
            {
                p.ChiTiet.Add(new ChiTietPhieuDTO
                {
                    MaVatTu = Convert.ToString(r["MaVatTu"]),
                    SoLuong = Convert.ToDecimal(r["SoLuong"]),
                    DonGia = Convert.ToDecimal(r["DonGia"])
                });
            }

            int id = bus.SaveVoucher(p);
            chiTiet.Rows.Clear();
            RefreshAll();

            MessageBox.Show("Luu phieu kho thanh cong. Ma phieu: " + id,
                "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private int? GetNcc()
        {
            if (cboLoaiPhieu.SelectedItem.ToString() == "XUAT")
                return null;

            if (cboNCC.SelectedValue == null)
                throw new Exception("Phieu nhap can chon nha cung cap.");

            return Convert.ToInt32(cboNCC.SelectedValue);
        }

        private void RunSafe(Action a)
        {
            try
            {
                a();
            }
            catch (Exception ex)
            {
                if (lblStatus != null)
                    lblStatus.Text = "Co loi";

                MessageBox.Show(ex.ToString(), "Loi InventoryDB",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
