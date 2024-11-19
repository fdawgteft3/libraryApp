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
        public IActionResult Search()
        {
            return View();
        }
        public IActionResult ViewBorrowSearchResult(List<Borrow_Record> records)
        {

            return View(records);
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
            return RedirectToAction("Index");
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
        [HttpPost]
        public async Task<ActionResult> searchRecordsByStudent(StudentUser user)
        {
            string baseURL = "http://localhost:7066/api/FilterBorrowByStudent";
            string queryParameter = $"?Email={user.Email}";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"{baseURL}{queryParameter}");
            HttpContent content = response.Content;
            string data = await content.ReadAsStringAsync();
            List<Borrow_Record> records = JsonConvert.DeserializeObject<List<Borrow_Record>>(data);
            return RedirectToAction("ViewBorrowSearchResult", records);
        }
        [HttpPost]
        public async Task<ActionResult> searchRecordsByDateRange(DateTime startDate, DateTime endDate)
        {
            string baseURL = "http://localhost:7066/api/FilterBorrowByDateRange";
            string queryParameter = $"?StartDate={startDate}&EndDate={endDate}";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"{baseURL}{queryParameter}");
            HttpContent content = response.Content;
            string data = await content.ReadAsStringAsync();
            List<Borrow_Record> records = JsonConvert.DeserializeObject<List<Borrow_Record>>(data);
            return RedirectToAction("ViewBorrowSearchResult", records);
        }
        [HttpPost]
        public async Task<IActionResult> deleteRecord(Borrow_Record br)
        {
            string baseURL = "http://localhost:7066/api/DeleteRecord";
            string queryParameter = $"?RecordNumber={br.RecordNumber}";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"{baseURL}{queryParameter}");
            HttpContent content = response.Content;
            string data = await content.ReadAsStringAsync();
            return RedirectToAction("Index");
        }
        
    }
}
