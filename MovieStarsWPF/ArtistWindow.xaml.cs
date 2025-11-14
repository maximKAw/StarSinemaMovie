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
using System.Data.OleDb;
using System.Windows.Media.Animation;
using System.Data.SqlClient;

namespace MovieStarsWPF
{
    /// <summary>
    /// Логика взаимодействия для ArtistWindow.xaml
    /// </summary>
    public partial class ArtistWindow : Window
    {
        Query Podkl; //Определим наш объект класса
        public int TekCodArtist = 0;
        public string Tekzapros = "";

        public ArtistWindow()
        {
            InitializeComponent();

            //Инициализируем. В качестве параметра передаем строку подключения
            Podkl = new Query(ConnectionString.ConnStr);

            Tekzapros = "select CodArtist,Fam,Name,Otch,StrFoto from Table_Artist ";
            //Tekzapros += " order by Fam,Name,Otch";

            DoubleAnimation bAnim = new DoubleAnimation
            {
                From = 0,
                To = 115,
                Duration = TimeSpan.FromSeconds(3)
            };
            Knopka_Add.BeginAnimation(Button.WidthProperty, bAnim);
            Knopka_Change.BeginAnimation(Button.WidthProperty, bAnim);
            Knopka_Delete.BeginAnimation(Button.WidthProperty, bAnim);
            Knopka_Nagrada.BeginAnimation(Button.WidthProperty, bAnim);
            Knopka_Film.BeginAnimation(Button.WidthProperty, bAnim);
            Knopka_Biography.BeginAnimation(Button.WidthProperty, bAnim);
            Exit.BeginAnimation(Button.WidthProperty, bAnim);
        }

        private void ArtistWin_Loaded(object sender, RoutedEventArgs e)
        {
            if (ClassStr.TekPrava != "Администратор")
            {
                Knopka_Add.Visibility = Visibility.Hidden;
                Knopka_Change.Visibility = Visibility.Hidden;
                Knopka_Delete.Visibility = Visibility.Hidden;
            }


            string zapros = Tekzapros;
            zapros += "order by Fam,Name,Otch";
            DBGridArtist.ItemsSource = Podkl.GetDataFromTable(zapros).DefaultView;
            //------------Установить курсор
            if (DBGridArtist.Items.Count > 0)
            {
                object item2 = DBGridArtist.Items[0];
                DBGridArtist.SelectedItem = item2; // выделить нужную строку 
            }
            //------------
        }

        //Кнопка - Добавить
        private void Knopka_Add_Click(object sender, RoutedEventArgs e)
        {
            ClassStr.TekNameFileFoto = "";

            ArtistInfo Artistinfo = new ArtistInfo();
            Artistinfo.RegimOper = 2; //2- режим создания
            Artistinfo.ShowDialog();//блокируются предыдущие формы, активна только текущая (новая открытая)
            this.TekCodArtist = Artistinfo.CheckArtist;
            if (TekCodArtist < 0) //Если был выход в окне с данными по доктору, т.е., данные не добавлены
                return;

            string zapros = Tekzapros;
            if (TextBoxPoiskFam.Text != "")
            {
                zapros += "where Fam like '%" + TextBoxPoiskFam.Text + "%' order by Fam,Name,Otch";
            }
            else
            {
                zapros += "order by Fam,Name,Otch";
            }

            DBGridArtist.ItemsSource = Podkl.GetDataFromTable(zapros).DefaultView;

            //Установить курсор на добавленную запись
            string searchValue = Convert.ToString(TekCodArtist);
            //Создаем и заполняем список из datagrid
            List<string> list = new List<string>();
            try
            {
                foreach (DataRowView row3 in DBGridArtist.Items)
                {
                    list.Add(row3["CodArtist"].ToString());
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

            for (int i = 0; i < list.Count; i++)
                if (list[i] == searchValue)
                {
                    object item2 = DBGridArtist.Items[i];
                    DBGridArtist.SelectedItem = item2; // выделить нужную строку 
                    DBGridArtist.ScrollIntoView(item2);// Прокрутить к нужной строке
                    break;
                }
            ////-----Конец установки курсора

        }

        //Кнопка - Удалить
        private void Knopka_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DBGridArtist.Items.Count < 1)
            {
                MessageBox.Show("Отсутствуют данные");
                return;
            }

            //Передаем индекс текущей записи в DBGridArtist
            DataRowView row = (DataRowView)DBGridArtist.SelectedItem;
            Podkl.DeleteFromTable("Table_Artist", "CodArtist", int.Parse(row["CodArtist"].ToString()));

            string zapros = Tekzapros;
            if (TextBoxPoiskFam.Text != "")
            {
                zapros += "where Fam like '%" + TextBoxPoiskFam.Text + "%' order by Fam,Name,Otch";
            }
            else
            {
                zapros += "order by Fam,Name,Otch";
            }
            DBGridArtist.ItemsSource = Podkl.GetDataFromTable(zapros).DefaultView;

            //------------Установить курсор
            if (DBGridArtist.Items.Count > 0)
            {
                object item2 = DBGridArtist.Items[0];
                DBGridArtist.SelectedItem = item2; // выделить нужную строку 
            }
            //------------

            MessageBox.Show("Удаление выполнено успешно");
        }

