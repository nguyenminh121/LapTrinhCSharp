using System.ComponentModel.DataAnnotations;

namespace WebCSharp.Models
{
    public class Curriculum
    {
        public int Id { get; set; }

        [Display(Name = "Ngành")]
        public int? MajorId { get; set; }

        [Display(Name = "Ngành")]
        public string? MajorName { get; set; }

        [Display(Name = "Khóa tuy?n sinh")]
        public int? IntakeId { get; set; }

        [Display(Name = "Khóa tuy?n sinh")]
        public string? IntakeCode { get; set; }

        [Display(Name = "T?ng tín ch?")]
        public int? TotalCredits { get; set; }

        [Display(Name = "Ngày t?o")]
        public DateTime? CreatedAt { get; set; }
    }
}
