using System.ComponentModel.DataAnnotations;

namespace WebCSharp.Models
{
    public class Syllabus
    {
        public int Id { get; set; }

        [Display(Name = "Môn h?c")]
        public int? CourseId { get; set; }

        [Display(Name = "Môn h?c")]
        public string? CourseName { get; set; }

        [StringLength(50)]
        [Display(Name = "Phiên b?n")]
        public string? Version { get; set; }

        [Display(Name = "Ngôn ng?")]
        public string? Language { get; set; }

        [Display(Name = "Mô t?")]
        public string? Description { get; set; }

        [Display(Name = "Ngày ?i?u ch?nh")]
        [DataType(DataType.Date)]
        public DateTime? AdjustmentDate { get; set; }

        [StringLength(255)]
        [Display(Name = "Ng??i phê duy?t")]
        public string? ApprovedBy { get; set; }

        [Display(Name = "Ngày t?o")]
        public DateTime? CreatedAt { get; set; }
    }
}
