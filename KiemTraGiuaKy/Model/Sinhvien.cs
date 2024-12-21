namespace KiemTraGiuaKy.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Sinhvien")]
    public partial class Sinhvien
    {
        [Key]
        [StringLength(10)]
        public string MSSV { get; set; }

        [Required]
        [StringLength(40)]
        public string HoTenSV { get; set; }

        [StringLength(3)]
        public string MaLop { get; set; }

        public DateTime? NgaySinh { get; set; }

        public virtual Lop Lop { get; set; }
    }
}