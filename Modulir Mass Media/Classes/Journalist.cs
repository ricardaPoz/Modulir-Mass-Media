using System;
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

    public enum JournalistType
    {
        Text,
        Video
    }
    public class ProductCreatedEventArgs
    {
        public InformationProduct InformationProduct { get; private set; }
        public ProductCreatedEventArgs(InformationProduct informationProduct) => InformationProduct = informationProduct;
    }
    public class Journalist
    {
        public delegate void ProductCreatedtHendler(object sender, ProductCreatedEventArgs e);
        public event ProductCreatedtHendler InformationProductCreated;

        public string PassportId { get; private set; }
        public string NameJournalist { get; private set; }
        public JournalistType TypeJournalist { get; private set; }

        private Timer creationTimer = new Timer();

        public Journalist(string passportId, string nameJournalist, JournalistType typeJournalist)
        {
            this.NameJournalist = nameJournalist;
            TypeJournalist = typeJournalist;
            PassportId = passportId;

            creationTimer.AutoReset = true;
            Random workingTime = new Random();
            creationTimer.Interval = workingTime.Next(5000, 10000);
            creationTimer.Elapsed += CreationTimer_Elapsed;
        }

        private void CreationTimer_Elapsed(object sender, ElapsedEventArgs e) => CreateInformationProduct();

        public void Employment()
        {
            Task.Run
               (
                   () => { StartWorking(); }
               );
        }

        public void Dismissal() => creationTimer.Stop();
        private void StartWorking() => creationTimer.Start();
        private void CreateInformationProduct()
        {
            if (TypeJournalist == JournalistType.Text)
            {
                SqlConnection sqlConnection = new SqlConnection(@$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName}\Data\ModulationData.mdf;Integrated Security=True");
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand(@"select min([id]) from [Text] where [take] = 'false'", sqlConnection);

                try { _ = (int)command.ExecuteScalar(); }
                catch { return; }

                int idNews = (int)command.ExecuteScalar();
                command = new SqlCommand($@"update [Text] set [take] = 1 where [id] = {idNews}", sqlConnection);
                command.ExecuteNonQuery();

                command = new SqlCommand($@"select [link] from [Text] where [id] = {idNews}", sqlConnection);
                string linkNews = (string)command.ExecuteScalar();

                command = new SqlCommand($@"select [title] from [Text] where [id] = {idNews}", sqlConnection);
                string titleNews = (string)command.ExecuteScalar();


                WebClient openNews = new WebClient();
                Stream stream = openNews.OpenRead(linkNews);
                StreamReader streamReader = new StreamReader(stream);
                string htmlDocument = streamReader.ReadToEnd();
                streamReader.Close();

                List<string> ListInformationProduct = new List<string>();

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

                string category = (string)command.ExecuteScalar();

                InformationProduct informationProduct = new InformationProduct(titleNews, string.Concat(ListInformationProduct.Select(e => e + "\n")), linkNews, category, InformationProductType.Text);

                sqlConnection.Close();

                InformationProductCreated?.Invoke(this, new ProductCreatedEventArgs(informationProduct));
            }
            else if (TypeJournalist == JournalistType.Video)
            {
                SqlConnection sqlConnection = new SqlConnection(@$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName}\Data\ModulationData.mdf;Integrated Security=True");
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand(@"select min([id]) from [Video] where [take] = 'false'", sqlConnection);

                try { _ = (int)command.ExecuteScalar(); }
                catch { return; }
                int idNews = (int)command.ExecuteScalar();

                command = new SqlCommand($@"select [link] from [Video] where [id] = {idNews}", sqlConnection);
                string linkNews = (string)command.ExecuteScalar();

                string contentNews = (string)command.ExecuteScalar();

                command = new SqlCommand($@"select [title] from [Video] where [id] = {idNews}", sqlConnection);
                string titleNews = (string)command.ExecuteScalar();

                command = new SqlCommand($@"select [category] from [Video] where [id] = {idNews}", sqlConnection);
                string category = (string)command.ExecuteScalar();


                InformationProduct informationProduct = new InformationProduct(titleNews, contentNews, linkNews, category, InformationProductType.Video);
                command = new SqlCommand($@"update [Video] set [take] = 1 where [id] = {idNews}", sqlConnection);
                command.ExecuteNonQuery();
                sqlConnection.Close();

                InformationProductCreated?.Invoke(this, new ProductCreatedEventArgs(informationProduct));
            }
        }
    }
   
}
