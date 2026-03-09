using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebCSharp.Models;

namespace WebCSharp.Controllers
{
    public class IntakeController : Controller
    {
        private readonly string _connectionString;

        public IntakeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            var list = new List<Intake>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT id, code, year, created_at FROM intakes ORDER BY id", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Intake
                {
                    Id = reader.GetInt32(0),
                    Code = reader.GetString(1),
                    Year = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                    CreatedAt = reader.IsDBNull(3) ? null : reader.GetDateTime(3)
                });
            }
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Intake model)
        {
            if (!ModelState.IsValid) return View(model);

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("INSERT INTO intakes (code, year) VALUES (@code, @year)", conn);
            cmd.Parameters.AddWithValue("@code", model.Code);
            cmd.Parameters.AddWithValue("@year", (object?)model.Year ?? DBNull.Value);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Intake? item = null;
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT id, code, year FROM intakes WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item = new Intake
                {
                    Id = reader.GetInt32(0),
                    Code = reader.GetString(1),
                    Year = reader.IsDBNull(2) ? null : reader.GetInt32(2)
                };
            }
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(Intake model)
        {
            if (!ModelState.IsValid) return View(model);

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("UPDATE intakes SET code = @code, year = @year WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", model.Id);
            cmd.Parameters.AddWithValue("@code", model.Code);
            cmd.Parameters.AddWithValue("@year", (object?)model.Year ?? DBNull.Value);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Intake? item = null;
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT id, code, year FROM intakes WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item = new Intake
                {
                    Id = reader.GetInt32(0),
                    Code = reader.GetString(1),
                    Year = reader.IsDBNull(2) ? null : reader.GetInt32(2)
                };
            }
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM intakes WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }
    }
}
