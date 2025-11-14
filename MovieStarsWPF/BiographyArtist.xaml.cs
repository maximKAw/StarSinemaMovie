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
using System.Windows.Markup;

namespace MovieStarsWPF
{
    /// <summary>
    /// Логика взаимодействия для BiographyArtist.xaml
    /// </summary>
    public partial class BiographyArtist : Window
    {
        Query Podkl; //Определим наш объект класса
        public int text_ID;
        public int RegimOper, CheckBiography;

        public BiographyArtist()
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

        //Обработчик загрузки окна
        private void BiographyArtistWin_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable bufferTable_Artist, bufferTable_Biography;
            bufferTable_Biography = new DataTable();
            bufferTable_Artist = new DataTable();

            if (ClassStr.TekPrava != "Администратор")
            {
                Save.Visibility = Visibility.Hidden;
            }

            BiographyA.Title = "Данные по биографии (" + ClassStr.TekFamNameArtist + ")'";

            //Проверяем, есть ли биография на артиста
            //Если нет ее, то создаем, иначе - отображаем данные биографии
            string Szapros = "select * from Table_Biography ";
            Szapros += " where CodArtist = ";
            Szapros += ClassStr.TekCodArtist.ToString();
            var cmd = new SqlCommand(@Szapros);
            cmd.Connection = new SqlConnection(ConnectionString.ConnStr);
            cmd.Connection.Open();
            int totalRecords = Convert.ToInt32(cmd.ExecuteScalar());
            if (totalRecords > 0)
            {
                RegimOper = 1;
            }
            else
            {
                RegimOper = 2;
            }
            //-----Конец поиска



            string zapros = "";
            switch (RegimOper)
            {
                case 1: //Режим редактирования

                    zapros = "select * from Table_Biography ";
                    zapros += " where CodArtist = ";
                    zapros += ClassStr.TekCodArtist.ToString();
                    bufferTable_Biography = Podkl.GetDataFromTable(zapros);
                    text_ID = int.Parse(bufferTable_Biography.Rows[0]["CodBiography"].ToString());

                    //Запроляняем поля для ввода информации
                    DataR.Text = bufferTable_Biography.Rows[0]["DataR"].ToString();
                    TextBoxFIO_Mother.Text = bufferTable_Biography.Rows[0]["FIO_Mother"].ToString();
                    TextBoxFIO_Father.Text = bufferTable_Biography.Rows[0]["FIO_Father"].ToString();
                    TextBoxCountry.Text = bufferTable_Biography.Rows[0]["Country"].ToString();
                    TextBoxRost.Text = bufferTable_Biography.Rows[0]["Rost"].ToString();
                    ComboBoxPol.Text = bufferTable_Biography.Rows[0]["Pol"].ToString();
                    TextBoxInform.Text = bufferTable_Biography.Rows[0]["Inform"].ToString();
                    break;
                case 2: //Режим создания
                    //Запроляняем поля для ввода информации
                    DataR.Text = DateTime.Today.ToString();
                    TextBoxFIO_Mother.Text = "";
                    TextBoxFIO_Father.Text = "";
                    TextBoxCountry.Text = "";
                    TextBoxRost.Text = "";
                    ComboBoxPol.Text = "";
                    TextBoxInform.Text = "";
                    break;
            }

            DataR.Focus();
        }

        //Кнопка - Сохранить
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //Выполняем проверки заполненных полей данных
            if (TextBoxFIO_Mother.Text == "")
            {
                MessageBox.Show("Укажите ФИО матери артиста");
                TextBoxFIO_Mother.Focus();
                return;
            }

            if (TextBoxFIO_Father.Text == "")
            {
                MessageBox.Show("Укажите ФИО отца артиста");
                TextBoxFIO_Father.Focus();
                return;
            }

            if (ComboBoxPol.Text == "")
            {
                MessageBox.Show("Выберите пол артиста");
                ComboBoxPol.Focus();
                return;
            }


            string StrJson = "";

            DateTime d1 = (DateTime)DataR.SelectedDate;
            string sData = d1.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

            switch (RegimOper)
            {
                case 1: //Режим редактирования
                    ClassBiography Per_1 = new ClassBiography(text_ID,
                                                    d1, TextBoxFIO_Mother.Text,
                                                    TextBoxFIO_Father.Text,
                                                    TextBoxCountry.Text,
                                                    int.Parse(TextBoxRost.Text.ToString()),
                                                    ComboBoxPol.Text,
                                                    TextBoxInform.Text,
                                                    ClassStr.TekCodArtist
                                                    );
                    StrJson = JsonConvert.SerializeObject(Per_1);
                    Podkl.Update_JSON("Table_Biography", StrJson);
                    break;
                case 2: //Режим создания
                    ClassBiography Per_2 = new ClassBiography(0,
                                                    d1, TextBoxFIO_Mother.Text,
                                                    TextBoxFIO_Father.Text,
                                                    TextBoxCountry.Text,
                                                    int.Parse(TextBoxRost.Text.ToString()),
                                                    ComboBoxPol.Text,
                                                    TextBoxInform.Text,
                                                    ClassStr.TekCodArtist
                                                    );
                    StrJson = JsonConvert.SerializeObject(Per_2);
                    CheckBiography = Podkl.Add_JSON("Table_Biography", StrJson);
                    break;
            }


            Close();
        }

        //Кнопка - Выход
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            CheckBiography = -1;
            Close();
        }

        //Обработчик валидации в полях с датами
        private void Data1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789 .".IndexOf(e.Text) < 0;
        }
    }
}
