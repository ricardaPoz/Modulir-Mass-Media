using CodeHollow.FeedReader;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Timer = System.Timers.Timer;

namespace Modulir_Mass_Media.Classes
{
    public enum TypeRssInfo
    {
        Text,
        Video
    }
    public class RssParser
    {
        private Timer updateTimer = new Timer();
        private bool isWork = false;
        public string LinkRss { get; private set; } 
        public TypeRssInfo TypeRss { get; private set; }
        public RssParser(string linkToRSS, TypeRssInfo typeRssInfo)
        {
            LinkRss = linkToRSS;
            TypeRss = typeRssInfo;

            updateTimer.AutoReset = true;
            updateTimer.Interval = 30000;
            updateTimer.Elapsed += UpdatePeriod;

            ViewModel.RssStart += RssStart;
        }
        private void RssStart(RssParser rssParser)
        {
            if (isWork) return;
            isWork = true;
            UpdateInfo();
            updateTimer.Start();
        }
        private void UpdatePeriod(object sender, ElapsedEventArgs e) => UpdateInfo();
        private void UpdateInfo()
        {
            Task.WaitAll
                (
                    Task.Run(() => GetNews(LinkRss, TypeRss))
                );
        }
        private void GetNews(string linkToRSS, TypeRssInfo typeRssInfo)
        {
            SqlConnection sqlConnection;
            sqlConnection = new SqlConnection(@$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName}\Data\ModulationData.mdf;Integrated Security=True");

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
            }
            catch (Exception exception) { MessageBox.Show($"{exception}");  }
        }
    }
}
