namespace Pay_API_NKH.DTOs
{
    public class TransferInternalRequest
    {
        public string FromAccount { get; set; } = string.Empty;
        public string ToAccountOrPhone { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}