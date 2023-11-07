using System.ComponentModel.DataAnnotations;

namespace Digital_Library.Domain.Entities
{
    public class Book :Entity
    {
        [StringLength(150)]
        public string Title { get; set; } = null!;
        [StringLength(150)]
        public string Author { get; set; } = null!;
        public string? Description { get; set; }
        public int PagesCount { get; set; }
        [StringLength(300)]
        public string? ImageUrl { get; set; }
        [StringLength(300)]
        public string Filename { get; set; } = null!;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
