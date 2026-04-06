using LuyenTap.Models;

namespace LuyenTap.Services
{
    public interface IStudentDatabaseReader
    {
        Task<HomeIndexViewModel> ReadAllAsync(CancellationToken cancellationToken = default);
    }
}
