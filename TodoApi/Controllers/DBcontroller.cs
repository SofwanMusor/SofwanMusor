using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;


namespace BookStoreApi.Controllers
{
    [ApiController]
    [Route("api/Test")]
    public class BooksController : ControllerBase
    {
        private readonly IBooksService _booksService;

        public BooksController(IBooksService booksService)
        {
            _booksService = booksService;
        }

        [HttpGet]
        public async Task<List<Book>> Get() =>
            await _booksService.GetAsync();

        [HttpPost]
        public async Task<IActionResult> Post(Book newBook)
        {
            await _booksService.CreateAsync(newBook);

            return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
        }

        [HttpPost("insert")]
        public async Task<IActionResult> Insert(Book newBook)
        {
            await _booksService.InsertAsync(newBook);

            return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
        }

        [HttpGet("{keyword}")]
        public async Task<ActionResult<List<Book>>> Find(string keyword)
        {
            var books = await _booksService.FindAsync(keyword);
            if (books == null || books.Count == 0)
            {
                return NotFound();
            }
            return books;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Book bookIn)
        {
            var book = await _booksService.GetAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            bookIn.Id = id; // ตั้งค่า Id ของหนังสือใหม่
            await _booksService.UpdateAsync(id, bookIn);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var book = await _booksService.GetAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            await _booksService.DeleteAsync(id);

            return NoContent();
        }

        [HttpDelete("bulk")]
        public async Task<IActionResult> DeleteBulk(List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest("No IDs provided");
            }

            await _booksService.DeleteManyAsync(ids);

            return NoContent();
        }
        [HttpPost("bulk")]
        public async Task<IActionResult> InsertBulk(List<Book> books)
        {
            if (books == null || books.Count == 0)
            {
                return BadRequest("No books provided");
            }

            await _booksService.InsertManyAsync(books);

            return Ok("Books inserted successfully");
        }
        //////////////////////////////////////////////////// สิ้นสุด Collections and Methods //////////////////////////////////////////////////////////////////

        [HttpGet("FictionUnder50")]
        public async Task<ActionResult<List<Book>>> GetFictionBooksUnder50()
        {
            var result = await _booksService.GetAsync();
            var fictionBooksUnder50 = result.Where(book => book.Category == "Fiction" && book.Price < 50).ToList();
            return fictionBooksUnder50;
        }
        [HttpGet("FictionOrNonFiction")]
        public async Task<ActionResult<List<Book>>> GetFictionOrNonFictionBooks()
        {
            var result = await _booksService.GetAsync();
            var fictionOrNonFictionBooks = result.Where(book => book.Category == "Fiction" || book.Category == "Non-fiction").ToList();
            return fictionOrNonFictionBooks;
        }

        // Example 3: Logical NOT (!)
        [HttpGet("NotFiction")]
        public async Task<ActionResult<List<Book>>> GetNonFictionBooks()
        {
            var result = await _booksService.GetAsync();
            var nonFictionBooks = result.Where(book => !(book.Category == "Fiction")).ToList();
            return nonFictionBooks;
        }
        //////////////////////////////////////////////////// สิ้นสุด Logical Operators //////////////////////////////////////////////////////////////////

        [HttpGet("Exists/{field}")]
        public async Task<ActionResult<List<Book>>> GetBooksWhereFieldExists(string field)
        {
            var filter = Builders<Book>.Filter.Exists(field);
            var books = await _booksService.FindAsync(filter);
            return books;
        }
        [HttpGet("BooksOfType/{type}")]
        public async Task<ActionResult<List<Book>>> GetBooksOfType(BsonType type)
        {
            var filter = Builders<Book>.Filter.Type("Price", type);
            var books = await _booksService.FindAsync(filter);
            return books;
        }
        // Example: Using $regex operator
        [HttpGet("Regex/{pattern}")]
        public async Task<ActionResult<List<Book>>> GetBooksMatchingPattern(string pattern)
        {
            var regexPattern = new BsonRegularExpression(pattern, "i"); // สร้าง BsonRegularExpression จาก pattern ที่รับมา
            var filter = Builders<Book>.Filter.Regex("Name", regexPattern); // ใช้ BsonRegularExpression เป็นค่า Regex ในตัวกรอง
            var books = await _booksService.FindAsync(filter);
            return books;
        }
        //////////////////////////////////////////////////// สิ้นสุด Element Operators //////////////////////////////////////////////////////////////////

