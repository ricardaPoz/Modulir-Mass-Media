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

namespace Modulir_Mass_Media
{
    /// <summary>
    /// Логика взаимодействия для Page.xaml
    /// </summary>
    public partial class Page : Window
    {
        public  Page()
        {
            InitializeComponent();
            text.Text = " Вашингтон, 13 июня. Совместные затраты России и Китая на оборону превзошли военный бюджет США, заявил председатель Комитета начальников штабов американских ВС генерал Марк Милли.";
            text.Text += "\n\n Этой причиной сенатор Джим Инхоф объяснил необходимость увеличения военного финансирования в Соединенных Штатах. По его словам, при подсчете бюджетов стоит учитывать покупательную способность валют. В итоге выходит, что КНР тратит на армию 604 миллиарда долларов, а Россия — около 200 миллиардов долларов.";
            text.Text += "\n\n При этом, по информации аналитического института SIPRI, номинальные траты на оборону за 2020 год в США составили 778 миллиардов долларов, в Китае — 252 миллиарда долларов и 61,7 миллиарда долларов в России.";
            text.Text += "\n\n Ранее генерал Милли жаловался на то, что Москва и Пекин продолжают скрывать информацию о затратах на военные нужды, а потому данные об оборонных бюджетах остаются загадкой для США.";
            text.Text += "\n\n Ранее генерал Милли жаловался на то, что Москва и Пекин продолжают скрывать информацию о затратах на военные нужды, а потому данные об оборонных бюджетах остаются загадкой для США.";
            //PlayV();
        }

        private async void PlayV()
        {
            var youtube = new YoutubeClient();
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync("https://www.youtube.com/watch?v=JdfxN-2vEzE");
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

        private void chbLike_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
