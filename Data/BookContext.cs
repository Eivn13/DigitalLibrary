using Microsoft.EntityFrameworkCore;
using Digital_library.Models;

namespace Digital_library.Data;

public class BookContext : DbContext
{
    public BookContext (DbContextOptions<BookContext> options) : base (options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<User> Users => Set<User>();
}