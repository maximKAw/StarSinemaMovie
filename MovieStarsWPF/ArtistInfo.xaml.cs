using MovieStarsWPF.Podkl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;

namespace MovieStarsWPF
{
    /// <summary>
    /// Логика взаимодействия для ArtistInfo.xaml
    /// </summary>
    public partial class ArtistInfo : Window
    {
        Query Podkl; //Определим наш объект класса
        public int text_ID;
        public int RegimOper, CheckArtist;

        public ArtistInfo()
        {
            InitializeComponent();

            //Инициализируем. В качестве параметра передаем строку подключения
            Podkl = new Query(ConnectionString.ConnStr);
        }

        //Обработчик горячих клавиш ESC и ENTER
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Exit_Click(sender, e);
            }
            else if (e.Key == Key.Enter)
            {
                Save_Click(sender, e);
            }
        }

        //Кнопка - Выход
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            CheckArtist = -1;
            Close();
        }

        //Кнопка - Сохранить
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //Выполняем проверки заполненных полей данных
            if (TextBoxFam.Text == "")
            {
                MessageBox.Show("Введите фамилию артиста");
                TextBoxFam.Focus();
                return;
            }

            string StrJson = "";


            switch (RegimOper)
            {
                case 1: //Режим редактирования
                    ClassArtist Per_1 = new ClassArtist(text_ID,
                                                      TextBoxFam.Text, TextBoxName.Text, TextBoxOtch.Text, ClassStr.TekNameFileFoto);
                    StrJson = JsonConvert.SerializeObject(Per_1);
                    Podkl.Update_JSON("Table_Artist", StrJson);
                    break;
                case 2: //Режим создания
                    ClassArtist Per_2 = new ClassArtist(0,
                                                      TextBoxFam.Text, TextBoxName.Text, TextBoxOtch.Text, ClassStr.TekNameFileFoto);
                    StrJson = JsonConvert.SerializeObject(Per_2);
                    CheckArtist = Podkl.Add_JSON("Table_Artist", StrJson);
                    break;
            }

            Close();
        }

        //Загрузка фото
        private void LoadImageToControl(string Namefile)
        {
            if (System.IO.File.Exists(Namefile))
            {
                // Создаем BitmapImage и загружаем изображение
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(Namefile, UriKind.RelativeOrAbsolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Загружаем изображение сразу
                bitmap.EndInit();

                // Грузим фото в компонент Image
                ImageFoto.Source = bitmap;
            }
            else
            {
                // Если файл не найден, очищаем Image
                ImageFoto.Source = null;
                MessageBox.Show("Файл не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Обработчик изменения фото
        private void ChangeFoto(object sender, MouseButtonEventArgs e)
        {
            // Создаем диалог выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                // Фильтр для выбора изображений
                Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp|All files (*.*)|*.*",
                InitialDirectory = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\Фото\\"
            };

            // Если файл выбран
            if (openFileDialog.ShowDialog() == true)
            {
                // Получаем путь к выбранному файлу
                string imagePath = openFileDialog.FileName;

                ClassStr.TekNameFileFoto = System.IO.Path.GetFileName(imagePath);

                // Загружаем изображение в компонент Image
                LoadImageToControl(imagePath);
            }
        }

        //Обработчик удаления фото
        private void DeleteFoto(object sender, MouseButtonEventArgs e)
        {
            ClassStr.TekNameFileFoto = "";
            string filePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) +
                              "\\Фото\\ПустоеФото.jpg";

            LoadImageToControl(filePath);
        }

        //Обработчик загрузки окна
        private void ArtistWin_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable bufferTable_Artist;
            bufferTable_Artist = new DataTable();
            string zapros = "";

            if (ClassStr.TekPrava != "Администратор")
            {
                Save.Visibility = Visibility.Hidden;
            }

            switch (RegimOper)
            {
                case 1: //Режим редактирования
                    zapros = "select CodArtist,Fam,Name,Otch from Table_Artist";
                    zapros += " where CodArtist = ";
                    zapros += text_ID;
                    bufferTable_Artist = Podkl.GetDataFromTable(zapros);

                    //Запроляняем поля для ввода информации
                    TextBoxFam.Text = bufferTable_Artist.Rows[0]["Fam"].ToString();
                    TextBoxName.Text = bufferTable_Artist.Rows[0]["Name"].ToString();
                    TextBoxOtch.Text = bufferTable_Artist.Rows[0]["Otch"].ToString();
                    break;
                case 2: //Режим создания
                    //Запроляняем поля для ввода информации
                    TextBoxFam.Text = "";
                    TextBoxName.Text = "";
                    TextBoxOtch.Text = "";
                    break;
            }

            string filePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) +
                              "\\Фото\\" + (ClassStr.TekNameFileFoto == "" ? "ПустоеФото.jpg":ClassStr.TekNameFileFoto);

            LoadImageToControl(filePath);

            TextBoxFam.Focus();
        }
    }
}
