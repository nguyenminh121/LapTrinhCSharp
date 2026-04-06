using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuyenTap.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [Column("ProductID")]
        public int Id { get; set; }

        [Column("CategoryID")]
        [Range(1, int.MaxValue, ErrorMessage = "CategoryID phai lon hon 0")]
        public int CategoryId { get; set; }

        [Column("ModelNumber")]
        [StringLength(50)]
        public string? ModelNumber { get; set; }

        [Column("ModelName")]
        [StringLength(100)]
        public string? ModelName { get; set; }

        [Column("ProductImage")]
        [StringLength(255)]
        public string? ProductImage { get; set; }

        [Column("UnitCost", TypeName = "money")]
        [Range(0.01, 999999999, ErrorMessage = "Unit cost phai lon hon 0")]
        public decimal UnitCost { get; set; }

        [Column("Description")]
        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
