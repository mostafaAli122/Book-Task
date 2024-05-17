using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmTest.Domain.Interfaces
{
    public interface IBookRepository
    {
        Task IndexAllBooksAsync();

        Task<IEnumerable<Book>> GetBooksAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
    }
}