        //Кнопка - Выход
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //Кнопка - Изменить
        private void Knopka_Change_Click(object sender, RoutedEventArgs e)
        {
            if (DBGridArtist.Items.Count < 1)
            {
                MessageBox.Show("Отсутствуют данные");
                return;
            }

            //Запоминаем позицию в DBGrid, чтобы после редактирования в другом окне установить курсор на эту строку
            int iStr = DBGridArtist.SelectedIndex;

            ClassStr.TekNameFileFoto = ((DataRowView)DBGridArtist.SelectedItems[0]).Row["StrFoto"].ToString();

            ArtistInfo Artistinfo = new ArtistInfo();
            Artistinfo.RegimOper = 1; //1- режим редактирования
            Artistinfo.text_ID = int.Parse(((DataRowView)DBGridArtist.SelectedItems[0]).Row["CodArtist"].ToString());
            Artistinfo.ShowDialog(); //блокируются предыдущие формы, активна только текущая (новая открытая)

            string zapros = Tekzapros;
            if (TextBoxPoiskFam.Text != "")
            {
                zapros += "where Fam like '%" + TextBoxPoiskFam.Text + "%' order by Fam,Name,Otch";
            }
            else
            {
                zapros += "order by Fam,Name,Otch";
            }
            DBGridArtist.ItemsSource = Podkl.GetDataFromTable(zapros).DefaultView;

            //Установить курсор на измененную запись
            object item2 = DBGridArtist.Items[iStr];
            DBGridArtist.SelectedItem = item2; // выделить нужную строку 
            DBGridArtist.ScrollIntoView(item2);// Прокрутить к нужной строке
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Exit_Click(sender, e);
            }
        }

        private void TextBoxPoiskFam_TextChanged(object sender, TextChangedEventArgs e)
        {
            string zapros = Tekzapros;
            if (TextBoxPoiskFam.Text != "")
            {
                zapros += "where Fam like '%" + TextBoxPoiskFam.Text + "%' order by Fam,Name,Otch";
            }
            else
            {
                zapros += "order by Fam,Name,Otch";
            }
            DBGridArtist.ItemsSource = Podkl.GetDataFromTable(zapros).DefaultView;

            //------------Установить курсор
            if (DBGridArtist.Items.Count > 0)
            {
                object item2 = DBGridArtist.Items[0];
                DBGridArtist.SelectedItem = item2; // выделить нужную строку 
            }
            //------------

            TextBoxPoiskFam.Focus();
        }

        //Кнопка - Награды
        private void Knopka_Nagrada_Click(object sender, RoutedEventArgs e)
        {
            if (DBGridArtist.Items.Count < 1)
            {
                MessageBox.Show("Отсутствуют данные");
                return;
            }

            //Запоминаем позицию в DBGrid, чтобы после редактирования в другом окне установить курсор на эту строку
            int iStr = DBGridArtist.SelectedIndex;

            ClassStr.TekSpravochnik = 2; //Награды
            ClassStr.TekCodArtist = int.Parse(((DataRowView)DBGridArtist.SelectedItems[0]).Row["CodArtist"].ToString());
            ClassStr.TekFamNameArtist = ((DataRowView)DBGridArtist.SelectedItems[0]).Row["Fam"].ToString()  + " " +
                                        ((DataRowView)DBGridArtist.SelectedItems[0]).Row["Name"].ToString() + " " +
                                        ((DataRowView)DBGridArtist.SelectedItems[0]).Row["Otch"].ToString();
            SpravochnikWindow Per = new SpravochnikWindow();
            Per.ShowDialog(); //блокируются предыдущие формы, активна только текущая (новая открытая)

            string zapros = Tekzapros;
            if (TextBoxPoiskFam.Text != "")
            {
                zapros += "where Fam like '%" + TextBoxPoiskFam.Text + "%' order by Fam,Name,Otch";
            }
            else
            {
                zapros += "order by Fam,Name,Otch";
            }
            DBGridArtist.ItemsSource = Podkl.GetDataFromTable(zapros).DefaultView;

            //Установить курсор на измененную запись
            object item2 = DBGridArtist.Items[iStr];
            DBGridArtist.SelectedItem = item2; // выделить нужную строку 
            DBGridArtist.ScrollIntoView(item2);// Прокрутить к нужной строке
        }

