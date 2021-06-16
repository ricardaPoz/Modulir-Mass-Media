using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Modulir_Mass_Media.Classes;
using Microsoft.Win32;
using System.Data.SqlClient;
using System.IO;
using CodeHollow.FeedReader;

namespace Modulir_Mass_Media
{
    /// <summary>
    /// Логика взаимодействия для Configuration.xaml
    /// </summary>
    public partial class Configuration : Window
    {
        private ObservableCollection<MassMedia> medias;
        private ObservableCollection<Journalist> journalists;

        public Configuration()
        {
            InitializeComponent();
            medias = new ObservableCollection<MassMedia>();
            journalists = new ObservableCollection<Journalist>();
            listViewSMI.ItemsSource = medias;
            listViewJournalist.ItemsSource = journalists;
            medias.Add(new MassMedia("вфывфы|"));
        }

        private void cmbUncoverAndHide_Unchecked(object sender, RoutedEventArgs e) => WindowState = WindowState.Normal;
        private void cmbUncoverAndHide_Checked(object sender, RoutedEventArgs e) => WindowState = WindowState.Maximized;
        private void closeForm_MouseDown(object sender, MouseButtonEventArgs e) => Close();

        private void container_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void btnAddMedia_Click(object sender, RoutedEventArgs e)
        {
            tbxError.Visibility = Visibility.Collapsed;
            borderNameRSS.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderCmbRSS.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderNameJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderTypeJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderNameMassMedia.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));

