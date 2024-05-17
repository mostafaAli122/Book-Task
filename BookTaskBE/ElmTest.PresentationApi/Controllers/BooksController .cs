using ElmTest.Application;
using ElmTest.Domain;
using Microsoft.AspNetCore.Mvc;

namespace ElmTest.PresentationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
       
        private readonly ILogger<BooksController> _logger;
        private readonly BookService _bookService;

        public BooksController(ILogger<BooksController> logger, BookService bookService)
        {
            _logger = logger;
            _bookService = bookService;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Book>>> SearchBooks([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search term cannot be empty.");
            }

            var books = await _bookService.SearchBooksAsync(query);
            return Ok(books);
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Page number and page size must be greater than zero.");
            }

            var books = await _bookService.GetBooksAsync(pageNumber, pageSize);
            return Ok(books);
        }

        [HttpPost("index")]
        public async Task<IActionResult> IndexAllBooks()
        {
            await _bookService.IndexAllBooksAsync();
            return Ok("All books have been indexed successfully.");
        }

    }
}
