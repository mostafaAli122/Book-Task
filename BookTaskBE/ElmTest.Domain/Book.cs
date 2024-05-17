namespace ElmTest.Domain
{
    public class Book
    {
        public int BookId { get; set; }
        public string BookInfo { get; set; }
        public BookInfo BookInfoObj { get; set; } = new();
        public DateTime LastModified { get; set; }
    }
    public class BookInfo
    {
        public string BookTitle { get; set; }
        public string BookDescription { get; set; }
        public string Author { get; set; }
        public DateTime PublishDate { get; set; }
        public string CoverBase64 { get; set; }
    }
}
