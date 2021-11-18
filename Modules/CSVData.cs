using CsvHelper;
using SberValidator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper.Configuration;

namespace SberValidator.Modules
{
    class CSVData
    {
        /// <summary>
        /// Список кодировок
        /// </summary>
        private static List<Coding> listEncoding = new List<Coding>()
        {
            new Coding(){Title = "UTF-8", Encoding = Encoding.UTF8},
            new Coding(){Title = "Default", Encoding = Encoding.Default},
            new Coding(){Title = "ASCII", Encoding = Encoding.ASCII}
        };

        // Гетер
        public static List<Coding> GetEncodings() => listEncoding;

        /// <summary>
        /// Получить кодировку текущего файла
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Coding getEncoding(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            using (StreamReader reader = new StreamReader(stream, Encoding.Default))
            {
                reader.Read();
                return listEncoding.SingleOrDefault(item => item.Encoding == reader.CurrentEncoding); ;
            }
        }

        /// <summary>
        /// Чтение / Проверка данных, заливка в листы
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <param name="listSber"></param>
        /// <param name="listErrors"></param>
        public static void Read(string path, Encoding encoding, ref BindingList<Sber> listSber, ref BindingList<SberError>  listErrors)
        {
            using (StreamReader reader = new StreamReader(path, encoding, false))
            {
                string rowData;
                int line = 1;

                while ((rowData = reader.ReadLine()) != null)
                {
                    Regex CSVParser = new Regex(";(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                    string[] X = CSVParser.Split(rowData.Replace("\"", ""));

                    Regex regexResult = new Regex("=");
                    Match m = regexResult.Match(X[0]);
                    if (String.IsNullOrEmpty(X[0]) || m.Success) continue;

                    try
                    {
                        //string date = DateTime.Parse(X[0]).ToString("dd-MM-yyyy");
                        //string[] time = X[1].Split('-');
                        //string time1 = String.Format("{0}-{1}-{2}", int.Parse(time[0]).ToString("00"), int.Parse(time[1]).ToString("00"), int.Parse(time[2]).ToString("00"));
                        //string code3 = X[2];
                        //string codeV4 = X[3];
                        //long code5 = long.Parse(X[4]);
                        //long lic6 = long.Parse(X[5]);
                        //string username7 = X[6];
                        //string address8 = X[7];
                        //int codeService9 = Convert.ToInt32(X[8]);
                        //decimal payment10 = decimal.Parse(X[9].Replace(".", ","));
                        //int codeFine11 = int.Parse(X[10]);
                        //decimal fine12 = decimal.Parse(X[11].Replace(".", ","));
                        //int period69 = int.Parse(X[68]);
                        //decimal paymentSber70 = decimal.Parse(X[69].Replace(".", ","));
                        //decimal withoutCommission71 = decimal.Parse(X[70].Replace(".", ","));
                        //decimal commision72 = decimal.Parse(X[71].Replace(".", ","));

                        //listSber.Add(new Sber
                        //{
                        //    Date1 = date,
                        //    Time2 = time1,
                        //    Code3 = code3,
                        //    CodeV4 = codeV4,
                        //    Code5 = code5,
                        //    Lic6 = lic6,
                        //    Username7 = username7,
                        //    Address8 = address8,
                        //    CodeServise9 = codeService9,
                        //    Payment10 = payment10,
                        //    CodeFine11 = codeFine11,
                        //    Fine12 = fine12,
                        //    Period69 = period69,
                        //    PaymentSber70 = paymentSber70,
                        //    WithoutСommission71 = withoutCommission71,
                        //    Commision72 = commision72
                        //});

                        listSber.Add(ParseToSber(X));

                    }
                    catch (Exception error)
                    {
                        listErrors.Add(new SberError
                        {
                            Title = $"Строка: {line}",
                            Line = line,
                            RowData = rowData,
                            Error = (X.Length == 72) ? error.Message : $"В строке ({X.Length}) {Helper.Declension(X.Length, "столбец", "столбца", "столбцов")}, а должно быть (72) {Helper.Declension(72, "столбец", "столбца", "столбцов")}, скорее всего смешение данных.",
                            StackTrace = error.StackTrace.ToString()
                        });
                        continue;
                    }
                    line++;
                }
            }
        }

