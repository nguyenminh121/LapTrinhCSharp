using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebCSharp.Models;

namespace WebCSharp.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString;

        public HomeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            var counts = new Dictionary<string, int>();
            string[] tables = ["faculties", "instructors", "majors", "intakes", "courses", "curriculums", "syllabi"];

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            foreach (var table in tables)
            {
                using var cmd = new SqlCommand($"SELECT COUNT(*) FROM [{table}]", conn);
                try
                {
                    counts[table] = (int)cmd.ExecuteScalar();
                }
                catch
                {
                    counts[table] = 0;
                }
            }

            ViewBag.Counts = counts;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

