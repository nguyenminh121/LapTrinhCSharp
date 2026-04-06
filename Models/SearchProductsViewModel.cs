namespace LuyenTap.Models
{
    public class SearchProductsViewModel
    {
        public string Keyword { get; set; } = string.Empty;
        public List<ProductSearchResult> Results { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }
}
