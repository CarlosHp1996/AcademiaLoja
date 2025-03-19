namespace AcademiaLoja.Application.Models.Responses.Security
{
    public class CreateUserResponse
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public required string Message { get; set; }
    }
}
