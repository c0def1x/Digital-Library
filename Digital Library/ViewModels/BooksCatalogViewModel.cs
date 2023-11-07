using Digital_Library.Domain.Entities;

namespace Digital_Library.ViewModels
{
    public class BooksCatalogViewModel
    {
        public List<Book> Books { get; set; }
        public List<Category> Categories { get; set; }
    }
}
