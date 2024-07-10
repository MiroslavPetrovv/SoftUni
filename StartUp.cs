namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;
    using System.Net.WebSockets;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);
            Console.WriteLine(CountBooks(db,12));
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            Enum.TryParse(command, true, out AgeRestriction ageRestriction);
            var booksTitle = context.Books
                .Where(b=> b.AgeRestriction == ageRestriction)
                .Select(b => b.Title)
                .OrderBy(t => t)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var book in booksTitle)
            {
                sb.AppendLine($"{book}");
            }
            return sb.ToString().TrimEnd();        
        }

        public static  string GetGoldenBooks(BookShopContext context)
        {
            
            var booksTitle = context.Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .Select(b => new
                {
                    b.Title,
                    b.BookId
                })         
                .OrderBy(b=>b.BookId)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var book in booksTitle)
            {
                sb.AppendLine(book.Title);
            }
            return sb.ToString().TrimEnd();
           

        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var booksTitle = context.Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    Title = b.Title,
                    Price = b.Price,
                })
                .OrderByDescending(b => b.Price)
                .ToArray();

            StringBuilder sb  = new StringBuilder();

            foreach (var book in booksTitle)
            {
                sb.AppendLine($"{book.Title} - ${book.Price}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
         

            var booksTitle = context.Books
                .Where(b=>b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year !=year)
                .Select(b=> new
                {
                    Id=b.BookId,
                    Title=b.Title,
                })
                .OrderBy(b=>b.Id)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var book in booksTitle)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            

            // Filter books that do not have any of the categories in the list
            var booksTitle = context.Books
                .Where(b => b.BookCategories.All(c=> input.Contains(c.Category.Name)))
                .Select(b => b.Title)
                .OrderBy(t=>t)
                .ToArray();

            return string.Join(Environment.NewLine, booksTitle);
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var parseDate = DateTime.ParseExact(date, "dd-MM-yyyy",CultureInfo.InvariantCulture);

            var booksTitles = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value < parseDate)
                .Select(b => new
                {
                    Title = b.Title,
                    editionType = b.EditionType,
                    Price = b.Price,
                    ReleaseDate = b.ReleaseDate
                    //ReleaseDate =b.ReleaseDate.HasValue ? 
                    //    b.ReleaseDate.Value.ToString("dd-MM-yyyy") : string.Empty
                })
                .OrderByDescending(b=>b.ReleaseDate)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var bt in booksTitles)
            {
                sb.AppendLine($"{bt.Title} - {bt.editionType} - ${bt.Price}");
            }

            return sb.ToString().TrimEnd();




        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var bookAuhtors = context.Authors
                .Where(a=> a .FirstName.Substring(a.FirstName.Length-input.Length) == input)
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName
                })
                .OrderBy(a => a.FullName)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var a in bookAuhtors)
            {
                sb.AppendLine($"{a.FullName}");
            }

            return sb.ToString().TrimEnd();
            
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var booksTitle = context.Books
                .Where(b => b.Title.Contains(input))
                .Select(b => b.Title)
                .OrderBy(t => t)
                .ToArray();

            return string.Join(Environment.NewLine, booksTitle);
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var bookTitle = context.Books
                .Where(b => b.Author.LastName.ToLower()
                 .StartsWith(input.ToLower()))
                .Select(b => new
                {
                    Title = b.Title,
                    AuthorName = b.Author.FirstName + " " + b.Author.LastName,
                    Id = b.BookId

                })
                .OrderBy(b => b.Id)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var a in bookTitle)
            {
                sb.AppendLine($"{a.Title} ({a.AuthorName})");
            }

            return sb.ToString().TrimEnd();
       
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var booksCount = context.Books
              .Where(b => b.Title.Length > lengthCheck).Count();

            return booksCount;
        }
    }
}