        [HttpGet("ElemMatch/{min}/{max}")]
        public async Task<ActionResult<List<Book>>> GetBooksInPriceRange(decimal min, decimal max)
        {
            var filter = Builders<Book>.Filter.ElemMatch(x => x.Prices, Builders<Price>.Filter.Gte("Value", min) & Builders<Price>.Filter.Lte("Value", max));
            var books = await _booksService.FindAsync(filter);
            return books;
        }

        [HttpGet("Size/{size}")]
        public async Task<ActionResult<List<Book>>> GetBooksByCommentCount(int size)
        {
            var filter = Builders<Book>.Filter.Size(x => x.Comments, size);
            var books = await _booksService.FindAsync(filter);
            return books;
        }

        [HttpGet("All/{tag1}/{tag2}")]
        public async Task<ActionResult<List<Book>>> GetBooksByTags(string tag1, string tag2)
        {
            var filter = Builders<Book>.Filter.All(x => x.Tags, new List<string> { tag1, tag2 });
            var books = await _booksService.FindAsync(filter);
            return books;
        }

        [HttpGet("In/{category}")]
        public async Task<ActionResult<List<Book>>> GetBooksByCategory(string category)
        {
            var filter = Builders<Book>.Filter.In(x => x.Category, new List<string> { category });
            var books = await _booksService.FindAsync(filter);
            return books;
        }

        [HttpGet("Nin/{status1}/{status2}")]
        public async Task<ActionResult<List<Book>>> GetBooksByStatus(string status1, string status2)
        {
            var filter = Builders<Book>.Filter.Nin(x => x.Status, new List<string> { status1, status2 });
            var books = await _booksService.FindAsync(filter);
            return books;
        }
        //////////////////////////////////////////////////// สิ้นสุด Array Operators //////////////////////////////////////////////////////////////////

        [HttpGet("price-eq/{price}")]
        public async Task<ActionResult<List<Book>>> GetBooksByPriceEquals(decimal price)
        {
            var filter = Builders<Book>.Filter.Eq(x => x.Price, price);
            var books = await _booksService.FindAsync(filter);
            return Ok(books);
        }
        [HttpGet("price-ne/{price}")]
        public async Task<ActionResult<List<Book>>> GetBooksByPriceNotEquals(decimal price)
        {
            var filter = Builders<Book>.Filter.Ne(x => x.Price, price);
            var books = await _booksService.FindAsync(filter);
            return Ok(books);
        }

        [HttpGet("price-gt/{price}")]
        public async Task<ActionResult<List<Book>>> GetBooksByPriceGreaterThan(decimal price)
        {
            var filter = Builders<Book>.Filter.Gt(x => x.Price, price);
            var books = await _booksService.FindAsync(filter);
            return Ok(books);
        }

        [HttpGet("price-gte/{price}")]
        public async Task<ActionResult<List<Book>>> GetBooksByPriceGreaterThanOrEqual(decimal price)
        {
            var filter = Builders<Book>.Filter.Gte(x => x.Price, price);
            var books = await _booksService.FindAsync(filter);
            return Ok(books);
        }

        [HttpGet("price-lt/{price}")]
        public async Task<ActionResult<List<Book>>> GetBooksByPriceLessThan(decimal price)
        {
            var filter = Builders<Book>.Filter.Lt(x => x.Price, price);
            var books = await _booksService.FindAsync(filter);
            return Ok(books);
        }

        [HttpGet("price-lte/{price}")]
        public async Task<ActionResult<List<Book>>> GetBooksByPriceLessThanOrEqual(decimal price)
        {
            var filter = Builders<Book>.Filter.Lte(x => x.Price, price);
            var books = await _booksService.FindAsync(filter);
            return Ok(books);
        }
        //////////////////////////////////////////////////// สิ้นสุด Comparison Operators //////////////////////////////////////////////////////////////////
    }
}
