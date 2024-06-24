using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace wpf_test
{
    /// <summary>
    /// Логика взаимодействия для PDFViewer.xaml
    /// </summary>
    public partial class PDFViewer : Window
    {
        private MainWindow _mainWindow;
        private FileSystemWatcher _fileWatcher;
        private static string path_temp_folder = System.IO.Path.Combine(Environment.CurrentDirectory, "Temp_PDF\\");
        private static string path_temp_pdf = System.IO.Path.Combine(Environment.CurrentDirectory, "Temp_PDF\\Temp_IUL_PDF.pdf");


        public PDFViewer(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            InitializeComponent();
            ShowForm();
            InitializeFileWatcher();
            InitializeWebView();

            _mainWindow.LocationChanged += MainFormChanged;
            _mainWindow.SizeChanged += MainFormChanged;

            this.Closed += PDFViewer_Closed;
            this.SourceInitialized += new EventHandler(Window_SourceInitialized);
        }
    

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            // полкучаем дискриптор окна
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            // создаем объект FromHwnd(hwnd) класса HwndSource и добавляем обработчик (перехватчик) сообщений Windows, указывая метод, который будет обрабатывать сообщения (в данном случае WndProc)
            HwndSource.FromHwnd(hwnd).AddHook(new HwndSourceHook(WndProc));
        }


        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_NCLBUTTONDOWN = 0x00A1; // Код сообщения для нажатия на неклиентскую область окна
            const int HTCAPTION = 2;             // Код, указывающий на заголовок окна

            // Проверяем, что получено сообщение о нажатии на заголовок окна
            if (msg == WM_NCLBUTTONDOWN && wParam.ToInt32() == HTCAPTION) 
            {
                handled = true; // Перехватываем сообщение и предотвращаем перемещение, ставим true = сообщение обработано
            }

            return IntPtr.Zero; // Возвращаемое значение может быть использовано для передачи данных обратно в систему
        }


        private void ShowForm()
        {
            this.Top = _mainWindow.Top;
            this.Left = _mainWindow.Left + _mainWindow.ActualWidth;
            this.Height = _mainWindow.Height;
        }

     
        private void MainFormChanged(object sender, EventArgs e)
        {
            ShowForm();
        }



        private void InitializeFileWatcher()
        {
            _fileWatcher = new FileSystemWatcher
            {
                Path = System.IO.Path.GetDirectoryName((path_temp_pdf)),
                Filter = System.IO.Path.GetFileName((path_temp_pdf)),
                NotifyFilter = NotifyFilters.LastWrite
            };

            _fileWatcher.Changed += OnPdfFileChanged;
            _fileWatcher.EnableRaisingEvents = true;
        }





        private async void OnPdfFileChanged(object sender, FileSystemEventArgs e)
        {
            // Ожидание завершения записи файла, чтобы избежать ошибки при перезагрузке
            await Task.Delay(200);

            // Перезагрузить PDF в WebView2
            Dispatcher.Invoke(() =>
            {
                webView.Reload();
            });
        }



        private async void InitializeWebView()
        {
            await webView.EnsureCoreWebView2Async(null);

            var folder = new DirectoryInfo(path_temp_folder);

            if (folder.GetFiles().Length>0)
            {
                webView.Source = new Uri(path_temp_pdf);
            }
        }
       

        private void PDFViewer_Closed(object? sender, EventArgs e)
        {
            _fileWatcher.Dispose();
        }

    }

}
