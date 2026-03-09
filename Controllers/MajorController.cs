using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using WebCSharp.Models;

namespace WebCSharp.Controllers
{
    public class MajorController : Controller
    {
        private readonly string _connectionString;

        public MajorController(IConfiguration configuration)
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

        private List<SelectListItem> GetDegreeLevelSelectList()
        {
            return new List<SelectListItem>
            {
                new("-- Ch?n trình ?? --", ""),
                new("C? nhân (Bachelor)", "bachelor"),
                new("Th?c s? (Master)", "master"),
                new("Ti?n s? (PhD)", "phd")
            };
        }

        public IActionResult Index()
        {
            var list = new List<Major>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT m.id, m.faculty_id, f.name AS faculty_name, m.name, m.degree_level, m.required_credits, m.created_at, m.updated_at " +
                "FROM majors m LEFT JOIN faculties f ON m.faculty_id = f.id ORDER BY m.id", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Major
                {
                    Id = reader.GetInt32(0),
                    FacultyId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                    FacultyName = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Name = reader.GetString(3),
                    DegreeLevel = reader.IsDBNull(4) ? null : reader.GetString(4),
                    RequiredCredits = reader.IsDBNull(5) ? null : reader.GetInt32(5),
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
            ViewBag.DegreeLevels = GetDegreeLevelSelectList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Major model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Faculties = GetFacultySelectList();
                ViewBag.DegreeLevels = GetDegreeLevelSelectList();
                return View(model);
            }

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "INSERT INTO majors (faculty_id, name, degree_level, required_credits) VALUES (@faculty_id, @name, @degree_level, @required_credits)", conn);
            cmd.Parameters.AddWithValue("@faculty_id", (object?)model.FacultyId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@name", model.Name);
            cmd.Parameters.AddWithValue("@degree_level", (object?)model.DegreeLevel ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@required_credits", (object?)model.RequiredCredits ?? DBNull.Value);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Faculties = GetFacultySelectList();
            ViewBag.DegreeLevels = GetDegreeLevelSelectList();
            Major? item = null;
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT id, faculty_id, name, degree_level, required_credits FROM majors WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item = new Major
                {
                    Id = reader.GetInt32(0),
                    FacultyId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                    Name = reader.GetString(2),
                    DegreeLevel = reader.IsDBNull(3) ? null : reader.GetString(3),
                    RequiredCredits = reader.IsDBNull(4) ? null : reader.GetInt32(4)
                };
            }
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(Major model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Faculties = GetFacultySelectList();
                ViewBag.DegreeLevels = GetDegreeLevelSelectList();
                return View(model);
            }

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE majors SET faculty_id = @faculty_id, name = @name, degree_level = @degree_level, required_credits = @required_credits, updated_at = GETDATE() WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", model.Id);
            cmd.Parameters.AddWithValue("@faculty_id", (object?)model.FacultyId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@name", model.Name);
            cmd.Parameters.AddWithValue("@degree_level", (object?)model.DegreeLevel ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@required_credits", (object?)model.RequiredCredits ?? DBNull.Value);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Major? item = null;
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT m.id, m.name, m.degree_level, m.required_credits, f.name AS faculty_name " +
                "FROM majors m LEFT JOIN faculties f ON m.faculty_id = f.id WHERE m.id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item = new Major
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    DegreeLevel = reader.IsDBNull(2) ? null : reader.GetString(2),
                    RequiredCredits = reader.IsDBNull(3) ? null : reader.GetInt32(3),
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
            using var cmd = new SqlCommand("DELETE FROM majors WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }
    }
}
