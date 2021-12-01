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
using Modulir_Mass_Media.Helpers;
using System.Data;

namespace Modulir_Mass_Media
{
    /// <summary>
    /// Логика взаимодействия для Configuration.xaml
    /// </summary>
    public partial class Configuration : Window
    {
        public Configuration()
        {
            InitializeComponent();

            DataContext = ViewModel.JournalistsNotBusy;


            ViewModel.InitializationMedia.Execute(null);
            ViewModel.InitializationJournalist.Execute(null);
            ViewModel.InitializationRSS.Execute(null);

            ViewModel.AddMediaAccepted += AddMediaAccepted;
            ViewModel.AddJournalistAccepted += AddJournalistAccepted;
            ViewModel.AddRSSAccepted += AddRSSAccepted;
            ViewModel.RecruitmentAccepted += RecruitmentAccepted;
            ViewModel.MediaSelectionAccepted += MediaSelectionAccepted;
            ViewModel.RemoveNotBussyAccepted += RemoveNotBussyAccepted;
            ViewModel.RemoveSMIAccepted += RemoveSMIAccepted;
            ViewModel.RemoveWorkingJournalistAccepted += RemoveWorkingJournalistAccepted;
            ViewModel.RemoveRSSAccepted += RemoveRSSAccepted;
        }