        //Кнопка - Фильмы
        private void Knopka_Film_Click(object sender, RoutedEventArgs e)
        {
            if (DBGridArtist.Items.Count < 1)
            {
                MessageBox.Show("Отсутствуют данные");
                return;
            }

            //Запоминаем позицию в DBGrid, чтобы после редактирования в другом окне установить курсор на эту строку
            int iStr = DBGridArtist.SelectedIndex;

            ClassStr.TekSpravochnik = 1; //Фильмы
            ClassStr.TekCodArtist = int.Parse(((DataRowView)DBGridArtist.SelectedItems[0]).Row["CodArtist"].ToString());
            ClassStr.TekFamNameArtist = ((DataRowView)DBGridArtist.SelectedItems[0]).Row["Fam"].ToString() + " " +
                                        ((DataRowView)DBGridArtist.SelectedItems[0]).Row["Name"].ToString() + " " +
                                        ((DataRowView)DBGridArtist.SelectedItems[0]).Row["Otch"].ToString();
            SpravochnikWindow Per = new SpravochnikWindow();
            Per.ShowDialog(); //блокируются предыдущие формы, активна только текущая (новая открытая)

            string zapros = Tekzapros;
            if (TextBoxPoiskFam.Text != "")
            {
                zapros += "where Fam like '%" + TextBoxPoiskFam.Text + "%' order by Fam,Name,Otch";
            }
            else
            {
                zapros += "order by Fam,Name,Otch";
            }
            DBGridArtist.ItemsSource = Podkl.GetDataFromTable(zapros).DefaultView;

            //Установить курсор на измененную запись
            object item2 = DBGridArtist.Items[iStr];
            DBGridArtist.SelectedItem = item2; // выделить нужную строку 
            DBGridArtist.ScrollIntoView(item2);// Прокрутить к нужной строке
        }

        //Кнопка - Биография
        private void Knopka_Biography_Click(object sender, RoutedEventArgs e)
        {
            if (DBGridArtist.Items.Count < 1)
            {
                MessageBox.Show("Отсутствуют данные");
                return;
            }

            //Запоминаем позицию в DBGrid, чтобы после редактирования в другом окне установить курсор на эту строку
            int iStr = DBGridArtist.SelectedIndex;

            ClassStr.TekCodArtist = int.Parse(((DataRowView)DBGridArtist.SelectedItems[0]).Row["CodArtist"].ToString());
            ClassStr.TekFamNameArtist = ((DataRowView)DBGridArtist.SelectedItems[0]).Row["Fam"].ToString() + " " +
                                        ((DataRowView)DBGridArtist.SelectedItems[0]).Row["Name"].ToString() + " " +
                                        ((DataRowView)DBGridArtist.SelectedItems[0]).Row["Otch"].ToString();
            BiographyArtist Per = new BiographyArtist();
            Per.ShowDialog(); //блокируются предыдущие формы, активна только текущая (новая открытая)

            string zapros = Tekzapros;
            if (TextBoxPoiskFam.Text != "")
            {
                zapros += "where Fam like '%" + TextBoxPoiskFam.Text + "%' order by Fam,Name,Otch";
            }
            else
            {
                zapros += "order by Fam,Name,Otch";
            }
            DBGridArtist.ItemsSource = Podkl.GetDataFromTable(zapros).DefaultView;

            //Установить курсор на измененную запись
            object item2 = DBGridArtist.Items[iStr];
            DBGridArtist.SelectedItem = item2; // выделить нужную строку 
            DBGridArtist.ScrollIntoView(item2);// Прокрутить к нужной строке
        }

        private void DBGridArtist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
