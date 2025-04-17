namespace AcademiaLoja.Application.Models.Responses.Security
{
    public class GetUserByIdResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
