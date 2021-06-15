using CodeHollow.FeedReader;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Timer = System.Timers.Timer;

namespace Modulir_Mass_Media.Classes
{
    enum TypeRssInfo
    {
        Text,
        Audio,
        Video
    }

    class RssParser
    {
        /*public delegate void RssParserHendler(object sender, EventArgs e);
        public event RssParserHendler ParseCompleted;*/
        private Timer updateTimer = new Timer();

        private bool isWork = false;


        public RssParser()
        {
            updateTimer.AutoReset = true;
            //UpdatePeriod.Interval = 3600000;
            updateTimer.Interval = 30000;
            updateTimer.Elapsed += UpdatePeriod_Elapsed;
        }

        public RssParser(double updateInterval)
        {
            updateTimer.AutoReset = true;
            updateTimer.Interval = updateInterval;
            updateTimer.Elapsed += UpdatePeriod_Elapsed;
        }

        public void StartParsing()
        {
            if (isWork) return;
            isWork = true;
            UpdateInfo();
            updateTimer.Start();
        }

        public void StopParsing()
        {
            if (!isWork) return;
            isWork = false;
            updateTimer.Stop();
        }

        private void UpdatePeriod_Elapsed(object sender, ElapsedEventArgs e) => UpdateInfo();
        private void UpdateInfo()
        {
            Task.WaitAll
                (
                    Task.Run(() => GetNews(@"https://www.vedomosti.ru/rss/news", TypeRssInfo.Text)),
                    Task.Run(() => GetNews(@"https://www.echo.msk.ru/podcasts/daidudja.rss", TypeRssInfo.Audio)),
                    Task.Run(() => GetNews(@"https://www.youtube.com/feeds/videos.xml?channel_id=UCFU30dGHNhZ-hkh0R10LhLw", TypeRssInfo.Video))
                );
        }

        private void GetNews(string linkToRSS, TypeRssInfo typeRssInfo)
        {
            SqlConnection sqlConnection;
            sqlConnection = new SqlConnection(@$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName}\NewsStore\Rss.mdf;Integrated Security=True");

            try
            {
                Feed rssFile = FeedReader.Read(linkToRSS);
                sqlConnection.Open();
                SqlCommand command;

                string request = $"Insert Into {typeRssInfo}(title, link, date_publications, take, category) values";

                DateTime date;
                DateTime datePublications;

                command = new SqlCommand($"SELECT COUNT(*) FROM {typeRssInfo}", sqlConnection);

                if ((int)command.ExecuteScalar() == 0)
                {
                    List<FeedItem> newsList = rssFile.Items.ToArray().Reverse().ToList();
                    foreach (var item in newsList)
                    {
                        date = item.PublishingDate.Value;
                        datePublications = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second).AddHours(3);
                        string title = item.Title.Trim();
                        string category = (item.Categories.Count == 0 ? "Категория отсутствует" : item.Categories.ToList()[0]);

                        if (item == newsList[newsList.Count - 1]) request += $"(N'{title.Replace("'", "")}', N'{item.Link}', '{$"{datePublications.Year}-{datePublications.Month}-{datePublications.Day} {datePublications.Hour}:{datePublications.Minute}:{datePublications.Second}"}', {0}, N'{category}')";
                        else request += $"(N'{title.Replace("'", "")}', N'{item.Link}', '{$"{datePublications.Year}-{datePublications.Month}-{datePublications.Day} {datePublications.Hour}:{datePublications.Minute}:{datePublications.Second}"}', {0}, N'{category}'),";
                    }
                    command = new SqlCommand(request, sqlConnection);
                    command.ExecuteNonQuery();
                }
                else
                {
                    command = new SqlCommand($"select max([date_publications]) from {typeRssInfo}", sqlConnection);
                    DateTime newestDate = (DateTime)command.ExecuteScalar();
                    var x = rssFile.Items.ToArray().Reverse().ToList();
                    List<FeedItem> filteredNewsList = rssFile.Items.Where(e => e.PublishingDate.Value.AddHours(3) > newestDate).ToArray().Reverse().ToList();
                    if (filteredNewsList.Count != 0)
                    {
                        foreach (var item in filteredNewsList)
                        {
                            date = item.PublishingDate.Value;
                            datePublications = new DateTime(date.Year, item.PublishingDate.Value.Month, date.Day, date.Hour, date.Minute, date.Second).AddHours(3);
                            string title = item.Title.Trim();
                            string category = (item.Categories.Count == 0 ? "Категория отсутствует" : item.Categories.ToList()[0]);

                            if (item == filteredNewsList[filteredNewsList.Count - 1]) request += $"(N'{title.Replace("'", "")}', N'{item.Link}', '{$"{datePublications.Year}-{datePublications.Month}-{datePublications.Day} {datePublications.Hour}:{datePublications.Minute}:{datePublications.Second}"}', {0}, N'{category}')";
                            else request += $"(N'{title.Replace("'", "")}', N'{item.Link}', '{$"{datePublications.Year}-{datePublications.Month}-{datePublications.Day} {datePublications.Hour}:{datePublications.Minute}:{datePublications.Second}"}', {0}, N'{category}'),";
                        }
                        command = new SqlCommand(request, sqlConnection);
                        command.ExecuteNonQuery();
                    }
                }
                sqlConnection.Close();
                //ParseCompleted?.Invoke(this, new EventArgs());
            }
            catch (Exception exception) { MessageBox.Show($"{exception}");  }
        }
    }
}
