using System.ComponentModel.DataAnnotations;

namespace WebCSharp.Models
{
    public class Faculty
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên khoa là b?t bu?c")]
        [StringLength(255)]
        [Display(Name = "Tên khoa")]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "??a ch?")]
        public string? Address { get; set; }

        [Display(Name = "Ngày t?o")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Ngày c?p nh?t")]
        public DateTime? UpdatedAt { get; set; }
    }
}
