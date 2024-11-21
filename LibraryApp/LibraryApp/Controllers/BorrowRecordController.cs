using Microsoft.AspNetCore.Mvc;
using LibraryApp.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace LibraryApp.Controllers
{
    public class BorrowRecordController : Controller
    {
        private readonly S22024Group4ProjectContext _context;

        public BorrowRecordController(S22024Group4ProjectContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page ?? 1;
            List<Borrow_Record> items = await viewAllBorrowRecordData();

            return View(items.ToPagedList(pageNumber, 5));
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
        public IActionResult EditRecord(Borrow_Record br)
        {

            return View(br);
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
            return View("ViewBorrowSearchResult", records);
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
            return View("ViewBorrowSearchResult", records);
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
        [HttpPost]
        public async Task<IActionResult> updateFees(Borrow_Record br)
        {
            string baseURL = "http://localhost:7066/api/CalculateFee";
            string queryParameter = $"?RecordId={br.RecordNumber}";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"{baseURL}{queryParameter}");
            HttpContent content = response.Content;
            string data = await content.ReadAsStringAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> updateRecordData(Borrow_Record br)
        {
            string baseURL = "http://localhost:7066/api/EditRecord";
            string queryParameter = $"?RecordNumber={br.RecordNumber}&DueDate={br.DueDate}&OutStandingFee={br.OutstandingFee}&AmountPaid={br.AmountPaid}&IsActive={br.IsActive}";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"{baseURL}{queryParameter}");
            HttpContent content = response.Content;
            string data = await content.ReadAsStringAsync();
            return RedirectToAction("Index");
        }

    }
}
