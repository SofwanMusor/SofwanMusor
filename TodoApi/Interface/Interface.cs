using BookStoreApi.Models;
using MongoDB.Driver;


namespace BookStoreApi.Services
{
    public interface IBooksService
    {
        Task<List<Book>> GetAsync();
        Task<Book> GetAsync(string id);
        Task CreateAsync(Book newBook);
        Task<Book> InsertAsync(Book book);
        Task<List<Book>> FindAsync(string keyword);
        Task<bool> UpdateAsync(string id, Book bookIn);
        Task DeleteAsync(string id);
        Task DeleteManyAsync(List<string> ids);
        Task InsertManyAsync(List<Book> books);
        Task<List<Book>> FindAsync(FilterDefinition<Book> filter);

    }
}
