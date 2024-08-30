using Digital_library.Models;
using Digital_library.Services;
using Microsoft.AspNetCore.Mvc;

namespace Digital_library.Controllers;

[ApiController]
[Route("[controller]")]
public class BookController : ControllerBase
{
    BookService _service;

    public BookController(BookService service)
    {
        _service = service;
    }

    [HttpPost]
    public IActionResult Create(Book newBook)
    {
        var book = _service.Create(newBook);
        return CreatedAtAction(nameof(GetById), new { id = book!.Id}, book);
    }

    [HttpGet]
    public ActionResult<Book> GetById(int id)
    {
        var book = _service.GetById(id);

        if (book is not null)
        {
            return book;
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPut("{id}")]
    public IActionResult UpdateBook(int id, Book book)
    {
        var bookToUpdate = _service.GetById(id);
        if (bookToUpdate is not null)
        {
            var (success, message) = _service.UpdateBook(id, book);

            if (!success)
            {
                return BadRequest(new { error = message });
            }
            else
            {
                return Ok(message);
            }
        }
        else
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var book = _service.GetById(id);

        if (book is not null)
        {
            var (success, message) = _service.DeleteById(id);
            
            if (!success)
            {
                return BadRequest(new { error = message });
            }
            else
            {
                return Ok(message);
            }
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPut("borrowbook")]
    public IActionResult BorrowBook(int bookId, int userId, int until)
    {
        var book = _service.GetById(bookId);

        if(book is not null)
        {
            var (success, message) = _service.BorrowBook(bookId, userId, until);

            if (!success)
            {
                return BadRequest(new { error = message });
            }
            else
            {
                return Ok(message);
            }
        }
        else
        {
            return NotFound();
        }

    }

    [HttpPut("returnbook")]
    public IActionResult ReturnBook(int bookId, int userId)
    {
        var book = _service.GetById(bookId);

        if (book is not null)
        {
            var (success, message) = _service.ReturnBook(bookId, userId);

            if (!success)
            {
                return BadRequest(new { error = message });
            }
            else
            {
                return Ok(message);
            }
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("checknotices")]
    public List<String?> CheckNotices()
    {
        return _service.GetNotices();
    }
    
}