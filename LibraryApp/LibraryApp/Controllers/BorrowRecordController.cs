using Microsoft.AspNetCore.Mvc;
using LibraryApp.Models;
using Newtonsoft.Json;

namespace LibraryApp.Controllers
{
    public class BorrowRecordController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<Borrow_Record> items = await viewAllBorrowRecordData();

            if (items == null)
            {
                items = new List<Borrow_Record>(); // Initialize to prevent null issues
            }

            return View(items);
        }
        public IActionResult AddBorrowRecord()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> addBorrowRecordData(Borrow_Record br)
        {
            string baseURL = "http://localhost:7066/api/AddBorrowRecord";
            string queryParameter = $"?ISBN={br.Isbn}&DueDate={br.DueDate}&LateFee={br.LateFee}&Email={br.UserId}";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"{baseURL}{queryParameter}");
            HttpContent content = response.Content;
            string data = await content.ReadAsStringAsync();
            return data.ToString();
        }
        [HttpPost]
        public async Task<List<Borrow_Record>> viewAllBorrowRecordData()
        {
            string baseURL = "http://localhost:7066/api/ViewAllBorrowRecords";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(baseURL);
            HttpContent content = response.Content;
            string data = await content.ReadAsStringAsync();
            List<Borrow_Record> records = JsonConvert.DeserializeObject<List<Borrow_Record>>(data);
            return records;
        }
    }
}
