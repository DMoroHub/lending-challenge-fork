namespace LendingService.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public decimal BalanceLeft { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public Offer Offer { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
