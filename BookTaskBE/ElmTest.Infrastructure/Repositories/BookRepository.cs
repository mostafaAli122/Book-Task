using Dapper;
using ElmTest.Domain;
using ElmTest.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using Nest;
using System.Text.Json;
namespace ElmTest.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly string _connectionString;
        private readonly IElasticClient _elasticClient;

        public BookRepository(Uri elasticsearchUri, string connectionString)
        {
            _connectionString = connectionString;
            var settings = new ConnectionSettings(elasticsearchUri).DefaultIndex("books");
            _elasticClient = new ElasticClient(settings);
        }

        public async Task IndexAllBooksAsync()
        {
            const string query = @"
        SELECT BookId, BookInfo
        FROM Book";

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var books = await db.QueryAsync<Book>(query);

                var bookList = books.ToList();
                var batchSize = 200; // Adjust based on your environment
                var tasks = new List<Task>();

                Parallel.ForEach(
                    System.Collections.Concurrent.Partitioner.Create(0, bookList.Count, batchSize),
                    new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                    range =>
                    {
                        var bulkRequest = new BulkRequest("books")
                        {
                            Operations = new List<IBulkOperation>()
                        };

                        for (var i = range.Item1; i < range.Item2; i++)
                        {
                            var book = bookList[i];
                            var bookInfo = JsonSerializer.Deserialize<BookInfo>(book.BookInfo);
                            if (bookInfo != null)
                            {
                                book.BookInfoObj.BookTitle = bookInfo.BookTitle;
                                book.BookInfoObj.BookDescription = bookInfo.BookDescription;
                                book.BookInfoObj.Author = bookInfo.Author;
                                book.BookInfoObj.PublishDate = bookInfo.PublishDate;
                            }

                            bulkRequest.Operations.Add(new BulkIndexOperation<Book>(book));
                        }

                        tasks.Add(_elasticClient.BulkAsync(bulkRequest));
                    }
                );

                await Task.WhenAll(tasks);
            }
        }
        public async Task<IEnumerable<Book>> GetBooksAsync(int pageNumber, int pageSize)
        {
            var searchResponse = await _elasticClient.SearchAsync<Book>(s => s
           .From((pageNumber - 1) * pageSize) 
           .Size(pageSize) 
           .Sort(sort => sort.Descending(doc => doc.BookInfoObj.PublishDate)) 
       );

            return searchResponse.Documents.ToList();
        }
        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
        {
            //    var searchResponse = await _elasticClient.SearchAsync<Book>(s => s
            //    .Index("books")
            //    .Query(q => q
            //        .MultiMatch(m => m
            //            .Fields(f => f
            //                .Field(p => p.BookInfoObj.BookTitle)
            //                .Field(p => p.BookInfoObj.BookDescription)
            //                .Field(p => p.BookInfoObj.Author)
            //                .Field(p => p.BookInfoObj.PublishDate)
            //            )
            //            .Query(searchTerm)
            //            .Fuzziness(Fuzziness.Auto)
            //        )
            //    )
            //);
            var searchResponse = _elasticClient.Search<Book>(s => s
             .Query(q => q
                 .Bool(b => b
                     .Should(
                         bs => bs.Match(m => m.Field("bookInfoObj.bookTitle").Query(searchTerm)),
                         bs => bs.Match(m => m.Field("bookInfoObj.bookDescription").Query(searchTerm)),
                         bs => bs.Match(m => m.Field("bookInfoObj.author").Query(searchTerm))
                         //bs => bs.Match(m => m.Field("bookInfoObj.publishDate").Query(searchTerm))
                     )
                 )
             )
         );
            return searchResponse.Documents;
        }
    }
}
