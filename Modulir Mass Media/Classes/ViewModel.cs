using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using CodeHollow.FeedReader;
using Modulir_Mass_Media.Helpers;
using Xceed.Wpf.Toolkit;

namespace Modulir_Mass_Media.Classes
{
    public static class ViewModel
    {
        public static ObservableCollection<RssParser> RssParser { get; set; }
        public static ObservableCollection<MassMedia> Medias { get; set; }
        public static ObservableCollection<Journalist> JournalistsNotBusy { get; set; }
        public static ObservableCollection<Journalist> JournalistsWorking { get; set; }
        public static ObservableCollection<MassMediaInformationProduct> MediaProduct { get; set; }
        public static ObservableCollection<MassMediaInformationProduct> MediaProductBySubscription { get; set; }
        public static ObservableCollection<MassMedia> SubscriptionMassMedia { get; set; }

        static ViewModel()
        {
            Medias = new ObservableCollection<MassMedia>();
            JournalistsNotBusy = new ObservableCollection<Journalist>();
            JournalistsWorking = new ObservableCollection<Journalist>();
            RssParser = new ObservableCollection<RssParser>();
            MediaProduct = new ObservableCollection<MassMediaInformationProduct>();
            MediaProductBySubscription = new ObservableCollection<MassMediaInformationProduct>();
            SubscriptionMassMedia = new ObservableCollection<MassMedia>();
        }

        #region Поля
        static Client client;

        private static string connectionString = @$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName}\Data\ModulationData.mdf;Integrated Security=True";
        private static ICommand registrationCommand;
        private static ICommand authorizationCommand;
        private static ICommand loginWthoutRegistration;
                 
        private static ICommand openConfigurationCommand;
        private static ICommand initializationMedia;
        private static ICommand initializationJournalist;
        private static ICommand initializationRSS;
        private static ICommand addMediaCommand;
        private static ICommand addJournalistCommand;
        private static ICommand addRSSCommand;
        private static ICommand recruitmentCommand;
        private static ICommand mediaSelectionChanged;
        private static ICommand removeNotBussyJornalistCommand;
        private static ICommand removeSMICommand;
        private static ICommand removeWorkingJournalistCommand;
        private static ICommand removeRSSCommand;
                 
        private static ICommand subscribedToMediaCommand;
        private static ICommand readMoreCommand;
        private static ICommand unbscribeCommand;
        private static ICommand pageClosingCommand;
                 
        private static object locker = new object();

        public static event Action<bool, string> AuthorizationAccepted;
        public static event Action<bool, string> RegistrationAccepted;
        public static event Action<bool, string, ElementChanged> AddMediaAccepted;
        public static event Action<bool, string, ElementChanged> AddJournalistAccepted;
        public static event Action<bool, string, ElementChanged> AddRSSAccepted;
        public static event Action<bool, string, ElementChanged> RecruitmentAccepted;
        public static event Action<bool, string, ElementChanged> MediaSelectionAccepted;
        public static event Action<bool, string, ElementChanged> RemoveNotBussyAccepted;
        public static event Action<bool, string, ElementChanged> RemoveSMIAccepted;
        public static event Action<bool, string, ElementChanged> RemoveWorkingJournalistAccepted;
        public static event Action<bool, string, ElementChanged> RemoveRSSAccepted;

        public static event Action<MassMedia> SubscribedToMedia;
        public static event Action<MassMedia> UnscribedToMedia;
        public static event Action<int, string> EditLikeCommand;
        public static event Action<int, string> EditHaHaCommand;
        public static event Action<int, string> EditWowCommand;
        public static event Action<int, string> EditSadCommand;
        public static event Action<int, string> EditAngryCommand;
        public static event Action<int, string> EditDisLikeCommand;
        public static event Action<MassMedia, Journalist> JournalistHired;
        public static event Action<RssParser> RssStart;

