using Digital_library.Models;

namespace Digital_library.Data
{
    public static class DbInitializer
    {
        public static void Initialize(BookContext context)
        {
            if (context.Books.Any()
                && context.Users.Any())
            {
                return;
            }

            DateTime today = DateTime.Today;

            var u1 = new User {Name = "User1", Email = "aaaa@aaa.com"};
            var u2 = new User {Name = "User2", Email = "bbbb@bbbbbb.com"};

            var books = new Book[]
            {
                new Book
                {
                    Name = "Book1",
                    BorrowedBy = u1,
                    Until = today.AddDays(14)
                },
                new Book
                {
                    Name = "Book2",
                    BorrowedBy = u2,
                    Until = today.AddHours(20)
                },
                new Book
                {
                    Name = "Book3",
                }
            };
            context.Books.AddRange(books);
            context.SaveChanges();
        }
    }
}