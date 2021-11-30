using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using Modulir_Mass_Media.Classes;

namespace Modulir_Mass_Media
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        #region Найстройка формы
        private void closeForm_MouseDown(object sender, MouseButtonEventArgs e) => Close();
        private void uncoverWorm_MouseDown(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Minimized;
        private void container_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }
        private void cmbUncoverAndHide_Checked(object sender, RoutedEventArgs e) => WindowState = WindowState.Maximized;
        private void cmbUncoverAndHide_Unchecked(object sender, RoutedEventArgs e) => WindowState = WindowState.Normal;
        #endregion

        private void unsubscribe_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.UnbscribedToMediaCommand.Execute(new Tuple<object>(clientSubscribedMedia.SelectedValue as MassMedia));
        }

        private void btnReadMore_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ReadMoreCommand.Execute(new Tuple<object>(listViewAllMediaProduct.SelectedValue as MassMediaInformationProduct));
        }

        private void btnReadNews_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ReadMoreCommand.Execute(new Tuple<object>(listViewSubNews.SelectedValue as MassMediaInformationProduct));
        }
    }

}