        /// <summary>
        /// Парс в модель
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Sber ParseToSber(string[] data)
        {
            string date1 = DateTime.Parse(data[0]).ToString("dd-MM-yyyy");
            string[] time = data[1].Split('-');
            string time2 = String.Format("{0}-{1}-{2}", int.Parse(time[0]).ToString("00"), int.Parse(time[1]).ToString("00"), int.Parse(time[2]).ToString("00"));
            string code3 = data[2];
            string codeV4 = data[3];
            long code5 = long.Parse(data[4]);
            long lic6 = long.Parse(data[5]);
            string username7 = data[6];
            string address8 = data[7];
            int codeService9 = Convert.ToInt32(data[8]);
            decimal payment10 = decimal.Parse(data[9].Replace(".", ","));
            int codeFine11 = int.Parse(data[10]);
            decimal fine12 = decimal.Parse(data[11].Replace(".", ","));
            int period69 = int.Parse(data[68]);
            decimal paymentSber70 = decimal.Parse(data[69].Replace(".", ","));
            decimal withoutCommission71 = decimal.Parse(data[70].Replace(".", ","));
            decimal commision72 = decimal.Parse(data[71].Replace(".", ","));

            return new Sber
            {
                Date1 = date1,
                Time2 = time2,
                Code3 = code3,
                CodeV4 = codeV4,
                Code5 = code5,
                Lic6 = lic6,
                Username7 = username7,
                Address8 = address8,
                CodeServise9 = codeService9,
                Payment10 = payment10,
                CodeFine11 = codeFine11,
                Fine12 = fine12,
                Period69 = period69,
                PaymentSber70 = paymentSber70,
                WithoutСommission71 = withoutCommission71,
                Commision72 = commision72
            };
        }
        
        /// <summary>
        /// Валидировать колонки
        /// </summary>
        /// <param name="dObject"></param>
        /// <returns></returns>
        public static List<string> Validator(IDictionary<string, object> dObject)
        {
            object date1, time2, code3, codeV4, code5, lic6, username7, address8, codeService9, payment10, codeFine11, fine12, period69, paymentSber70, withoutCommission71, commision72;
            dObject.TryGetValue("Date1", out date1);
            dObject.TryGetValue("Time2", out time2);
            dObject.TryGetValue("Code3", out code3);
            dObject.TryGetValue("CodeV4", out codeV4);
            dObject.TryGetValue("Code5", out code5);
            dObject.TryGetValue("Lic6", out lic6);
            dObject.TryGetValue("Username7", out username7);
            dObject.TryGetValue("Address8", out address8);
            dObject.TryGetValue("CodeServise9", out codeService9);
            dObject.TryGetValue("Payment10", out payment10);
            dObject.TryGetValue("CodeFine11", out codeFine11);
            dObject.TryGetValue("Fine12", out fine12);
            dObject.TryGetValue("Period69", out period69);
            dObject.TryGetValue("PaymentSber70", out paymentSber70);
            dObject.TryGetValue("WithoutСommission71", out withoutCommission71);
            dObject.TryGetValue("Commision72", out commision72);

            List<string> listColumns = new List<string>();

            if (!Helper.Validate(date1.ToString(), @"^\d{2}-\d{2}-\d{4}$")) listColumns.Add("Date1");
            if(!Helper.Validate(time2.ToString(), @"^\d{2}-\d{2}-\d{2}$")) listColumns.Add("Time2");
            if(!Helper.Validate(code3.ToString(), @"^\d*/?\d*$")) listColumns.Add("Code3");
            if(!Helper.Validate(codeV4.ToString(), @"^\d*V?$")) listColumns.Add("CodeV4");
            if(!Helper.Validate(code5.ToString(), @"^\d*$")) listColumns.Add("Code5");
            if(!Helper.Validate(lic6.ToString(), @"^\d*$")) listColumns.Add("Lic6");
            if(!Helper.Validate(username7.ToString(), @"^([А-Яа-я])+([А-Яа-я., ])*$")) listColumns.Add("Username7");
            if(!Helper.Validate(address8.ToString(), @"^([А-Яа-я])+([А-Яа-я .,1-9])*$")) listColumns.Add("Address8");
            if(!Helper.Validate(codeService9.ToString(), @"^\d*$")) listColumns.Add("CodeServise9");
            if(!Helper.Validate(payment10.ToString(), @"^\d*.\d*$")) listColumns.Add("Payment10 ");
            if(!Helper.Validate(codeFine11.ToString(), @"^\d*$")) listColumns.Add("CodeFine11");
            if(!Helper.Validate(fine12.ToString(), @"^\d*.\d*$")) listColumns.Add("Fine12");
            if(!Helper.Validate(period69.ToString(), @"^\d*$")) listColumns.Add("Period69");
            if(!Helper.Validate(paymentSber70.ToString(), @"^\d*,\d*$")) listColumns.Add("PaymentSber70");
            if(!Helper.Validate(withoutCommission71.ToString(), @"^\d*,\d*$")) listColumns.Add("WithoutСommission71");
            if(!Helper.Validate(commision72.ToString(), @"^\d*,\d*$")) listColumns.Add("Commision72");

            return listColumns;
        }

