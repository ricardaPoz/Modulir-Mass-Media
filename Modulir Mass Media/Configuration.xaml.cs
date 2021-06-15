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
    /// Логика взаимодействия для Configuration.xaml
    /// </summary>
    public partial class Configuration : Window
    {
        public Configuration()
        {
            InitializeComponent();
        }

        private void cmbUncoverAndHide_Unchecked(object sender, RoutedEventArgs e) => WindowState = WindowState.Normal;
        private void cmbUncoverAndHide_Checked(object sender, RoutedEventArgs e) => WindowState = WindowState.Maximized;
        private void closeForm_MouseDown(object sender, MouseButtonEventArgs e) => Close();

        private void container_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }
    }
}
