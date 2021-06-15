using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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

        private void Viewbox_MouseDown(object sender, MouseButtonEventArgs e) => Close();

        private void container_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

   

        private void btnCome_Click(object sender, RoutedEventArgs e)
        {
            if (pbPassword.Password.Length == 0 || tbLogin.Text.Length == 0)
            {
                borderLogin.BorderBrush = Brushes.Red;
                borderPassword.BorderBrush = Brushes.Red;
                errorWrite.Visibility = Visibility.Visible;
                errorTextBlock.Text = "Введите login и password";
            }
            else
            {
                borderLogin.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
                borderPassword.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
                errorWrite.Visibility = Visibility.Collapsed;
            }

            SqlConnection sqlConnection;
            sqlConnection = new SqlConnection(@$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName}\NewsStore\Rss.mdf;Integrated Security=True");
            sqlConnection.Open();

            SqlCommand command;
            
            command = new SqlCommand($"select count([Login]) from Client where [Login] = N'{tbLogin.Text}'", sqlConnection);

            
            
            int countСoincidences = (int)command.ExecuteScalar();
            if (countСoincidences == 0)
            {
                borderLogin.BorderBrush = Brushes.Red;
                errorWrite.Visibility = Visibility.Visible;
                errorTextBlock.Text = "Пользователь не найден";
                sqlConnection.Close();
                return;
            }
            else
            {
                command = new SqlCommand($"select [Password] from Client where [Login] = N'{tbLogin.Text}'", sqlConnection);
                string password = (string)command.ExecuteScalar();

                if (BCrypt.Net.BCrypt.Verify(pbPassword.Password, password))
                {
                    sqlConnection.Close();
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.ShowDialog();
                }
                else
                {
                    borderLogin.BorderBrush = Brushes.Red;
                    errorWrite.Visibility = Visibility.Visible;
                    errorTextBlock.Text = "Неверный логин или пароль";
                    sqlConnection.Close();
                    return;
                }
            }
        }

        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            if (pbPassword.Password.Length == 0 || tbLogin.Text.Length == 0)
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

            SqlConnection sqlConnection;
            sqlConnection = new SqlConnection(@$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName}\NewsStore\Rss.mdf;Integrated Security=True");
            sqlConnection.Open();

            SqlCommand command;

            command = new SqlCommand($"select count([Login]) from Client where [Login] = N'{tbLogin.Text}'",sqlConnection);
            
            int countСoincidences = (int)command.ExecuteScalar();
            if (countСoincidences == 1)
            {
                borderLogin.BorderBrush = Brushes.Red;
                errorTextBlock.Foreground = Brushes.Red;
                errorWrite.Visibility = Visibility.Visible;
                errorTextBlock.Text = "Такой логин уже существует";
                sqlConnection.Close();
                return;
            }
            else
            {
                command = new SqlCommand($"Insert Into Client([Login], [Password]) values(N'{tbLogin.Text}', N'{BCrypt.Net.BCrypt.HashPassword(pbPassword.Password)}')", sqlConnection);
                command.ExecuteNonQuery();
                errorTextBlock.Foreground = Brushes.Green;
                errorWrite.Visibility = Visibility.Visible;
                errorTextBlock.Text = "Клиент успешно зарегистрирован";
                sqlConnection.Close();
            }
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
    }
}
