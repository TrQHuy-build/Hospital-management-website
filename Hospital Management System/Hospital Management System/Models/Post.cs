using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Hospital_Management_System.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        public string Slug { get; set; }

        [Required]
        [AllowHtml]
        public string Content { get; set; }

        [StringLength(500)]
        [Display(Name = "Thumbnail URL")]
        public string ThumbnailUrl { get; set; }

        [StringLength(100)]
        public string Author { get; set; }

        public DateTime PublishedDate { get; set; } = DateTime.UtcNow;

        public bool Status { get; set; } = true;
    }
}
