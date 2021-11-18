using System.ComponentModel;

namespace SberValidator.Models
{
    public class Sber : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Date1 { get; set; }
        public string Time2 { get; set; }
        public string Code3 { get; set; }
        public string CodeV4 { get; set; }
        public long Code5 { get; set; }
        public long Lic6 { get; set; }
        public string Username7 { get; set; }
        public string Address8 { get; set; }
        public int CodeServise9 { get; set; }
        public decimal Payment10 { get; set; } = 0;
        public int CodeFine11 { get; set; } = 0;
        public decimal Fine12 { get; set; } = 0;
        //56 space
        public int Period69 { get; set; } = 0;// 0120
        public decimal PaymentSber70 { get; set; }       = 0;
        public decimal WithoutСommission71 { get; set; } = 0;
        public decimal Commision72 { get; set; }         = 0;

    }
}
