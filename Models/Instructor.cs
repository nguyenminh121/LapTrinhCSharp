using System.ComponentModel.DataAnnotations;

namespace WebCSharp.Models
{
    public class Instructor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên gi?ng viên là b?t bu?c")]
        [StringLength(255)]
        [Display(Name = "Tên gi?ng viên")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không h?p l?")]
        [StringLength(255)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(100)]
        [Display(Name = "Ch?c danh")]
        public string? Title { get; set; }

        [Display(Name = "Khoa")]
        public int? FacultyId { get; set; }

        [Display(Name = "Khoa")]
        public string? FacultyName { get; set; }

        [Display(Name = "Ngày t?o")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Ngày c?p nh?t")]
        public DateTime? UpdatedAt { get; set; }
    }
}
