using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using WebCSharp.Models;

namespace WebCSharp.Controllers
{
    public class InstructorController : Controller
    {
        private readonly string _connectionString;

        public InstructorController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private List<SelectListItem> GetFacultySelectList()
        {
            var items = new List<SelectListItem> { new("-- Ch?n khoa --", "") };
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT id, name FROM faculties ORDER BY name", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new SelectListItem(reader.GetString(1), reader.GetInt32(0).ToString()));
            }
            return items;
        }

        public IActionResult Index()
        {
            var list = new List<Instructor>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT i.id, i.name, i.email, i.title, i.faculty_id, f.name AS faculty_name, i.created_at, i.updated_at " +
                "FROM instructors i LEFT JOIN faculties f ON i.faculty_id = f.id ORDER BY i.id", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Instructor
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Title = reader.IsDBNull(3) ? null : reader.GetString(3),
                    FacultyId = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                    FacultyName = reader.IsDBNull(5) ? null : reader.GetString(5),
                    CreatedAt = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                    UpdatedAt = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
                });
            }
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Faculties = GetFacultySelectList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Instructor model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Faculties = GetFacultySelectList();
                return View(model);
            }

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "INSERT INTO instructors (name, email, title, faculty_id) VALUES (@name, @email, @title, @faculty_id)", conn);
            cmd.Parameters.AddWithValue("@name", model.Name);
            cmd.Parameters.AddWithValue("@email", (object?)model.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@title", (object?)model.Title ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@faculty_id", (object?)model.FacultyId ?? DBNull.Value);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Faculties = GetFacultySelectList();
            Instructor? item = null;
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT id, name, email, title, faculty_id FROM instructors WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item = new Instructor
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Title = reader.IsDBNull(3) ? null : reader.GetString(3),
                    FacultyId = reader.IsDBNull(4) ? null : reader.GetInt32(4)
                };
            }
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(Instructor model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Faculties = GetFacultySelectList();
                return View(model);
            }

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE instructors SET name = @name, email = @email, title = @title, faculty_id = @faculty_id, updated_at = GETDATE() WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", model.Id);
            cmd.Parameters.AddWithValue("@name", model.Name);
            cmd.Parameters.AddWithValue("@email", (object?)model.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@title", (object?)model.Title ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@faculty_id", (object?)model.FacultyId ?? DBNull.Value);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Instructor? item = null;
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT i.id, i.name, i.email, i.title, f.name AS faculty_name " +
                "FROM instructors i LEFT JOIN faculties f ON i.faculty_id = f.id WHERE i.id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item = new Instructor
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Title = reader.IsDBNull(3) ? null : reader.GetString(3),
                    FacultyName = reader.IsDBNull(4) ? null : reader.GetString(4)
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
            using var cmd = new SqlCommand("DELETE FROM instructors WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }
    }
}