        #region Accepted
        private void RemoveRSSAccepted(bool removeRSSAccepted, string messege, ElementChanged elementChanged)
        {
            if (removeRSSAccepted) DisplayNotification(Brushes.Green, messege, elementChanged);
            else DisplayNotification(Brushes.Red, messege, elementChanged);
        }
        private void RemoveWorkingJournalistAccepted(bool removeWorkingJournalistAccepted, string messege, ElementChanged elementChanged)
        {
            if (removeWorkingJournalistAccepted) DisplayNotification(Brushes.Green, messege, elementChanged);
            else DisplayNotification(Brushes.Red, messege, elementChanged);
        }
        private void RemoveSMIAccepted(bool removeSMIAccepted, string messege, ElementChanged elementChanged)
        {
            if (removeSMIAccepted) DisplayNotification(Brushes.Green, messege, elementChanged);
            else DisplayNotification(Brushes.Red, messege, elementChanged);
        }
        private void RemoveNotBussyAccepted(bool removeNotBussyAccepted, string messege, ElementChanged elementChanged)
        {
            if (removeNotBussyAccepted) DisplayNotification(Brushes.Green, messege, elementChanged);
            else DisplayNotification(Brushes.Red, messege, elementChanged);
        }
        private void MediaSelectionAccepted(bool selectionAccepted, string messege, ElementChanged elementChanged)
        {
            if (selectionAccepted) DisplayNotification(Brushes.Green, messege, elementChanged);
            else DisplayNotification(Brushes.Red, messege, elementChanged);
        }
        private void RecruitmentAccepted(bool recruitmentAccepted, string messege, ElementChanged elementChanged)
        {
            if (recruitmentAccepted) DisplayNotification(Brushes.Green, messege, elementChanged);
            else DisplayNotification(Brushes.Red, messege, elementChanged);
        }
        private void AddRSSAccepted(bool addRSSAccepted, string messege, ElementChanged elementChanged)
        {
            if (addRSSAccepted) DisplayNotification(Brushes.Green, messege, elementChanged);
            else DisplayNotification(Brushes.Red, messege, elementChanged);
        }
        private void AddJournalistAccepted(bool addJournalistAccepted, string messege, ElementChanged elementChanged)
        {
            if (addJournalistAccepted) DisplayNotification(Brushes.Green, messege, elementChanged);
            else DisplayNotification(Brushes.Red, messege, elementChanged);
        }
        private void AddMediaAccepted(bool addMediaAccepted, string messege, ElementChanged elementChanged)
        {
            if (addMediaAccepted) DisplayNotification(Brushes.Green, messege, elementChanged);
            else DisplayNotification(Brushes.Red, messege, elementChanged);
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

        #region Color Chenge
        private void DisplayNotification(Brush brush, string messege, ElementChanged elementChanged)
        {
            switch (elementChanged)
            {
                case ElementChanged.AddMedia:
                    {
                        borderNameMassMedia.BorderBrush = brush;
                        tbxError.Visibility = Visibility.Visible;
                        tbxError.Foreground = brush;
                        tbxError.Text = messege;
                        break;
                    }
                case ElementChanged.AddJournalist:
                    {
                        tbxError.Visibility = Visibility.Visible;
                        tbxError.Text = messege;
                        tbxError.Foreground = brush;
                        borderNameJournalist.BorderBrush = brush;
                        borderTypeJournalist.BorderBrush = brush;
                        borderPassportID.BorderBrush = brush;
                        break;
                    }
                case ElementChanged.AddRSS:
                    {
                        tbxError.Visibility = Visibility.Visible;
                        tbxError.Text = messege;
                        tbxError.Foreground = brush;
                        borderNameRSS.BorderBrush = brush;
                        borderCmbRSS.BorderBrush = brush;
                        break;
                    }
                case ElementChanged.RecruitmentJournalist:
                    {
                        tbxError.Visibility = Visibility.Visible;
                        tbxError.Text = messege;
                        tbxError.Foreground = brush;
                        borderListSMI.BorderBrush = brush;
                        borderlNotBusyJournalist.BorderBrush = brush;
                        break;
                    }
                case ElementChanged.RemoveJournalistNotBusy:
                    {
                        tbxError.Visibility = Visibility.Visible;
                        tbxError.Text = messege;
                        tbxError.Foreground = brush;
                        borderlNotBusyJournalist.BorderBrush = brush;
                        break;
                    }
                case ElementChanged.RemoveJournalistWorking:
                    {
                        tbxError.Visibility = Visibility.Visible;
                        tbxError.Text = messege;
                        tbxError.Foreground = brush;
                        borderWorkingJournalist.BorderBrush = brush;
                        break;
                    }
                case ElementChanged.RemoveRSS:
                    {
                        tbxError.Visibility = Visibility.Visible;
                        tbxError.Text = messege;
                        tbxError.Foreground = brush;
                        borderListViewRSS.BorderBrush = brush;
                        break;
                    }
                case ElementChanged.RemoveSMI:
                    {
                        tbxError.Visibility = Visibility.Visible;
                        tbxError.Text = messege;
                        tbxError.Foreground = brush;
                        borderListSMI.BorderBrush = brush;
                        break;
                    }
                case ElementChanged.ExeptionWorkDataBase:
                    {
                        tbxError.Visibility = Visibility.Visible;
                        tbxError.Text = messege;
                        tbxError.Foreground = brush;
                        break;
                    }
                case ElementChanged.MediaSelectionChanged:
                    {
                        tbxError.Visibility = Visibility.Collapsed;
                        break;
                    }
            }
        }

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

        #region Добавление элементов
        private void btnAddMedia_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();

            if (string.IsNullOrEmpty(tbxNameMassMedia.Text))
            {
                DisplayNotification(Brushes.Red, "Введите наименование СМИ", ElementChanged.AddMedia);
                return;
            }
            else ViewModel.AddMediaCommand.Execute(new Tuple<object, object>(tbxNameMassMedia.Text, null));
        }

        private void btnAddJournalist_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();

            if (string.IsNullOrEmpty(tbxNameJournalist.Text) || cmbTypeJournalist.SelectedIndex == -1 || !tbxPassportId.IsMaskFull)
            {
                DisplayNotification(Brushes.Red, "Укажите тип журналиста, ФИО журналиста и его номер паспорта (номер паспорта должен быть заполнен полностью)", ElementChanged.AddJournalist);
                return;
            }
            else ViewModel.AddJournalistCommand.Execute(new Tuple<object, object, object>(tbxNameJournalist.Text, tbxPassportId.Text, cmbTypeJournalist.Text));
        }

        private void btnAddRSS_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();

