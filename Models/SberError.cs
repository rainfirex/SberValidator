namespace SberValidator.Models
{
    public class SberError
    {
        public string Title { get; set; }
        public int Line { get; set; }
        public string RowData { get; set; }
        public string Error { get; set; }
        public string StackTrace { get; set; }
    }
}
