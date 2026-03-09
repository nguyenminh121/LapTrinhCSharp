using System.ComponentModel.DataAnnotations;

namespace WebCSharp.Models
{
    public class Major
    {
        public int Id { get; set; }

        [Display(Name = "Khoa")]
        public int? FacultyId { get; set; }

        [Display(Name = "Khoa")]
        public string? FacultyName { get; set; }

        [Required(ErrorMessage = "Tên ngành là b?t bu?c")]
        [StringLength(255)]
        [Display(Name = "Tên ngành")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Trình ??")]
        public string? DegreeLevel { get; set; }

        [Display(Name = "S? tín ch? yêu c?u")]
        public int? RequiredCredits { get; set; }

        [Display(Name = "Ngày t?o")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Ngày c?p nh?t")]
        public DateTime? UpdatedAt { get; set; }
    }
}
