using System.ComponentModel.DataAnnotations;

namespace LuyenTap.Models
{
    public class AdminUser
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Password { get; set; } = string.Empty;
    }
}
