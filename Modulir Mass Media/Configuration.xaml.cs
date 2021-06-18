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
using System.Linq;
using System.Data;

namespace Modulir_Mass_Media
{
    /// <summary>
    /// Логика взаимодействия для Configuration.xaml
    /// </summary>
    public partial class Configuration : Window
    {
        #region Создание переменных
        private SqlConnection sqlConnection = new SqlConnection(@$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName}\NewsStore\Rss.mdf;Integrated Security=True");
        private ObservableCollection<MassMedia> medias;
        private ObservableCollection<Journalist> journalistsNotBusy;
        private ObservableCollection<Journalist> journalistsWorking;
        private ObservableCollection<RssParser> rssParsers;
        #endregion

        #region Инициализация СМИ, Неработающих журналистов, RSS ссылок
        private void InitializationSMI()
        {
            sqlConnection.Open();

            SqlCommand command;
            command = new SqlCommand("select NameSMI from SMI", sqlConnection);

            SqlDataReader reader = command.ExecuteReader();
            if(reader.HasRows)
            {
                while (reader.Read())
                {
                    medias.Add(new MassMedia(reader.GetString(0)));
                }
            }
            sqlConnection.Close();
        }
        private void InitializationJournalist()
        {
            sqlConnection.Open();
            SqlCommand command;
            command = new SqlCommand(@"select PassportId, NameJournalist, [Type] from Journalist where [NameSMI] is null", sqlConnection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    journalistsNotBusy.Add(reader.GetString(2) == "Text" ? new JournalistText(reader.GetString(0), reader.GetString(1), "Текстовый журналист") : new JournalistVideo(reader.GetString(0), reader.GetString(1), "Видео журналист"));
                }
            }
            sqlConnection.Close();

        }
        private void InitializationRSS()
        {
            sqlConnection.Open();
            SqlCommand command;

            command = new SqlCommand(@"select [LinkRSS] from RSS", sqlConnection);

            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    rssParsers.Add(new RssParser(reader.GetString(0)));
                }
            }
            sqlConnection.Close();
        }
        #endregion

        #region Закрытие, сворачивание, развертывание, передвижение формы
        private void cmbUncoverAndHide_Unchecked(object sender, RoutedEventArgs e) => WindowState = WindowState.Normal;
        private void cmbUncoverAndHide_Checked(object sender, RoutedEventArgs e) => WindowState = WindowState.Maximized;
        private void closeForm_MouseDown(object sender, MouseButtonEventArgs e) => Close();
        private void container_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }
        #endregion

        #region Добавление элементов
        private void btnAddMedia_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();

            if (string.IsNullOrEmpty(tbxNameMassMedia.Text))
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
            ResetColor();

            if (string.IsNullOrEmpty(tbxNameJournalist.Text) || cmbTypeJournalist.Text is null)
            {
                tbxError.Visibility = Visibility.Visible;
                tbxError.Text = "Укажите тип журналиста, ФИО журналиста и его номер паспорта";
                tbxError.Foreground = Brushes.Red;
                borderNameJournalist.BorderBrush = Brushes.Red;
                borderTypeJournalist.BorderBrush = Brushes.Red;
                borderPassportID.BorderBrush = Brushes.Red;
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

            command = new SqlCommand($"select count([PassportId]) from Journalist where [PassportId] = N'{tbxPassportId.Text}'", sqlConnection);
            int countСoincidences = (int)command.ExecuteScalar();
            if (countСoincidences == 1)
            {
                sqlConnection.Close();
                tbxError.Foreground = Brushes.Red;
                tbxError.Visibility = Visibility.Visible;
                tbxError.Text = "Журналист с таким паспортом уже существует";
                borderPassportID.BorderBrush = Brushes.Red;
                return;
            }


            string type = cmbTypeJournalist.Text == "Текст" ? "Text" : "Video";
            command = new SqlCommand($"Insert Into Journalist(PassportId, [NameJournalist], [Type]) values(N'{tbxPassportId.Text}', N'{tbxNameJournalist.Text}', N'{type}')", sqlConnection);
            command.ExecuteNonQuery();
            sqlConnection.Close();
            borderNameJournalist.BorderBrush = Brushes.Green;
            borderTypeJournalist.BorderBrush = Brushes.Green;
            borderPassportID.BorderBrush = Brushes.Green;
            tbxError.Foreground = Brushes.Green;
            journalistsNotBusy.Add(cmbTypeJournalist.Text == "Текст" ? new JournalistText(tbxPassportId.Text, tbxNameJournalist.Text, "Текстовый журналист") : new JournalistVideo(tbxPassportId.Text, tbxNameJournalist.Text, "Видео журналист"));
            tbxError.Visibility = Visibility.Visible;

            tbxError.Text = "Журналист успешно добавлен";

            #endregion
        }

        private void btnAddRSS_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();

            if (string.IsNullOrEmpty(tbxNameRSS.Text) || cmbTypeRSS.SelectedItem is null)
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
                    rssParsers.Add(new RssParser(tbxNameRSS.Text));

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

        private void listViewSMI_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResetColor();

            if (listViewSMI.SelectedValue as MassMedia is null) { sqlConnection.Close(); return; }

            journalistsWorking.Clear();

            sqlConnection.Open();
            SqlCommand command;

            try
            {
                command = new SqlCommand($"select * from Journalist where [NameSMI] = N'{(listViewSMI.SelectedValue as MassMedia).NameMedia}'", sqlConnection);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (journalistsWorking.Any(i => i.PassportId == reader.GetString(0))) { sqlConnection.Close(); return; }
                        journalistsWorking.Add(reader.GetString(2) == "Text" ? new JournalistText(reader.GetString(0), reader.GetString(1), "Текстовый журналист") : new JournalistVideo(reader.GetString(0), reader.GetString(1), "Видео журналист"));
                    }
                }
                sqlConnection.Close();
                tbxError.Visibility = Visibility.Collapsed;
            }
            catch { sqlConnection.Close(); }
            
        }

        private void btnRecruitment_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();

            if (listViewNotBusyJournalist.SelectedItem is null || listViewSMI.SelectedItem is null)
            {
                tbxError.Visibility = Visibility.Visible;
                tbxError.Text = "Для найма необходимо выбрать СМИ и журналиста";
                tbxError.Foreground = Brushes.Red;
                borderListSMI.BorderBrush = Brushes.Red;
                borderlNotBusyJournalist.BorderBrush = Brushes.Red;
                return;
            }
            else
            {
                tbxError.Visibility = Visibility.Collapsed;
                borderListSMI.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
                borderlNotBusyJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            }

            sqlConnection.Open();

            SqlCommand command;

            string typeJournalist = (listViewNotBusyJournalist.SelectedValue as Journalist).TypeJournalist;
            string nameJournalist = (listViewNotBusyJournalist.SelectedValue as Journalist).NameJournalist;
            string passportJournalist = (listViewNotBusyJournalist.SelectedValue as Journalist).PassportId;



            command = new SqlCommand($@"Update Journalist set [NameSMI] = N'{(listViewSMI.SelectedValue as MassMedia).NameMedia}' where [PassportId] = N'{passportJournalist}'", sqlConnection);
            command.ExecuteNonQuery();
            sqlConnection.Close();
            journalistsWorking.Add(typeJournalist == "Текстовый журналист" ? new JournalistText(passportJournalist, nameJournalist, typeJournalist) : new JournalistVideo(passportJournalist, nameJournalist, typeJournalist));
            journalistsNotBusy.Remove(listViewNotBusyJournalist.SelectedValue as Journalist);
        }
        #endregion

        #region Color Chenge
        private void textBox_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetColor();
        }
        private void listView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ResetColor();
        }
        private void comboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ResetColor();
        }

        private void tbxPassportId_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetColor();
        }

        private void listViewRSS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResetColor();
        }
        private void listViewNotBusyJournalist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResetColor();
        }

        private void ResetColor()
        {
            tbxError.Visibility = Visibility.Collapsed;
            borderNameRSS.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderCmbRSS.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderNameJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderTypeJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderNameMassMedia.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderListSMI.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderlNotBusyJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderPassportID.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderWorkingJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            borderListViewRSS.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
        }
        #endregion

        #region Удаление
        private void btnRemoveNotBussyJornalist_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();

            if (listViewNotBusyJournalist.SelectedItem is null)
            {
                tbxError.Visibility = Visibility.Visible;
                tbxError.Text = "Для удаления необходимо выбрать журналиста";
                tbxError.Foreground = Brushes.Red;
                borderlNotBusyJournalist.BorderBrush = Brushes.Red;
                return;
            }
            else
            {
                tbxError.Visibility = Visibility.Collapsed;
                borderlNotBusyJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            }

            sqlConnection.Open();

            SqlCommand command;

            command = new SqlCommand($"delete from Journalist where PassportId = N'{(listViewNotBusyJournalist.SelectedValue as Journalist).PassportId}'", sqlConnection);
            command.ExecuteNonQuery();
            sqlConnection.Close();

            journalistsNotBusy.Remove(listViewNotBusyJournalist.SelectedValue as Journalist);
            tbxError.Visibility = Visibility.Visible;
            tbxError.Text = "Журналист удален";
            tbxError.Foreground = Brushes.Green;
        }
        private void btnRemoveSMI_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();

            if (listViewSMI.SelectedItem is null)
            {
                tbxError.Visibility = Visibility.Visible;
                tbxError.Text = "Для удаления необходимо выбрать СМИ";
                tbxError.Foreground = Brushes.Red;
                borderListSMI.BorderBrush = Brushes.Red;
                return;
            }
            else
            {
                tbxError.Visibility = Visibility.Collapsed;
                borderlNotBusyJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            }

            sqlConnection.Open();

            SqlCommand command;
            command = new SqlCommand($"update Journalist set [NameSMI] = NULL where [NameSMI] = N'{(listViewSMI.SelectedValue as MassMedia).NameMedia}'", sqlConnection);
            command.ExecuteNonQuery();


            command = new SqlCommand($"delete from SMI where [NameSMI] = N'{(listViewSMI.SelectedValue as MassMedia).NameMedia}'", sqlConnection);
            command.ExecuteNonQuery();
            sqlConnection.Close();

            medias.Remove(listViewSMI.SelectedValue as MassMedia);
            journalistsWorking.Clear();
            journalistsNotBusy.Clear();
            InitializationJournalist();

            tbxError.Visibility = Visibility.Visible;
            tbxError.Text = "СМИ удалено";
            tbxError.Foreground = Brushes.Green;
        }
        private void btnRemoveWorkingJournalist_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();

            if (listViewWorkingJournalist.SelectedItem is null)
            {
                tbxError.Visibility = Visibility.Visible;
                tbxError.Text = "Для удаления необходимо выбрать Журналиста";
                tbxError.Foreground = Brushes.Red;
                borderWorkingJournalist.BorderBrush = Brushes.Red;
                return;
            }
            else
            {
                tbxError.Visibility = Visibility.Collapsed;
                borderWorkingJournalist.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            }

            sqlConnection.Open();

            SqlCommand command;

            command = new SqlCommand($"update Journalist set [NameSMI] = NULL where [PassportId] = N'{(listViewWorkingJournalist.SelectedValue as Journalist).PassportId}' and [NameSMI] = N'{(listViewSMI.SelectedValue as MassMedia).NameMedia}'", sqlConnection);
            command.ExecuteNonQuery();
            sqlConnection.Close();

            journalistsWorking.Remove(listViewWorkingJournalist.SelectedValue as Journalist);
            journalistsNotBusy.Clear();
            InitializationJournalist();

            tbxError.Visibility = Visibility.Visible;
            tbxError.Text = "Журналист удален";
            tbxError.Foreground = Brushes.Green;
        }
        private void btnRemoveRSS_Click(object sender, RoutedEventArgs e)
        {
            if (listViewRSS.SelectedItem is null)
            {
                tbxError.Visibility = Visibility.Visible;
                tbxError.Text = "Для удаления необходимо выбрать RSS ссылку";
                tbxError.Foreground = Brushes.Red;
                borderListViewRSS.BorderBrush = Brushes.Red;
                return;
            }
            else
            {
                tbxError.Visibility = Visibility.Collapsed;
                borderListViewRSS.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 216, 216));
            }

            sqlConnection.Open();

            SqlCommand command;
            command = new SqlCommand($"delete from RSS where [LinkRSS] = N'{(listViewRSS.SelectedValue as RssParser).LinkRss}'", sqlConnection);
            command.ExecuteNonQuery();
            sqlConnection.Close();

            rssParsers.Remove(listViewRSS.SelectedValue as RssParser);

            tbxError.Visibility = Visibility.Visible;
            tbxError.Text = "RSS ссылка удалена";
            tbxError.Foreground = Brushes.Green;
        }
        #endregion

        public Configuration()
        {
            InitializeComponent();
            medias = new ObservableCollection<MassMedia>();
            journalistsNotBusy = new ObservableCollection<Journalist>();
            journalistsWorking = new ObservableCollection<Journalist>();
            rssParsers = new ObservableCollection<RssParser>();
            listViewRSS.ItemsSource = rssParsers;
            listViewSMI.ItemsSource = medias;
            listViewNotBusyJournalist.ItemsSource = journalistsNotBusy;
            listViewWorkingJournalist.ItemsSource = journalistsWorking;
            InitializationSMI();
            InitializationJournalist();
            InitializationRSS();


        }
       
    }
}
