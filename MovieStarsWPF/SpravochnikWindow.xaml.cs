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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;

namespace MovieStarsWPF
{
    /// <summary>
    /// Логика взаимодействия для SpravochnikWindow.xaml
    /// </summary>
    public partial class SpravochnikWindow : Window
    {
        Query Podkl; //Определим наш объект класса
        int Regim = 0; //1- режим редактирования, 2 - режим создания
        //public int VidSpravochnik;

        public SpravochnikWindow()
        {
            InitializeComponent();

            //Инициализируем. В качестве параметра передаем строку подключения
            Podkl = new Query(ConnectionString.ConnStr);

            DoubleAnimation bAnim = new DoubleAnimation
            {
                From = 0,
                To = 110,
                Duration = TimeSpan.FromSeconds(3)
            };

            //Для появления эффекта с кнопками
            Knopka_Add.BeginAnimation(Button.WidthProperty, bAnim);
            Knopka_Change.BeginAnimation(Button.WidthProperty, bAnim);
            Knopka_Delete.BeginAnimation(Button.WidthProperty, bAnim);
            if (ClassStr.TekSpravochnik == 1)
            {
                Knopka_Foto.BeginAnimation(Button.WidthProperty, bAnim);
            }
            else
            {
                Knopka_Foto.Visibility = Visibility.Hidden;
            }

            Exit.BeginAnimation(Button.WidthProperty, bAnim);
        }

