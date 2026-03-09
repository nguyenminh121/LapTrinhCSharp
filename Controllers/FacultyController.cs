using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebCSharp.Models;

namespace WebCSharp.Controllers
{
    public class FacultyController : Controller
    {
        private readonly string _connectionString;

        public FacultyController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            var list = new List<Faculty>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT id, name, address, created_at, updated_at FROM faculties ORDER BY id", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Faculty
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Address = reader.IsDBNull(2) ? null : reader.GetString(2),
                    CreatedAt = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    UpdatedAt = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
                });
            }
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Faculty model)
        {
            if (!ModelState.IsValid) return View(model);

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("INSERT INTO faculties (name, address) VALUES (@name, @address)", conn);
            cmd.Parameters.AddWithValue("@name", model.Name);
            cmd.Parameters.AddWithValue("@address", (object?)model.Address ?? DBNull.Value);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Faculty? faculty = null;
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT id, name, address, created_at, updated_at FROM faculties WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                faculty = new Faculty
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Address = reader.IsDBNull(2) ? null : reader.GetString(2),
                    CreatedAt = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    UpdatedAt = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
                };
            }
            if (faculty == null) return NotFound();
            return View(faculty);
        }

        [HttpPost]
        public IActionResult Edit(Faculty model)
        {
            if (!ModelState.IsValid) return View(model);

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("UPDATE faculties SET name = @name, address = @address, updated_at = GETDATE() WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", model.Id);
            cmd.Parameters.AddWithValue("@name", model.Name);
            cmd.Parameters.AddWithValue("@address", (object?)model.Address ?? DBNull.Value);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Faculty? faculty = null;
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT id, name, address FROM faculties WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                faculty = new Faculty
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Address = reader.IsDBNull(2) ? null : reader.GetString(2)
                };
            }
            if (faculty == null) return NotFound();
            return View(faculty);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM faculties WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }
    }
}
