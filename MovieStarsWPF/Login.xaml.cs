using MovieStarsWPF.Podkl;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json; //для работы с JSON
using static MovieStarsWPF.Podkl.ClassStr;

namespace MovieStarsWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class Login : Window
    {
        Query Podkl; //Определим наш объект класса

        public Login()
        {
            InitializeComponent();
        }

        //Обработчик горячих клавиш на окне программы
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ButtonExit_Click(sender, e);
            }
        }

        //Обработчик загрузки окна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextBoxLogin.Focus();//фокус в поле Логин
        }

        //Кнопка - Вход
        private void ButtonVhod_Click(object sender, RoutedEventArgs e)
        {
            string login = TextBoxLogin.Text.Trim();
            string passw = TextBoxPassword.Text.Trim();

            if (login != "admin" || passw != "admin")
            {
                if(login != "user" || passw != "user")
                {
                    //неверные логин или пароль, выводим сообщение
                    MessageBox.Show(this, "Проверьте правильность введенной информации. Неверный логин или пароль.", "Режим авторизации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TextBoxLogin.Focus();//фокус в поле Логин
                    return;
                }
            }

            ClassStr.TekPrava = (login == "admin" ? "Администратор" : "Пользователь");

            //Прячем текущее окно
            this.Visibility = Visibility.Collapsed;
            MainMenu mainMenu = new MainMenu();
            mainMenu.ShowDialog(); //вызов главного окна
            Close();//Закрыть окно программы
        }

        //Обработчик горячих клавиш в поле Логин
        private void TextBoxLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ButtonExit_Click(sender, e); //Выход
            }
            else if (e.Key == Key.Enter)
            {
                ButtonVhod_Click(sender, e);//Вход
            }
        }

        //Кнопка Выход
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            App.IsExit = true;
            Close();//Закрыть окно программы
        }
    }
}
