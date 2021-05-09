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
using System.Windows.Navigation;
using System.Windows.Shapes;
using  Modulir_Mass_Media.Classes;

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
            ListView ls = new ListView();
            //Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#312E2B"));

            //RssParser rss = new RssParser(10000);
            //rss.StartParsing();
        }
    }
}