        #endregion
        private static void InitializationMediaJournalistWorker()
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand(@"select * from Journalist where [NameSMI] is not null", sqlConnection);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string nameMedia = reader.GetString(3);
                        string passportId = reader.GetString(0);
                        Journalist journalist = new Journalist(reader.GetString(0), reader.GetString(1), (JournalistType)Enum.Parse(typeof(JournalistType), reader.GetString(2)));
                        JournalistsWorking.Add(journalist);

                        MassMedia media = Medias.FirstOrDefault(e => e.NameMedia == nameMedia);
                        if (media != null) JournalistHired?.Invoke(media, journalist);
                    }
                }
                reader.Close();
                sqlConnection.Close();

            }
            catch { }
        }
        public static ICommand SubscribedToMediaCommand => subscribedToMediaCommand ??= new RelayCommand(obj =>
        {
            if (!(obj is MassMediaInformationProduct massMediaInformationProduct)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
                SqlCommand command;

                MassMedia massMedia = Medias.FirstOrDefault(e => e.NameMedia == massMediaInformationProduct.NameMassMedia);
                if (massMedia != null) SubscribedToMedia?.Invoke(massMedia);
                if (!SubscriptionMassMedia.Contains(massMedia))
                {
                    SubscriptionMassMedia.Add(massMedia);
                    command = new SqlCommand(@$"insert into MediaSubscription([NameSMI], [Login]) values(N'{massMediaInformationProduct.NameMassMedia}', N'{client.Login}')", sqlConnection);
                    command.ExecuteNonQuery();
                }
                sqlConnection.Close();
            }
            catch { }
        }
        );
        public static ICommand UnbscribedToMediaCommand => unbscribeCommand ??= new RelayCommand(obj =>
        {
            var objOne = obj as Tuple<object>;
            if (!(objOne.Item1 is MassMedia mediaInformationProduct)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand(@$"delete from MediaSubscription where [NameSMI] = N'{mediaInformationProduct.NameMedia}' and [Login] = N'{client.Login}'", sqlConnection);
                command.ExecuteNonQuery();


                UnscribedToMedia?.Invoke(mediaInformationProduct);

                sqlConnection.Close();
            }
            catch { }
        }
       );
        public static Client Client
        {
            get { return client; }
        }
        public static ICommand InitializationRSS => initializationRSS ??= new RelayCommand(obj =>
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand(@"select [LinkRSS], [TypeRSS] from RSS", sqlConnection);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RssParser.Add(new RssParser(reader.GetString(0), (TypeRssInfo)Enum.Parse(typeof(TypeRssInfo), reader.GetString(1))));
                    }
                }
                reader.Close();
                sqlConnection.Close();
            }
            catch { }
        }
        );
        public static ICommand InitializationJournalist => initializationJournalist ??= new RelayCommand(obj =>
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand command;
            command = new SqlCommand(@"select PassportId, NameJournalist, [Type] from Journalist where [NameSMI] is null", sqlConnection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    JournalistsNotBusy.Add(reader.GetString(2) == "Text" ? new Journalist(reader.GetString(0), reader.GetString(1), JournalistType.Text) : new Journalist(reader.GetString(0), reader.GetString(1), JournalistType.Video));
                }
            }
            sqlConnection.Close();
        }
        );
        public static ICommand InitializationMedia => initializationMedia ??= new RelayCommand(obj =>
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand command;
            command = new SqlCommand("select NameSMI from SMI", sqlConnection);

            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Medias.Add(new MassMedia(reader.GetString(0)));
                }
            }
            reader.Close();
            sqlConnection.Close();
        }
        );
        public static ICommand AddMediaCommand => addMediaCommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo) = obj as Tuple<object, object>;
            if (!(objOne is string nameMedia) || !(objTwo is null)) return;
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) AuthorizationAccepted?.Invoke(false, "Отсутствует подключение к базе данных");
                else
                {
                    SqlCommand command;

                    command = new SqlCommand($"select count([NameSMI]) from SMI where [NameSMI] = N'{nameMedia}'", sqlConnection);
                    int countСoincidences = (int)command.ExecuteScalar();
                    if (countСoincidences == 1)
                    {
                        sqlConnection.Close();
                        AddMediaAccepted.Invoke(false, "Такое наименование СМИ уже существует", ElementChanged.AddMedia);
                        return;
                    }
                    else
                    {
                        command = new SqlCommand($"Insert Into SMI([NameSMI]) values(N'{nameMedia}')", sqlConnection);
                        command.ExecuteNonQuery();
                        sqlConnection.Close();
                        AddMediaAccepted.Invoke(true, "СМИ успешно добавлено", ElementChanged.AddMedia);
                        Medias.Add(new MassMedia(nameMedia));
                    }
                }
            }
            catch { AddMediaAccepted?.Invoke(false, "Ошибка работы с базой данных", ElementChanged.ExeptionWorkDataBase); }
        }
        );
        public static ICommand AddJournalistCommand => addJournalistCommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo, objThree) = obj as Tuple<object, object, object>;
            if (!(objOne is string nameJournalist) || !(objTwo is string passportId) || !(objThree is string typeJournalist)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) AddJournalistAccepted?.Invoke(false, "Отсутствует подключение к базе данных", ElementChanged.ExeptionWorkDataBase);
                else
                {
                    SqlCommand command;

                    command = new SqlCommand($"select count([PassportId]) from Journalist where [PassportId] = N'{passportId}'", sqlConnection);
                    int countСoincidences = (int)command.ExecuteScalar();
                    if (countСoincidences == 1)
                    {
                        sqlConnection.Close();
                        AddJournalistAccepted.Invoke(false, "Журналист с таким паспортом уже существует", ElementChanged.AddJournalist);
                        return;
                    }
                    else
                    {
                        string type = typeJournalist == "Текст" ? "Text" : "Video";
                        command = new SqlCommand($"Insert Into Journalist(PassportId, [NameJournalist], [Type]) values(N'{passportId}', N'{nameJournalist}', N'{type}')", sqlConnection);
                        command.ExecuteNonQuery();
                        sqlConnection.Close();
                        AddJournalistAccepted.Invoke(true, "Журналист успешно добавлен", ElementChanged.AddJournalist);
                        JournalistsNotBusy.Add(typeJournalist == "Текст" ? new Journalist(passportId, nameJournalist, JournalistType.Text) : new Journalist(passportId, nameJournalist, JournalistType.Video));
                    }
                }
            }
            catch { AddJournalistAccepted?.Invoke(false, "Ошибка работы с базой данных", ElementChanged.ExeptionWorkDataBase); }
        }
        );
        public static ICommand AddRssCommand => addRSSCommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo) = obj as Tuple<object, object>;
            if (!(objOne is string nameRSS) || !(objTwo is TypeRssInfo typeRSS)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) AddRSSAccepted?.Invoke(false, "Отсутствует подключение к базе данных", ElementChanged.ExeptionWorkDataBase);
                else
                {
                    try
                    {
                        FeedReader.Read(nameRSS);

                        SqlCommand command;

                        command = new SqlCommand($"select count([LinkRSS]) from RSS where [LinkRSS] = N'{nameRSS}'", sqlConnection);

                        int countСoincidences = (int)command.ExecuteScalar();
                        if (countСoincidences == 1)
                        {
                            sqlConnection.Close();
                            AddRSSAccepted.Invoke(false, "Введенная ссылка на RSS уже существует", ElementChanged.AddRSS);
                            return;
                        }
                        else
                        {
                            command = new SqlCommand($"Insert Into RSS([LinkRSS], [TypeRSS]) values(N'{nameRSS}', N'{typeRSS}')", sqlConnection);
                            command.ExecuteNonQuery();
                            sqlConnection.Close();
                            AddRSSAccepted.Invoke(true, "Ссылка на RSS успешно добавлена", ElementChanged.AddRSS);
                            RssParser.Add(new RssParser(nameRSS, typeRSS));
                        }
                    }
                    catch
                    {
                        AddRSSAccepted.Invoke(false, "Ссылка не распознана", ElementChanged.AddRSS);
                        return;
                    }
                }
            }
            catch { AddRSSAccepted.Invoke(false, "Ошибка работы с базой данных", ElementChanged.ExeptionWorkDataBase); }

        }
        );
        public static ICommand MediaSelectionChanged => mediaSelectionChanged ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo) = obj as Tuple<object, object>;
            if (!(objOne is string nameMedia) || !(objTwo is null)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) MediaSelectionAccepted?.Invoke(false, "Отсутствует подключение к базе данных", ElementChanged.ExeptionWorkDataBase);
                else
                {
                    JournalistsWorking.Clear();

                    SqlCommand command;

                    try
                    {
                        command = new SqlCommand($"select * from Journalist where [NameSMI] = N'{nameMedia}'", sqlConnection);

                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if (JournalistsWorking.Any(i => i.PassportId == reader.GetString(0))) { sqlConnection.Close(); return; }
                                JournalistsWorking.Add(reader.GetString(2) == "Text" ? new Journalist(reader.GetString(0), reader.GetString(1), JournalistType.Text) : new Journalist(reader.GetString(0), reader.GetString(1), JournalistType.Video));
                            }
                        }
                        sqlConnection.Close();
                        MediaSelectionAccepted.Invoke(true, "", ElementChanged.MediaSelectionChanged);
                    }
                    catch { sqlConnection.Close(); }
                }
            }
            catch { MediaSelectionAccepted.Invoke(false, "Ошибка работы с базой данных", ElementChanged.ExeptionWorkDataBase); }
        }
        );
        public static ICommand RecruitmentCommand => recruitmentCommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo, objThree, objFour, objFive) = obj as Tuple<object, object, object, object, object>;
            if (!(objOne is string nameMedia) || !(objTwo is string passportJournalist) || !(objThree is string nameJournalist) || !(objFour is JournalistType typeJournalist) || !(objFive is Journalist journalist)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) RecruitmentAccepted?.Invoke(false, "Отсутствует подключение к базе данных", ElementChanged.ExeptionWorkDataBase);
                else
                {
                    SqlCommand command;

                    command = new SqlCommand($@"Update Journalist set [NameSMI] = N'{nameMedia}' where [PassportId] = N'{passportJournalist}'", sqlConnection);
                    command.ExecuteNonQuery();
                    sqlConnection.Close();
                    JournalistsWorking.Add(typeJournalist == JournalistType.Text ? new Journalist(passportJournalist, nameJournalist, typeJournalist) : new Journalist(passportJournalist, nameJournalist, typeJournalist));
                    JournalistsNotBusy.Remove(journalist);
                }
            }
            catch { RecruitmentAccepted.Invoke(false, "Ошибка работы с базой данных", ElementChanged.ExeptionWorkDataBase); }
        }
        );
        public static ICommand RemoveRssCommand => removeRSSCommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo) = obj as Tuple<object, object>;
            if (!(objOne is string linkRss) || !(objTwo is RssParser rssParser)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) RemoveRSSAccepted?.Invoke(false, "Отсутствует подключение к базе данных", ElementChanged.ExeptionWorkDataBase);
                else
                {
                    SqlCommand command;
                    command = new SqlCommand($"delete from RSS where [LinkRSS] = N'{linkRss}'", sqlConnection);
                    command.ExecuteNonQuery();
                    sqlConnection.Close();

                    RssParser.Remove(rssParser);

                    RemoveRSSAccepted?.Invoke(true, "RSS ссылка удалена", ElementChanged.RemoveRSS);
                }
            }
            catch { RemoveRSSAccepted.Invoke(false, "Ошибка работы с базой данных", ElementChanged.ExeptionWorkDataBase); }
        }
        );
        public static ICommand RemoveMediaCommand => removeSMICommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo) = obj as Tuple<object, object>;
            if (!(objOne is string nameMedia) || !(objTwo is MassMedia massMedia)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) RemoveSMIAccepted?.Invoke(false, "Отсутствует подключение к базе данных", ElementChanged.ExeptionWorkDataBase);
                else
                {
                    SqlCommand command;
                    command = new SqlCommand($"update Journalist set [NameSMI] = NULL where [NameSMI] = N'{nameMedia}'", sqlConnection);
                    command.ExecuteNonQuery();

                    command = new SqlCommand($"delete [MediaSubscription] where [NameSMI] = N'{nameMedia}'", sqlConnection);
                    command.ExecuteNonQuery();

                    command = new SqlCommand($"delete from SMI where [NameSMI] = N'{nameMedia}'", sqlConnection);
                    command.ExecuteNonQuery();
                    sqlConnection.Close();

                    Medias.Remove(massMedia);
                    JournalistsWorking.Clear();
                    JournalistsNotBusy.Clear();
                    ViewModel.InitializationJournalist.Execute(null);

                    RemoveSMIAccepted.Invoke(true, "СМИ удалено", ElementChanged.RemoveSMI);
                }
            }
            catch(Exception e) { RemoveSMIAccepted.Invoke(false, "Ошибка работы с базой данных", ElementChanged.ExeptionWorkDataBase); }
        }
        );
        public static ICommand RemoveWorkingJournalistCommand => removeWorkingJournalistCommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo, objThree) = obj as Tuple<object, object, object>;
            if (!(objOne is string nameMedia) || !(objTwo is string passportId) || !(objThree is Journalist journalist)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) RemoveWorkingJournalistAccepted?.Invoke(false, "Отсутствует подключение к базе данных", ElementChanged.ExeptionWorkDataBase);
                else
                {
                    SqlCommand command;

                    command = new SqlCommand($"update Journalist set [NameSMI] = NULL where [PassportId] = N'{passportId}' and [NameSMI] = N'{nameMedia}'", sqlConnection);
                    command.ExecuteNonQuery();
                    sqlConnection.Close();

                    JournalistsWorking.Remove(journalist);
                    JournalistsNotBusy.Clear();
                    ViewModel.InitializationJournalist.Execute(null);

                    RemoveWorkingJournalistAccepted.Invoke(true, "Журналист удален", ElementChanged.RemoveJournalistWorking);
                }
            }
            catch { RemoveWorkingJournalistAccepted.Invoke(false, "Ошибка работы с базой данных", ElementChanged.ExeptionWorkDataBase); }
        }
        );
        public static ICommand RemoveNotBussyJornalistCommand => removeNotBussyJornalistCommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo) = obj as Tuple<object, object>;
            if (!(objOne is string passportId) || !(objTwo is Journalist journalist)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) RecruitmentAccepted?.Invoke(false, "Отсутствует подключение к базе данных", ElementChanged.ExeptionWorkDataBase);
                else
                {
                    SqlCommand command;

                    command = new SqlCommand($"delete from Journalist where PassportId = N'{passportId}'", sqlConnection);
                    command.ExecuteNonQuery();
                    sqlConnection.Close();

                    JournalistsNotBusy.Remove(journalist);
                    RemoveNotBussyAccepted.Invoke(true, "Журналист удален", ElementChanged.RemoveJournalistNotBusy);
                }
            }
            catch { RemoveNotBussyAccepted.Invoke(false, "Ошибка работы с базой данных", ElementChanged.ExeptionWorkDataBase); }
        }
        );
        public static ICommand OpenConfigurationCommand => openConfigurationCommand ??= new RelayCommand(obj =>
        {
            Medias.Clear();
            JournalistsNotBusy.Clear();
            JournalistsWorking.Clear();
            RssParser.Clear();
            Configuration configuration = new Configuration();

            configuration.ShowDialog();
        });
        public static ICommand LoginWthoutRegistration => loginWthoutRegistration ??= new RelayCommand(obj =>
        {
            client = null;
            RssParser.Clear();
            Medias.Clear();
            JournalistsWorking.Clear();
            MediaProductBySubscription.Clear();
            SubscriptionMassMedia.Clear();

            InitializationMedia.Execute(null);
            InitializationRSS.Execute(null);

            foreach (var item in RssParser) RssStart?.Invoke(item);

            InitializationMediaJournalistWorker();

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) AuthorizationAccepted?.Invoke(false, "Отсутствует подключение к базе данных");
                else
                {
                    SqlCommand command;

                    command = new SqlCommand(@"select * from Journalist where [NameSMI] is not null", sqlConnection);
                    SqlDataReader reader;
                    reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string nameMedia = reader.GetString(3);
                            string passportId = reader.GetString(0);
                            Journalist journalist = new Journalist(reader.GetString(0), reader.GetString(1), (JournalistType)Enum.Parse(typeof(JournalistType), reader.GetString(2)));

                            MassMedia media = Medias.FirstOrDefault(e => e.NameMedia == nameMedia);
                            media.ProductRelese += MediaProductRelese;
                            if (media != null) JournalistHired?.Invoke(media, journalist);

                        }
                    }
                    reader.Close();
                    sqlConnection.Close();
                }
            }
            catch { }

            MainWindow window = new MainWindow();
            window.ShowDialog();

        }
        );
        public static ICommand AuthorizationCommand => authorizationCommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo) = obj as Tuple<object, object>;
            if (!(objOne is string login) || !(objTwo is string password)) return;

            RssParser.Clear();
            Medias.Clear();
            JournalistsWorking.Clear();
            SubscriptionMassMedia.Clear();

            InitializationMedia.Execute(null);
            InitializationRSS.Execute(null);

            foreach (var item in RssParser) RssStart?.Invoke(item);

            InitializationMediaJournalistWorker();

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) AuthorizationAccepted?.Invoke(false, "Отсутствует подключение к базе данных");
                else
                {
                    SqlCommand command;
                    command = new SqlCommand($"select count([Login]) from Client where [Login] = N'{login}'", sqlConnection);

                    int countСoincidences = (int)command.ExecuteScalar();
                    if (countСoincidences == 0)
                    {
                        sqlConnection.Close();
                        AuthorizationAccepted?.Invoke(false, "Пользователь не найден");
                        return;
                    }
                    else
                    {
                        command = new SqlCommand($"select [Password] from Client where [Login] = N'{login}'", sqlConnection);
                        string passwordTrue = (string)command.ExecuteScalar();
                        if (BCrypt.Net.BCrypt.Verify(password, passwordTrue))
                        {
                            AuthorizationAccepted?.Invoke(true, "Вход выполнен");

                            command = new SqlCommand(@"select * from Journalist where [NameSMI] is not null", sqlConnection);
                            SqlDataReader reader;
                            reader = command.ExecuteReader();

                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    string nameMedia = reader.GetString(3);
                                    string passportId = reader.GetString(0);
                                    Journalist journalist = new Journalist(reader.GetString(0), reader.GetString(1), (JournalistType)Enum.Parse(typeof(JournalistType), reader.GetString(2)));

                                    MassMedia media = Medias.FirstOrDefault(e => e.NameMedia == nameMedia);
                                    media.ProductRelese += MediaProductRelese;
                                    if (media != null) JournalistHired?.Invoke(media, journalist);
                                }
                            }
                            reader.Close();

                            client = new Client(login);
                            client.NewsReceivedBySubscription += ClientNewsReceivedBySubscription;
                            client.MediaUnSubscripted += MediaUnSubscripted;

                            command = new SqlCommand($"select [NameSMI] from MediaSubscription where [Login] = N'{client.Login}'", sqlConnection);
                            reader = command.ExecuteReader();

                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    string nameMedia = reader.GetString(0);
                                    MassMedia media = Medias.FirstOrDefault(e => e.NameMedia == nameMedia);
                                    SubscriptionMassMedia.Add(media);
                                    SubscribedToMedia?.Invoke(media);
                                }
                            }

                            sqlConnection.Close();

                            MainWindow mainWindow = new MainWindow();
                            mainWindow.ShowDialog();
                        }
                        else
                        {
                            AuthorizationAccepted?.Invoke(false, "Неверный логин или пароль");
                            sqlConnection.Close();
                            return;
                        }
                    }
                }
            }
            catch (Exception e) { AuthorizationAccepted?.Invoke(false, "Ошибка работы с базой данных"); }
        }
        );

        private static void MediaProductRelese(MassMediaInformationProduct e)
        {
            lock (locker)
            {
                if (!MediaProduct.Contains(e))
                {
                    App.Current.Dispatcher.Invoke(() => MediaProduct.Insert(0, e));
                }
            }
        }
        private static void MediaUnSubscripted(MassMedia e)
        {
            SubscriptionMassMedia.Remove(e);
        }
        public static ICommand ReadMoreCommand => readMoreCommand ??= new RelayCommand(obj =>
       {
           var objOne = obj as Tuple<object>;
           if (!(objOne.Item1 is MassMediaInformationProduct mediaInformationProduct)) return;

           Page page = new Page(MediaProduct.FirstOrDefault(e => e.InformationProduct.LinkProduct == mediaInformationProduct.InformationProduct.LinkProduct));
           page.ShowDialog();
       }
        );
        private static void ClientNewsReceivedBySubscription(MassMediaInformationProduct e)
        {
            lock (locker)
            {
                if (!MediaProductBySubscription.Contains(e))
                {
                    App.Current.Dispatcher.Invoke(() => MediaProductBySubscription.Insert(0, e));
                }
            }
        }
        public static ICommand RegistrationCommand => registrationCommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo) = obj as Tuple<object, object>;
            if (!(objOne is string login) || !(objTwo is string password)) return;
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) AuthorizationAccepted?.Invoke(false, "Отсутствует подключение к базе данных");
                else
                {
                    SqlCommand command;
                    command = new SqlCommand($"select count([Login]) from Client where [Login] = N'{login}'", sqlConnection);

                    int countСoincidences = (int)command.ExecuteScalar();
                    if (countСoincidences == 1)
                    {
                        RegistrationAccepted.Invoke(false, "Такой логин уже существует");
                        sqlConnection.Close();
                        return;
                    }
                    else
                    {
                        command = new SqlCommand($"Insert Into Client([Login], [Password]) values(N'{login}', N'{BCrypt.Net.BCrypt.HashPassword(password)}')", sqlConnection);
                        command.ExecuteNonQuery();
                        RegistrationAccepted.Invoke(true, "Клиент успешно зарегистрирован");
                        sqlConnection.Close();
                    }
                }
            }
            catch { RegistrationAccepted?.Invoke(false, "Ошибка работы с базой данных"); }
        }
        );
        public static ICommand PageClosingCommand => pageClosingCommand ??= new RelayCommand(obj =>
        {
            if (!(obj is EmotionCommandParametr parameters)) return;

            EditLikeCommand?.Invoke(parameters.Like, parameters.Link);
            EditHaHaCommand?.Invoke(parameters.HaHa, parameters.Link);
            EditWowCommand?.Invoke(parameters.Wow, parameters.Link);
            EditSadCommand?.Invoke(parameters.Sad, parameters.Link);
            EditAngryCommand?.Invoke(parameters.Angry, parameters.Link);
            EditDisLikeCommand?.Invoke(parameters.DisLike, parameters.Link);
        }
        );
    }
}
