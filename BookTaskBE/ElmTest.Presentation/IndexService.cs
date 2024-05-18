
using ElmTest.Domain.Interfaces;

namespace ElmTest.Application
{
    public class IndexService : IIndexService
    {
        private readonly IBookRepository _bookRepository;

        public IndexService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task IndexAllBooksAsync()
        {
            await _bookRepository.IndexAllBooksAsync();
        }
    }
}
