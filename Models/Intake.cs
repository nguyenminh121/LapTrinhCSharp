using System.ComponentModel.DataAnnotations;

namespace WebCSharp.Models
{
    public class Intake
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã khóa là b?t bu?c")]
        [StringLength(50)]
        [Display(Name = "Mã khóa")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "N?m")]
        public int? Year { get; set; }

        [Display(Name = "Ngày t?o")]
        public DateTime? CreatedAt { get; set; }
    }
}
