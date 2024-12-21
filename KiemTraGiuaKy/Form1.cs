using KiemTraGiuaKy.Model;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace KiemTraGiuaKy
{
    public partial class Form1 : Form
    {
        private Model1 dbContext;

        public Form1()
        {
            InitializeComponent();
            dbContext = new Model1();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDanhSachSinhVien();
            LoadDanhSachLopHoc();
            ResetControls();
        }

        private void LoadDanhSachSinhVien()
        {
            dgvSinhVien.DataSource = dbContext.Sinhvien
                .Select(s => new
                {
                    MSSV = s.MaSV,
                    HoTenSV = s.HoTenSV,
                    NgaySinh = s.NgaySinh,
                    Tenlop = s.Lop.TenLop
                })
                .ToList();
        }

        private void LoadDanhSachLopHoc()
        {
            cmbLophoc.DataSource = dbContext.Lop.ToList();
            cmbLophoc.DisplayMember = "TenLop";
            cmbLophoc.ValueMember = "MaLop";
            cmbLophoc.SelectedIndex = -1;
        }

        private void ResetControls()
        {
            txtMasv.Clear();
            txtHoten.Clear();
            dtNgaysinh.Value = DateTime.Now;
            cmbLophoc.SelectedIndex = -1;
            txtMasv.Enabled = true;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }

        private void btnThem_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMasv.Text) || string.IsNullOrEmpty(txtHoten.Text) || cmbLophoc.SelectedIndex == -1)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo");
                    return;
                }

                var existingStudent = dbContext.Sinhvien.Find(txtMasv.Text.Trim());
                if (existingStudent != null)
                {
                    MessageBox.Show("Mã sinh viên đã tồn tại!", "Lỗi");
                    return;
                }

                var sinhVien = new Sinhvien
                {
                    MaSV = txtMasv.Text.Trim(),
                    HoTenSV = txtHoten.Text.Trim(),
                    NgaySinh = dtNgaysinh.Value,
                    MaLop = cmbLophoc.SelectedValue.ToString()
                };

                dbContext.Sinhvien.Add(sinhVien);
                dbContext.SaveChanges();
                MessageBox.Show("Thêm sinh viên thành công!", "Thông báo");

                LoadDanhSachSinhVien();
                ResetControls();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var validationError in ex.EntityValidationErrors)
                {
                    foreach (var error in validationError.ValidationErrors)
                    {
                        MessageBox.Show($"Thuộc tính: {error.PropertyName}, Lỗi: {error.ErrorMessage}", "Lỗi");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi");
            }
        }

        private void btnSua_Click_1(object sender, EventArgs e)
        {
            try
            {
                var sinhVien = dbContext.Sinhvien.Find(txtMasv.Text.Trim());
                if (sinhVien != null)
                {
                    sinhVien.HoTenSV = txtHoten.Text.Trim();
                    sinhVien.NgaySinh = dtNgaysinh.Value;
                    sinhVien.MaLop = cmbLophoc.SelectedValue.ToString();

                    dbContext.SaveChanges();
                    MessageBox.Show("Cập nhật sinh viên thành công!", "Thông báo");

                    LoadDanhSachSinhVien();
                    ResetControls();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên này!", "Lỗi");
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var validationError in ex.EntityValidationErrors)
                {
                    foreach (var error in validationError.ValidationErrors)
                    {
                        MessageBox.Show($"Thuộc tính: {error.PropertyName}, Lỗi: {error.ErrorMessage}", "Lỗi");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi");
            }
        }

        private void btnXoa_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    var sinhVien = dbContext.Sinhvien.Find(txtMasv.Text.Trim());
                    if (sinhVien != null)
                    {
                        dbContext.Sinhvien.Remove(sinhVien);
                        dbContext.SaveChanges();
                        MessageBox.Show("Xóa sinh viên thành công!", "Thông báo");

                        LoadDanhSachSinhVien();
                        ResetControls();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên này!", "Lỗi");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi");
                }
            }
        }

        private void dgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtMasv.Text = dgvSinhVien.Rows[e.RowIndex].Cells["MSSV"].Value.ToString();
                txtHoten.Text = dgvSinhVien.Rows[e.RowIndex].Cells["HoTenSV"].Value.ToString();
                dtNgaysinh.Value = Convert.ToDateTime(dgvSinhVien.Rows[e.RowIndex].Cells["NgaySinh"].Value);
                cmbLophoc.Text = dgvSinhVien.Rows[e.RowIndex].Cells["Tenlop"].Value.ToString();

                btnSua.Enabled = true;
                btnXoa.Enabled = true;
                txtMasv.Enabled = false;
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            try
            {
                string keyword = txtTim.Text.Trim(); // Lấy từ khóa tìm kiếm từ TextBox

                if (string.IsNullOrEmpty(keyword))
                {
                    MessageBox.Show("Vui lòng nhập từ khóa để tìm kiếm!", "Thông báo");
                    LoadDanhSachSinhVien(); // Hiển thị toàn bộ danh sách nếu từ khóa trống
                    return;
                }

                // Tìm kiếm sinh viên theo tên (sử dụng Contains để tìm chuỗi con)
                var result = dbContext.Sinhvien
                    .Where(s => s.HoTenSV.Contains(keyword))
                    .Select(s => new
                    {
                        MSSV = s.MaSV,
                        HoTenSV = s.HoTenSV,
                        NgaySinh = s.NgaySinh,
                        Tenlop = s.Lop.TenLop,
                    })
                    .ToList();

                if (result.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sinh viên nào khớp với từ khóa!", "Thông báo");
                    LoadDanhSachSinhVien(); // Hiển thị lại toàn bộ danh sách nếu không tìm thấy
                }
                else
                {
                    dgvSinhVien.DataSource = result; // Hiển thị kết quả tìm kiếm trong DataGridView
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi");
            }
        }

        private void txtTim_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTim.Text))
            {
                LoadDanhSachSinhVien(); // Hiển thị lại toàn bộ danh sách sinh viên
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                   "Bạn có chắc chắn muốn thoát không?",    // Nội dung thông báo
                   "Xác nhận thoát",                         // Tiêu đề của hộp thoại
                   MessageBoxButtons.YesNo,                  // Các nút trong hộp thoại
                   MessageBoxIcon.Question);                 // Biểu tượng trong hộp thoại

            // Nếu người dùng chọn "Yes", đóng ứng dụng
            if (result == DialogResult.Yes)
            {
                this.Close(); // Đóng form
            }
        }

        private void dgvSinhVien_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvSinhVien.Columns[e.ColumnIndex].Name == "NgaySinh" && e.Value != null)
            {
                // Định dạng ngày tháng theo kiểu "dd/MM/yyyy"
                e.Value = Convert.ToDateTime(e.Value).ToString("dd/MM/yyyy");
                e.FormattingApplied = true;
            }
        }
    }
}