            if (tbxNameMassMedia.Text.Length == 0)
            {
                borderNameMassMedia.BorderBrush = Brushes.Red;
                tbxError.Visibility = Visibility.Visible;
                tbxError.Foreground = Brushes.Red;
                tbxError.Text = "Введите наименование СМИ";
                return;
            }
            else { borderNameMassMedia.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216)); tbxError.Visibility = Visibility.Collapsed; }

            #region Добавление СМИ
            SqlConnection sqlConnection;
            sqlConnection = new SqlConnection(@$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName}\NewsStore\Rss.mdf;Integrated Security=True");
            sqlConnection.Open();

            SqlCommand command;

            command = new SqlCommand($"select count([NameSMI]) from SMI where [NameSMI] = N'{tbxNameMassMedia.Text}'", sqlConnection);

            int countСoincidences = (int)command.ExecuteScalar();
            if (countСoincidences == 1)
            {
                sqlConnection.Close();
                tbxError.Foreground = Brushes.Red;
                tbxError.Visibility = Visibility.Visible;
                tbxError.Text = "Такое наименование СМИ уже существует";
                borderNameMassMedia.BorderBrush = Brushes.Red;
                return;
            }
            else
            {
                command = new SqlCommand($"Insert Into SMI([NameSMI]) values(N'{tbxNameMassMedia.Text}')", sqlConnection);
                command.ExecuteNonQuery();
                sqlConnection.Close();
                borderNameMassMedia.BorderBrush = Brushes.Green;
                tbxError.Text = "СМИ успешно добавлено";
                tbxError.Visibility = Visibility.Visible;
                tbxError.Foreground = Brushes.Green;
                medias.Add(new MassMedia(tbxNameMassMedia.Text));
            }
            #endregion
        }

        private void btnAddJournalist_Click(object sender, RoutedEventArgs e)
        {
            tbxError.Visibility = Visibility.Collapsed;
            borderNameRSS.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderCmbRSS.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderNameJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderTypeJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderNameMassMedia.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));

            if (tbxNameJournalist.Text.Length == 0 || cmbTypeJournalist.Text is null)
            {
                tbxError.Visibility = Visibility.Visible;
                tbxError.Text = "Введите ФИО журналиста";
                tbxError.Foreground = Brushes.Red;
                borderNameJournalist.BorderBrush = Brushes.Red;
                borderTypeJournalist.BorderBrush = Brushes.Red;
                return;
            }
            else
            {
                borderNameJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
                borderTypeJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
                tbxError.Visibility = Visibility.Collapsed;
            }

            #region Добавление Журналиста
            SqlConnection sqlConnection;
            sqlConnection = new SqlConnection(@$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName}\NewsStore\Rss.mdf;Integrated Security=True");
            sqlConnection.Open();

            SqlCommand command;

            
            command = new SqlCommand($"Insert Into Journalist([NameJournalist], [Type]) values(N'{tbxNameJournalist.Text}', N'{cmbTypeJournalist.Text}')", sqlConnection);
            command.ExecuteNonQuery();
            sqlConnection.Close();
            borderNameJournalist.BorderBrush = Brushes.Green;
            borderTypeJournalist.BorderBrush = Brushes.Green;
            tbxError.Foreground = Brushes.Green;
            journalists.Add(cmbTypeJournalist.Text == "Текст" ? new JournalistText(tbxNameJournalist.Text) : new JournalistVideo(tbxNameJournalist.Text));
            tbxError.Visibility = Visibility.Visible;

            tbxError.Text = "Журналист успешно добавлен";

            #endregion
        }

        private void btnAddRSS_Click(object sender, RoutedEventArgs e)
        {
            tbxError.Visibility = Visibility.Collapsed;
            borderNameRSS.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderCmbRSS.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderNameJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderTypeJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderNameMassMedia.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));

            if (tbxNameRSS.Text.Length == 0 || cmbTypeRSS.SelectedItem is null)
            {
                tbxError.Visibility = Visibility.Visible;
                tbxError.Text = "Введите ссылку на RSS или выберите тип RSS";
                tbxError.Foreground = Brushes.Red;
                borderNameRSS.BorderBrush = Brushes.Red;
                borderCmbRSS.BorderBrush = Brushes.Red;
                return;
            }
            else
            {
                borderNameJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
                borderCmbRSS.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
                tbxError.Visibility = Visibility.Collapsed;
            }
            try
            {
                FeedReader.Read(tbxNameRSS.Text);
                #region Добавление RSS
                SqlConnection sqlConnection;
                sqlConnection = new SqlConnection(@$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName}\NewsStore\Rss.mdf;Integrated Security=True");
                sqlConnection.Open();

                SqlCommand command;


                command = new SqlCommand($"select count([LinkRSS]) from RSS where [LinkRSS] = N'{tbxNameRSS.Text}'", sqlConnection);

                int countСoincidences = (int)command.ExecuteScalar();
                if (countСoincidences == 1)
                {
                    sqlConnection.Close();
                    tbxError.Foreground = Brushes.Red;
                    tbxError.Visibility = Visibility.Visible;
                    tbxError.Text = "Введенная ссылка на RSS уже существует";
                    borderNameRSS.BorderBrush = Brushes.Red;
                    borderCmbRSS.BorderBrush = Brushes.Red;
                    return;
                }
                else
                {
                    string typeRss = cmbTypeRSS.Text == "Текст" ? "Text" : "Video"; 
                    command = new SqlCommand($"Insert Into RSS([LinkRSS], [TypeRSS]) values(N'{tbxNameRSS.Text}', N'{typeRss}')", sqlConnection);
                    command.ExecuteNonQuery();
                    sqlConnection.Close();
                    borderNameRSS.BorderBrush = Brushes.Green;
                    borderCmbRSS.BorderBrush = Brushes.Green;
                    tbxError.Foreground = Brushes.Green;
                    tbxError.Visibility = Visibility.Visible;
                    tbxError.Text = "Ссылка на RSS успешно добавлена";
                }
                #endregion
            }
            catch
            {
                tbxError.Visibility = Visibility.Visible;
                tbxError.Text = "Ссылка не распознана";
                borderNameRSS.BorderBrush = Brushes.Red;
                borderCmbRSS.BorderBrush = Brushes.Red;
                return;
            }
        }

        #region Color Chenge
        private void textBox_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            tbxError.Visibility = Visibility.Collapsed;
            borderNameRSS.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderCmbRSS.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderNameJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderTypeJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderNameMassMedia.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
        }

        private void comboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            tbxError.Visibility = Visibility.Collapsed;
            borderNameRSS.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderCmbRSS.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderNameJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderTypeJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderNameMassMedia.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
        }
        #endregion
    }
}
