using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebCSharp.Models;

namespace WebCSharp.Controllers
{
    public class CourseController : Controller
    {
        private readonly string _connectionString;

        public CourseController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            var list = new List<Course>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT id, code, name_vi, name_en, credit, lecture_hours, self_study_hours, description, created_at FROM courses ORDER BY id", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Course
                {
                    Id = reader.GetInt32(0),
                    Code = reader.GetString(1),
                    NameVi = reader.IsDBNull(2) ? null : reader.GetString(2),
                    NameEn = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Credit = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                    LectureHours = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                    SelfStudyHours = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                    Description = reader.IsDBNull(7) ? null : reader.GetString(7),
                    CreatedAt = reader.IsDBNull(8) ? null : reader.GetDateTime(8)
                });
            }
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Course model)
        {
            if (!ModelState.IsValid) return View(model);

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "INSERT INTO courses (code, name_vi, name_en, credit, lecture_hours, self_study_hours, description) " +
                "VALUES (@code, @name_vi, @name_en, @credit, @lecture_hours, @self_study_hours, @description)", conn);
            cmd.Parameters.AddWithValue("@code", model.Code);
            cmd.Parameters.AddWithValue("@name_vi", (object?)model.NameVi ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@name_en", (object?)model.NameEn ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@credit", (object?)model.Credit ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@lecture_hours", (object?)model.LectureHours ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@self_study_hours", (object?)model.SelfStudyHours ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@description", (object?)model.Description ?? DBNull.Value);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Course? item = null;
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT id, code, name_vi, name_en, credit, lecture_hours, self_study_hours, description FROM courses WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item = new Course
                {
                    Id = reader.GetInt32(0),
                    Code = reader.GetString(1),
                    NameVi = reader.IsDBNull(2) ? null : reader.GetString(2),
                    NameEn = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Credit = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                    LectureHours = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                    SelfStudyHours = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                    Description = reader.IsDBNull(7) ? null : reader.GetString(7)
                };
            }
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(Course model)
        {
            if (!ModelState.IsValid) return View(model);

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE courses SET code = @code, name_vi = @name_vi, name_en = @name_en, credit = @credit, " +
                "lecture_hours = @lecture_hours, self_study_hours = @self_study_hours, description = @description WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", model.Id);
            cmd.Parameters.AddWithValue("@code", model.Code);
            cmd.Parameters.AddWithValue("@name_vi", (object?)model.NameVi ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@name_en", (object?)model.NameEn ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@credit", (object?)model.Credit ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@lecture_hours", (object?)model.LectureHours ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@self_study_hours", (object?)model.SelfStudyHours ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@description", (object?)model.Description ?? DBNull.Value);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Course? item = null;
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT id, code, name_vi, name_en, credit FROM courses WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item = new Course
                {
                    Id = reader.GetInt32(0),
                    Code = reader.GetString(1),
                    NameVi = reader.IsDBNull(2) ? null : reader.GetString(2),
                    NameEn = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Credit = reader.IsDBNull(4) ? null : reader.GetInt32(4)
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
            using var cmd = new SqlCommand("DELETE FROM courses WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }
    }
}
