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

namespace LibraryFunctions
{
    public static class Function1
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
                else{
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
        public static async Task<IActionResult> RUN2([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            //_logger.LogInformation("C# HTTP trigger function processed a request.");
            string Email = req.Query["Email"];
            //return new OkObjectResult("Ssortry cant find product" + pID);

            using (S22024Group4ProjectContext ctx = new S22024Group4ProjectContext())
            {
                StudentUser su = ctx.StudentUsers.Where(x => x.Email == Email).FirstOrDefault();
                AspNetUser user = ctx.AspNetUsers.Where(u=> u.Email == Email).FirstOrDefault(); 
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
    }
}
