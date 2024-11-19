using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using LibraryFunctions.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace LibraryFunctions
{

    public class Function1
    {
        [FunctionName("AddStudent")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string email = req.Query["Email"];
            string fName = req.Query["FName"];
            string lName = req.Query["LName"];


            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                AspNetUser us = ctx.AspNetUsers.Where(user => user.UserName == email).FirstOrDefault();
                AspNetRole role = ctx.AspNetRoles.Where(r => r.Id == "8e21d754-1c57-4a5d-a584-e465d77bf3fe").FirstOrDefault();
                if (us == null)
                {
                    return new BadRequestObjectResult("Please enter email of vaild user.");
                }
                else
                {
                    StudentUser st = new StudentUser();
                    st.UserId = us.Id;
                    st.FirstName = fName;
                    st.LastName = lName;
                    st.Email = us.Email;
                    st.Password = us.PasswordHash;
                    st.SelectedRoleIds = "8e21d754-1c57-4a5d-a584-e465d77bf3fe";
                    us.Roles.Add(role);
                    ctx.StudentUsers.Add(st);

                    ctx.SaveChanges();
                    return new OkObjectResult("Successfully Added Student.");
                }
            }

            return new OkObjectResult("Maybe Check cs Unsuccessful");
        }
        [FunctionName("ViewStudents")]
        public static async Task<IActionResult> Run1(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                List<StudentUser> students = ctx.StudentUsers.ToList();
                return new OkObjectResult(students);
            }

        }

        [FunctionName("SearchStudentEmail")]
        public static async Task<IActionResult> RUN5(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            string email = req.Query["Email"];

            if (string.IsNullOrWhiteSpace(email))
            {
                return new BadRequestObjectResult("Name parameter is required.");
            }

            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                var matchingStudents = await ctx.StudentUsers.Where(x => x.Email.Contains(email)).ToListAsync();

                if (!matchingStudents.Any())
                {
                    return new NotFoundObjectResult("No students found with the provided Email.");
                }

                return new OkObjectResult(matchingStudents);
            }
        }
        [FunctionName("DeleteStudent")]
        public static async Task<IActionResult> RUN2([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string Email = req.Query["Email"];
            //return new OkObjectResult("Ssortry cant find product" + pID);

            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                StudentUser su = ctx.StudentUsers.Where(x => x.Email == Email).FirstOrDefault();
                AspNetUser user = ctx.AspNetUsers.Where(u => u.Email == Email).FirstOrDefault();
                AspNetRole role = ctx.AspNetRoles.Where(r => r.Id == "8e21d754-1c57-4a5d-a584-e465d77bf3fe").FirstOrDefault();
                if (su != null)
                {
                    ctx.StudentUsers.Remove(su);
                    user.Roles.Remove(role);
                    ctx.SaveChanges();
                    return new OkObjectResult("Deleted");
                }
                else
                {
                    return new OkObjectResult("Sorry cant find this Student");
                }
            }
        }
        [FunctionName("ViewAllBorrowRecords")]
        public IActionResult Run4([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            
            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                // Get existing BorrowRecords according to UserId
                List<Borrow_Record> records = ctx.Borrow_Records.ToList();

                return new OkObjectResult(records);
            }
        }
        [FunctionName("AddBorrowRecord")]
        public IActionResult abr([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            // Extract parameters from the query string
            string isbn = req.Query["ISBN"];
            string email = req.Query["Email"];
            DateTime dueDate = DateTime.Parse(req.Query["DueDate"]);
            decimal lateFee = decimal.Parse(req.Query["LateFee"]);

            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                // Get existing BookCopy and StudentUser
                BookCopy bc = ctx.BookCopies.FirstOrDefault(b => b.Isbn == isbn);
                StudentUser user = ctx.StudentUsers.FirstOrDefault(s => s.Email == email);

                if (bc == null)
                {
                    return new BadRequestObjectResult("Invalid ISBN. The BookCopy does not exist.");
                }
                else if (bc.IsBorrowed == true)
                {
                    return new BadRequestObjectResult("This copy of the book is already being borrowed.");
                }else if(user == null)
                {
                    return new BadRequestObjectResult("This student does not exist.");
                }
                else
                {
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
        [FunctionName("FilterBorrowByStudent")]
        public IActionResult Run2([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            // Extract parameters from the query string
            string userId = req.Query["UserId"];

            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                // Get existing BorrowRecords according to UserId
                List<Borrow_Record> records = ctx.Borrow_Records.Where(b => b.UserId == userId).ToList();

                return new OkObjectResult(records);
            }
        }
        [FunctionName("FilterBorrowByDateRange")]
        public IActionResult Run6([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            // Extract parameters from the query string
            DateTime startDate = DateTime.Parse(req.Query["StartDate"]);
            DateTime endDate = DateTime.Parse(req.Query["EndDate"]);
            if (startDate > DateTime.Now ||  endDate > DateTime.Now)
            {
                return new BadRequestObjectResult("Dates must not be in the future. Use YYYY-MM-DD format");
            }
            else
            {
                using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
                {
                    // Get existing BorrowRecords according to Date Range
                    List<Borrow_Record> records = ctx.Borrow_Records.ToList();

                    //Create List variable to store search results
                    List<Borrow_Record> results = new List<Borrow_Record>();

                    foreach (var record in records)
                    {
                        if (record.DateBorrowed >= startDate && record.DateBorrowed <= endDate)
                        {
                            results.Add(record);
                        }
                    }

                    return new OkObjectResult(results);
                }
            }
            
        }
        [FunctionName("CalculateFee")]
        public IActionResult Run7([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            // Extract parameters from the query string
            int recordId =int.Parse( req.Query["RecordId"]);

            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                // Get existing BorrowRecords according to UserId
                Borrow_Record record = ctx.Borrow_Records.Where(b => b.RecordNumber == recordId).FirstOrDefault();
                if(record.ReturnedDate == null && record.DueDate<DateTime.Now)
                {
                    int daysLate = (DateTime.Now - record.DueDate).Days;
                    record.OutstandingFee = daysLate * record.LateFee; // outstanding fee equals the late fee multiplied by number of days that it is late
                    ctx.SaveChanges();
                }

                return new OkObjectResult("");
            }
        }
        [FunctionName("ReturnBook")]
        public IActionResult Run8([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            // Extract parameters from the query string
            int recordId = int.Parse(req.Query["RecordId"]);

            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                // Get existing BorrowRecords according to UserId
                Borrow_Record record = ctx.Borrow_Records.Where(b => b.RecordNumber == recordId).FirstOrDefault();
                BookCopy bc = ctx.BookCopies.Where(b => b.Isbn == record.Isbn).FirstOrDefault();
                record.ReturnedDate = DateTime.Now;
                bc.IsBorrowed = false;
                if(record.DueDate < record.ReturnedDate)
                {
                    int daysLate = (DateTime.Now - record.DueDate).Days;
                    record.OutstandingFee = daysLate * record.LateFee;
                }
                else
                {
                    record.IsActive = false;
                }
                ctx.SaveChanges();

                return new OkObjectResult(record);
            }
        }
    }
}
