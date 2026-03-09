using System.ComponentModel.DataAnnotations;

namespace WebCSharp.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã môn h?c là b?t bu?c")]
        [StringLength(50)]
        [Display(Name = "Mã môn")]
        public string Code { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Tên (Ti?ng Vi?t)")]
        public string? NameVi { get; set; }

        [StringLength(255)]
        [Display(Name = "Tên (Ti?ng Anh)")]
        public string? NameEn { get; set; }

        [Display(Name = "S? tín ch?")]
        public int? Credit { get; set; }

        [Display(Name = "Gi? lý thuy?t")]
        public int? LectureHours { get; set; }

        [Display(Name = "Gi? t? h?c")]
        public int? SelfStudyHours { get; set; }

        [Display(Name = "Mô t?")]
        public string? Description { get; set; }

        [Display(Name = "Ngày t?o")]
        public DateTime? CreatedAt { get; set; }
    }
}
