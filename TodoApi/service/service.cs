using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BookStoreApi.Services
{
    public class BooksService : IBooksService
    {
        private readonly IMongoCollection<Book> _booksCollection;

        public BooksService(IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings)
        {
            var client = new MongoClient(bookStoreDatabaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(bookStoreDatabaseSettings.Value.DatabaseName);

            _booksCollection = database.GetCollection<Book>(bookStoreDatabaseSettings.Value.BooksCollectionName);
        }

        public async Task<List<Book>> GetAsync() =>
            await _booksCollection.Find(_ => true).ToListAsync();

        public async Task<Book> GetAsync(string id) =>
            await _booksCollection.Find(book => book.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Book newBook) =>
            await _booksCollection.InsertOneAsync(newBook);

        public async Task<Book> InsertAsync(Book book)
        {
            await _booksCollection.InsertOneAsync(book);
            return book;
        }

        public async Task<List<Book>> FindAsync(string keyword)
        {
            var filter = Builders<Book>.Filter.Regex("BookName", new BsonRegularExpression(keyword, "i"));
            return await _booksCollection.Find(filter).ToListAsync();
        }

        public async Task<bool> UpdateAsync(string id, Book bookIn) =>
            (await _booksCollection.ReplaceOneAsync(book => book.Id == id, bookIn)).IsAcknowledged;

        public async Task DeleteAsync(string id)
        {
            await _booksCollection.DeleteOneAsync(book => book.Id == id);
        }

        public async Task DeleteManyAsync(List<string> ids)
        {
            var filter = Builders<Book>.Filter.In(book => book.Id, ids);
            await _booksCollection.DeleteManyAsync(filter);
        }
        public async Task InsertManyAsync(List<Book> books)
        {
            await _booksCollection.InsertManyAsync(books);
        }

        public async Task<List<Book>> FindAsync(FilterDefinition<Book> filter)
        {
            return await _booksCollection.Find(filter).ToListAsync();
        }
    }
}
