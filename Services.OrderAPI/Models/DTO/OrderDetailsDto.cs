using System.ComponentModel.DataAnnotations.Schema;

namespace Services.OrderAPI.Models.DTO
{
    public class OrderDetailsDto
    {
        public int OrderDetailsId { get; set; }
        public int OrderMasterId { get; set; }
        public int ProductId { get; set; }
        public ProductDto? Product { get; set; }
        public string? ProductName { get; set; }
        public double? Price { get; set; }
        public int Count { get; set; }
    }
}
