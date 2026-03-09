using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using WebCSharp.Models;

namespace WebCSharp.Controllers
{
    public class CurriculumController : Controller
    {
        private readonly string _connectionString;

        public CurriculumController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private List<SelectListItem> GetMajorSelectList()
        {
            var items = new List<SelectListItem> { new("-- Ch?n ngành --", "") };
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT id, name FROM majors ORDER BY name", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new SelectListItem(reader.GetString(1), reader.GetInt32(0).ToString()));
            }
            return items;
        }

        private List<SelectListItem> GetIntakeSelectList()
        {
            var items = new List<SelectListItem> { new("-- Ch?n khóa --", "") };
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT id, code, year FROM intakes ORDER BY year DESC, code", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var label = reader.GetString(1);
                if (!reader.IsDBNull(2)) label += $" ({reader.GetInt32(2)})";
                items.Add(new SelectListItem(label, reader.GetInt32(0).ToString()));
            }
            return items;
        }

        public IActionResult Index()
        {
            var list = new List<Curriculum>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT c.id, c.major_id, m.name AS major_name, c.intake_id, i.code AS intake_code, c.total_credits, c.created_at " +
                "FROM curriculums c LEFT JOIN majors m ON c.major_id = m.id LEFT JOIN intakes i ON c.intake_id = i.id ORDER BY c.id", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Curriculum
                {
                    Id = reader.GetInt32(0),
                    MajorId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                    MajorName = reader.IsDBNull(2) ? null : reader.GetString(2),
                    IntakeId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    IntakeCode = reader.IsDBNull(4) ? null : reader.GetString(4),
                    TotalCredits = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                    CreatedAt = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
                });
            }
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Majors = GetMajorSelectList();
            ViewBag.Intakes = GetIntakeSelectList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Curriculum model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Majors = GetMajorSelectList();
                ViewBag.Intakes = GetIntakeSelectList();
                return View(model);
            }

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "INSERT INTO curriculums (major_id, intake_id, total_credits) VALUES (@major_id, @intake_id, @total_credits)", conn);
            cmd.Parameters.AddWithValue("@major_id", (object?)model.MajorId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@intake_id", (object?)model.IntakeId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@total_credits", (object?)model.TotalCredits ?? DBNull.Value);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Majors = GetMajorSelectList();
            ViewBag.Intakes = GetIntakeSelectList();
            Curriculum? item = null;
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT id, major_id, intake_id, total_credits FROM curriculums WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item = new Curriculum
                {
                    Id = reader.GetInt32(0),
                    MajorId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                    IntakeId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                    TotalCredits = reader.IsDBNull(3) ? null : reader.GetInt32(3)
                };
            }
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(Curriculum model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Majors = GetMajorSelectList();
                ViewBag.Intakes = GetIntakeSelectList();
                return View(model);
            }

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE curriculums SET major_id = @major_id, intake_id = @intake_id, total_credits = @total_credits WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", model.Id);
            cmd.Parameters.AddWithValue("@major_id", (object?)model.MajorId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@intake_id", (object?)model.IntakeId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@total_credits", (object?)model.TotalCredits ?? DBNull.Value);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Curriculum? item = null;
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT c.id, m.name AS major_name, i.code AS intake_code, c.total_credits " +
                "FROM curriculums c LEFT JOIN majors m ON c.major_id = m.id LEFT JOIN intakes i ON c.intake_id = i.id WHERE c.id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item = new Curriculum
                {
                    Id = reader.GetInt32(0),
                    MajorName = reader.IsDBNull(1) ? null : reader.GetString(1),
                    IntakeCode = reader.IsDBNull(2) ? null : reader.GetString(2),
                    TotalCredits = reader.IsDBNull(3) ? null : reader.GetInt32(3)
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
            using var cmd = new SqlCommand("DELETE FROM curriculums WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }
    }
}
