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
        //Student Azure Functions--------------------------------------------------------------------------
        [FunctionName("AddStudent")]// Function to add new student user
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            //Extract parameters from query string
            string email = req.Query["Email"];
            string fName = req.Query["FName"];
            string lName = req.Query["LName"];

            //Get user object and student role from database
            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                AspNetUser us = ctx.AspNetUsers.Where(user => user.UserName == email).FirstOrDefault();
                AspNetRole role = ctx.AspNetRoles.Where(r => r.Id == "8e21d754-1c57-4a5d-a584-e465d77bf3fe").FirstOrDefault();
                if (us == null)// if user not found - error message
                {
                    return new BadRequestObjectResult("Please enter email of vaild user.");
                }
                else// add user to student user table with name parameters and student role
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

            // return all student users in database
            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                List<StudentUser> students = ctx.StudentUsers.ToList();
                return new OkObjectResult(students);
            }

        }

        [FunctionName("SearchStudentEmail")]// Search student by Email
        public static async Task<IActionResult> RUN5(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {   //Extract parameter from query string
            string email = req.Query["Email"];

            if (string.IsNullOrWhiteSpace(email))// if email empty - show error
            {
                return new BadRequestObjectResult("Email parameter is required.");
            }

            //use database to find student user with matching email
            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                var matchingStudents = await ctx.StudentUsers.Where(x => x.Email.Contains(email)).ToListAsync();

                if (!matchingStudents.Any())// error message is no student has the email given
                {
                    return new NotFoundObjectResult("No students found with the provided Email.");
                }

                return new OkObjectResult(matchingStudents);
            }
        }
        [FunctionName("DeleteStudent")]//Function to delete user as a student
        public static async Task<IActionResult> RUN2([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            //Extrat parameters from the query string
            string Email = req.Query["Email"];
            
            //Search for student user, user it is linked to and the role of student in the database
            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                StudentUser su = ctx.StudentUsers.Where(x => x.Email == Email).FirstOrDefault();
                AspNetUser user = ctx.AspNetUsers.Where(u => u.Email == Email).FirstOrDefault();
                AspNetRole role = ctx.AspNetRoles.Where(r => r.Id == "8e21d754-1c57-4a5d-a584-e465d77bf3fe").FirstOrDefault();
                if (su != null)// if student found, delete as student and adjust user to remove student role
                {
                    ctx.StudentUsers.Remove(su);
                    user.Roles.Remove(role);
                    ctx.SaveChanges();
                    return new OkObjectResult("Deleted");
                }
                else// if student not found - show error message
                {
                    return new OkObjectResult("Sorry cant find this Student");
                }
            }
        }

        //BorrowRecord Azure Functions----------------------------------------------------------------------
        [FunctionName("ViewAllBorrowRecords")]//Function to return all borrow records in the database whether active or not
        public IActionResult Run4([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            
            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                // Get existing BorrowRecords from database
                List<Borrow_Record> records = ctx.Borrow_Records.ToList();
                
                return new OkObjectResult(records);
            }
        }
        [FunctionName("AddBorrowRecord")]// Function to add new borrow records
        public IActionResult abr([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            // Extract parameters from the query string
            int id = int.Parse(req.Query["BookId"]);
            string email = req.Query["Email"];
            DateTime dueDate = DateTime.Now.AddDays(14);
            decimal lateFee = decimal.Parse("2.00");

            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                // Get BookCopy and StudentUser
                BookCopy bc = ctx.BookCopies.FirstOrDefault(bc => bc.BookId == id && bc.IsBorrowed == false);
                StudentUser user = ctx.StudentUsers.FirstOrDefault(s => s.Email == email);

                if (bc == null)
                {
                    return new BadRequestObjectResult("Could not find an available copy of that title");
                }
                else if (bc.IsBorrowed == true)
                {
                    return new BadRequestObjectResult("This copy of the book is already being borrowed.");
                }else if(user == null)
                {
                    return new BadRequestObjectResult($"This student does not exist. Email: {email}");
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
        [FunctionName("FilterBorrowByStudent")]// Function to search borrow history of a student
        public IActionResult xyz([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            // Extract parameters from the query string
            string email = req.Query["Email"];

            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                // Get existing BorrowRecords according to User Email
                StudentUser user = ctx.StudentUsers.Where(x=>x.Email==email).FirstOrDefault();
                List<Borrow_Record> records = ctx.Borrow_Records.Where(b => b.UserId == user.UserId).ToList();

                return new OkObjectResult(records);
            }
        }
        [FunctionName("FilterBorrowByDateRange")]// Function to search borrow records within date range
        public IActionResult Run6([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            // Extract parameters from the query string
            DateTime startDate = DateTime.Parse(req.Query["StartDate"]);
            DateTime endDate = DateTime.Parse(req.Query["EndDate"]);
            if (startDate > DateTime.Now ||  endDate > DateTime.Now) // ensure the start date and end date are not set in the future
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

                    foreach (var record in records) // if records match the search parameters add to the results list
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
        [FunctionName("CalculateFee")]//Function calculates the fees owed on the borrow records
        public IActionResult Run7([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            // Extract parameters from the query string
            int recordId =int.Parse( req.Query["RecordId"]);

            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                // Get existing BorrowRecords according to Record Number
                Borrow_Record record = ctx.Borrow_Records.Where(b => b.RecordNumber == recordId).FirstOrDefault();

                //Checks that the book has not been returned yet and that the due date is in the past
                if(record.ReturnedDate == null && record.DueDate<DateTime.Now)
                {
                    int daysLate = (DateTime.Now - record.DueDate).Days;
                    if (daysLate >= 1)
                    {
                        record.OutstandingFee = daysLate * record.LateFee; // outstanding fee equals the late fee multiplied by number of days that it is late
                        ctx.SaveChanges();
                    }
                    
                }

                return new OkObjectResult(record);
            }
        }
        [FunctionName("ReturnBook")]//function to set Book Copy object to available to borrow
        public IActionResult Run8([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            // Extract parameters from the query string
            string isbn = req.Query["ISBN"];

            //Use database context to find specified book copy object
            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                // Get borrow record that contains the book copy object
                BookCopy bc = ctx.BookCopies.Where(b => b.Isbn == isbn).FirstOrDefault();
                Borrow_Record record = ctx.Borrow_Records.Where(b => b.Isbn == isbn && b.ReturnedDate==null).FirstOrDefault();
                
                //update objects to show the book is returned and the borrow record is either updated with the fees if late or set to inactive
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
        [FunctionName("DeleteRecord")]//A soft delete function for borrow records that change the record to inactive
        public static async Task<IActionResult> dr([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            int recordNumber = int.Parse(req.Query["RecordNumber"]);//requires a record number to specify the record needing to be deleted

            //Use database context to find specified borrow record object
            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                Borrow_Record br = ctx.Borrow_Records.Where(x => x.RecordNumber == recordNumber).FirstOrDefault();
                
                if (br != null)// if record found - set as inactive and save changes in database
                {
                    br.IsActive = false;
                    ctx.SaveChanges();
                    return new OkObjectResult("Made Record Inactive");
                }
                else //if record not found -show error message
                {
                    return new OkObjectResult("Sorry cannot find this Record");
                }
            }
        }
        [FunctionName("EditRecord")]//Function to update information on existing Borrow Record
        public static async Task<IActionResult> runner([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            //Get record number to pull information of specific borrow record
            int recordNumber = int.Parse(req.Query["RecordNumber"]);
            //Get Input required to updating information for variables that are accessible to change
            DateTime dueDate = DateTime.Parse(req.Query["DueDate"]);
            decimal outStandingFee = decimal.Parse(req.Query["OutStandingFee"]);
            decimal amountPaid = decimal.Parse(req.Query["AmountPaid"]);
            bool isActive = bool.Parse(req.Query["IsActive"]);

            //Use the database context to search database for object of borrow record
            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                Borrow_Record br = ctx.Borrow_Records.Where(b => b.RecordNumber == recordNumber).FirstOrDefault();
                if (br == null)//if borrow record not found - show error message
                {
                    return new BadRequestObjectResult("Borrow Record was not found");
                }else if (dueDate < DateTime.Now) // if dueDate is trying to be changed to a date in the past - show error message
                {
                    return new BadRequestObjectResult("Due date cannot be updated to a past date.");
                }
                else //Update borrow record with information that the user input and save changes to database
                {
                    br.DueDate = dueDate;
                    br.OutstandingFee = outStandingFee;
                    br.AmountPaid = amountPaid;
                    br.IsActive = isActive;
                    ctx.SaveChanges();
                    return new OkObjectResult(br);
                }
            }
        }
    }
}
