using ElmTest.Domain;
using ElmTest.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ElmTest.Application
{
    public class BookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IDistributedCache _cache;

        public BookService(IBookRepository bookRepository, IDistributedCache cache)
        {
            _bookRepository = bookRepository;
            _cache = cache;
        }

        public async Task<IEnumerable<Book>> GetBooksAsync(int pageNumber, int pageSize)
        {
            var cacheKey = $"Books_{pageNumber}_{pageSize}";
            var cachedBooks = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedBooks))
            {
                return JsonSerializer.Deserialize<IEnumerable<Book>>(cachedBooks);
            }

            var books = await _bookRepository.GetBooksAsync(pageNumber, pageSize);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(books), cacheOptions);

            return books;
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
        {
            var cacheKey = $"Books_{searchTerm}";
            var cachedBooks = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedBooks))
            {
                return JsonSerializer.Deserialize<IEnumerable<Book>>(cachedBooks);
            }

            var books = await _bookRepository.SearchBooksAsync(searchTerm);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(books), cacheOptions);

            return books;
        }

        public async Task IndexAllBooksAsync()
        {
            await _bookRepository.IndexAllBooksAsync();
        }
    }

}
