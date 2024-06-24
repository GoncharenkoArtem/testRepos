using Microsoft.Win32;
using project_1;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Threading;
using System.Text.Json;
using Org.BouncyCastle.Asn1.X509;
using static iTextSharp.text.pdf.AcroFields;
using Org.BouncyCastle.Asn1.X509.SigI;
using System.Drawing.Printing;


namespace wpf_test
{
    
    public partial class MainWindow : Window
    {
        DB_users database = new DB_users();
        Settings settings = new Settings();
        Settings settings_old = new Settings();


        private PDFViewer _pdfViewer;
        private PersonForm _personForm;

        private ObservableCollection<FileData> ListviewList_1;
        private ObservableCollection<UserData> ListviewList_2;
        

        private int comboboxIndex;


        private DispatcherTimer _typingTimer;


       // путь для сохранения настроек
        private static DirectoryInfo settingsFolder = Directory.CreateDirectory(System.IO.Path.Combine(Environment.CurrentDirectory, "Settings"));
        private static string settingsPath = settingsFolder.FullName + "\\appsettings.json";
        private static string path_temp_pdf = System.IO.Path.Combine(Environment.CurrentDirectory, "Temp_PDF\\Temp_IUL_PDF.pdf"); 

        public MainWindow()
        {
            InitializeComponent();

            ListviewList_1 = new ObservableCollection<FileData>();
            ListviewList_2 = new ObservableCollection<UserData>();

            lv1.ItemsSource = ListviewList_1;
            lv2.ItemsSource = ListviewList_2;


            tb1.TextChanged += (sender, e) =>
            {
                _typingTimer.Stop(); // Останавливаем таймер при каждом изменении текста
                _typingTimer.Start(); // Перезапускаем таймер
            };

            tb2.TextChanged += (sender, e) =>
            {
                _typingTimer.Stop(); // Останавливаем таймер при каждом изменении текста
                _typingTimer.Start(); // Перезапускаем таймер
            };

            tb_izm.TextChanged += (sender, e) =>
            {
                _typingTimer.Stop(); // Останавливаем таймер при каждом изменении текста
                _typingTimer.Start(); // Перезапускаем таймер
            };

            ListviewList_1.CollectionChanged += ListviewList_1_CollectionChanged;
            ListviewList_2.CollectionChanged += ListviewList_2_CollectionChanged;
            Combobox_form.SelectionChanged += Combobox_form_SelectionChanged;
            this.Closed += MainWindow_Closed;

            lv1.SizeChanged += lv1_SizeChanged;

            // дисеарилизуем данные настроек
            if (File.Exists(settingsPath))
            {
                string json = File.ReadAllText(settingsPath);
                settings = JsonSerializer.Deserialize<Settings>(json);
            }

            // таймер для событий textbox
            _typingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000) // Устанавливаем задержку 500 мс
            };
            _typingTimer.Tick += TypingTimer_Tick;
            Combobox_form.SelectedIndex = 0;
        }

        private void Combobox_form_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RebuildPDF();
        }
        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            _typingTimer.Stop(); // Останавливаем таймер
            RebuildPDF();
        }
        private void Tb1_TextChanged(object sender, RoutedEventArgs e)
        {
            RebuildPDF();
        }
        private void Tb2_TextChanged(object sender, TextChangedEventArgs e)
        {
            RebuildPDF();
        }
        private void Tb_izm_TextChanged(object sender, TextChangedEventArgs e)
        {
            RebuildPDF();
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
   
            if (_pdfViewer != null )
            {
                _pdfViewer.Close();
            }
            
            if (_personForm != null)
            {
                _personForm.Close();
            }

            Form_closing();
        }

        private void lv1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double totalWidth = lv1.ActualWidth - 10;

            if (lv1.View is GridView gridView_lv1)
            {
                foreach (var column in gridView_lv1.Columns)
                {
                    if (column.Width != Double.NaN)
                    {
                        column.Width = totalWidth / gridView_lv1.Columns.Count;
                     
                    }
                }
            }

            if (lv2.View is GridView gridView_lv2)
            {
                gridView_lv2.Columns[0].Width = totalWidth * 0.333;
                gridView_lv2.Columns[1].Width = totalWidth * 0.333;
                gridView_lv2.Columns[2].Width = totalWidth * 0.150;
                gridView_lv2.Columns[3].Width = totalWidth * 0.177;
            }
        }

        private void bt1_Click(object sender, RoutedEventArgs e) // добавить файл в Listview
        {

                // получаем имена дисков 
                //(File.Exist (settings.open_file_path) не работает) поэтому создам DirectoryInfo и проверим путь, то сбрасываем до первого попавшегося локального диска
                var folder = new DirectoryInfo(settings.open_file_path.ToString());
                if (folder.Exists==false)
                {
                    DriveInfo[] allDrives = DriveInfo.GetDrives();
                    settings.open_file_path = allDrives[0].Name;
                }
  

                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = settings.open_file_path,
                    Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt",
                    FilterIndex = 1,
                    RestoreDirectory = true
                };


                if (openFileDialog.ShowDialog() == true)
                {
                    //запись пути в settings
                    if (openFileDialog.FileName != null) 
                    {
                        string directoryPath = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                        settings.open_file_path = directoryPath; 
                    }

                    FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                    CRC_32 crc_32 = new CRC_32(openFileDialog.FileName);

                        FileData fileData = new FileData {
                        File_Name = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName),
                        File_Sum = crc_32.Get(),
                        File_Size = fileInfo.Length,
                        File_Date = fileInfo.LastWriteTime.ToString(),};

                        if (ListviewList_1.Any(i => i.File_Name == fileData.File_Name && i.File_Date == fileData.File_Date))
                        {
                            MessageBox.Show($"Файл {fileData.File_Name} от {fileData.File_Date} уже добавлен" , "Ошибка" , MessageBoxButton.OK , MessageBoxImage.Exclamation);
                        }
                        else
                        {
                            ListviewList_1.Add(fileData);
                            lv1.Items.Refresh();
                        }
                        // проверка кол-ва ДЭ и активация combobox
                        //Type_of_IUL_component_show();
                }
        }

        private void bt2_Click(object sender, RoutedEventArgs e) // убрать файл из Listview
        {
            if (lv1.SelectedIndex != -1)
            {
                ListviewList_1.RemoveAt(lv1.SelectedIndex);
                lv1.Items.Refresh();
                Type_of_IUL_component_show();
            }
        }

        private void bt3_Click(object sender, RoutedEventArgs e) // элемент Listview вверх
        {
           ListviewElemevtUp(lv1, ref ListviewList_1);
        }

        private void bt4_Click(object sender, RoutedEventArgs e) // элемент Listview вниз
        {
            ListviewElemevtdDown(lv1, ref ListviewList_1);
        }

        private void bt6_Click(object sender, RoutedEventArgs e) // убрать файл из Listview
        {
        
            if (lv2.SelectedIndex != -1)
            {
                ListviewList_2.RemoveAt(lv2.SelectedIndex);
                lv2.Items.Refresh();
            }
        }

        private void bt7_Click(object sender, RoutedEventArgs e) // редактируем сотрудника
        {
            // Если не выбран ни один элемент во втором списке, то выходим
            if (lv2.SelectedIndex == -1) 
            { MessageBox.Show("Для редактирования выберите элемент в списке", "Не выбран элемент" , MessageBoxButton.OK, MessageBoxImage.Information);
              return;
            }

            // Если _pdfViewer уже открыт, активируем его, иначе создаем новый экземпляр
            if (_personForm == null || !_personForm.IsVisible)
            {
                _personForm = new PersonForm(this, false, ListviewList_2, lv2.SelectedIndex);
            }
            else
            {
                _personForm.Focus();
            }
        }

        private void bt8_Click(object sender, RoutedEventArgs e) // новый сотрудник
        {
            // Если _pdfViewer уже открыт, активируем его, иначе создаем новый экземпляр
            if (_personForm == null || !_personForm.IsVisible)
            {
                _personForm = new PersonForm(this, true, ListviewList_2, lv2.SelectedIndex);
            }
            else
            {
                _personForm.Focus();
            }
        }

        private void bt9_Click(object sender, RoutedEventArgs e) // элемент Listview вверх
        {
            ListviewElemevtUp (lv2, ref ListviewList_2);
        }

        private void bt10_Click (object sender, RoutedEventArgs e) // элемент Listview вниз
        {
            ListviewElemevtdDown (lv2, ref ListviewList_2);
        }

        private void bt_Close(object sender, RoutedEventArgs e) // элемент Listview вниз
        {
           this.Close();
        }

        private void bt_View(object sender, RoutedEventArgs e) // элемент Listview вниз
        {
            RebuildPDF();

            // Если _pdfViewer уже открыт, активируем его, иначе создаем новый экземпляр
            if (_pdfViewer != null || _pdfViewer?.IsVisible == false)
            {
                _pdfViewer.Close();
            }

            _pdfViewer = new PDFViewer(this);
            _pdfViewer.Show();
        }

        private void bt5_Click(object sender, RoutedEventArgs e)
        {

            List<string> templist_FIO = new List<string>();  // временный список, чтобы собрать ФИО для combobox
            List<string> templist_position = new List<string>();  // временный список, чтобы собрать уникальные должности для combobox

            database.ChangeTracker.Clear(); 

            foreach (var item in database.users)
            {
                if (item.second_name != "нет данных" && item.short_FIO != null)
                {
                    templist_FIO.Add(item.short_FIO);
                    if (item.position != null && item.position != "") templist_position.Add(item.position);
                }
            }

            templist_position = templist_position.Distinct().ToList(); // делаем list с уникальными значениями position из его же набора 

            UserData userData = new UserData()
            {
                FIO = templist_FIO,
                Position = templist_position
            };

            if (ListviewList_2.Count - 1 > -1) RebuildComboboxFIO(ref ListviewList_2, ListviewList_2.Count - 1); 

            ListviewList_2.Add(userData);
            lv2.Items.Refresh();
        }

        private void NewValue_Combobox_2(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            int index = GetIndexCombobox(comboBox);

            string selectedValue = comboBox?.Text as string ?? "";

            if (index > -1)
            {
                if (ListviewList_2[index].SelectedPosition_pos == selectedValue) { return; }
                ListviewList_2[index].SelectedPosition_pos = selectedValue;
            }
        }

        private void NewValue_Combobox_1(object sender, EventArgs e)
         {
            ComboBox comboBox = sender as ComboBox;
            int index = GetIndexCombobox(comboBox);

            string selectedValue = comboBox?.Text as string ?? "";

            if (index > -1)
            {
                if (ListviewList_2[index].SelectedPosition_fio == selectedValue) { return; }
                ListviewList_2[index].SelectedPosition_fio = selectedValue;
                RebuildComboboxFIO(ref ListviewList_2, index);
            }
        }

        private void Edit_Combobox_1(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            int index = GetIndexCombobox(comboBox);
          
            database.ChangeTracker.Clear();

            if (comboBox?.SelectedItem == null)
            { return;}

            string selectedValue = comboBox?.SelectedItem as string ?? "";

            var selected_val = database.users.FirstOrDefault(n => n.short_FIO == selectedValue); // поиск значения FIO в базе
            if (selected_val != null)
            {
                ListviewList_2[index].Second_name = selected_val.second_name;
                ListviewList_2[index].SelectedPosition_pos = selected_val.position ?? "";  // Ставим позицию (position) в соответствии с FIO

                if (selected_val.image_path != "нет данных" && selected_val.image_path != null) // Ставим чекбокс для подписи
                {
                    ListviewList_2[index].Sign = true;
                    ListviewList_2[index].SignEnabled = true;
                    ListviewList_2[index].SignPath = selected_val.image_path;
                }
                else
                {
                    ListviewList_2[index].SelectedPosition_fio = selectedValue;
                   // RebuildComboboxFIO(ref ListviewList_2, index);
                    ListviewList_2[index].Sign = false;
                    ListviewList_2[index].SignEnabled = false;
                    ListviewList_2[index].SignPath = "";
                }
                lv2.Items.Refresh();

            } else {

                ListviewList_2[index].SelectedPosition_fio = selectedValue;
               // RebuildComboboxFIO(ref ListviewList_2, index);
                ListviewList_2[index].Sign = false;
                ListviewList_2[index].SignEnabled = false;
                ListviewList_2[index].SignPath ="";
                lv2.Items.Refresh();

            }


            if (ListviewList_2[index].Date == null)
            {
                string formattedDate;
                if (ListviewList_1.Count > 0)
                {   // Парсим строку в DateTime, указывая исходный формат
                    DateTime dateTime = DateTime.ParseExact(ListviewList_1[0].File_Date, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    // Преобразуем в строку с новым форматом
                    formattedDate = dateTime.ToString("dd.MM.yy");
                }
                else
                {
                    DateTime now = DateTime.Now;
                    formattedDate = now.ToString("dd.MM.yy");
                }
                ListviewList_2[index].Date = formattedDate;
            }
        }





        // ====================================     доп функции      ====================================
        private void ListviewElemevtUp<T>(ListView listview, ref ObservableCollection<T> listview_source)  // перемещение элемента в listview вверх
        {
            if (listview.Items.Count < 2) { return; }
            int _selected = listview.SelectedIndex;
            if (_selected <= 0) { return; }
            var temp = listview_source[_selected - 1];
            listview_source[_selected - 1] = listview_source[_selected];  
            listview_source[_selected] = temp;
            listview.Items.Refresh();
            listview.SelectedIndex = _selected-1;
        }




        private void ListviewElemevtdDown<T>(ListView listview, ref ObservableCollection<T> listview_source) // перемещение элемента в listview вниз
        {
            if (listview.Items.Count < 2) { return; }
            int _selected = listview.SelectedIndex;
            if (_selected ==  - 1) { return; }
            if (_selected == listview.Items.Count-1) { return; }
            var temp = listview_source[_selected + 1];
            listview_source[_selected + 1] = listview_source[_selected];
            listview_source[_selected] = temp;
            listview.Items.Refresh();
            listview.SelectedIndex = _selected + 1;
        }

        private List<string> PackVal()
        {
            List<string> list = new List<string>(3);
            list.Add(tb_izm.Text);
            list.Add(tb1.Text);
            list.Add(tb2.Text);
            return list;
        }


        private int GetIndexCombobox(object cbox) 
        {
            ComboBox comboBox = cbox as ComboBox;
            // Получаем элемент данных, к которому привязан ComboBox
            var userData = comboBox?.DataContext as UserData;
            string selectedValue = comboBox?.SelectedItem as string ?? "";
            // Получаем индекс строки, к которой относится ComboBox
            int index = lv2.Items.IndexOf(userData);
            return index;
        }


        private void RebuildComboboxFIO (ref ObservableCollection<UserData> List, int index) 
        {
            if (List != null)
            {
                string new_val = List[index].SelectedPosition_fio?? ""; // получаем значение 

                    if (new_val != "") 
                    { bool match = List[index].FIO.Any(n => n == new_val); // проверяем, есть ли в списке такое значение
                    if (match == false)
                    {
                        // проверяем последнее значение в списке есть ли оно в БД
                        var last_val = ListviewList_2[index].FIO[ListviewList_2[index].FIO.Count - 1];
                        var search_val = database.users.FirstOrDefault(n => n.short_FIO == last_val); // поиск значения FIO в базе

                        // если нет совпадений последнего значения в списке с БД 
                        if (search_val == null) ListviewList_2[index].FIO.RemoveAt(ListviewList_2[index].FIO.Count - 1);
                        ListviewList_2[index].FIO.Add(new_val);
                        ListviewList_2[index].Second_name = new_val;
                        ListviewList_2[index].Sign = false;
                        ListviewList_2[index].SignEnabled = false;

                        if (ListviewList_2[index].Date == "")
                        {
                            string formattedDate;
                            if (ListviewList_1.Count > 0)
                            {   // Парсим строку в DateTime, указывая исходный формат
                                DateTime dateTime = DateTime.ParseExact(ListviewList_1[0].File_Date, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                // Преобразуем в строку с новым форматом
                                formattedDate = dateTime.ToString("dd.MM.yy");
                            }
                            else
                            {
                                DateTime now = DateTime.Now;
                                formattedDate = now.ToString("dd.MM.yy");
                            }
                            ListviewList_2[index].Date = formattedDate;
                        }

                        lv2.Items.Refresh();
                    }
                    }
            }
        }





        private void ListviewList_1_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RebuildPDF();
        }

        private void ListviewList_2_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Подписываемся на PropertyChanged для каждого нового элемента
            if (e.NewItems != null)
            {
                foreach (UserData newItem in e.NewItems)
                {
                    newItem.PropertyChanged += UserData_PropertyChanged;


                    newItem.ChangedSign += NewItem_ChangedSign;
                }
            }

            // Отписываемся от PropertyChanged для удалённых элементов
            if (e.OldItems != null)
            { 
                foreach (UserData oldItem in e.OldItems)
                {
                    oldItem.PropertyChanged -= UserData_PropertyChanged;
                }
            }

            RebuildPDF();
        }





        // Событие для обработки всех выделенных элементов
        private void NewItem_ChangedSign(object? sender, PropertyChangedEventArgs e)
        {
            UserData lv = sender as UserData;

            if (lv2.SelectedItems.Count > 1)
            {
                foreach (UserData item in lv2.SelectedItems)
                {
                    if (item.SignEnabled == true) { item.Sign = lv.Sign; }
                }
            }
        }





        private void UserData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RebuildPDF();
        }





        private void RebuildPDF() 
        {
            List<string> packval = PackVal(); // упакуем переменные
            PDF_Creator pdf = new PDF_Creator(ListviewList_1, ListviewList_2, packval);
            pdf.GeneratePdf(Combobox_form.SelectedIndex + 1);
        }

         




        private void Type_of_IUL_component_show() 
        {
            //if (ListviewList_1.Count > 1)
            //{
            //    Combobox_form.IsEnabled = true;

            //}else{
            //    Combobox_form.IsEnabled = false;
            //    Combobox_form.SelectedIndex = 0;
            //}
        }






        private void Form_closing() 
        {

            var path_pdf = new DirectoryInfo(System.IO.Path.GetDirectoryName(path_temp_pdf.ToString() ));
            if (path_pdf.GetFiles().Length == 0) 
            {
                return; 
            }

            const string message ="Сохранить результат?";
            const string caption = "Выход";
            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

            // Если сохраняем
            if (result == MessageBoxResult.Yes)
            {
                // получаем имена дисков 
                //(File.Exist (settings.open_file_path) не работает) поэтому создам DirectoryInfo и проверим путь, то сбрасываем до первого попавшегося локального диска
                var path = settings.save_file_path == null ? null : new DirectoryInfo(settings.save_file_path );
                
                if (path?.Exists == false || path == null)
                {
                    DriveInfo[] allDrives = DriveInfo.GetDrives();
                    settings.save_file_path = allDrives[0].Name;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog // создаем новое окно сохранения
                {
                    InitialDirectory = settings.save_file_path,
                    Filter = "PDF документ(*.pdf)|*.pdf",
                    FilterIndex = 1,
                    RestoreDirectory = true
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    //запись пути в settings
                    if (saveFileDialog.FileName != null)
                    {
                        string directoryPath = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);
                        settings.save_file_path = directoryPath;

                        if (File.Exists(saveFileDialog.FileName) == false)
                        {
                            File.Copy(path_temp_pdf, saveFileDialog.FileName); // просто копируем файл
                        }else{ 
                            System.IO.File.Delete(saveFileDialog.FileName);    // сначала удаляем файл
                            File.Copy(path_temp_pdf, saveFileDialog.FileName); // копируем файл
                        }
                    }
                }
            }

            // запись параметров в settings
            string json_old = File.ReadAllText(settingsPath);
            settings_old = JsonSerializer.Deserialize<Settings> (json_old);
            settings.sign_path = settings_old.sign_path;

            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(settingsPath, json);
        }

        
    }
}



