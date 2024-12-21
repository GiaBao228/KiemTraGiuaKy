using KiemTraGiuaKy.Model;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace KiemTraGiuaKy
{
    public partial class Form1 : Form
    {
        // Kết nối đến SQL Server
        SqlConnection conn = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=QL_SINHVIEN;Integrated Security=True");

        public Form1()
        {
            InitializeComponent();
        }
        private void EnsureConnectionOpen()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDanhSachSinhVien();
            LoadDanhSachLopHoc();
            ResetControls();
        }

        // Load danh sách sinh viên vào DataGridView
        private void LoadDanhSachSinhVien()
        {
            try
            {
                EnsureConnectionOpen(); // Kiểm tra kết nối trước khi mở
                SqlDataAdapter da = new SqlDataAdapter("SELECT MSSV, HoTenSV, CONVERT(VARCHAR(10), NgaySinh, 103) AS NgaySinh, MaLop FROM Sinhvien", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvSinhVien.DataSource = dt; // dgvSinhVien là tên DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                conn.Close(); // Đóng kết nối
            }
        }

        // Load danh sách lớp học vào ComboBox
        private void LoadDanhSachLopHoc()
        {
            cmbLophoc.Items.Clear();
            try
            {
                EnsureConnectionOpen(); // Kiểm tra kết nối trước khi mở
                SqlCommand cmd = new SqlCommand("SELECT * FROM Lop", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cmbLophoc.Items.Add(reader["MaLop"].ToString());
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                conn.Close(); // Đóng kết nối
            }
        }

        // Reset các controls
        private void ResetControls()
        {
            txtMasv.Clear();
            txtHoten.Clear();
            dtNgaysinh.Value = DateTime.Now;
            cmbLophoc.SelectedIndex = -1;
            txtMasv.Enabled = true;
        }

        // Thêm mới sinh viên
        private void btnThem_Click(object sender, EventArgs e)
        {
            ResetControls();
            txtMasv.Focus();
        }

        // Lưu sinh viên
        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Sinhvien (MSSV, HoTenSV, NgaySinh, MaLop) VALUES (@MSSV, @HoTenSV, @NgaySinh, @MaLop)", conn);
                cmd.Parameters.AddWithValue("@MSSV", txtMasv.Text);
                cmd.Parameters.AddWithValue("@HoTenSV", txtHoten.Text);
                cmd.Parameters.AddWithValue("@NgaySinh", dtNgaysinh.Value);
                cmd.Parameters.AddWithValue("@MaLop", cmbLophoc.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Thêm sinh viên thành công!");
                LoadDanhSachSinhVien();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        // Sửa thông tin sinh viên
        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Sinhvien SET HoTenSV=@HoTenSV, NgaySinh=@NgaySinh, MaLop=@MaLop WHERE MSSV=@MSSV", conn);
                cmd.Parameters.AddWithValue("@MSSV", txtMasv.Text);
                cmd.Parameters.AddWithValue("@HoTenSV", txtHoten.Text);
                cmd.Parameters.AddWithValue("@NgaySinh", dtNgaysinh.Value);
                cmd.Parameters.AddWithValue("@MaLop", cmbLophoc.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Cập nhật sinh viên thành công!");
                LoadDanhSachSinhVien();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        // Xóa sinh viên
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Sinhvien WHERE MSSV=@MSSV", conn);
                    cmd.Parameters.AddWithValue("@MSSV", txtMasv.Text);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Xóa sinh viên thành công!");
                    LoadDanhSachSinhVien();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        // Tìm kiếm sinh viên
        private void btnTim_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Sinhvien WHERE MSSV LIKE @Keyword OR HoTenSV LIKE @Keyword", conn);
                da.SelectCommand.Parameters.AddWithValue("@Keyword", "%" + txtTim.Text + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvSinhVien.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        // Xử lý chọn dòng trong DataGridView
        private void dgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSinhVien.Rows[e.RowIndex];
                txtMasv.Text = row.Cells["MSSV"].Value.ToString();
                txtHoten.Text = row.Cells["HoTenSV"].Value.ToString();
                dtNgaysinh.Value = Convert.ToDateTime(row.Cells["NgaySinh"].Value);
                cmbLophoc.Text = row.Cells["MaLop"].Value.ToString();
            }
        }

        // Thoát ứng dụng
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvSinhVien_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSinhVien.Rows[e.RowIndex];
                txtMasv.Text = row.Cells["MSSV"].Value.ToString();
                txtHoten.Text = row.Cells["HoTenSV"].Value.ToString();
                dtNgaysinh.Value = Convert.ToDateTime(row.Cells["NgaySinh"].Value);
                cmbLophoc.Text = row.Cells["MaLop"].Value.ToString();

                btnSua.Enabled = true;
                btnXoa.Enabled = true;
            }
        }

        private void btnXoa_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    EnsureConnectionOpen(); // Kiểm tra kết nối trước khi mở
                    SqlCommand cmd = new SqlCommand("DELETE FROM Sinhvien WHERE MSSV=@MSSV", conn);
                    cmd.Parameters.AddWithValue("@MSSV", txtMasv.Text);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Xóa sinh viên thành công!");
                    LoadDanhSachSinhVien();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
                finally
                {
                    conn.Close(); // Đóng kết nối
                }
            
        }
        }

        private void btnThem_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMasv.Text) || string.IsNullOrEmpty(txtHoten.Text) || cmbLophoc.SelectedIndex == -1)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }

                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Sinhvien (MSSV, HoTenSV, NgaySinh, MaLop) VALUES (@MSSV, @HoTenSV, @NgaySinh, @MaLop)", conn);
                cmd.Parameters.AddWithValue("@MSSV", txtMasv.Text);
                cmd.Parameters.AddWithValue("@HoTenSV", txtHoten.Text);
                cmd.Parameters.AddWithValue("@NgaySinh", dtNgaysinh.Value);
                cmd.Parameters.AddWithValue("@MaLop", cmbLophoc.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Thêm sinh viên thành công!");
                LoadDanhSachSinhVien();
                ResetControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnSua_Click_1(object sender, EventArgs e)
        {
            try
            {
                EnsureConnectionOpen(); // Kiểm tra kết nối trước khi mở
                SqlCommand cmd = new SqlCommand("UPDATE Sinhvien SET HoTenSV=@HoTenSV, NgaySinh=@NgaySinh, MaLop=@MaLop WHERE MSSV=@MSSV", conn);
                cmd.Parameters.AddWithValue("@MSSV", txtMasv.Text);
                cmd.Parameters.AddWithValue("@HoTenSV", txtHoten.Text);
                cmd.Parameters.AddWithValue("@NgaySinh", dtNgaysinh.Value);
                cmd.Parameters.AddWithValue("@MaLop", cmbLophoc.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Cập nhật sinh viên thành công!");
                LoadDanhSachSinhVien();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                conn.Close(); // Đóng kết nối
            }
        }

        private void btnLuu_Click_1(object sender, EventArgs e)
        {
            try
            {
                EnsureConnectionOpen(); // Kiểm tra kết nối trước khi mở
                SqlCommand cmd = new SqlCommand("INSERT INTO Sinhvien (MSSV, HoTenSV, NgaySinh, MaLop) VALUES (@MSSV, @HoTenSV, @NgaySinh, @MaLop)", conn);
                cmd.Parameters.AddWithValue("@MSSV", txtMasv.Text);
                cmd.Parameters.AddWithValue("@HoTenSV", txtHoten.Text);
                cmd.Parameters.AddWithValue("@NgaySinh", dtNgaysinh.Value);
                cmd.Parameters.AddWithValue("@MaLop", cmbLophoc.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Thêm sinh viên thành công!");
                LoadDanhSachSinhVien();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                conn.Close(); // Đóng kết nối
            }
        }

        private void btnTim_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTim.Text))
            {
                MessageBox.Show("Vui lòng nhập từ khóa tìm kiếm!");
                return;
            }

            try
            {
                EnsureConnectionOpen(); // Kiểm tra kết nối trước khi mở
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Sinhvien WHERE MSSV LIKE @Keyword OR HoTenSV LIKE @Keyword", conn);
                da.SelectCommand.Parameters.AddWithValue("@Keyword", "%" + txtTim.Text.Trim() + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sinh viên trong danh sách!", "Thông báo");
                }
                else
                {
                    dgvSinhVien.DataSource = dt; // Hiển thị kết quả lên DataGridView
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                conn.Close(); // Đóng kết nối
            }
        }

        private void btnThoat_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close(); // Đóng ứng dụng nếu chọn Yes
            }
        }

        private void dgvSinhVien_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvSinhVien.Columns[e.ColumnIndex].Name == "NgaySinh" && e.Value != null)
            {
                e.Value = Convert.ToDateTime(e.Value).ToString("dd/MM/yyyy");
                e.FormattingApplied = true;
            }
        }

        private void txtTim_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTim.Text))
            {
                LoadDanhSachSinhVien(); // Hiển thị lại toàn bộ danh sách nếu ô tìm kiếm rỗng
            }
        }
    }
}