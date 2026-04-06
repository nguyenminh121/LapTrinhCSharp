namespace LuyenTap.Models
{
    public class HomeIndexViewModel
    {
        public string SourceDatabase { get; set; } = string.Empty;
        public List<DatabaseTableData> Tables { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }
}