            if (string.IsNullOrEmpty(tbxNameRSS.Text) || cmbTypeRSS.SelectedItem is null)
            {
                DisplayNotification(Brushes.Red, "Введите ссылку на RSS или выберите тип RSS", ElementChanged.AddRSS);
                return;
            }
            else ViewModel.AddRssCommand.Execute(new Tuple<object, object>(tbxNameRSS.Text, (TypeRssInfo)Enum.Parse(typeof(TypeRssInfo) , cmbTypeRSS.Text == "Текст" ? "Text" : "Video")));

        }

        private void listViewSMI_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResetColor();

            if (listViewSMI.SelectedValue as MassMedia is null) { return; }
            else ViewModel.MediaSelectionChanged.Execute(new Tuple<object, object>((listViewSMI.SelectedValue as MassMedia).NameMedia, null));

        }

        private void btnRecruitment_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();

            if (listViewNotBusyJournalist.SelectedItem is null || listViewSMI.SelectedItem is null)
            {
                DisplayNotification(Brushes.Red, "Для найма необходимо выбрать СМИ и журналиста", ElementChanged.RecruitmentJournalist);
                return;
            }
            else
            {
                string nameMedia = (listViewSMI.SelectedValue as MassMedia).NameMedia;
                string passportJournalist = (listViewNotBusyJournalist.SelectedValue as Journalist).PassportId;
                string nameJournalist = (listViewNotBusyJournalist.SelectedValue as Journalist).NameJournalist;
                JournalistType typeJournalist = (listViewNotBusyJournalist.SelectedValue as Journalist).TypeJournalist;

                ViewModel.RecruitmentCommand.Execute(new Tuple<object, object, object, object, object>(nameMedia, passportJournalist, nameJournalist, typeJournalist, listViewNotBusyJournalist.SelectedValue as Journalist));
            }
        }
        #endregion

        #region Удаление
        private void btnRemoveNotBussyJornalist_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();

            if (listViewNotBusyJournalist.SelectedItem is null)
            {
                DisplayNotification(Brushes.Red, "Для удаления необходимо выбрать журналиста", ElementChanged.RemoveJournalistNotBusy);
                return;
            }
            else ViewModel.RemoveNotBussyJornalistCommand.Execute(new Tuple<object, object>((listViewNotBusyJournalist.SelectedValue as Journalist).PassportId, listViewNotBusyJournalist.SelectedValue as Journalist));
        }
        private void btnRemoveSMI_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();

            if (listViewSMI.SelectedItem is null)
            {
                DisplayNotification(Brushes.Red, "Для удаления необходимо выбрать СМИ", ElementChanged.RemoveSMI);
                return;
            }
            else ViewModel.RemoveMediaCommand.Execute(new Tuple<object, object>((listViewSMI.SelectedValue as MassMedia).NameMedia, listViewSMI.SelectedValue as MassMedia));
        }
        private void btnRemoveWorkingJournalist_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();

            if (listViewWorkingJournalist.SelectedItem is null)
            {
                DisplayNotification(Brushes.Red, "Для удаления необходимо выбрать Журналиста", ElementChanged.RemoveJournalistWorking);
                return;
            }
            else ViewModel.RemoveWorkingJournalistCommand.Execute(new Tuple<object, object, object>((listViewSMI.SelectedValue as MassMedia).NameMedia, (listViewWorkingJournalist.SelectedValue as Journalist).PassportId, listViewWorkingJournalist.SelectedValue as Journalist));
        }
        private void btnRemoveRSS_Click(object sender, RoutedEventArgs e)
        {
            if (listViewRSS.SelectedItem is null)
            {
                DisplayNotification(Brushes.Red, "Для удаления необходимо выбрать RSS ссылку", ElementChanged.RemoveRSS);
                
                return;
            }
            else ViewModel.RemoveRssCommand.Execute(new Tuple<object, object>((listViewRSS.SelectedValue as RssParser).LinkRss, listViewRSS.SelectedValue as RssParser));
        }
        #endregion
    }

}
