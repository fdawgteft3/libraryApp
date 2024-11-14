using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using LibraryAzureFunctions.Models;
using Google.Protobuf;

namespace LibraryAzureFunctions
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("AddBook")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {

            string categoryName = req.Query["Name"];
            string title = req.Query["Title"];
            string publisher = req.Query["Publisher"];
            string publishYearStr = req.Query["publishYear"];
            string description = req.Query["Description"];
            
            bool isEmpty = string.IsNullOrEmpty(categoryName) || string.IsNullOrEmpty(title)||string.IsNullOrEmpty(publisher)||string.IsNullOrEmpty(description)|| !int.TryParse(publishYearStr, out int publishYear);
            if (isEmpty)
            {
                return new BadRequestObjectResult("Please enter valid input for all fields");
            }
            // Access the database context
            using (var dbContext = new S22024Group4ProjectContext()) {
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
        
        [Function("SoftDeleteBookCopy")]
        public static async Task<ActionResult> R([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            string isbn = req.Query["ISBN"];

            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                BookCopy b = ctx.BookCopies.Where(bc => bc.Isbn == isbn).FirstOrDefault();
                b.IsActive = false;
                ctx.SaveChanges();

                return new OkObjectResult("BookCopy has been successfully made inactive");
            }

        }
        [Function("SoftDeleteBorrowRecord")]
        public static async Task<ActionResult> sdbr([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            int recordNumber = int.Parse(req.Query["RecordNumber"]);

            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                BorrowRecord br = ctx.BorrowRecords.Where(b => b.RecordNumber == recordNumber).FirstOrDefault();
                br.IsActive = false;
                ctx.SaveChanges();

                return new OkObjectResult("Borrow record has been successfully made inactive");
            }

        }
        [Function("CalculateLateFee")]
        public static async Task<ActionResult> clf([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            int recordNumber = int.Parse(req.Query["RecordNumber"]);
            DateTime dueDate = DateTime.Parse(req.Query["DueDate"]);
            DateTime? returnDate = string.IsNullOrEmpty(req.Query["ReturnDate"]) ? (DateTime?)null : DateTime.Parse(req.Query["ReturnDate"]);
            decimal lateFee = decimal.Parse(req.Query["LateFee"]);
            decimal outstandingFee = decimal.Parse(req.Query["OutstandingFee"]);
            decimal amountPaid = decimal.Parse(req.Query["AmountPaid"]);


            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                BorrowRecord br = ctx.BorrowRecords.FirstOrDefault(b => b.RecordNumber == recordNumber);
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
                br.OustandingFee = outstandingFee - amountPaid;

                //Save changes
                ctx.SaveChanges();

                return new OkObjectResult("Outstanding fees have been updated.");
            }

        }
    }
   
}
