using MovieStarsWPF.Podkl;
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

namespace MovieStarsWPF
{
    /// <summary>
    /// Логика взаимодействия для MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        //Обработчик загрузки формы
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ClassStr.TekPrava != "Администратор")
            {
                //MenuArtist.IsEnabled = false;
            }
        }

        //Обработчик горячей клавиши ESC
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                MenuItemExit_Click(sender, e);
            }
        }

        //Обработчик меню Выход
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            App.IsExit = true;
            Close();
        }

        //Пункт меню "Артисты"
        private void Artist_Click(object sender, RoutedEventArgs e)
        {
            ArtistWindow Per = new ArtistWindow();
            Per.ShowDialog();
        }
    }
}
