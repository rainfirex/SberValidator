using SberValidator.Modules;
using SberValidator.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace SberValidator.Data
{
    public delegate void ResultReadFile();

    class DataCenter
    {
        /// <summary>
        /// Список валидных элементов
        /// </summary>
        protected BindingList<Sber> listSber;

        /// <summary>
        /// Список инвалидных элементов
        /// </summary>
        protected BindingList<SberError> listErrors;       

        // Гетеры
        public BindingList<Sber> GetSber() => listSber;

        public BindingList<SberError> GetSberErrors() => listErrors;        

        /// <summary>
        /// Количество успешных записей
        /// </summary>
        public int CountSber
        {
            get { return this.listSber.Count; }
        }

        /// <summary>
        /// Количество записей с ошибкой
        /// </summary>
        public int CountSberError
        {
            get { return this.listErrors.Count; }
        }

        /// <summary>
        /// Результат выполнения чтения файла
        /// </summary>
        private ResultReadFile ResultReadFile;

        public DataCenter(ResultReadFile ResultReadFile = null)
        {
            this.ResultReadFile = ResultReadFile;
        }        

        /// <summary>
        /// Прочитать файл
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        public void ReadFile(string path, Encoding encoding)
        {
            listSber = new BindingList<Sber>();
            listErrors = new BindingList<SberError>();

            CSVData.Read(path, encoding, ref listSber, ref listErrors);            

            if (ResultReadFile != null)
                this.ResultReadFile();
        }

        /// <summary>
        /// Экспортировать валидные элементы
        /// </summary>
        /// <param name="path"></param>
        /// <param name="calculateString"></param>
        public void ExportSuccess(string path, CalculateString calculateString)
        {
            FileInfo fInfo = new FileInfo(path);            
            string pathFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{fInfo.Name}";
            CSVData.SaveSber(listSber, pathFile, calculateString);
        }

        // Экспортировать инвалидные элементы
        public void ExportErrors(string path)
        {
            FileInfo fInfo = new FileInfo(path);
            string pathFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{fInfo.Name}";
            CSVData.SaveSberError(listErrors, pathFile);
        }

        /// <summary>
        /// Добавить в список
        /// </summary>
        /// <param name="sber"></param>
        internal void AddSber(Sber sber)
        {
            this.listSber.Add(sber);
        }

        /// <summary>
        /// Удалить из списка
        /// </summary>
        /// <param name="sberEror"></param>
        internal void DeleteSberError(SberError sberEror)
        {
            this.listErrors.Remove(sberEror);
        }
    }
}
