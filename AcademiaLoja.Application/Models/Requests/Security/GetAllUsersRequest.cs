namespace AcademiaLoja.Application.Models.Requests.Security
{
    public class GetAllUsersRequest
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? SortBy { get; set; }
        public bool? SortAscending { get; set; }
        public string? SearchTerm { get; set; }
    }
}
