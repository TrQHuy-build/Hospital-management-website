using System;
using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.Models
{
    public class Article
    {
        [Key]
        public int ArticleId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

    [Required]
    [StringLength(255)]
    [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Slug chỉ được chứa chữ thường, số và dấu gạch ngang.")]
        public string Slug { get; set; }

        [StringLength(500)]
        public string Thumbnail { get; set; }

        [Required]
        public string Content { get; set; }

        [StringLength(150)]
        public string Author { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool Status { get; set; } = true;
    }
}
