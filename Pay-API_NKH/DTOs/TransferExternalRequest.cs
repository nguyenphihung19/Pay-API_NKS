namespace Pay_API_NKH.DTOs
{
    public class TransferExternalRequest
    {
        public string FromAccount { get; set; } = string.Empty;
        public string ToBankCode { get; set; } = string.Empty;
        public string ToAccount { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}