using SberValidator.Models;
using System;
using System.ComponentModel;
using System.IO;

namespace SberValidator.Modules
{
    class Calculater
    {
        private static CalculateString calculateString;

        /// <summary>
        /// Суммировать столбцы
        /// </summary>
        /// <param name="listSber"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static CalculateString Calculate(BindingList<Sber> listSber, string path)
        {
            decimal withoutCommision71Sum = 0;
            decimal paymentSber70 = 0;
            decimal commision72 = 0;

            foreach (var item in listSber)
            {
                withoutCommision71Sum += item.WithoutСommission71;
                paymentSber70 += item.PaymentSber70;
                commision72 += item.Commision72;
            }
            FileInfo fInfo = new FileInfo(path);
            string name = fInfo.Name;
            string[] namePart = name.Split('_');
            string sDate = namePart[2].Replace(".txt", "").Replace(".csv","");

            DateTime d = DateTime.ParseExact(sDate, "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture);

            calculateString = new CalculateString()
            {
                Count = listSber.Count,
                WithoutCommision71 = withoutCommision71Sum,
                PaymentSber70 = paymentSber70,
                Commision72 = commision72,
                Number = namePart[1],
                DateString = d.ToString("dd-MM-yyyy")
            };

            return calculateString;
        }
    }
}
