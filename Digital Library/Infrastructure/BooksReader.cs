using Digital_Library.Domain.Entities;
using Digital_Library.Domain.Services;

namespace Digital_Library.Infrastructure
{
    public class BooksReader :IBooksReader
    {
        private readonly IRepository<Book> _repository;
        private readonly IRepository<Category> _categories;

        public BooksReader (IRepository<Book> books, IRepository<Category> categories)
        {
            _repository = books;
            _categories = categories;
        }

        public async Task<Book?> FindBookAsync (int bookId) =>
    await _repository.FindAsync(bookId);

        public async Task<List<Book>> FindBooksAsync (string searchString, int categoryId) => (searchString, categoryId) switch
        {
            ("" or null, 0) => await _repository.GetAllAsync(),
            (_, 0) => await _repository.FindWhere(book => book.Title.Contains(searchString) || book.Author.Contains(searchString)),
            (_, _) => await _repository.FindWhere(book => book.CategoryId == categoryId &&
                (book.Title.Contains(searchString) || book.Author.Contains(searchString))),
        };

        public async Task<List<Book>> GetAllBooksAsync () => await _repository.GetAllAsync();

        public async Task<List<Category>> GetCategoriesAsync () => await _categories.GetAllAsync();
    }
}
