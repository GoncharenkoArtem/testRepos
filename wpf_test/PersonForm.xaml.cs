using Microsoft.Win32;
using project_1;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media.Imaging;

namespace wpf_test
{

    public partial class PersonForm : Window
    {
        DB_users DB = new DB_users();

        private string _path_of_sign;
        private bool _new_person;
        private MainWindow _mainWindow;
        ObservableCollection<UserData> _ListviewList_2;
        int _index;
        bool _editPerson;

        Settings settings = new Settings();

        // путь для сохранения настроек
        private static DirectoryInfo settingsFolder = Directory.CreateDirectory(System.IO.Path.Combine(Environment.CurrentDirectory, "Settings"));
        private static string settingsPath = settingsFolder.FullName + "\\appsettings.json";

        public PersonForm(MainWindow mainWindow, bool new_person, ObservableCollection<UserData> ListviewList_2, int index)
        {
            InitializeComponent();

            _new_person = new_person;
            _mainWindow = mainWindow;
            _ListviewList_2 = ListviewList_2;
            _index = index;
       


            if (!new_person) // если это изменение параметров
            {
                if (EditPerson())
                {
                    _editPerson = true;
                    ShowForm();
                }
            }
            else
            {
                _editPerson = false;
                ShowForm(); 
            }


            // дисеарилизуем данные настроек
            if (File.Exists(settingsPath))
            {
                string json = File.ReadAllText(settingsPath);
                settings = JsonSerializer.Deserialize<Settings>(json);
            }
        }







        private bool EditPerson()
        {
                                 
            var search_val = DB.users.FirstOrDefault(n => n.short_FIO == _ListviewList_2[_index].SelectedPosition_fio); // поиск значения FIO в базе
            if (search_val == null) // если не нашли совпадение
            {
                if (_ListviewList_2[_index].SelectedPosition_fio == null || _ListviewList_2[_index].SelectedPosition_fio == "") //если FIO  = пусто поле
                { 
                 return false;
                }

            MessageBox.Show($" Сотрудник {_ListviewList_2[_index].SelectedPosition_fio} не найден в базе сотрудников ", "Не найден элемент", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
            }

            // Если мы не вылетели и нашли FIO в базе, то заполняем данные
            Textbox_secondname.Text = search_val.second_name;
            Textbox_firstname.Text = search_val.first_name;
            Textbox_patronymic_name.Text = search_val.patronymic_name;
            Textbox_position.Text = search_val.position;
            _path_of_sign = search_val.image_path;

            // Подружаем картинку на форму, если есть путь к ней
            if (_path_of_sign!= null) 
            {
                var uri = new Uri(_path_of_sign);
                var newImage = new BitmapImage(uri);
                image_sign.Source = newImage;
            }

            button_delete_person.Visibility = Visibility.Visible;
            return true;

        }






        private void ShowForm()
        {
            this.Top = _mainWindow.Top+166;
            this.Left = _mainWindow.Left + _mainWindow.ActualWidth/2 - 270/2-20;
            this.Show();
        }





        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }




