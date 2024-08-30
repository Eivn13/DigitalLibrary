using Digital_library.Controllers;
using Digital_library.Models;
using Digital_library.Services;
using Digital_library.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitallibraryTest
{
    public class BookControllerTests
    {
    private readonly BookController _controller;
    private readonly BookService _bookService;
    private readonly BookContext _context;

        public BookControllerTests()
        {
            var options = new DbContextOptionsBuilder<BookContext>()
                .UseInMemoryDatabase(databaseName: "TestBookContextDb")
                .Options;

            _context = new BookContext(options);

            _bookService = new BookService(_context);

            _controller = new BookController(_bookService);

            SeedDb();
        }

        private void SeedDb()
        {
            
            DateTime today = DateTime.Today;

            var u1 = new User {Name = "User1", Email = "aaaa@aaa.com"};
            var u2 = new User {Name = "User2", Email = "bbbb@bbbbbb.com"};

            _context.Books.AddRange(
                new Book
                {
                    Id = 1,
                    Name = "Book1",
                    BorrowedBy = u1,
                    Until = today.AddDays(14)
                },
                new Book
                {
                    Id = 2,
                    Name = "Book2",
                    BorrowedBy = u2,
                    Until = today.AddHours(20)
                },
                new Book
                {
                    Id = 3,
                    Name = "Book3",
                }
            );
            _context.SaveChanges();
        }

        [Fact]
        public void Create_ReturnsCreatedResult_WhenBookIsCreated()
        {
            var newBook = new Book { Name = "Book4" };

            var result = _controller.Create(newBook);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<Book>(createdResult.Value);
            Assert.Equal(newBook.Id, returnValue.Id);
            Assert.Equal(newBook.Name, returnValue.Name);
        }

        [Fact]
        public void GetById_ReturnsBook_WhenBookExists()
        {
            var bookId = 1;

            var result = _controller.GetById(bookId);

            Assert.NotNull(result.Value);
            Assert.Equal(bookId, result.Value.Id);
        }

        [Fact]
        public void GetById_ReturnsNotFound_WhenBookDoesNotExist()
        {
            var bookId = 500;

            var result = _controller.GetById(bookId);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        
        [Fact]
        public void UpdateBook_ReturnsOkObject_WhenBookGetsUpdated()
        {
            var bookId = 1;
            var newBook = new Book { Name = "Dante's Inferno"};

            var updateResult = _controller.UpdateBook(bookId, new Book { Name = "Dante's Inferno"});
            var getBookResult = _controller.GetById(bookId);
            var returnValue = Assert.IsType<Book>(getBookResult.Value);

            Assert.IsType<OkObjectResult>(updateResult);
            Assert.Equal(newBook.Name, returnValue.Name);
        }

        [Fact]
        public void UpdateBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            var bookId = 100;

            var updateResult = _controller.UpdateBook(bookId, new Book { Name = "Dante's Inferno 2"});

            Assert.IsType<NotFoundResult>(updateResult);
        }

        [Fact]
        public void DeleteBook_ReturnsOkObject_WhenBookGetsDeleted()
        {
            var bookId = 1;

            var deleteResult = _controller.Delete(bookId);
            var getBookResult = _controller.GetById(bookId);

            Assert.IsType<OkObjectResult>(deleteResult);
            Assert.IsType<NotFoundResult>(getBookResult.Result);
        }

        [Fact]
        public void DeleteBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            var bookId = 100;

            var deleteResult = _controller.Delete(bookId);

            Assert.IsType<NotFoundResult>(deleteResult);
        }

        [Fact]
        public void BorrowBook_ReturnsOkObject_WhenBookGetsBorrowed()
        {
            var bookId = 3;
            var userId = 2;
            var until = DateTime.Today.AddDays(5);
            
            var borrowResult = _controller.BorrowBook(bookId, userId, 5);
            var getBookResult = _controller.GetById(bookId);
            var returnValue = Assert.IsType<Book>(getBookResult.Value);
            var returnedDate = returnValue.Until ?? null;

            Assert.IsType<OkObjectResult>(borrowResult);
            Assert.Equal(bookId, returnValue.Id);
            Assert.Equal(userId, returnValue.BorrowedBy?.Id);
            Assert.NotNull(returnedDate);
            Assert.Equal(until, returnedDate);
            Assert.Equal(bookId, returnValue.BorrowedBy?.BorrowedBooks?.Single(p => p.Id == bookId).Id);
        }

        [Fact]
        public void BorrowBook_ReturnsBadRequestObject_WhenBookIsAlreadyBorrowed()
        {
            var bookId = 3;
            var userId = 2;
            
            var borrowResult = _controller.BorrowBook(bookId, userId, 5);

            Assert.IsType<BadRequestObjectResult>(borrowResult);
        }

        [Fact]
        public void BorrowBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            var bookId = 100;
            var userId = 100;

            var borrowResult = _controller.BorrowBook(bookId, userId, 5);

            Assert.IsType<NotFoundResult>(borrowResult);
        }

        [Fact]
        public void ReturnBook_ReturnsOkObject_WhenBookGetsReturned()
        {
            var bookId = 3;
            var userId = 2;
            _controller.BorrowBook(bookId, userId, 5);

            var returnBookResult = _controller.ReturnBook(bookId, userId);
            var getBookResult = _controller.GetById(bookId);
            var returnValue = Assert.IsType<Book>(getBookResult.Value);

            Assert.IsType<OkObjectResult>(returnBookResult);
            Assert.Equal(bookId, returnValue.Id);
            Assert.Null(returnValue.BorrowedBy);
            Assert.Null(returnValue.Until);
        }

        [Fact]
        public void BorrowBook_ReturnsBadRequestObject_WhenUntilIsLessThan1OrMoreThan14()
        {
            var bookId = 3;
            var userId = 2;
            
            var borrowResult = _controller.BorrowBook(bookId, userId, 15);

            Assert.IsType<BadRequestObjectResult>(borrowResult);
        }

        [Fact]
        public void ReturnBook_ReturnsBadRequestObject_WhenBookOrUserDoesNotExist()
        {
            var bookId = 100;
            var userId = 100;

            var returnBookResult = _controller.ReturnBook(bookId, userId);

            Assert.IsType<NotFoundResult>(returnBookResult);
        }

        [Fact]
        public void ReturnBook_ReturnsBadRequestObject_WhenUserDoesNotHaveThisBookBorrowed()
        {
            var bookId = 2;
            var userId = 1;

            var returnBookResult = _controller.ReturnBook(bookId, userId);

            Assert.IsType<BadRequestObjectResult>(returnBookResult);
        }

        [Fact]
        public void GetNotices_ReturnsListOfStrings_WhenUserNeedsToReturnBookWithinDay()
        {
            var userEmail = "bbbb@bbbbbb.com";
            var returnNoticesResult = _controller.CheckNotices();

            Assert.Single(returnNoticesResult);
            Assert.Equal(userEmail, returnNoticesResult.First());
        }
    }
}
