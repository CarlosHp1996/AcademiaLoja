using AcademiaLoja.Application.Models.Dtos;

namespace AcademiaLoja.Application.Models.Responses.Security
{
    public class CreateUserResponse
    {
        public UserDto User { get; set; }
        public required string Message { get; set; }
    }
}
