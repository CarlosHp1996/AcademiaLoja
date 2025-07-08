namespace AcademiaLoja.Application.Models.Requests.Orders
{
    public class AddressRequest
    {
        public string Name { get; set; }
        public string Cep { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string? Complement { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}
