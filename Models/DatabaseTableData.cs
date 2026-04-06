namespace LuyenTap.Models
{
    public class DatabaseTableData
    {
        public string Schema { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public List<string> Columns { get; set; } = new();
        public List<Dictionary<string, string>> Rows { get; set; } = new();
    }
}
