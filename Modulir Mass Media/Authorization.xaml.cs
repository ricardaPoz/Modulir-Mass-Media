using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Modulir_Mass_Media
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private void pbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pbPassword.Password.Length > 0) waterMark.Visibility = Visibility.Collapsed;
            else waterMark.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (pbPassword.Password.Length < 1 || tbLogin.Text.Length == 0)
            {
                borderLogin.BorderBrush = Brushes.Red;
                borderPassword.BorderBrush = Brushes.Red;
                errorWrite.Visibility = Visibility.Visible;
                errorTextBlock.Text = "Введите логин или пароль";
            }
            else
            {
                borderLogin.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
                borderPassword.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
                errorWrite.Visibility = Visibility.Collapsed;
            }
        }

        private void Viewbox_MouseDown(object sender, MouseButtonEventArgs e) => Close();

        private void container_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void btnLoginNotAuthorization_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.ShowDialog();
        }
    }
}
