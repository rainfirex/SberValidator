using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SberValidator.Modules;
using SberValidator.Data;
using SberValidator.Models;

namespace SberValidator.Windows
{
    public partial class MainWindow : Window
    {
        private string path;
        private Coding coding;
        DataCenter dataCenter;
        CalculateString calculateString;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = String.Format("{0} v.{1} dev:{2}", Config.ProgrammName, Config.Version, Config.Programmist);
            listError.DisplayMemberPath = "Title";
            cmbBoxEncoding.DisplayMemberPath = "Title";
            cmbBoxEncoding.ItemsSource = CSVData.GetEncodings();
            TabError.Visibility = Visibility.Hidden;
            mnuExportSucces.IsEnabled = false;
            mnuExportErrors.IsEnabled = false;
            mnuRecalculateResult.IsEnabled = false;

            dataCenter = new DataCenter(ResultLoadFile);
        }

        /// <summary>
        /// Выбрать файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Data files (*.txt, *.csv)|*.txt;*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                path = openFileDialog.FileName;
                try
                {
                    coding = CSVData.getEncoding(path);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    openFileDialog.FileName = "";
                    return;
                }
                lblPath.Text = path;
                lblpathFile.Content = "Путь к файлу:";
                cmbBoxEncoding.SelectedItem = coding;
                TabError.Visibility = Visibility.Hidden;
                mnuExportSucces.IsEnabled = false;
                mnuExportErrors.IsEnabled = false;
                mnuRecalculateResult.IsEnabled = false;
            }
        }

        /// <summary>
        /// Чтение файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReadFile_Click(object sender, RoutedEventArgs e)
        {
            {
                lblError.Text = "";
                lblLine.Text = "";
                lblRowData.Text = "";
                lblStackTrace.Text = "";
                txtSberCount.Text = "";
                txtSberErrorCount.Text = "";
            }           

            if (cmbBoxEncoding.SelectedItem == null)
            {
                MessageBox.Show("Кодировка не выбрана.", "Отказа", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!String.IsNullOrEmpty(path))
            {
                coding = (Coding)cmbBoxEncoding.SelectedItem;
                btnRead.IsEnabled = false;
                cmbBoxEncoding.IsEnabled = false;

                Thread thread = new Thread(new ThreadStart(this.LoadFile));
                thread.IsBackground = true;
                thread.Start();                
            }
            else
                MessageBox.Show("Файл не выбран.", "Отказа", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Отобразить детальную информацию с ошибкой
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listError_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listError.SelectedItem != null)
            {
                SberError sberError = (SberError)listError.SelectedItem;

                lblLine.Text = $"№ Строка: {sberError.Line} - (Позиция элемента в оригинальном файле)";
                lblError.Text      = sberError.Error;
                lblStackTrace.Text = sberError.StackTrace;
                lblRowData.Text    = sberError.RowData;
            }
        }

        /// <summary>
        /// Прочитать файл
        /// </summary>
        private void LoadFile()
        {
            dataCenter.ReadFile(path, coding.Encoding);
        }

        /// <summary>
        /// Отобразить результат чтения файла
        /// </summary>
        private void ResultLoadFile()
        {
            Dispatcher.Invoke(() => {
                {
                    this.dataGrid.ItemsSource = dataCenter.GetSber();
                    listError.ItemsSource = dataCenter.GetSberErrors();
                }
                {
                    btnRead.IsEnabled = true;
                    cmbBoxEncoding.IsEnabled = true;
                    lblpathFile.Content = "Файл открыт:";
                    txtSberCount.Text = $"Успешных элементов: { dataCenter.CountSber }";
                    txtSberErrorCount.Text = $"Элементов с ошибкой: { dataCenter.CountSberError }";

                    if(dataGrid.Items.Count > 0)
                    {
                        mnuRecalculateResult.IsEnabled = true;
                        mnuExportSucces.IsEnabled = true;
                    }

                    if (listError.Items.Count > 0)
                    {
                        TabError.Visibility = Visibility.Visible;
                        mnuExportErrors.IsEnabled = true;
                    }
                }
                MessageBox.Show("Файл прочитан.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            });            
        }

        /// <summary>
        /// Закрыть программу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Пересчитать суммы строк
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Calculate(object sender, RoutedEventArgs e)
        {
            try
            {
                calculateString = Calculater.Calculate(dataCenter.GetSber(), path);
                txtBlockResultCalculate.Text = calculateString.ToString();
                txtBlockResultCalculate.Foreground = System.Windows.Media.Brushes.Green;
                MessageBox.Show("Пересчет завершен. Строка сформирована.\nМожно экспортировать!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }

        /// <summary>
        /// Экспорт успешных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportSucces(object sender, RoutedEventArgs e)
        {
            if (calculateString == null)
            {
                MessageBox.Show("Строка результат не задана.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            dataCenter.ExportSuccess(path, calculateString);
            MessageBox.Show("Экспорт завершен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Экспорт ошибочных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportErrors(object sender, RoutedEventArgs e)
        {
            dataCenter.ExportErrors(path);
            MessageBox.Show("Экспорт завершен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Редактор ошибок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listError_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (listError.SelectedItem != null)
            {
                SberError sberError = (SberError)listError.SelectedItem;
                EditWindow editWindow = new EditWindow(sberError);
                bool? result = editWindow.ShowDialog();
                if(result == true)
                {
                    Sber sber = editWindow.GetSber();
                    SberError sberEror = editWindow.GetSberError();

                    dataCenter.AddSber(sber);
                    dataCenter.DeleteSberError(sberEror);

                    txtSberCount.Text = $"Успешных элементов: { dataCenter.CountSber }";
                    txtSberErrorCount.Text = $"Элементов с ошибкой: { dataCenter.CountSberError }";

                    calculateString = null;
                    txtBlockResultCalculate.Text = "?";
                    txtBlockResultCalculate.Foreground = System.Windows.Media.Brushes.Red;

                    MessageBox.Show("Строка добавлена, выполните пересчет.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }                
        }
    }
}
