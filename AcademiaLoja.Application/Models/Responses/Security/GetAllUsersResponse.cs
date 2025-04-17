namespace AcademiaLoja.Application.Models.Responses.Security
{
    public class GetAllUsersResponse
    {
        public IEnumerable<UserDto> Users { get; set; }
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
