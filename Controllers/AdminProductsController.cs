using LuyenTap.Data;
using LuyenTap.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LuyenTap.Controllers
{
    [Authorize(Roles = "Admin", AuthenticationSchemes = "UserCookies")]
    public class AdminProductsController : Controller
    {
        private readonly AppDbContext _context;

        public AdminProductsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.OrderByDescending(x => x.Id).ToListAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategoryOptionsAsync();
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoryOptionsAsync(product.CategoryId);
                return View(product);
            }

            if (!await CategoryExistsAsync(product.CategoryId))
            {
                ModelState.AddModelError(nameof(Product.CategoryId), "CategoryID khong ton tai.");
                await LoadCategoryOptionsAsync(product.CategoryId);
                return View(product);
            }

            _context.Products.Add(product);

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex) when (IsForeignKeyViolation(ex))
            {
                ModelState.AddModelError(nameof(Product.CategoryId), "CategoryID khong ton tai.");
                await LoadCategoryOptionsAsync(product.CategoryId);
                return View(product);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await LoadCategoryOptionsAsync(product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoryOptionsAsync(product.CategoryId);
                return View(product);
            }

            if (!await CategoryExistsAsync(product.CategoryId))
            {
                ModelState.AddModelError(nameof(Product.CategoryId), "CategoryID khong ton tai.");
                await LoadCategoryOptionsAsync(product.CategoryId);
                return View(product);
            }

            var existing = await _context.Products.FindAsync(product.Id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.CategoryId = product.CategoryId;
            existing.ModelNumber = product.ModelNumber;
            existing.ModelName = product.ModelName;
            existing.ProductImage = product.ProductImage;
            existing.UnitCost = product.UnitCost;
            existing.Description = product.Description;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex) when (IsForeignKeyViolation(ex))
            {
                ModelState.AddModelError(nameof(Product.CategoryId), "CategoryID khong ton tai.");
                await LoadCategoryOptionsAsync(product.CategoryId);
                return View(product);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex) when (IsForeignKeyViolation(ex))
                {
                    ModelState.AddModelError(string.Empty, "Khong the xoa vi du lieu dang duoc lien ket boi bang khac.");
                    return View("Delete", product);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private static bool IsForeignKeyViolation(DbUpdateException ex)
        {
            var message = ex.InnerException?.Message ?? ex.Message;
            return message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase)
                || message.Contains("FK_", StringComparison.OrdinalIgnoreCase);
        }

        private async Task LoadCategoryOptionsAsync(int? selectedCategoryId = null)
        {
            ViewBag.CategoryOptions = await GetCategoryOptionsAsync(selectedCategoryId);
        }

        private async Task<List<SelectListItem>> GetCategoryOptionsAsync(int? selectedCategoryId = null)
        {
            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;

            if (shouldClose)
            {
                await connection.OpenAsync();
            }

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = "SELECT [CategoryID] FROM [Categories] ORDER BY [CategoryID]";

                var options = new List<SelectListItem>();
                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var categoryId = reader.GetInt32(0);
                    options.Add(new SelectListItem
                    {
                        Value = categoryId.ToString(),
                        Text = categoryId.ToString(),
                        Selected = selectedCategoryId.HasValue && selectedCategoryId.Value == categoryId
                    });
                }

                return options;
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private async Task<bool> CategoryExistsAsync(int categoryId)
        {
            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;

            if (shouldClose)
            {
                await connection.OpenAsync();
            }

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(1) FROM [Categories] WHERE [CategoryID] = @categoryId";

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@categoryId";
                parameter.Value = categoryId;
                command.Parameters.Add(parameter);

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
        }
    }
}
