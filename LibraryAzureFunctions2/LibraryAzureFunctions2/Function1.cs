using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using LibraryAzureFunctions2.Models;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LibraryAzureFunctions2
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [FunctionName("AddBook")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {

            string categoryName = req.Query["Name"];
            string title = req.Query["Title"];
            string publisher = req.Query["Publisher"];
            string publishYearStr = req.Query["publishYear"];
            string description = req.Query["Description"];

            bool isEmpty = string.IsNullOrEmpty(categoryName) || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(publisher) || string.IsNullOrEmpty(description) || !int.TryParse(publishYearStr, out int publishYear);
            if (isEmpty)
            {
                return new BadRequestObjectResult("Please enter valid input for all fields");
            }
            // Access the database context
            using (var dbContext = new S22024_Group4_ProjectContext())
            {
                // Check if the category already exists
                var category = dbContext.Categories.FirstOrDefault(c => c.CategoryName == categoryName);

                // If the category does not exist, create a new one
                if (category == null)
                {
                    category = new Category { CategoryName = categoryName };
                    dbContext.Categories.Add(category);
                    await dbContext.SaveChangesAsync();  // Save to generate the CategoryId
                }

                // Create a new Book entry
                var newBook = new Book
                {
                    Title = title,
                    Publisher = publisher,
                    PublishYear = int.Parse(publishYearStr),
                    Description = description,
                    CategoryId = category.CategoryId,  // Use the existing or newly created category ID
                                                       // Add other fields as necessary
                };

                dbContext.Books.Add(newBook);
                await dbContext.SaveChangesAsync();

                // Return success response with the created book details
                return new OkObjectResult(newBook);
            }
        }

        [FunctionName("SoftDeleteBookCopy")]
        public static async Task<ActionResult> R([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            string isbn = req.Query["ISBN"];

            using (S22024_Group4_ProjectContext ctx = new S22024_Group4_ProjectContext())
            {
                BookCopy b = ctx.BookCopies.Where(bc => bc.Isbn == isbn).FirstOrDefault();
                b.IsActive = false;
                ctx.SaveChanges();

                return new OkObjectResult("BookCopy has been successfully made inactive");
            }

        }
        [FunctionName("SoftDeleteBorrowRecord")]
        public static async Task<ActionResult> sdbr([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            int recordNumber = int.Parse(req.Query["RecordNumber"]);

            using (S22024_Group4_ProjectContext ctx = new S22024_Group4_ProjectContext())
            {
                Borrow_Record br = ctx.Borrow_Records.Where(b => b.RecordNumber == recordNumber).FirstOrDefault();
                br.IsActive = false;
                ctx.SaveChanges();

                return new OkObjectResult("Borrow record has been successfully made inactive");
            }

        }
        [FunctionName("CalculateLateFee")]
        public static async Task<ActionResult> clf([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            int recordNumber = int.Parse(req.Query["RecordNumber"]);
            DateTime dueDate = DateTime.Parse(req.Query["DueDate"]);
            DateTime? returnDate = string.IsNullOrEmpty(req.Query["ReturnDate"]) ? (DateTime?)null : DateTime.Parse(req.Query["ReturnDate"]);
            decimal lateFee = decimal.Parse(req.Query["LateFee"]);
            decimal outstandingFee = decimal.Parse(req.Query["OutstandingFee"]);
            decimal amountPaid = decimal.Parse(req.Query["AmountPaid"]);


            using (S22024_Group4_ProjectContext ctx = new S22024_Group4_ProjectContext())
            {
                Borrow_Record br = ctx.Borrow_Records.FirstOrDefault(b => b.RecordNumber == recordNumber);
                if (br == null)
                {
                    return new NotFoundObjectResult("Borrow record not found");
                }

                // Calculate the number of late days if returnDate is null and dueDate is in the past
                if (!returnDate.HasValue && dueDate < DateTime.Now)
                {
                    int daysLate = (DateTime.Now - dueDate).Days;
                    outstandingFee = daysLate * lateFee; // outstanding fee equals the late fee multiplied by number of days that it is late
                }

                // Update record with the calculated fees
                br.OutstandingFee = outstandingFee - amountPaid;

                //Save changes
                ctx.SaveChanges();

                return new OkObjectResult("Outstanding fees have been updated.");
            }

        }
        [FunctionName("UpdateBook")]
        public static async Task<ActionResult> up([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            int bookId = int.Parse(req.Query["BookId"]);
            string title = req.Query["Title"];
            string publisher = req.Query["Publisher"];
            int publishYear = int.Parse(req.Query["PublishYear"]);
            string description = req.Query["Description"];
            int categoryId = int.Parse(req.Query["CategoryId"]);

            using (S22024_Group4_ProjectContext ctx = new S22024_Group4_ProjectContext())
            {
                Book b = ctx.Books.Where(bk => bk.BookId == bookId).FirstOrDefault();
                //update book values

                b.Title = title;
                b.Publisher = publisher;
                b.PublishYear = publishYear;
                b.Description = description;
                b.CategoryId = categoryId;

                ctx.SaveChanges();

                return new OkObjectResult("Updated successfully");
            }


        }
        [FunctionName("SearchProduct")]
        public IActionResult s([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            int bookId = int.Parse(req.Query["BookId"]);

            using (S22024_Group4_ProjectContext ctx = new S22024_Group4_ProjectContext())
            {
                Book b = ctx.Books.Where(bk => bk.BookId == bookId).FirstOrDefault();
                return new OkObjectResult(b);
            }

        }
        [FunctionName("ViewBooks")]
        public static async Task<ActionResult> vb([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            using (S22024_Group4_ProjectContext ctx = new S22024_Group4_ProjectContext())
            {
                List<Book> books = ctx.Books.ToList();
                return new OkObjectResult(books);
            }


        }
        [FunctionName("AddBorrowRecord")]
        public IActionResult abr([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            string isbn = req.Query["ISBN"];
            string email = req.Query["Email"];
            DateTime dueDate = DateTime.Parse(req.Query["DueDate"]);
            decimal lateFee = decimal.Parse(req.Query["LateFee"]);

            using (S22024_Group4_ProjectContext ctx = new S22024_Group4_ProjectContext())
            {
                BookCopy bc = ctx.BookCopies.Where(b => b.Isbn == isbn).FirstOrDefault();
                StudentUsers user = ctx.StudentUserss.Where(s => s.Email == email).FirstOrDefault();
                Borrow_Record borrowRecord = new Borrow_Record();
                borrowRecord.Isbn = bc.Isbn;
                borrowRecord.UserId= user.UserId;
                borrowRecord.DateBorrowed = DateTime.Now;
                borrowRecord.DueDate= dueDate;
                borrowRecord.ReturnedDate= null;
                borrowRecord.LateFee= lateFee;
                borrowRecord.OutstandingFee= null;
                borrowRecord.AmountPaid= null;
                borrowRecord.IsActive= true;
                bc.IsBorrowed = true;
                ctx.Borrow_Records.Add(borrowRecord);

                return new OkObjectResult(borrowRecord);
            }

        }
        [FunctionName("AddBorrowRecordAI")]
        public IActionResult abc([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Extract parameters from the query string
            string isbn = req.Query["ISBN"];
            string email = req.Query["Email"];
            DateTime dueDate = DateTime.Parse(req.Query["DueDate"]);
            decimal lateFee = decimal.Parse(req.Query["LateFee"]);

            using (S22024_Group4_ProjectContext ctx = new S22024_Group4_ProjectContext())
            {
                // Get existing BookCopy and StudentUser
                BookCopy bc = ctx.BookCopies.FirstOrDefault(b => b.Isbn == isbn);
                StudentUsers user = ctx.StudentUserss.FirstOrDefault(s => s.Email == email);

                if (bc == null || user == null)
                {
                    return new BadRequestObjectResult("Invalid ISBN or Email. The BookCopy or StudentUser does not exist.");
                }

                // Create a new BorrowRecord
                Borrow_Record borrowRecord = new Borrow_Record
                {
                    Isbn = bc.Isbn,
                    UserId = user.UserId,
                    DateBorrowed = DateTime.Now,
                    DueDate = dueDate,
                    ReturnedDate = null,
                    LateFee = lateFee,
                    OutstandingFee = null,
                    AmountPaid = null,
                    IsActive = true
                };

                // Mark BookCopy as borrowed
                bc.IsBorrowed = true;

                // Add the BorrowRecord and save changes
                ctx.Borrow_Records.Add(borrowRecord);
                ctx.SaveChanges();

                // Return the BorrowRecord DTO or custom shaped data
                var result = new
                {
                    borrowRecord.RecordNumber,
                    borrowRecord.Isbn,
                    borrowRecord.UserId,
                    borrowRecord.DateBorrowed,
                    borrowRecord.DueDate,
                    borrowRecord.LateFee
                };

                return new OkObjectResult(result);
            }
        }

    }
}
