using KiemTraGiuaKy.Model;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace KiemTraGiuaKy
{
    public partial class Form1 : Form
    {
        // DbContext để làm việc với cơ sở dữ liệu
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

        // Hàm tải danh sách sinh viên vào DataGridView
        private void LoadDanhSachSinhVien()
        {
            dgvSinhVien.DataSource = dbContext.Sinhvien
                .Select(s => new
                {
                    MSSV = s.MSSV,
                    HoTenSV = s.HoTenSV,
                    NgaySinh = s.NgaySinh,
                    MaLop = s.MaLop
                })
                .ToList();
        }

        // Hàm tải danh sách lớp vào ComboBox
        private void LoadDanhSachLopHoc()
        {
            cmbLophoc.DataSource = dbContext.Lop.ToList();
            cmbLophoc.DisplayMember = "TenLop";
            cmbLophoc.ValueMember = "MaLop";
            cmbLophoc.SelectedIndex = -1;
        }

        // Hàm đặt lại controls về trạng thái ban đầu
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

        // Hàm thêm sinh viên
        private void btnThem_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đầu vào
                if (string.IsNullOrEmpty(txtMasv.Text) || string.IsNullOrEmpty(txtHoten.Text) || cmbLophoc.SelectedIndex == -1)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo");
                    return;
                }

                // Kiểm tra trùng lặp MSSV
                var existingStudent = dbContext.Sinhvien.Find(txtMasv.Text);
                if (existingStudent != null)
                {
                    MessageBox.Show("Mã sinh viên đã tồn tại!", "Lỗi");
                    return;
                }

                // Tạo đối tượng Sinhvien
                var sinhVien = new Sinhvien
                {
                    MSSV = txtMasv.Text.Trim(),
                    HoTenSV = txtHoten.Text.Trim(),
                    NgaySinh = dtNgaysinh.Value,
                    MaLop = cmbLophoc.SelectedValue.ToString()
                };

                // Thêm vào DbContext
                dbContext.Sinhvien.Add(sinhVien);
                dbContext.SaveChanges();

                MessageBox.Show("Thêm sinh viên thành công!", "Thông báo");

                // Cập nhật lại danh sách
                LoadDanhSachSinhVien();
                ResetControls();
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi chi tiết hơn
                MessageBox.Show($"Lỗi: {ex.Message}\n{ex.InnerException?.Message}", "Lỗi");
            }
        }

        // Hàm sửa sinh viên
        private void btnSua_Click_1(object sender, EventArgs e)
        {
            try
            {
                var sinhVien = dbContext.Sinhvien.Find(txtMasv.Text);
                if (sinhVien != null)
                {
                    sinhVien.HoTenSV = txtHoten.Text;
                    sinhVien.NgaySinh = dtNgaysinh.Value;
                    sinhVien.MaLop = cmbLophoc.SelectedValue.ToString();

                    dbContext.SaveChanges();

                    MessageBox.Show("Cập nhật sinh viên thành công!");
                    LoadDanhSachSinhVien();
                    ResetControls();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên này!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        // Hàm xóa sinh viên
        private void btnXoa_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    var sinhVien = dbContext.Sinhvien.Find(txtMasv.Text);
                    if (sinhVien != null)
                    {
                        dbContext.Sinhvien.Remove(sinhVien);
                        dbContext.SaveChanges();

                        MessageBox.Show("Xóa sinh viên thành công!");
                        LoadDanhSachSinhVien();
                        ResetControls();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên này!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        // Hàm tìm kiếm sinh viên
        private void btnTim_Click_1(object sender, EventArgs e)
        {
            try
            {
                string keyword = txtTim.Text.Trim(); // Lấy từ khóa tìm kiếm

                if (string.IsNullOrEmpty(keyword))
                {
                    MessageBox.Show("Vui lòng nhập từ khóa để tìm kiếm!", "Thông báo");
                    LoadDanhSachSinhVien(); // Hiển thị lại danh sách đầy đủ nếu từ khóa trống
                    return;
                }

                // Tìm kiếm sinh viên theo tên
                var result = dbContext.Sinhvien
                    .Where(s => s.HoTenSV.Contains(keyword))
                    .Select(s => new
                    {
                        MSSV = s.MSSV,
                        HoTenSV = s.HoTenSV,
                        NgaySinh = s.NgaySinh,
                        MaLop = s.MaLop
                    })
                    .ToList();

                if (result.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sinh viên nào khớp với từ khóa!", "Thông báo");
                    LoadDanhSachSinhVien(); // Hiển thị lại danh sách đầy đủ nếu không tìm thấy
                }
                else
                {
                    dgvSinhVien.DataSource = result; // Hiển thị kết quả tìm kiếm
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi");
            }
        }

        // Hàm thoát chương trình
        private void btnThoat_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        // Hàm định dạng hiển thị cột ngày sinh
        private void dgvSinhVien_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvSinhVien.Columns[e.ColumnIndex].Name == "NgaySinh" && e.Value != null)
            {
                e.Value = Convert.ToDateTime(e.Value).ToString("dd/MM/yyyy");
                e.FormattingApplied = true;
            }
        }

        // Hàm hiển thị lại danh sách sinh viên khi xóa ô tìm kiếm
        private void txtTim_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTim.Text))
            {
                LoadDanhSachSinhVien();
            }
        }

        // Xử lý chọn dòng trong DataGridView
        private void dgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtMasv.Text = dgvSinhVien.Rows[e.RowIndex].Cells["MSSV"].Value.ToString();
                txtHoten.Text = dgvSinhVien.Rows[e.RowIndex].Cells["HoTenSV"].Value.ToString();
                dtNgaysinh.Value = Convert.ToDateTime(dgvSinhVien.Rows[e.RowIndex].Cells["NgaySinh"].Value);
                cmbLophoc.SelectedValue = dgvSinhVien.Rows[e.RowIndex].Cells["MaLop"].Value.ToString();

                btnSua.Enabled = true;
                btnXoa.Enabled = true;
                txtMasv.Enabled = false;
            }
        }

        private void txtTim_TextChanged_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTim.Text))
            {
                LoadDanhSachSinhVien(); // Hiển thị lại toàn bộ danh sách nếu ô tìm kiếm rỗng
            }
        }
    }
}
