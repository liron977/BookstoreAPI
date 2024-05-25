using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using BookstoreAPI.Models;
using BookstoreAPI.Services;

namespace BookstoreAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class BookstoreController : ControllerBase
  {
    private readonly BookstoreService _bookstoreService;

    public BookstoreController(BookstoreService bookstoreService)
    {
      _bookstoreService = bookstoreService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Book>), 200)]
    public ActionResult<List<Book>> GetBooks()
    {
      return _bookstoreService.GetAllBooks();
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public IActionResult AddBook([FromBody] Book book)
    {
      if (book == null)
      {
        return BadRequest("Book is null.");
      }

      try
      {
        _bookstoreService.AddBook(book);
        return Ok();
      }
      catch (ArgumentException ex)
      {
        return BadRequest(ex.Message);
      }
    }
    [HttpPut("{isbn}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public IActionResult UpdateBook(string isbn, [FromBody] UpdateBookRequest request)
    {
      var book = new Book
      {
        Isbn = isbn, // Use the ISBN from the URL
        Title = request.Title,
        Authors = request.Authors,
        Category = request.Category,
        Year = request.Year,
        Price = request.Price,
        Cover = request.Cover
      };

      try
      {
        _bookstoreService.UpdateBook(isbn, book);
        return Ok();
      }
      catch (ArgumentException ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpDelete("{isbn}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public IActionResult DeleteBook(string isbn)
    {
      _bookstoreService.DeleteBook(isbn);
      return Ok();
    }

    [HttpGet("report")]
    [ProducesResponseType(typeof(string), 200)]
    public IActionResult GetReport()
    {
      var report = _bookstoreService.GenerateReport();
      return Content(report, "text/html");
    }
  }
}
