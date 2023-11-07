using Digital_Library.Domain.Entities;
using Digital_Library.Domain.Services;

namespace Digital_Library.Infrastructure
{
    public class BooksService :IBooksService
    {
        private readonly IRepository<Book> _books;

        public BooksService (IRepository<Book> books)
        {
            _books = books;
        }

        public async Task AddBook (Book book)
        {
            await _books.AddAsync(book);
        }

        public async Task DeleteBook (Book book)
        {
            await _books.DeleteAsync(book);
        }

        public async Task UpdateBook (Book book)
        {
            await _books.UpdateAsync(book);
        }


        private async Task CopyFromStream (Stream stream, string filename)
        {
            using (var writer = new FileStream(filename, FileMode.Create))
            {
                int count = 0;
                byte[] buffer = new byte[1024];
                do
                {
                    count = await stream.ReadAsync(buffer, 0, buffer.Length);
                    await writer.WriteAsync(buffer, 0, count);

                } while (count > 0);
            }
        }

        public async Task<string> LoadFile (Stream file, string path)
        {
            var filename = Path.GetRandomFileName() + ".pdf";
            var fullname = Path.Combine(path, filename);

            await CopyFromStream(file, fullname);

            return filename;
        }

        public async Task<string> LoadPhoto (Stream file, string path)
        {
            var filename = Path.GetRandomFileName() + ".png";
            var fullname = Path.Combine(path, filename);

            await CopyFromStream(file, fullname);

            return filename;
        }

    }
}
