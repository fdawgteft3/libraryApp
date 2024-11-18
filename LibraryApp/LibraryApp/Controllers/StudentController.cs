using LibraryApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LibraryApp.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddStudent()
        {
            return View();
        }
        public IActionResult SearchStudent()
        {
            return View();
        }
        public IActionResult DisplayStudent(StudentUser user)
        {
            return View(user);
        }
        public async Task<IActionResult> DeleteStudent(string email)
        {
            string output = await deleteStudent(email);
            return RedirectToAction("DisplayAllStudents");
        }
        public async Task<IActionResult> DisplayAllStudents()
        {
            List<StudentUser> students = await viewStudents();

            if (students == null)
            {
                students = new List<StudentUser>(); // Initialize to prevent null issues
            }
            return View(students);
        }

        [HttpPost]
        public async Task<IActionResult> addStudentData(StudentUser st)
        {
            string baseURL = "http://localhost:7066/api/AddStudent";
            string queryParameter = $"?Email={st.Email}&FName={st.FirstName}&LName={st.LastName}";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"{baseURL}{queryParameter}");
            HttpContent content = response.Content;
            string data = await content.ReadAsStringAsync();
            return RedirectToAction("DisplayAllStudents");
        }

        [HttpGet]
        public async Task<ActionResult> searchStudentEmail(string email)
        {
            string baseURL = "http://localhost:7066/api/SearchStudentEmail";
            string queryParameter = $"?Email={email}";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"{baseURL}{queryParameter}");
            HttpContent content = response.Content;
            string data = await content.ReadAsStringAsync();
            StudentUser student = JsonConvert.DeserializeObject<StudentUser>(data);
            //if (student == null)
            //{
            //    // Handle case where item is null, e.g., return NotFound()
            //    return NotFound();
            //}

            // Pass the Product object to the view
            return View("DisplayStudent", student); // Ensure the view name matches
                                                    // return View("UpdateProduct", item);


        }
        [HttpPost]
        public async Task<List<StudentUser>> viewStudents()
        {
            string baseURL = "http://localhost:7066/api/ViewStudents";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(baseURL);
            HttpContent content = response.Content;
            string data = await content.ReadAsStringAsync();
            List<StudentUser> students = JsonConvert.DeserializeObject<List<StudentUser>>(data);
            return students;
        }
       
        [HttpPost]
        public async Task<string> deleteStudent(string email)
        {
            string baseURL = "http://localhost:7066/api/DeleteStudent";
            string queryParameter = $"?Email={email}";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"{baseURL}{queryParameter}");
            HttpContent content = response.Content;
            string data = await content.ReadAsStringAsync();
            return data;
        }
    }
}
    

