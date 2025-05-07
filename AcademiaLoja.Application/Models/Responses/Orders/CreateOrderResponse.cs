﻿namespace AcademiaLoja.Application.Models.Responses.Orders
{
    public class CreateOrderResponse
    {
        public Guid OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
