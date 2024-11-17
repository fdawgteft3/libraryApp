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
                AspNetRole role = ctx.AspNetRoles.Where(r => r.Name == "Student").FirstOrDefault();
                if (us == null)
                {
                    return new BadRequestObjectResult("Please enter email of vaild user.");
                }
                else if (us.Roles.Contains(role)){

                    StudentUser st = new StudentUser();
                    st.UserId = us.Id;
                    st.FirstName = fName;
                    st.LastName = lName;
                    st.Email = us.Email;
                    st.Password = us.Password;
                    st.SelectedRoleIds = role.Id;
                    ctx.StudentUsers.Add(st);
                    ctx.SaveChanges();
                }
            }


                return new OkObjectResult("Successfully Added Student.");
        }
    }
}