        //Обработчик горячей клавиши ESC
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Exit_Click(sender, e);
            }
        }

        //Обработчик загрузки формы
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string STablName = "",
                   Szapros = "";

            if (ClassStr.TekPrava != "Администратор")
            {
                Knopka_Add.Visibility = Visibility.Hidden;
                Knopka_Change.Visibility = Visibility.Hidden;
                Knopka_Delete.Visibility = Visibility.Hidden;
                Knopka_Foto.Visibility = Visibility.Hidden;
                StackPanel_2.Visibility = Visibility.Hidden;
            }

            switch (ClassStr.TekSpravochnik)
            {
                case 1: //Фильмы
                    STablName = "Table_Film";
                    Spravochnik.Title = "Справочник 'Фильмы (" + ClassStr.TekFamNameArtist + ")'";
                    DBGridSpravochnik.Columns[0].Header = "Названия фильмов";
                    DBGridSpravochnik.Columns[1].Header = "Фото фильмов";
                    break;
                case 2: //Награды
                    STablName = "Table_Nagrada";
                    Spravochnik.Title = "Справочник 'Награды (" + ClassStr.TekFamNameArtist + ")'";
                    DBGridSpravochnik.Columns[0].Header = "Названия наград";
                    DBGridSpravochnik.Columns[0].Width = 550;
                    DBGridSpravochnik.Columns[1].Visibility = Visibility.Hidden;
                    break;
            }

            Szapros = "select * from " + STablName + " where CodArtist = " + ClassStr.TekCodArtist.ToString() + " order by Name";
            DBGridSpravochnik.ItemsSource = Podkl.GetDataFromTable(Szapros).DefaultView;
            //------------Установить курсор
            if (DBGridSpravochnik.Items.Count > 0)
            {
                object item2 = DBGridSpravochnik.Items[0];
                DBGridSpravochnik.SelectedItem = item2; // выделить нужную строку 
            }
            //------------
        }

        //Выход
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //Обработчик горячих клавиш в названии
        private void DBEdit_Nazvanie_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Exit_Click(sender, e);
            }
            else if (e.Key == Key.Enter)
            {
                if (Regim == 0)
                    Knopka_Add_Click(sender, e);
                else
                    Knopka_Change_Click(sender, e);
            }
        }

        //Добавить
        private void Knopka_Add_Click(object sender, RoutedEventArgs e)
        {
            string //SWarn = "",
                   STablName = "",
                   Szapros = "",
                   StrJson = "";

            if (DBEdit_Nazvanie.Text == "")
            {
                MessageBox.Show("Введите название");
                DBEdit_Nazvanie.Focus();
                return;
            }

            switch (ClassStr.TekSpravochnik)
            {
                case 1: //Фильмы
                    STablName = "Table_Film";
                    ClassFilm PerFilm = new ClassFilm(0, DBEdit_Nazvanie.Text, "", ClassStr.TekCodArtist);
                    StrJson = JsonConvert.SerializeObject(PerFilm);
                    break;
                case 2: //Награды
                    STablName = "Table_Nagrada";
                    ClassNagrada PerNagrada = new ClassNagrada(0, DBEdit_Nazvanie.Text, ClassStr.TekCodArtist);
                    StrJson = JsonConvert.SerializeObject(PerNagrada);
                    break;
            }

            //Поиск дублей 
            Szapros = "SELECT COUNT(*) FROM " + STablName + " where Name = @PName";
            var cmd = new SqlCommand(@Szapros);
            cmd.Connection = new SqlConnection(ConnectionString.ConnStr);
            cmd.Connection.Open();
            cmd.Parameters.AddWithValue("PName", DBEdit_Nazvanie.Text);
            int totalRecords = Convert.ToInt32(cmd.ExecuteScalar());
            if (totalRecords > 0)
            {
                MessageBox.Show("Укажите другое название. Данное значение (" + DBEdit_Nazvanie.Text +
                                ") уже есть в таблице!", "ВНИМАНИЕ");
                DBEdit_Nazvanie.Focus();
                return;
            }
            //-----Конец поиска


            //Добавляем новую запись и обновляем данные в табличке
            Podkl.Add_JSON(STablName, StrJson);
            Szapros = "select * from " + STablName + " where CodArtist = " + ClassStr.TekCodArtist.ToString() + " order by Name";
            DBGridSpravochnik.ItemsSource = Podkl.GetDataFromTable(Szapros).DefaultView;

            //Установить курсор на добавленную запись
            string searchValue = DBEdit_Nazvanie.Text;
            //Создаем и заполняем список из datagrid
            List<string> list = new List<string>();
            foreach (DataRowView row3 in DBGridSpravochnik.Items)
            {
                list.Add(row3["Name"].ToString());
            }

            for (int i = 0; i < list.Count; i++)
                if (list[i] == DBEdit_Nazvanie.Text)
                {
                    object item2 = DBGridSpravochnik.Items[i];
                    DBGridSpravochnik.SelectedItem = item2; // выделить нужную строку 
                    DBGridSpravochnik.ScrollIntoView(item2);// Прокрутить к нужной строке
                    break;
                }
            ////-----Конец установки курсора

            DBEdit_Nazvanie.Text = "";
            DBEdit_Cena.Text = "";
            DBEdit_Nazvanie.Focus();
        }

        //Обработчик кнопки Изменить
        private void Knopka_Change_Click(object sender, RoutedEventArgs e)
        {
            string STablName = "",
                   Szapros = "",
                   StrJson = "";

            if (DBGridSpravochnik.Items.Count < 1)
            {
                MessageBox.Show("Отсутствуют данные");
                return;
            }

            if (DBEdit_Nazvanie.Text == "")
            {
                MessageBox.Show("Введите название");
                DBEdit_Nazvanie.Focus();
                return;
            }

            int index = DBGridSpravochnik.SelectedIndex;
            if (DBGridSpravochnik.SelectedIndex < 0)
            {
                MessageBox.Show("Выберите данные для изменения");
                return;
            }

            DataRowView row = (DataRowView)DBGridSpravochnik.SelectedItem;

            switch (ClassStr.TekSpravochnik)
            {
                case 1: //Фильмы
                    STablName = "Table_Film";
                    ClassFilm PerFilm = new ClassFilm(int.Parse(row["CodFilm"].ToString()), DBEdit_Nazvanie.Text, row["StrFoto"].ToString(), ClassStr.TekCodArtist);
                    StrJson = JsonConvert.SerializeObject(PerFilm);
                    break;
                case 2: //Награды
                    STablName = "Table_Nagrada";
                    ClassNagrada PerNagrada = new ClassNagrada(int.Parse(row["CodNagrada"].ToString()), DBEdit_Nazvanie.Text, ClassStr.TekCodArtist);
                    StrJson = JsonConvert.SerializeObject(PerNagrada);
                    break;
            }


            Podkl.Update_JSON(STablName, StrJson);
            Regim = 0;

            Szapros = "select * from " + STablName + " where CodArtist = " + ClassStr.TekCodArtist.ToString() + " order by Name";
            DBGridSpravochnik.ItemsSource = Podkl.GetDataFromTable(Szapros).DefaultView;

            //------------Установить курсор
            if (DBGridSpravochnik.Items.Count > 0)
            {
                object item2 = DBGridSpravochnik.Items[index];
                DBGridSpravochnik.SelectedItem = item2; // выделить нужную строку 
            }
            //------------

            DBEdit_Nazvanie.Text = "";
            DBEdit_Nazvanie.Focus();
        }

        //Удалить
        private void Knopka_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DBGridSpravochnik.Items.Count < 1)
            {
                MessageBox.Show("Отсутствуют данные");
                return;
            }

            string STablName = "",
                   Szapros = "",
                   SCod = "";

            switch (ClassStr.TekSpravochnik)
            {
                case 1: //Фильмы
                    STablName = "Table_Film";
                    SCod = "CodFilm";
                    break;
                case 2: //Награды
                    STablName = "Table_Nagrada";
                    SCod = "CodNagrada";
                    break;
            }


            //Передаем индекс текущей записи в DBGridSpravochnik
            DataRowView row = (DataRowView)DBGridSpravochnik.SelectedItem;
            Podkl.DeleteFromTable(STablName, SCod, int.Parse(row[SCod].ToString()));
            Szapros = "select * from " + STablName + " where CodArtist = " + ClassStr.TekCodArtist.ToString() + " order by Name";
            DBGridSpravochnik.ItemsSource = Podkl.GetDataFromTable(Szapros).DefaultView;

            //------------Установить курсор
            if (DBGridSpravochnik.Items.Count > 0)
            {
                object item2 = DBGridSpravochnik.Items[0];
                DBGridSpravochnik.SelectedItem = item2; // выделить нужную строку 
            }
            //------------

            MessageBox.Show("Удаление выполнено успешно");
        }

        //Выбор по двойному щелчку мыши
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DBGridSpravochnik.SelectedItems.Count == 0) return;
            DBEdit_Nazvanie.Text = ((DataRowView)DBGridSpravochnik.SelectedItems[0]).Row["Name"].ToString();
            DBEdit_Nazvanie.Focus();
            Regim = 1; // Режим редактирования
        }

        //Фото
        private void Foto_Click(object sender, RoutedEventArgs e)
        {
            if (DBGridSpravochnik.SelectedItems.Count == 0) return;

            //Запоминаем позицию в DBGrid, чтобы после редактирования в другом окне установить курсор на эту строку
            int iStr = DBGridSpravochnik.SelectedIndex;

            ClassStr.TekCodFilm = int.Parse(((DataRowView)DBGridSpravochnik.SelectedItems[0]).Row["CodFilm"].ToString());
            ClassStr.TekNameFilm = ((DataRowView)DBGridSpravochnik.SelectedItems[0]).Row["Name"].ToString();
            ClassStr.TekNameFileFoto = ((DataRowView)DBGridSpravochnik.SelectedItems[0]).Row["StrFoto"].ToString();
            ClassStr.TekCodArtist = int.Parse(((DataRowView)DBGridSpravochnik.SelectedItems[0]).Row["CodArtist"].ToString());

            ProsmotrFoto ProsmotrFoto = new ProsmotrFoto();
            ProsmotrFoto.ShowDialog(); //блокируются предыдущие формы, активна только текущая (новая открытая)

            string zapros = "select * from Table_Film where CodArtist = " + ClassStr.TekCodArtist.ToString() + " order by Name";

            DBGridSpravochnik.ItemsSource = Podkl.GetDataFromTable(zapros).DefaultView;

            //Установить курсор на измененную запись
            object item2 = DBGridSpravochnik.Items[iStr];
            DBGridSpravochnik.SelectedItem = item2; // выделить нужную строку 
            DBGridSpravochnik.ScrollIntoView(item2);// Прокрутить к нужной строке
        }
    }
}
