using System.ComponentModel.DataAnnotations;

namespace Data.Dtos.ProductionOrders
{
    public class CreateProductionOrderDto
    {
        [Required(ErrorMessage = "Numer zlecenia jest wymagany.")]
        [StringLength(50)]
        public string OrderNumber { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Produkt jest wymagany.")]
        public int ProductId { get; set; }

        [Range(typeof(decimal), "0,001", "999999999999999,999",
            ErrorMessage = "Planowana ilość musi być większa od zera.")]
        public decimal PlannedQuantity { get; set; }

        public DateOnly? PlannedStartDate { get; set; }

        public DateOnly? PlannedEndDate { get; set; }

        public int? CustomerId { get; set; }

        [StringLength(20)]
        public string? ProductionOrderType { get; set; }
    }
}
