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
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using Modulir_Mass_Media.Classes;
using Modulir_Mass_Media.Helpers;

namespace Modulir_Mass_Media
{
    /// <summary>
    /// Логика взаимодействия для Page.xaml
    /// </summary>
    public partial class Page : Window
    {
        private MassMediaInformationProduct mediaProduct;

        public  Page()
        {
            InitializeComponent();
           
        }
        public Page(MassMediaInformationProduct mediaProduct)
        {
            InitializeComponent();
            DataContext = ViewModel.SubscriptionMassMedia;
            this.mediaProduct = mediaProduct;
            #region 
            tbxLike.Text = mediaProduct.InformationProduct.Like.ToString();
            tbxWow.Text = mediaProduct.InformationProduct.Wow.ToString();
            tbSad.Text = mediaProduct.InformationProduct.Sad.ToString();
            tbAngry.Text = mediaProduct.InformationProduct.Angry.ToString();
            tbDisLike.Text = mediaProduct.InformationProduct.DisLike.ToString();
            tbxHaHa.Text = mediaProduct.InformationProduct.HaHa.ToString();

            if (mediaProduct.InformationProduct.ContentProduct.Contains("youtube"))
            {
                videoPlayer.Visibility = Visibility.Visible;
                textBoxTitle.Text = mediaProduct.InformationProduct.TitleProduct;
                textBoxDatePublication.Text = mediaProduct.DatePublication.ToString();
                PlayVideo(mediaProduct.InformationProduct.ContentProduct);
            }
            else
            {
                textBlockContent.Visibility = Visibility.Visible;
                textBlockContent.Text = mediaProduct.InformationProduct.ContentProduct;
                textBlockContent.Text = mediaProduct.InformationProduct.ContentProduct;
                textBoxTitle.Text = mediaProduct.InformationProduct.TitleProduct;
                textBoxDatePublication.Text = mediaProduct.DatePublication.ToString();
            }
            #endregion
        }

        private async void PlayVideo(string link)
        {
            var youtube = new YoutubeClient();
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(link);
            var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
            videoPlayer.Source = new Uri(streamInfo.Url);
        }

        private void cmbUncoverAndHide_Unchecked(object sender, RoutedEventArgs e) => WindowState = WindowState.Normal;
        private void cmbUncoverAndHide_Checked(object sender, RoutedEventArgs e) => WindowState = WindowState.Maximized;
        private void closeForm_MouseDown(object sender, MouseButtonEventArgs e) => Close();
        private void container_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void btnSubscribe_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SubscribedToMediaCommand.Execute(mediaProduct);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewModel.PageClosingCommand.Execute(new EmotionCommandParametr(mediaProduct.InformationProduct.ProductType, mediaProduct.InformationProduct.LinkProduct, int.Parse(tbxLike.Text), int.Parse(tbxHaHa.Text), int.Parse(tbxWow.Text), int.Parse(tbSad.Text), int.Parse(tbAngry.Text), int.Parse(tbDisLike.Text)));
        }

        #region Эмоции
        private void cbxLike_Checked(object sender, RoutedEventArgs e) => tbxLike.Text = (int.Parse(tbxLike.Text) + 1).ToString();
        private void cbxLike_Unchecked(object sender, RoutedEventArgs e) => tbxLike.Text = (int.Parse(tbxLike.Text) - 1).ToString();
        private void cbxHaHa_Checked(object sender, RoutedEventArgs e) => tbxHaHa.Text = (int.Parse(tbxHaHa.Text) + 1).ToString();
        private void cbxHaHa_Unchecked(object sender, RoutedEventArgs e) => tbxHaHa.Text = (int.Parse(tbxHaHa.Text) - 1).ToString();
        private void cbxWow_Checked(object sender, RoutedEventArgs e) => tbxWow.Text = (int.Parse(tbxWow.Text) + 1).ToString();
        private void cbxWow_Unchecked(object sender, RoutedEventArgs e) => tbxWow.Text = (int.Parse(tbxWow.Text) - 1).ToString();
        private void cbxSad_Checked(object sender, RoutedEventArgs e) => tbSad.Text = (int.Parse(tbSad.Text) + 1).ToString();
        private void cbxSad_Unchecked(object sender, RoutedEventArgs e) => tbSad.Text = (int.Parse(tbSad.Text) - 1).ToString();
        private void cbxAngry_Checked(object sender, RoutedEventArgs e) => tbAngry.Text = (int.Parse(tbAngry.Text) + 1).ToString();
        private void cbxAngry_Unchecked(object sender, RoutedEventArgs e) => tbAngry.Text = (int.Parse(tbAngry.Text) - 1).ToString();
        private void DisLike_Checked(object sender, RoutedEventArgs e) => tbDisLike.Text = (int.Parse(tbDisLike.Text) + 1).ToString();
        private void DisLike_Unchecked(object sender, RoutedEventArgs e) => tbDisLike.Text = (int.Parse(tbDisLike.Text) - 1).ToString();

        #endregion
    }
}
