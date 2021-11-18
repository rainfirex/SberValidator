using SberValidator.Modules;
using SberValidator.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Linq;
using System.Windows.Media;

namespace SberValidator.Windows
{
    public partial class EditWindow : Window
    {
        private SberError _sberError;
        private Sber sber;
        dynamic obj;

        public Sber GetSber() => sber;

        internal SberError GetSberError() => _sberError;

        public EditWindow(SberError sberError)
        {
            this._sberError = sberError;

            InitializeComponent();
        }       

        private void frmEdit_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = "Редактор строки";

            List<ExpandoObject> listExp = new List<ExpandoObject>();

            string[] data = this._sberError.RowData.Split(';');

            Array.Resize(ref data, 72);

            obj = new ExpandoObject();
            IDictionary<string, object> dObject = obj;
            dObject.Add("Date1", data[0] ?? "");
            dObject.Add("Time2", data[1] ?? "");
            dObject.Add("Code3", data[2] ?? "");
            dObject.Add("CodeV4", data[3] ?? "");
            dObject.Add("Code5", data[4] ?? "");
            dObject.Add("Lic6", data[5] ?? "");
            dObject.Add("Username7", data[6] ?? "");
            dObject.Add("Address8", data[7] ?? "");
            dObject.Add("CodeServise9", data[8] ?? "");
            dObject.Add("Payment10", data[9] ?? "");
            dObject.Add("CodeFine11", data[10] ?? "");
            dObject.Add("Fine12", data[11] ?? "");
            for (int i = 13; i <= 68; i++)
            {
                dObject.Add("empty" + i, data[i]);
            }
            dObject.Add("Period69", data[68] ?? "");
            dObject.Add("PaymentSber70", data[69] ?? "");
            dObject.Add("WithoutСommission71", data[70] ?? "");
            dObject.Add("Commision72", data[71] ?? "");

            listExp.Add(obj);

            foreach (var item in dObject)
            {
                DataGridTextColumn c = new DataGridTextColumn();
                c.Header = item.Key;
                c.Binding = new Binding(item.Key);
                dataGrid.Columns.Add(c);
            }

            dataGrid.ItemsSource = listExp;
            rTxtBox.Document.Blocks.Add(new Paragraph(new Run(this._sberError.RowData)));
        }

        private void btnParse_Click(object sender, RoutedEventArgs e)
        {
            if (sber != null)
            {
                MessageBox.Show("Исправление было выполнено.","Отказ", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            try
            {
                ExpandoObject obj = (ExpandoObject)dataGrid.Items[0];
                IDictionary<string, object> dObj = obj;

                List<string> list = new List<string>();

                foreach (var property in dObj)
                {
                    list.Add((string)property.Value);
                }
                string[] data = list.ToArray();

                sber = CSVData.ParseToSber(data);
                txtWarning.Foreground = System.Windows.Media.Brushes.Green;
                txtWarning.Text = "Запись исправлена";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAddSuccess_Click(object sender, RoutedEventArgs e)
        {
            if(sber == null)
            {
                MessageBox.Show("Строка не исправлена","Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.DialogResult = true;
        }

        private void btnValidate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Style styleNormal = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
                styleNormal.Setters.Add(new Setter(ToolTipService.ToolTipProperty, ""));
                styleNormal.Setters.Add(new Setter
                {
                    Property = BackgroundProperty,
                    Value
                    = Brushes.WhiteSmoke
                });

                Style styleError = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
                styleError.Setters.Add(new Setter(ToolTipService.ToolTipProperty, "Содержит ошибку"));
                styleError.Setters.Add(new Setter
                {
                    Property = BackgroundProperty,
                    Value
                    = Brushes.Red
                });

                foreach (var c in dataGrid.Columns)
                    c.HeaderStyle = styleNormal;

                List<string> listValidColumn = CSVData.Validator(obj);
                if (listValidColumn.Count > 0)
                {
                    foreach (var columnName in listValidColumn)
                    {
                        var col = dataGrid.Columns.Single(c => c.Header.ToString().ToUpper() == columnName.ToUpper());
                        col.HeaderStyle = styleError;
                    }
                    MessageBox.Show("Строка содержит некорректные форматы в столбцах.", "Валидатор", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("Строка готова к испралению.", "Валидатор", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }
    }
}
