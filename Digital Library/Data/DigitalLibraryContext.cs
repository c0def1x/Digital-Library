using Digital_Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Digital_Library.Data
{
    public class DigitalLibraryContext :DbContext
    {
        public DigitalLibraryContext (DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
