﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

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
            //Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#312E2B"));
            /*    ModulationMediaWork modulationMediaWork = new ModulationMediaWork();
                modulationMediaWork.ModulationMedia();*/
            /*    RssParser rss = new RssParser(100);
                rss.StartParsing();*/

            //var client = new YoutubeClient();
            //var stream = client.Videos.Streams.GetManifestAsync("https://www.youtube.com/watch?v=0QQmkHlJPsQ");
            //var streamInfo = stream.GetMuxed().WithHighestVideoQuality();
            //media.Source = ()

            /*PlayVideo();*/
            /*DataContext = modulationMediaWork;
            listView.ItemsSource = modulationMediaWork.MassMediaInformationProducts;
            MessageBox.Show(modulationMediaWork.MassMediaInformationProducts.Count.ToString());*/
        }

        private void closeForm_MouseDown(object sender, MouseButtonEventArgs e) => Close();
        private void uncoverWorm_MouseDown(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Minimized;

        private void container_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }
        private void cmbUncoverAndHide_Checked(object sender, RoutedEventArgs e) => WindowState = WindowState.Maximized;

        private void cmbUncoverAndHide_Unchecked(object sender, RoutedEventArgs e) => WindowState = WindowState.Normal;

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        /*public async void PlayVideo()
{
var youtube = new YoutubeClient();

var streamManifest = await youtube.Videos.Streams.GetManifestAsync("https://www.youtube.com/watch?v=JdfxN-2vEzE");
var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
media.Source = new Uri(streamInfo.Url);
}*/
    }

}
