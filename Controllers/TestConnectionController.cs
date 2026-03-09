using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LapTrinhCSharp.Controllers
{
    public class ProductController : Controller
    {
        // Add database name to connection string
        string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=BikeStores;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Command Timeout=30";

        public IActionResult Index()
        {
            List<string> productNames = new List<string>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT product_name FROM production.products";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader["product_name"].ToString();
                            productNames.Add(name);
                        }
                    }
                }
            }

            return View(productNames);
        }
    }
}