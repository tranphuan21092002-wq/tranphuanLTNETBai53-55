
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace CafePOS
{
    public class Form1 : Form
    {
        FlowLayoutPanel tables;
        ComboBox cats, dishes;
        NumericUpDown qty;
        DataGridView grid;
        Label chosen, total;

        DataTable menu, catTable, tableTable, order;

        int tableId = 0;
        int invoiceId = 0;
        string tableName = "";

        public Form1()
        {
            Text = "Cafe POS System";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(850, 550);

            Build();

            if (Database.TestConnection())
                Shown += (s, e) => LoadAll();
        }

        // TAO GIAO DIEN
        void Build()
        {
            SuspendLayout();

            Panel head = new Panel
            {
                Dock = DockStyle.Top,
                Height = 58,
                BackColor = Color.DarkSlateGray
            };

            head.Controls.Add(new Label
            {
                Text = "CAFE POS SYSTEM",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Left = 18,
                Top = 10
            });

            Panel leftPanel = new Panel();
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Width = 330;
            leftPanel.Padding = new Padding(5);

            Panel rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Padding = new Padding(5);

            Controls.Add(rightPanel);
            Controls.Add(leftPanel);
            Controls.Add(head);
            head.BringToFront();

            // COT SO DO BAN
            leftPanel.Padding = new Padding(8);

            Label lblTables = new Label
            {
                Text = "SO DO BAN",
                Dock = DockStyle.Top,
                Height = 32,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            tables = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(4)
            };

            leftPanel.Controls.Add(tables);
            leftPanel.Controls.Add(lblTables);

            // COT GOI MON
            Panel right = rightPanel;
            right.Padding = new Padding(10);

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Margin = new Padding(0)
            };

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 35));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 42));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 48));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 90));

            right.Controls.Add(layout);

            // BAN DANG CHON
            chosen = new Label
            {
                Text = "Chua chon ban",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };

            layout.Controls.Add(chosen, 0, 0);

            // DANH MUC
            FlowLayoutPanel categoryPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 5, 0, 0)
            };

            categoryPanel.Controls.Add(new Label
            {
                Text = "Danh muc:",
                Width = 80,
                Height = 28,
                TextAlign = ContentAlignment.MiddleLeft
            });

            cats = new ComboBox
            {
                Width = 220,
                Height = 28,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cats.SelectedIndexChanged += (s, e) => FilterDishes();
            categoryPanel.Controls.Add(cats);

            layout.Controls.Add(categoryPanel, 0, 1);

            // CHON MON VA SO LUONG
            FlowLayoutPanel dishPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 5, 0, 0)
            };

            dishPanel.Controls.Add(new Label
            {
                Text = "Mon an:",
                Width = 65,
                Height = 28,
                TextAlign = ContentAlignment.MiddleLeft
            });

            dishes = new ComboBox
            {
                Width = 180,
                Height = 28,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            dishPanel.Controls.Add(dishes);

            dishPanel.Controls.Add(new Label
            {
                Text = "SL:",
                Width = 28,
                Height = 28,
                TextAlign = ContentAlignment.MiddleCenter
            });

            qty = new NumericUpDown
            {
                Width = 60,
                Minimum = 1,
                Maximum = 99,
                Value = 1
            };

            dishPanel.Controls.Add(qty);

            Button add = Btn(
                "Them mon", 0, 0, 100, Color.RoyalBlue);

            add.Margin = new Padding(5, 0, 0, 0);
            add.Click += (s, e) => AddDish();

            dishPanel.Controls.Add(add);

            layout.Controls.Add(dishPanel, 0, 2);

            // BANG HOA DON
            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                Margin = new Padding(0, 5, 0, 5)
            };

            layout.Controls.Add(grid, 0, 3);

            // TONG TIEN VA NUT CHUC NANG
            TableLayoutPanel bottom = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Margin = new Padding(0)
            };

            bottom.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            bottom.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 220));

            bottom.RowStyles.Add(
                new RowStyle(SizeType.Percent, 50));

            bottom.RowStyles.Add(
                new RowStyle(SizeType.Percent, 50));

            total = new Label
            {
                Text = "Tong tien: 0 VND",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };

            bottom.Controls.Add(total, 0, 0);
            bottom.SetRowSpan(total, 2);

            FlowLayoutPanel buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                AutoScroll = true
            };

            Button pay = Btn(
                "Thanh toan", 0, 0, 105, Color.ForestGreen);

            pay.Click += (s, e) => Pay();

            Button refresh = Btn(
                "Lam moi", 0, 0, 95, Color.DimGray);

            refresh.Click += (s, e) => LoadAll();

            buttons.Controls.Add(pay);
            buttons.Controls.Add(refresh);

            bottom.Controls.Add(buttons, 1, 0);
            bottom.SetRowSpan(buttons, 2);

            layout.Controls.Add(bottom, 0, 4);

            ResumeLayout(true);
        }

        // HAM TAO NUT
        Button Btn(string text, int x, int y, int w, Color color)
        {
            return new Button
            {
                Text = text,
                Left = x,
                Top = y,
                Width = w,
                Height = 32,
                BackColor = color,
                ForeColor = Color.White,
                UseVisualStyleBackColor = false,
                Margin = new Padding(3)
            };
        }

        // TAI DU LIEU
        void LoadAll()
        {
            try
            {
                using (MySqlConnection c = Database.GetConnection())
                {
                    c.Open();

                    catTable = new DataTable();

                    using (MySqlDataAdapter adapter =
                        new MySqlDataAdapter(
                            "SELECT MaDanhMuc, TenDanhMuc " +
                            "FROM DanhMuc ORDER BY TenDanhMuc", c))
                    {
                        adapter.Fill(catTable);
                    }

                    cats.DataSource = null;
                    cats.DisplayMember = "TenDanhMuc";
                    cats.ValueMember = "MaDanhMuc";
                    cats.DataSource = catTable;

                    menu = new DataTable();

                    using (MySqlDataAdapter adapter =
                        new MySqlDataAdapter(
                            "SELECT MaMon, TenMon, MaDanhMuc, DonGia " +
                            "FROM MonAn WHERE TrangThai = 1 " +
                            "ORDER BY TenMon", c))
                    {
                        adapter.Fill(menu);
                    }

                    tableTable = new DataTable();

                    using (MySqlDataAdapter adapter =
                        new MySqlDataAdapter(
                            "SELECT MaBan, TenBan, TrangThai " +
                            "FROM BanAn ORDER BY MaBan", c))
                    {
                        adapter.Fill(tableTable);
                    }
                }

                FilterDishes();
                RenderTables();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Khong tai duoc du lieu MySQL:\n" + ex.Message,
                    "Loi ket noi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // LOC MON THEO DANH MUC
        void FilterDishes()
        {
            if (menu == null || cats.SelectedValue == null)
                return;

            int id;

            if (!int.TryParse(cats.SelectedValue.ToString(), out id))
                return;

            DataView view = new DataView(menu);
            view.RowFilter = "MaDanhMuc = " + id;

            dishes.DataSource = view;
            dishes.DisplayMember = "TenMon";
            dishes.ValueMember = "MaMon";
        }

        // HIEN THI SO DO BAN
        void RenderTables()
        {
            if (tableTable == null)
                return;

            tables.Controls.Clear();

            foreach (DataRow r in tableTable.Rows)
            {
                int id = Convert.ToInt32(r["MaBan"]);
                string name = r["TenBan"].ToString();
                string status = r["TrangThai"].ToString();

                bool empty = status == "Trong" ||
                             status == "Trống";

                Button b = new Button
                {
                    Text = name + "\n" +
                           (empty ? "Trong" : "Co khach"),
                    Tag = id,
                    Width = 130,
                    Height = 72,
                    BackColor = empty
                        ? Color.ForestGreen
                        : Color.Firebrick,
                    ForeColor = Color.White,
                    Margin = new Padding(5)
                };

                b.Click += (s, e) => SelectTable(id, name);
                tables.Controls.Add(b);
            }
        }

        // CHON BAN VA TAI HOA DON CHUA THANH TOAN
        void SelectTable(int id, string name)
        {
            tableId = id;
            tableName = name;
            chosen.Text = "Ban dang chon: " + name;
            invoiceId = 0;

            order = new DataTable();

            try
            {
                using (MySqlConnection c = Database.GetConnection())
                {
                    c.Open();

                    using (MySqlCommand cmd = new MySqlCommand(
                        "SELECT MaHoaDon FROM HoaDon " +
                        "WHERE MaBan = @b " +
                        "AND TrangThai = 'Chua thanh toan' " +
                        "ORDER BY MaHoaDon DESC LIMIT 1", c))
                    {
                        cmd.Parameters.AddWithValue("@b", id);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                            invoiceId = Convert.ToInt32(result);
                    }

                    if (invoiceId > 0)
                    {
                        string sql =
                            "SELECT m.TenMon, c.SoLuong, c.DonGia, " +
                            "(c.SoLuong * c.DonGia) AS ThanhTien " +
                            "FROM ChiTietHoaDon c " +
                            "JOIN MonAn m ON c.MaMon = m.MaMon " +
                            "WHERE c.MaHoaDon = @id";

                        using (MySqlDataAdapter adapter =
                            new MySqlDataAdapter(sql, c))
                        {
                            adapter.SelectCommand.Parameters.AddWithValue(
                                "@id", invoiceId);

                            adapter.Fill(order);
                        }
                    }
                    else
                    {
                        order.Columns.Add("TenMon", typeof(string));
                        order.Columns.Add("SoLuong", typeof(int));
                        order.Columns.Add("DonGia", typeof(decimal));
                        order.Columns.Add("ThanhTien", typeof(decimal));
                    }
                }

                grid.DataSource = order;
                Sum();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi chon ban:\n" + ex.Message);
            }
        }

        // THEM MON VAO HOA DON
        void AddDish()
        {
            if (tableId == 0 || dishes.SelectedValue == null)
            {
                MessageBox.Show("Hay chon ban va mon truoc.");
                return;
            }

            int dishId;

            if (!int.TryParse(dishes.SelectedValue.ToString(), out dishId))
            {
                MessageBox.Show("Mon an khong hop le.");
                return;
            }

            try
            {
                using (MySqlConnection c = Database.GetConnection())
                {
                    c.Open();

                    using (MySqlTransaction tx = c.BeginTransaction())
                    {
                        try
                        {
                            if (invoiceId == 0)
                            {
                                string sql =
                                    "INSERT INTO HoaDon " +
                                    "(MaBan, TongTien, TrangThai) " +
                                    "VALUES (@b, 0, 'Chua thanh toan')";

                                using (MySqlCommand cmd =
                                    new MySqlCommand(sql, c, tx))
                                {
                                    cmd.Parameters.AddWithValue("@b", tableId);
                                    cmd.ExecuteNonQuery();
                                }

                                using (MySqlCommand cmd =
                                    new MySqlCommand("SELECT LAST_INSERT_ID()", c, tx))
                                {
                                    invoiceId = Convert.ToInt32(cmd.ExecuteScalar());
                                }
                            }

                            decimal price;

                            using (MySqlCommand cmd = new MySqlCommand(
                                "SELECT DonGia FROM MonAn " +
                                "WHERE MaMon = @m AND TrangThai = 1",
                                c, tx))
                            {
                                cmd.Parameters.AddWithValue("@m", dishId);

                                object result = cmd.ExecuteScalar();

                                if (result == null || result == DBNull.Value)
                                    throw new Exception(
                                        "Mon khong ton tai hoac da ngung ban.");

                                price = Convert.ToDecimal(result);
                            }

                            int quantity = (int)qty.Value;
                            object existing;

                            using (MySqlCommand check = new MySqlCommand(
                                "SELECT MaChiTiet FROM ChiTietHoaDon " +
                                "WHERE MaHoaDon = @i AND MaMon = @m LIMIT 1",
                                c, tx))
                            {
                                check.Parameters.AddWithValue("@i", invoiceId);
                                check.Parameters.AddWithValue("@m", dishId);
                                existing = check.ExecuteScalar();
                            }

                            if (existing != null && existing != DBNull.Value)
                            {
                                using (MySqlCommand update = new MySqlCommand(
                                    "UPDATE ChiTietHoaDon " +
                                    "SET SoLuong = SoLuong + @q " +
                                    "WHERE MaHoaDon = @i AND MaMon = @m",
                                    c, tx))
                                {
                                    update.Parameters.AddWithValue("@q", quantity);
                                    update.Parameters.AddWithValue("@i", invoiceId);
                                    update.Parameters.AddWithValue("@m", dishId);
                                    update.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                using (MySqlCommand insert = new MySqlCommand(
                                    "INSERT INTO ChiTietHoaDon " +
                                    "(MaHoaDon, MaMon, SoLuong, DonGia) " +
                                    "VALUES (@i, @m, @q, @p)",
                                    c, tx))
                                {
                                    insert.Parameters.AddWithValue("@i", invoiceId);
                                    insert.Parameters.AddWithValue("@m", dishId);
                                    insert.Parameters.AddWithValue("@q", quantity);
                                    insert.Parameters.AddWithValue("@p", price);
                                    insert.ExecuteNonQuery();
                                }
                            }

                            using (MySqlCommand cmd = new MySqlCommand(
                                "UPDATE BanAn SET TrangThai = 'Co khach' " +
                                "WHERE MaBan = @b", c, tx))
                            {
                                cmd.Parameters.AddWithValue("@b", tableId);
                                cmd.ExecuteNonQuery();
                            }

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }

                LoadAll();
                SelectTable(tableId, tableName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi them mon:\n" + ex.Message);
            }
        }

        // TINH TONG TIEN
        void Sum()
        {
            decimal sum = 0;

            if (order != null && order.Columns.Contains("ThanhTien"))
            {
                foreach (DataRow r in order.Rows)
                {
                    if (r["ThanhTien"] != DBNull.Value)
                        sum += Convert.ToDecimal(r["ThanhTien"]);
                }
            }

            total.Text = "Tong tien: " + sum.ToString("N0") + " VND";
        }

        // THANH TOAN HOA DON
        void Pay()
        {
            if (invoiceId == 0)
            {
                MessageBox.Show("Ban chua co hoa don.");
                return;
            }

            if (MessageBox.Show(
                "Xac nhan thanh toan?",
                "Thanh toan",
                MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            try
            {
                using (MySqlConnection c = Database.GetConnection())
                {
                    c.Open();

                    using (MySqlTransaction tx = c.BeginTransaction())
                    {
                        try
                        {
                            decimal sum;

                            using (MySqlCommand cmd = new MySqlCommand(
                                "SELECT COALESCE(SUM(SoLuong * DonGia), 0) " +
                                "FROM ChiTietHoaDon WHERE MaHoaDon = @i",
                                c, tx))
                            {
                                cmd.Parameters.AddWithValue("@i", invoiceId);
                                sum = Convert.ToDecimal(cmd.ExecuteScalar());
                            }

                            using (MySqlCommand cmd = new MySqlCommand(
                                "UPDATE HoaDon SET TongTien = @s, " +
                                "TrangThai = 'Da thanh toan' " +
                                "WHERE MaHoaDon = @i", c, tx))
                            {
                                cmd.Parameters.AddWithValue("@s", sum);
                                cmd.Parameters.AddWithValue("@i", invoiceId);
                                cmd.ExecuteNonQuery();
                            }

                            using (MySqlCommand cmd = new MySqlCommand(
                                "UPDATE BanAn SET TrangThai = 'Trong' " +
                                "WHERE MaBan = @b", c, tx))
                            {
                                cmd.Parameters.AddWithValue("@b", tableId);
                                cmd.ExecuteNonQuery();
                            }

                            tx.Commit();

                            MessageBox.Show(
                                "Thanh toan thanh cong: " +
                                sum.ToString("N0") + " VND");
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }

                invoiceId = 0;
                LoadAll();
                SelectTable(tableId, tableName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi thanh toan:\n" + ex.Message);
            }
        }
    }
}
