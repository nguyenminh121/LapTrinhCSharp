using LuyenTap.Models;
using LuyenTap.Services;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LuyenTap.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStudentDatabaseReader _studentDatabaseReader;
        private readonly IConfiguration _configuration;

        public HomeController(IStudentDatabaseReader studentDatabaseReader, IConfiguration configuration)
        {
            _studentDatabaseReader = studentDatabaseReader;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = await _studentDatabaseReader.ReadAllAsync(cancellationToken);
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TinhLaiSuat()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Search(string? keyword, CancellationToken cancellationToken)
        {
            var model = new SearchProductsViewModel
            {
                Keyword = keyword?.Trim() ?? string.Empty
            };

            if (string.IsNullOrWhiteSpace(model.Keyword))
            {
                return View(model);
            }

            var connectionString = _configuration.GetConnectionString("StoreConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                model.ErrorMessage = "Connection string StoreConnection chua duoc cau hinh.";
                return View(model);
            }

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync(cancellationToken);

                const string sql = @"
                    SELECT ModelNumber, ModelName
                    FROM dbo.Products
                    WHERE ModelNumber LIKE @Keyword OR ModelName LIKE @Keyword
                    ORDER BY ModelName";

                await using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Keyword", $"%{model.Keyword}%");

                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    model.Results.Add(new ProductSearchResult
                    {
                        ModelNumber = reader["ModelNumber"]?.ToString() ?? string.Empty,
                        ModelName = reader["ModelName"]?.ToString() ?? string.Empty
                    });
                }
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"Khong the truy van bang Products trong DB store: {ex.Message}";
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
