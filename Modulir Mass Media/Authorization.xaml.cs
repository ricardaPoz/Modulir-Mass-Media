using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Modulir_Mass_Media.Classes;
namespace Modulir_Mass_Media
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        #region Перемещение, сворачивание формы
        private void pbPassword_PasswordChanged(object sender, RoutedEventArgs e) => waterMark.Visibility = pbPassword.Password.Length > 0 ? waterMark.Visibility = Visibility.Collapsed : waterMark.Visibility = Visibility.Visible;
        private void Viewbox_MouseDown(object sender, MouseButtonEventArgs e) => Close();
        private void container_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }
        #endregion

        #region Изменение цвета и отображения
        private void DisplayNotification(Brush brush, string messege)
        {
            borderLogin.BorderBrush = brush;
            errorTextBlock.Foreground = brush;
            errorWrite.Visibility = Visibility.Visible;
            errorTextBlock.Text = messege;
        }
        private void AuthorizationAccepted(object sender, bool authorizationAccepted, string message)
        {
            if (authorizationAccepted) DisplayNotification(Brushes.Green, message);
            else DisplayNotification(Brushes.Red, message);
        }
        private void RegistrationAccepted(object sender, bool registrationAccepted, string message)
        {
            if (registrationAccepted) DisplayNotification(Brushes.Green, message);
            else DisplayNotification(Brushes.Red, message);
        }
        private void tbLogin_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            errorWrite.Visibility = Visibility.Collapsed;
            borderLogin.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderPassword.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
        }
        private void pbPassword_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            errorWrite.Visibility = Visibility.Collapsed;
            borderLogin.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderPassword.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
        }
        #endregion

        private void btnCome_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(pbPassword.Password) || string.IsNullOrEmpty(tbLogin.Text))
            {
                DisplayNotification(Brushes.Red, "Введите логин и пароль");
                borderPassword.BorderBrush = Brushes.Red;
                return;
            }
            else ((ViewModel)DataContext).AuthorizationCommand.Execute(new Tuple<object, object>(tbLogin.Text, pbPassword.Password));
        }
        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(pbPassword.Password) || string.IsNullOrEmpty(tbLogin.Text))
            {
                DisplayNotification(Brushes.Red, "Введите логин и пароль");
                borderPassword.BorderBrush = Brushes.Red;
                return;
            }
            else ((ViewModel)DataContext).RegistrationCommand.Execute(new Tuple<object, object>(tbLogin.Text, pbPassword.Password));
        }

        public Authorization()
        {
            InitializeComponent();
            ((ViewModel)DataContext).AuthorizationAccepted += AuthorizationAccepted;
            ((ViewModel)DataContext).RegistrationAccepted += RegistrationAccepted;
        }

        private void btnSettingModelir_Click(object sender, RoutedEventArgs e)
        {
            ((ViewModel)DataContext).OpenConfigurationCommand.Execute(null);
        }

        private void btnLoginNotAuthorization_Click(object sender, RoutedEventArgs e)
        {
            ((ViewModel)DataContext).LoginWthoutRegistration.Execute(null);
        }
    }
}
