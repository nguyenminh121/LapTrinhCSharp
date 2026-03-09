using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using WebCSharp.Models;

namespace WebCSharp.Controllers
{
    public class SyllabusController : Controller
    {
        private readonly string _connectionString;

        public SyllabusController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private List<SelectListItem> GetCourseSelectList()
        {
            var items = new List<SelectListItem> { new("-- Ch?n môn h?c --", "") };
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT id, code, name_vi FROM courses ORDER BY code", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var label = reader.GetString(1);
                if (!reader.IsDBNull(2)) label += $" - {reader.GetString(2)}";
                items.Add(new SelectListItem(label, reader.GetInt32(0).ToString()));
            }
            return items;
        }

        private List<SelectListItem> GetLanguageSelectList()
        {
            return new List<SelectListItem>
            {
                new("-- Ch?n ngôn ng? --", ""),
                new("Ti?ng Vi?t", "vi"),
                new("Ti?ng Anh", "en")
            };
        }

        public IActionResult Index()
        {
            var list = new List<Syllabus>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT s.id, s.course_id, CONCAT(c.code, ' - ', c.name_vi) AS course_name, s.version, s.language, s.description, s.adjustment_date, s.approved_by, s.created_at " +
                "FROM syllabi s LEFT JOIN courses c ON s.course_id = c.id ORDER BY s.id", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Syllabus
                {
                    Id = reader.GetInt32(0),
                    CourseId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                    CourseName = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Version = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Language = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Description = reader.IsDBNull(5) ? null : reader.GetString(5),
                    AdjustmentDate = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                    ApprovedBy = reader.IsDBNull(7) ? null : reader.GetString(7),
                    CreatedAt = reader.IsDBNull(8) ? null : reader.GetDateTime(8)
                });
            }
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Courses = GetCourseSelectList();
            ViewBag.Languages = GetLanguageSelectList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Syllabus model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Courses = GetCourseSelectList();
                ViewBag.Languages = GetLanguageSelectList();
                return View(model);
            }

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "INSERT INTO syllabi (course_id, version, language, description, adjustment_date, approved_by) " +
                "VALUES (@course_id, @version, @language, @description, @adjustment_date, @approved_by)", conn);
            cmd.Parameters.AddWithValue("@course_id", (object?)model.CourseId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@version", (object?)model.Version ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@language", (object?)model.Language ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@description", (object?)model.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@adjustment_date", (object?)model.AdjustmentDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@approved_by", (object?)model.ApprovedBy ?? DBNull.Value);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Courses = GetCourseSelectList();
            ViewBag.Languages = GetLanguageSelectList();
            Syllabus? item = null;
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT id, course_id, version, language, description, adjustment_date, approved_by FROM syllabi WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item = new Syllabus
                {
                    Id = reader.GetInt32(0),
                    CourseId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                    Version = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Language = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                    AdjustmentDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                    ApprovedBy = reader.IsDBNull(6) ? null : reader.GetString(6)
                };
            }
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(Syllabus model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Courses = GetCourseSelectList();
                ViewBag.Languages = GetLanguageSelectList();
                return View(model);
            }

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE syllabi SET course_id = @course_id, version = @version, language = @language, " +
                "description = @description, adjustment_date = @adjustment_date, approved_by = @approved_by WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", model.Id);
            cmd.Parameters.AddWithValue("@course_id", (object?)model.CourseId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@version", (object?)model.Version ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@language", (object?)model.Language ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@description", (object?)model.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@adjustment_date", (object?)model.AdjustmentDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@approved_by", (object?)model.ApprovedBy ?? DBNull.Value);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Syllabus? item = null;
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT s.id, CONCAT(c.code, ' - ', c.name_vi) AS course_name, s.version, s.language, s.approved_by " +
                "FROM syllabi s LEFT JOIN courses c ON s.course_id = c.id WHERE s.id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item = new Syllabus
                {
                    Id = reader.GetInt32(0),
                    CourseName = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Version = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Language = reader.IsDBNull(3) ? null : reader.GetString(3),
                    ApprovedBy = reader.IsDBNull(4) ? null : reader.GetString(4)
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
            using var cmd = new SqlCommand("DELETE FROM syllabi WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }
    }
}
