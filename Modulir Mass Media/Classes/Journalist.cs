﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace Modulir_Mass_Media.Classes
{
    public class ProductCreatedEventArgs
    {
        public InformationProduct InformationProduct { get; private set; }
        public ProductCreatedEventArgs(InformationProduct informationProduct) => InformationProduct = informationProduct;
    }
    public abstract class Journalist
    {
        public delegate void ProductCreatedtHendler(object sender, ProductCreatedEventArgs e);
        public abstract event ProductCreatedtHendler InformationProductCreated;

        public string PassportId { get; set; }

        public string NameJournalist { get; set; }
        public string TypeJournalist { get; set; }

        private Timer creationTimer = new Timer();

        public Journalist(string passportId, string nameJournalist, string typeJournalist)
        {
            //this.rssParser = rssParser;
            this.NameJournalist = nameJournalist;
            TypeJournalist = typeJournalist;
            PassportId = passportId;
            //this.rssParser.ParseCompleted += Parser_ParseCompleted;
            creationTimer.AutoReset = true;
            Random workingTime = new Random();
            creationTimer.Interval = workingTime.Next(5000, 10000);
            creationTimer.Elapsed += CreationTimer_Elapsed;
        }

        private void CreationTimer_Elapsed(object sender, ElapsedEventArgs e) => CreateInformationProduct();
        //private void Parser_ParseCompleted(object sender, EventArgs e) => this.rssParser.ParseCompleted -= Parser_ParseCompleted;

        public void Employment()
        {
            Task.Run
               (
                   () => { StartWorking(); }
               );
        }

        public void Dismissal() => creationTimer.Stop();

        private void StartWorking() => creationTimer.Start();
        protected abstract void CreateInformationProduct();
    }

    class JournalistVideo : Journalist
    {
        public JournalistVideo(string passportId, string nameJournalist, string typeJournalist) : base(passportId, nameJournalist, typeJournalist) { }

        public override event ProductCreatedtHendler InformationProductCreated;
        
        protected override void CreateInformationProduct()
        {
            // Подключение к базе данных
            SqlConnection sqlConnection = new SqlConnection(@$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName}\NewsStore\Rss.mdf;Integrated Security=True");
            sqlConnection.Open();
            SqlCommand command;
            //

            // получение самой новой новости по ее максимальному ID
            command = new SqlCommand(@"select min([id]) from [Video] where [take] = 'false'", sqlConnection);

            try { _ = (int)command.ExecuteScalar(); }
            catch { return; }
            int idNews = (int)command.ExecuteScalar();

            // получение ссылки на видео для контента
            command = new SqlCommand($@"select [link] from [Video] where [id] = {idNews}", sqlConnection);
            string linkNews = (string)command.ExecuteScalar();

            string contentNews = (string)command.ExecuteScalar();

            // получение загаловка новости для добавление ее в информационный продукт 
            command = new SqlCommand($@"select [title] from [Video] where [id] = {idNews}", sqlConnection);
            string titleNews = (string)command.ExecuteScalar();

            command = new SqlCommand($@"select [category] from [Video] where [id] = {idNews}", sqlConnection);
            string category = (string)command.ExecuteScalar();


            InformationProduct informationProduct = new InformationProduct(titleNews, contentNews, linkNews, category);
            command = new SqlCommand($@"update [Video] set [take] = 1 where [id] = {idNews}", sqlConnection);
            command.ExecuteNonQuery();
            sqlConnection.Close();

            InformationProductCreated?.Invoke(this, new ProductCreatedEventArgs(informationProduct));
        }
    }

    class JournalistText : Journalist
    {
        public JournalistText(string passportId, string nameJournalist, string typeJournalist) : base(passportId, nameJournalist, typeJournalist) { }

        public override event ProductCreatedtHendler InformationProductCreated;

        protected override void CreateInformationProduct()
        {
            // Подключение к базе данных
            SqlConnection sqlConnection = new SqlConnection(@$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName}\NewsStore\Rss.mdf;Integrated Security=True");
            sqlConnection.Open();
            SqlCommand command;
            //

            // получение самой новой новости по ее максимальному ID
            command = new SqlCommand(@"select min([id]) from [Text] where [take] = 'false'", sqlConnection);

            try { _ = (int)command.ExecuteScalar(); }
            catch { return; }

            int idNews = (int)command.ExecuteScalar();
            command = new SqlCommand($@"update [Text] set [take] = 1 where [id] = {idNews}", sqlConnection);
            command.ExecuteNonQuery();

            // получение ссылки на новость 
            command = new SqlCommand($@"select [link] from [Text] where [id] = {idNews}", sqlConnection);
            string linkNews = (string)command.ExecuteScalar();

            // получение загаловка новости для добавление ее в информационный продукт 
            command = new SqlCommand($@"select [title] from [Text] where [id] = {idNews}", sqlConnection);
            string titleNews = (string)command.ExecuteScalar();


            // МЕТОД ПАРСИНГА ТЕКСТА С САЙТА
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // считываем весь html файл и помещаем его в строку 
            WebClient openNews = new WebClient();
            Stream stream = openNews.OpenRead(linkNews);
            StreamReader streamReader = new StreamReader(stream);
            string htmlDocument = streamReader.ReadToEnd();
            streamReader.Close();
            //

            List<string> ListInformationProduct = new List<string>();

            // получение из всего документа тэги <p> </p>
            string patternText = "(?nm)<p class=\"box-paragraph__text\">(?<unsorted_text>(\t|\r|\n|.)+?)</p>";
            Regex regex1 = new Regex(patternText, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            MatchCollection unsortedText = regex1.Matches(htmlDocument);
            foreach (Match item1 in unsortedText)
            {
                string textNews = item1.Groups["unsorted_text"].Value.ToString().Replace("&quot;", "\"").Replace("<em>", "").Replace("</em>", "").Replace("&nbsp;", " ");

                Regex regex2 = new Regex("<a(?<title>(\t|\r|\n|.)+?)>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                MatchCollection sortedText = regex2.Matches(textNews);
                foreach (Match item2 in sortedText)
                {
                    textNews = regex2.Replace(textNews, @"");
                }
                // строка с "чистым" текстом 
                string newTextNews = textNews.Replace("</a>", "").Replace("«", "\"").Replace("»", "\"");

                ListInformationProduct.Add(newTextNews);
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // Журналист взял новость 

            string category = (string)command.ExecuteScalar();

            InformationProduct informationProduct = new InformationProduct(titleNews, string.Concat(ListInformationProduct.Select(e => e + "\n")), linkNews, category);

            sqlConnection.Close();

            InformationProductCreated?.Invoke(this, new ProductCreatedEventArgs(informationProduct));
        }
    }
}