        /// <summary>
        /// Сохранить успешные элементы в csv
        /// </summary>
        /// <param name="listSber"></param>
        /// <param name="path"></param>
        /// <param name="calculateString"></param>
        public static void SaveSber(BindingList<Sber> listSber, string path, CalculateString calculateString)
        {
            CsvConfiguration csvConfiguration = new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture);

            //csvConfiguration.Quote = '"';
            csvConfiguration.Mode = CsvMode.NoEscape;

            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var csvWriter = new CsvWriter(writer, csvConfiguration))
            {
                foreach (Sber item in listSber)
                {
                    csvWriter.WriteField(item.Date1);
                    csvWriter.WriteField(item.Time2);
                    csvWriter.WriteField(item.Code3);
                    csvWriter.WriteField(item.CodeV4);
                    csvWriter.WriteField(item.Code5);
                    csvWriter.WriteField(item.Lic6);
                    csvWriter.WriteField(item.Username7);
                    csvWriter.WriteField(item.Address8);
                    csvWriter.WriteField(item.CodeServise9);
                    csvWriter.WriteField(item.Payment10.ToString("0.00").Replace(",", "."));
                    csvWriter.WriteField(item.CodeFine11);
                    csvWriter.WriteField(item.Fine12.ToString("0.00").Replace(",","."));
                    for (int i = 1; i <= 56; i++)
                    {
                        csvWriter.WriteField("");
                    }
                    csvWriter.WriteField(item.Period69.ToString("0000"));
                    csvWriter.WriteField(item.PaymentSber70.ToString("0.00"));
                    csvWriter.WriteField(item.WithoutСommission71.ToString("0.00"));
                    csvWriter.WriteField(item.Commision72.ToString("0.00"));
                    
                    csvWriter.NextRecord();                   
                }
                csvWriter.WriteField(calculateString.ToString());
                csvWriter.NextRecord();

                writer.Flush();
                var result = Encoding.UTF8.GetString(mem.ToArray());

                if (!String.IsNullOrEmpty(result))
                {
                    File.WriteAllText(path, result);
                }
            }
        }

        /// <summary>
        /// Сохранить ошибочные элементы
        /// </summary>
        /// <param name="listErrors"></param>
        /// <param name="pathFile"></param>
        internal static void SaveSberError(BindingList<SberError> listErrors, string pathFile)
        {
            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var csvWriter = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture))
            {
                foreach(SberError item in listErrors)
                {
                    csvWriter.WriteField($"Строка № {item.Line}\n");
                    csvWriter.WriteField($"Данные:\n{item.RowData}\n");
                    csvWriter.WriteField($"Ошибка: {item.Error}\n");
                    csvWriter.WriteField(new string('-', 150));
                    csvWriter.WriteField("\n");
                    csvWriter.WriteField($"Трассировка:\n{item.StackTrace}\n");
                    csvWriter.WriteField(new string('_', 150));
                    csvWriter.WriteField("\n");

                    csvWriter.NextRecord();
                }

                writer.Flush();
                string result = Encoding.UTF8.GetString(mem.ToArray());
                File.WriteAllText(pathFile, result);
            }
        }

        /// <summary>
        /// Сохранить лист моделей
        /// </summary>
        /// <param name="listErrors"></param>
        /// <param name="pathFile"></param>
        public static void SaveAllError(BindingList<SberError> listErrors, string pathFile)
        {
            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var csvWriter = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture))
            {
                csvWriter.WriteHeader<SberError>();
                csvWriter.WriteRecords(listErrors);

                writer.Flush();
                string result = Encoding.UTF8.GetString(mem.ToArray());
                File.WriteAllText(pathFile, result);
            }
        }

        /// <summary>
        /// Сохранить лист моделей 
        /// </summary>
        /// <param name="listErrors"></param>
        /// <param name="pathFile"></param>
        public static void SaveAllSber(BindingList<Sber> listSber, string pathFile)
        {
            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var csvWriter = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture))
            {
                csvWriter.WriteHeader<Sber>();
                csvWriter.WriteRecords(listSber);

                writer.Flush();
                string result = Encoding.UTF8.GetString(mem.ToArray());
                File.WriteAllText(pathFile, result);
            }
        }
    }
}
