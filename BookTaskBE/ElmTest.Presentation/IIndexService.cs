using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmTest.Application
{
    public interface IIndexService
    {
        Task IndexAllBooksAsync();
    }
}
