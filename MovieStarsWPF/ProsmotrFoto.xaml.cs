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
    /// Логика взаимодействия для ProsmotrFoto.xaml
    /// </summary>
    public partial class ProsmotrFoto : Window
    {
        Query Podkl; //Определим наш объект класса

        public ProsmotrFoto()
        {
            InitializeComponent();

            //Инициализируем. В качестве параметра передаем строку подключения
            Podkl = new Query(ConnectionString.ConnStr);
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
                InitialDirectory = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\Фильмы (Фото)\\"
            };

            // Если файл выбран
            if (openFileDialog.ShowDialog() == true)
            {
                // Получаем путь к выбранному файлу
                string imagePath = openFileDialog.FileName;

                ClassStr.TekNameFileFoto = System.IO.Path.GetFileName(imagePath);

                // Загружаем изображение в компонент Image
                LoadImageToControl(imagePath);

                //Сохраняем изображение в таблицу БД
                ClassFilm Per_1 = new ClassFilm(ClassStr.TekCodFilm, ClassStr.TekNameFilm, ClassStr.TekNameFileFoto, ClassStr.TekCodArtist);
                string StrJson = JsonConvert.SerializeObject(Per_1);
                Podkl.Update_JSON("Table_Film", StrJson);
                MessageBox.Show("Фото фильма изменено.", "Успешное изменение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //Обработчик горячих клавиш ESC
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                LabelExit_KeyDown(sender, e);
            }
        }

        //Выход
        private void ExitClick(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        //Обработчик удаления фото
        private void DeleteFoto(object sender, MouseButtonEventArgs e)
        {
            ClassStr.TekNameFileFoto = "";
            string filePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) +
                              "\\Фильмы (Фото)\\ПустоеФото.jpg";

            LoadImageToControl(filePath);

            //Сохраняем удаление в таблицу БД
            ClassFilm Per_1 = new ClassFilm(ClassStr.TekCodFilm, ClassStr.TekNameFilm, ClassStr.TekNameFileFoto, ClassStr.TekCodArtist);
            string StrJson = JsonConvert.SerializeObject(Per_1);
            Podkl.Update_JSON("Table_Film", StrJson);
            MessageBox.Show("Фото фильма удалено.", "Успешное удаление", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LabelExit_KeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }

        //Обработчик загрузки окна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable bufferTable_Artist;
            bufferTable_Artist = new DataTable();
            string zapros = "";

            if (ClassStr.TekPrava != "Администратор")
            {
                LabelChangeFoto.Visibility = Visibility.Hidden;
                LabelDeleteFoto.Visibility = Visibility.Hidden;
            }

            zapros = "select CodFilm,Name,StrFoto,CodArtist from Table_Film";
            zapros += " where CodArtist = ";
            zapros += ClassStr.TekCodArtist;
            bufferTable_Artist = Podkl.GetDataFromTable(zapros);
            //TextBoxFam.Text = bufferTable_Artist.Rows[0]["Fam"].ToString();

            string filePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) +
                              "\\Фильмы (Фото)\\" + (ClassStr.TekNameFileFoto == "" ? "ПустоеФото.jpg" : ClassStr.TekNameFileFoto);

            LoadImageToControl(filePath);
        }
    }
}
