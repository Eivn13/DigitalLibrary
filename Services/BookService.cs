using Digital_library.Models;
using Digital_library.Data;
using Microsoft.EntityFrameworkCore;

namespace Digital_library.Services;

public class BookService
{
    private readonly BookContext _context;

    public BookService(BookContext context)
    {
        _context = context;
    }

    public Book? Create(Book newBook)
    {
        _context.Books.Add(newBook);
        _context.SaveChanges();

        return newBook;
    }

    public Book? GetById(int id)
    {
        return _context.Books
            .Include(p => p.BorrowedBy)
            .AsNoTracking()
            .SingleOrDefault(p => p.Id == id);
    }

    public void UpdateBook(int bookId, Book book)
    {
        var bookToUpdate = _context.Books.Find(bookId);

        if (bookToUpdate is null)
        {
            throw new InvalidOperationException("Book does not exist.");
        }
        
        bookToUpdate.Name = book.Name;
        bookToUpdate.BorrowedBy = book.BorrowedBy;
        bookToUpdate.Until = book.Until;

        _context.SaveChanges();
    }

    public void DeleteById(int id)
    {
        var bookToDelete = _context.Books.Find(id);
        if (bookToDelete is not null)
        {
            _context.Books.Remove(bookToDelete);
            _context.SaveChanges();
        }
    }

    public void BorrowBook(int bookId, int userId, int until)
    {
        var bookToBorrow = _context.Books.Find(bookId);
        var user = _context.Users.Find(userId);

        if (bookToBorrow is null || user is null)
        {
            throw new InvalidOperationException("Book or user does not exist.");
        }

        if (bookToBorrow.BorrowedBy is not null)
        {
            throw new InvalidOperationException("Someone already borrowed this book.");
        }

        bookToBorrow.BorrowedBy = user;

        if (until < 1 || until > 14)
        {
            throw new InvalidOperationException("Cannot borrow book for less than a day or more than two weeks.");
        }
        
        DateTime today = DateTime.Today;
        bookToBorrow.Until = today.AddDays(until);
        user.BorrowedBooks?.Add(bookToBorrow);

        _context.SaveChanges();
    }

    public void ReturnBook(int bookId, int userId)
    {
        var borrowedBook = _context.Books.Find(bookId);
        var user = _context.Users.Find(userId);

        if (borrowedBook is null || user is null)
        {
            throw new InvalidOperationException("Book or user does not exist.");
        }

        var book = user.BorrowedBooks?.Single(p => p.Id == bookId);
        if (book is not null)
        {
            user.BorrowedBooks?.Remove(book);
            borrowedBook.BorrowedBy = null;
            borrowedBook.Until = null;
            _context.SaveChanges();
        }
    }

    public List<String?> GetNotices()
    {
        DateTime now = DateTime.Now;

        return _context.Books
            .Include(p => p.BorrowedBy)
            .AsNoTracking()
            .Where(book => book.Until <= now.AddDays(1) && book.Until > now)
            .Select(book => book.BorrowedBy != null ? book.BorrowedBy.Email : null)
            .Where(email => email != null)
            .ToList();
    }

}