        private void button_ok_Click(object sender, RoutedEventArgs e)
        {
            if (Textbox_secondname.Text == "") 
            {
                MessageBox.Show("Заполните поле Фамилия", "Не заполнены поля", MessageBoxButton.OK, MessageBoxImage.Warning); 
                return; 
            }

            string _old_FIO = _index > -1?_ListviewList_2[_index].SelectedPosition_fio: ""; 
            string _new_FIO = "";
            string _old_position = _index > -1 ? _ListviewList_2[_index].SelectedPosition_pos : ""; 
            string _new_position = "";


            if (_editPerson == true) // Если мы находимся в режиме редактирования
            {
                var search_val = DB.users.FirstOrDefault(n => n.short_FIO == _ListviewList_2[_index].SelectedPosition_fio); // поиск значения FIO в базе
                if (search_val != null)
                {
               
                    // Обновляем данные в БД
                    search_val.second_name = Textbox_secondname.Text;
                    search_val.first_name = Textbox_firstname.Text;
                    search_val.patronymic_name = Textbox_patronymic_name.Text;
                    search_val.position = Textbox_position.Text;
                    search_val.image_path = _path_of_sign;

                    _new_FIO = search_val.short_FIO;
                    _new_position = search_val.position;

                    var search_duplicate = DB.users.FirstOrDefault(n => n.short_FIO == _new_FIO);
                    if (search_duplicate != null && _old_FIO != _new_FIO)
                    {
                        MessageBox.Show($"Сотрудник {_new_FIO} уже существует в базе данных. Для редактирования данных сотрудника {_new_FIO} выберите его в списке \"Участники\" и нажмите кнопку \"Редактировать\" ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Сохраняем изменения в БД
                    DB.SaveChanges();
                }
            }
            else // Если это новый сотрудник
            {

                User user = new User()
                {
                    second_name = Textbox_secondname.Text,
                    first_name = Textbox_firstname.Text,
                    patronymic_name = Textbox_patronymic_name.Text,
                    position = Textbox_position.Text,
                    image_path = _path_of_sign != null? System.IO.Path.GetFullPath(_path_of_sign):null
                };

                _new_FIO = user.short_FIO;
                _new_position = user.position;
                _old_FIO = "";
                _old_position = "";


                var search_duplicate = DB.users.FirstOrDefault(n => n.short_FIO == _new_FIO);
                if (search_duplicate != null)
                {
                    MessageBox.Show($"Сотрудник {_new_FIO} уже существует в базе данных. Для редактирования данных сотрудника {_new_FIO} выберите его в списке \"Участники\" и нажмите кнопку \"Редактировать\" ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DB.Add(user);
                DB.SaveChanges();
            }


                // Редактируем списки Lisview
                EditList(_new_FIO, _new_position, _old_FIO, _old_position, _index, _editPerson);

                this.Close();

        }



        private void EditList(string _new_FIO , string _new_position , string _old_FIO , string _old_position, int _index, bool _editPerson )
        {
            List<string> templist_position = new List<string>();  // временный список, чтобы собрать уникальные должности для combobox

            foreach (var item in DB.users)
            {
                if (item.second_name != "нет данных" && item.short_FIO != null)
                {
                    if (item.position != null && item.position != "") templist_position.Add(item.position);
                }
            }

            templist_position = templist_position.Distinct().ToList(); // делаем list с уникальными значениями position из его же набора 


            // Проходимся по всем элемнтам списка ListviewList_2, используя лямбда-выражение для передачи параметров элемента списка и его порядкового номера
            foreach (var (item, index) in _ListviewList_2.Select((value, i) => (value, i)))
            {
                //Если это добавление нового сотрудника 
                if (_editPerson == false) { _ListviewList_2[index].FIO.Add(_new_FIO); }

                // Заменяем в списке элемента item старые значения (_old_FIO) на новые ( search_val.short_FIO)
                item.FIO = item.FIO.Select(fio => fio == _old_FIO ? _new_FIO : fio).ToList();
                item.Position = templist_position;  // обновляем список уникальных должностей


                // Если старое ФИО изменяемого элемента не совпадает с новым, убираем старое наименование из списка элемента item 
                if (_new_FIO != _old_FIO && _editPerson == true)
                {
                    item.FIO = item.FIO.Where(fio => fio != _old_FIO).ToList();
                    item.FIO = item.FIO.Where(fio => fio != "").ToList();  // Убираем пустые значения
                   
                    
                    if (_new_position == "" && _index == index) { item.SelectedPosition_pos = ""; }

                    if (_new_FIO == "" && _index == index)
                    {
                        item.Second_name = "";
                        item.SelectedPosition_fio = "";
                    }
                }


               // Если мы находимся в режиме редактирования
               if (_editPerson == true)
                {   
                    //Если это текущая редактируемая строка, или ФИО совпадает в другом выпадающем списке, то выставим значения
                    if (item.SelectedPosition_fio == _old_FIO)
                    {
                        item.SelectedPosition_fio = _new_FIO;
                        item.SelectedPosition_pos = _new_position;

                        //if (_index == index)
                        //{
                            if (_path_of_sign == "" || _path_of_sign == null)
                            {
                                item.Sign = false;
                                item.SignEnabled = false;
                            }
                            else
                            {

                                item.Sign = true;
                                item.SignEnabled = true;
                                item.SignPath = _path_of_sign;
                            }
                        //}
                    }
                }
            }
            
            // Обновляем компонент ListView lv2 наформe MainWindow
            _mainWindow.lv2.Items.Refresh();
        }



        private void button_picture_load_Click(object sender, RoutedEventArgs e)
        {
            var path = settings.sign_path == null ? null : new DirectoryInfo(settings.sign_path);

            if (path?.Exists == false || path == null)
            {
                DriveInfo[] allDrives = DriveInfo.GetDrives();
                settings.sign_path = allDrives[0].Name;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog // создаем новое окно сохранения
            {
                InitialDirectory = settings.sign_path,
                Filter = "Изображение (*.png;*.gif)|*.png;*.gif",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                settings.sign_path = System.IO.Path.GetDirectoryName(openFileDialog.FileName);

                var uri = new Uri(openFileDialog.FileName);
                var newImage = new BitmapImage(uri);
                image_sign.Source = newImage;
                _path_of_sign = System.IO.Path.GetFullPath( uri.OriginalString);

            }

            // запись параметров в settings
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(settingsPath, json);

        }




        private void button_picture_delete_Click(object sender, RoutedEventArgs e)
        {
            _path_of_sign = null;
            image_sign.Source =null;

        }



        private void button_delete_person_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы действительно хотите удалить сотрудника {Textbox_secondname.Text} из базы данных? ", "Удалить сотрудника" , MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var search_val = DB.users.FirstOrDefault(n => n.short_FIO == _ListviewList_2[_index].SelectedPosition_fio); // поиск значения FIO в базе
                DB.DeleteID(search_val.id);

                // Редактируем списки Lisview
                string _old_FIO = _index > -1 ? _ListviewList_2[_index].SelectedPosition_fio : "";
                string _new_FIO = "";
                string _old_position = _index > -1 ? _ListviewList_2[_index].SelectedPosition_pos : "";
                string _new_position = "";

                EditList(_new_FIO, _new_position, _old_FIO, _old_position, _index, _editPerson);
                this.Close();
             }
        }

    }

}