namespace SberValidator.Models
{
    class CalculateString
    {
        public int Count { get; set; } = 0;
        public decimal WithoutCommision71 { get; set; } = 0;
        public decimal PaymentSber70 { get; set; } = 0;
        public decimal Commision72 { get; set; } = 0;
        public string Number { get; set; }
        public string DateString { get; set; }

        public override string ToString()
        {
            return $"={Count};{PaymentSber70.ToString("0")};{WithoutCommision71.ToString("0")};{Commision72.ToString("0")};{Number};{DateString};".Replace(",",".");
        }
    }
}
