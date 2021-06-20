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

namespace Modulir_Mass_Media.Classes
{
    public class ViewModel
    {
        public ViewModel()
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
        Client client;

        private string connectionString = @$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName}\Data\ModulationData.mdf;Integrated Security=True";
        private ICommand registrationCommand;
        private ICommand authorizationCommand;
        private ICommand loginWthoutRegistration;

        private ICommand openConfigurationCommand;
        private ICommand initializationMedia;
        private ICommand initializationJournalist;
        private ICommand initializationRSS;
        private ICommand addMediaCommand;
        private ICommand addJournalistCommand;
        private ICommand addRSSCommand;
        private ICommand recruitmentCommand;
        private ICommand mediaSelectionChanged;
        private ICommand removeNotBussyJornalistCommand;
        private ICommand removeSMICommand;
        private ICommand removeWorkingJournalistCommand;
        private ICommand removeRSSCommand;

        private ICommand subscribedToMediaCommand;
        private ICommand readMoreCommand;
        private ICommand subscribeCommand;
        private ICommand unbscribeCommand;

        private ICommand pageClosingCommand;

        private object locker = new object();

        public ObservableCollection<RssParser> RssParser { get; set; }
        public ObservableCollection<MassMedia> Medias { get; set; }
        public ObservableCollection<Journalist> JournalistsNotBusy { get; set; }
        public ObservableCollection<Journalist> JournalistsWorking { get; set; }
        public ObservableCollection<MassMediaInformationProduct> MediaProduct { get; set; }
        public ObservableCollection<MassMediaInformationProduct> MediaProductBySubscription { get; set; }
        public ObservableCollection<MassMedia> SubscriptionMassMedia { get; set; }
        #endregion

        #region События 
        public event Action<object, bool, string> AuthorizationAccepted;
        public event Action<object, bool, string> RegistrationAccepted;
        public event Action<object, bool, string, ElementChanged> AddMediaAccepted;
        public event Action<object, bool, string, ElementChanged> AddJournalistAccepted;
        public event Action<object, bool, string, ElementChanged> AddRSSAccepted;
        public event Action<object, bool, string, ElementChanged> RecruitmentAccepted;
        public event Action<object, bool, string, ElementChanged> MediaSelectionAccepted;
        public event Action<object, bool, string, ElementChanged> RemoveNotBussyAccepted;
        public event Action<object, bool, string, ElementChanged> RemoveSMIAccepted;
        public event Action<object, bool, string, ElementChanged> RemoveWorkingJournalistAccepted;
        public event Action<object, bool, string, ElementChanged> RemoveRSSAccepted;

        public event Action<object, MassMedia> SubscribedToMedia;
        public event Action<object, MassMedia> UnscribedToMedia;

        #endregion
        private void InitializationMediaJournalistWorker()
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
                        if (media != null) media.HiringEmployee(journalist);
                    }
                }
                reader.Close();
                sqlConnection.Close();

            }
            catch { }
        }
        public ICommand SubscribedToMediaCommand => subscribedToMediaCommand ??= new RelayCommand(obj =>
        {
            if (!(obj is MassMediaInformationProduct massMediaInformationProduct)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
                SqlCommand command;

                MassMedia massMedia = Medias.FirstOrDefault(e => e.NameMedia == massMediaInformationProduct.NameMassMedia);
                if (massMedia != null) SubscribedToMedia?.Invoke(this, massMedia);
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
        public ICommand UnbscribedToMediaCommand => unbscribeCommand ??= new RelayCommand(obj =>
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


                UnscribedToMedia?.Invoke(this, mediaInformationProduct);

                sqlConnection.Close();
            }
            catch { }
        }
       );
        public Client Client
        {
            get { return client; }
        }
        public ICommand InitializationRSS => initializationRSS ??= new RelayCommand(obj =>
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
        public ICommand InitializationJournalist => initializationJournalist ??= new RelayCommand(obj =>
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
        public ICommand InitializationMedia => initializationMedia ??= new RelayCommand(obj =>
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
        public ICommand AddMediaCommand => addMediaCommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo) = obj as Tuple<object, object>;
            if (!(objOne is string nameMedia) || !(objTwo is null)) return;
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) AuthorizationAccepted?.Invoke(this, false, "Отсутствует подключение к базе данных");
                else
                {
                    SqlCommand command;

                    command = new SqlCommand($"select count([NameSMI]) from SMI where [NameSMI] = N'{nameMedia}'", sqlConnection);
                    int countСoincidences = (int)command.ExecuteScalar();
                    if (countСoincidences == 1)
                    {
                        sqlConnection.Close();
                        AddMediaAccepted.Invoke(this, false, "Такое наименование СМИ уже существует", ElementChanged.AddMedia);
                        return;
                    }
                    else
                    {
                        command = new SqlCommand($"Insert Into SMI([NameSMI]) values(N'{nameMedia}')", sqlConnection);
                        command.ExecuteNonQuery();
                        sqlConnection.Close();
                        AddMediaAccepted.Invoke(this, true, "СМИ успешно добавлено", ElementChanged.AddMedia);
                        Medias.Add(new MassMedia(nameMedia));
                    }
                }
            }
            catch { AddMediaAccepted?.Invoke(this, false, "Ошибка работы с базой данных", ElementChanged.ExeptionWorkDataBase); }
        }
        );
        public ICommand AddJournalistCommand => addJournalistCommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo, objThree) = obj as Tuple<object, object, object>;
            if (!(objOne is string nameJournalist) || !(objTwo is string passportId) || !(objThree is string typeJournalist)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) AddJournalistAccepted?.Invoke(this, false, "Отсутствует подключение к базе данных", ElementChanged.ExeptionWorkDataBase);
                else
                {
                    SqlCommand command;

                    command = new SqlCommand($"select count([PassportId]) from Journalist where [PassportId] = N'{passportId}'", sqlConnection);
                    int countСoincidences = (int)command.ExecuteScalar();
                    if (countСoincidences == 1)
                    {
                        sqlConnection.Close();
                        AddJournalistAccepted.Invoke(this, false, "Журналист с таким паспортом уже существует", ElementChanged.AddJournalist);
                        return;
                    }
                    else
                    {
                        string type = typeJournalist == "Текст" ? "Text" : "Video";
                        command = new SqlCommand($"Insert Into Journalist(PassportId, [NameJournalist], [Type]) values(N'{passportId}', N'{nameJournalist}', N'{type}')", sqlConnection);
                        command.ExecuteNonQuery();
                        sqlConnection.Close();
                        AddJournalistAccepted.Invoke(this, true, "Журналист успешно добавлен", ElementChanged.AddJournalist);
                        JournalistsNotBusy.Add(typeJournalist == "Текст" ? new Journalist(passportId, nameJournalist, JournalistType.Text) : new Journalist(passportId, nameJournalist, JournalistType.Video));
                    }
                }
            }
            catch { AddJournalistAccepted?.Invoke(this, false, "Ошибка работы с базой данных", ElementChanged.ExeptionWorkDataBase); }
        }
        );
        public ICommand AddRSSCommand => addRSSCommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo) = obj as Tuple<object, object>;
            if (!(objOne is string nameRSS) || !(objTwo is TypeRssInfo typeRSS)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) AddRSSAccepted?.Invoke(this, false, "Отсутствует подключение к базе данных", ElementChanged.ExeptionWorkDataBase);
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
                            AddRSSAccepted.Invoke(this, false, "Введенная ссылка на RSS уже существует", ElementChanged.AddRSS);
                            return;
                        }
                        else
                        {
                            command = new SqlCommand($"Insert Into RSS([LinkRSS], [TypeRSS]) values(N'{nameRSS}', N'{typeRSS}')", sqlConnection);
                            command.ExecuteNonQuery();
                            sqlConnection.Close();
                            AddRSSAccepted.Invoke(this, true, "Ссылка на RSS успешно добавлена", ElementChanged.AddRSS);
                            RssParser.Add(new RssParser(nameRSS, typeRSS));
                        }
                    }
                    catch
                    {
                        AddRSSAccepted.Invoke(this, false, "Ссылка не распознана", ElementChanged.AddRSS);
                        return;
                    }
                }
            }
            catch { AddRSSAccepted.Invoke(this, false, "Ошибка работы с базой данных", ElementChanged.ExeptionWorkDataBase); }

        }
        );
        public ICommand MediaSelectionChanged => mediaSelectionChanged ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo) = obj as Tuple<object, object>;
            if (!(objOne is string nameMedia) || !(objTwo is null)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) MediaSelectionAccepted?.Invoke(this, false, "Отсутствует подключение к базе данных", ElementChanged.ExeptionWorkDataBase);
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
                        MediaSelectionAccepted.Invoke(this, true, "", ElementChanged.MediaSelectionChanged);
                    }
                    catch { sqlConnection.Close(); }
                }
            }
            catch { MediaSelectionAccepted.Invoke(this, false, "Ошибка работы с базой данных", ElementChanged.ExeptionWorkDataBase); }
        }
        );
        public ICommand RecruitmentCommand => recruitmentCommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo, objThree, objFour, objFive) = obj as Tuple<object, object, object, object, object>;
            if (!(objOne is string nameMedia) || !(objTwo is string passportJournalist) || !(objThree is string nameJournalist) || !(objFour is JournalistType typeJournalist) || !(objFive is Journalist journalist)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) RecruitmentAccepted?.Invoke(this, false, "Отсутствует подключение к базе данных", ElementChanged.ExeptionWorkDataBase);
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
            catch { RecruitmentAccepted.Invoke(this, false, "Ошибка работы с базой данных", ElementChanged.ExeptionWorkDataBase); }
        }
        );
        public ICommand RemoveRSSCommand => removeRSSCommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo) = obj as Tuple<object, object>;
            if (!(objOne is string linkRss) || !(objTwo is RssParser rssParser)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) RemoveRSSAccepted?.Invoke(this, false, "Отсутствует подключение к базе данных", ElementChanged.ExeptionWorkDataBase);
                else
                {
                    SqlCommand command;
                    command = new SqlCommand($"delete from RSS where [LinkRSS] = N'{linkRss}'", sqlConnection);
                    command.ExecuteNonQuery();
                    sqlConnection.Close();

                    RssParser.Remove(rssParser);

                    RemoveRSSAccepted?.Invoke(this, true, "RSS ссылка удалена", ElementChanged.RemoveRSS);
                }
            }
            catch { RemoveRSSAccepted.Invoke(this, false, "Ошибка работы с базой данных", ElementChanged.ExeptionWorkDataBase); }
        }
        );
        public ICommand RemoveSMICommand => removeSMICommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo) = obj as Tuple<object, object>;
            if (!(objOne is string nameMedia) || !(objTwo is MassMedia massMedia)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) RemoveSMIAccepted?.Invoke(this, false, "Отсутствует подключение к базе данных", ElementChanged.ExeptionWorkDataBase);
                else
                {
                    SqlCommand command;
                    command = new SqlCommand($"update Journalist set [NameSMI] = NULL where [NameSMI] = N'{nameMedia}'", sqlConnection);
                    command.ExecuteNonQuery();


                    command = new SqlCommand($"delete from SMI where [NameSMI] = N'{nameMedia}'", sqlConnection);
                    command.ExecuteNonQuery();
                    sqlConnection.Close();

                    Medias.Remove(massMedia);
                    JournalistsWorking.Clear();
                    JournalistsNotBusy.Clear();
                    this.InitializationJournalist.Execute(null);

                    RemoveSMIAccepted.Invoke(this, true, "СМИ удалено", ElementChanged.RemoveSMI);
                }
            }
            catch { RemoveSMIAccepted.Invoke(this, false, "Ошибка работы с базой данных", ElementChanged.ExeptionWorkDataBase); }
        }
        );
        public ICommand RemoveWorkingJournalistCommand => removeWorkingJournalistCommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo, objThree) = obj as Tuple<object, object, object>;
            if (!(objOne is string nameMedia) || !(objTwo is string passportId) || !(objThree is Journalist journalist)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) RemoveWorkingJournalistAccepted?.Invoke(this, false, "Отсутствует подключение к базе данных", ElementChanged.ExeptionWorkDataBase);
                else
                {
                    SqlCommand command;

                    command = new SqlCommand($"update Journalist set [NameSMI] = NULL where [PassportId] = N'{passportId}' and [NameSMI] = N'{nameMedia}'", sqlConnection);
                    command.ExecuteNonQuery();
                    sqlConnection.Close();

                    JournalistsWorking.Remove(journalist);
                    JournalistsNotBusy.Clear();
                    this.InitializationJournalist.Execute(null);

                    RemoveWorkingJournalistAccepted.Invoke(this, true, "Журналист удален", ElementChanged.RemoveJournalistWorking);
                }
            }
            catch { RemoveWorkingJournalistAccepted.Invoke(this, false, "Ошибка работы с базой данных", ElementChanged.ExeptionWorkDataBase); }
        }
        );
        public ICommand RemoveNotBussyJornalistCommand => removeNotBussyJornalistCommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo) = obj as Tuple<object, object>;
            if (!(objOne is string passportId) || !(objTwo is Journalist journalist)) return;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) RecruitmentAccepted?.Invoke(this, false, "Отсутствует подключение к базе данных", ElementChanged.ExeptionWorkDataBase);
                else
                {
                    SqlCommand command;

                    command = new SqlCommand($"delete from Journalist where PassportId = N'{passportId}'", sqlConnection);
                    command.ExecuteNonQuery();
                    sqlConnection.Close();

                    JournalistsNotBusy.Remove(journalist);
                    RemoveNotBussyAccepted.Invoke(this, true, "Журналист удален", ElementChanged.RemoveJournalistNotBusy);
                }
            }
            catch { RemoveNotBussyAccepted.Invoke(this, false, "Ошибка работы с базой данных", ElementChanged.ExeptionWorkDataBase); }
        }
        );
        public ICommand OpenConfigurationCommand => openConfigurationCommand ??= new RelayCommand(obj =>
        {
            Medias.Clear();
            JournalistsNotBusy.Clear();
            JournalistsWorking.Clear();
            RssParser.Clear();
            Configuration configuration = new Configuration(this);
            configuration.ShowDialog();
        });
        public ICommand LoginWthoutRegistration => loginWthoutRegistration ??= new RelayCommand(obj =>
        {
            RssParser.Clear();
            Medias.Clear();
            JournalistsWorking.Clear();
            MediaProductBySubscription.Clear();
            SubscriptionMassMedia.Clear();

            this.InitializationMedia.Execute(null);
            this.InitializationRSS.Execute(null);

            foreach (var item in RssParser) item.StartParsing();

            InitializationMediaJournalistWorker();

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) AuthorizationAccepted?.Invoke(this, false, "Отсутствует подключение к базе данных");
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
                            media.ProductRelese += Media_ProductRelese;
                            if (media != null) media.HiringEmployee(journalist);
                        }
                    }
                    reader.Close();
                    sqlConnection.Close();
                }
            }
            catch { }

            MainWindow window = new MainWindow(this);
            window.ShowDialog();
        }
        );
        public ICommand AuthorizationCommand => authorizationCommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo) = obj as Tuple<object, object>;
            if (!(objOne is string login) || !(objTwo is string password)) return;

            RssParser.Clear();
            Medias.Clear();
            JournalistsWorking.Clear();
            SubscriptionMassMedia.Clear();

            this.InitializationMedia.Execute(null);
            this.InitializationRSS.Execute(null);

            foreach (var item in RssParser) item.StartParsing();

            InitializationMediaJournalistWorker();


            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) AuthorizationAccepted?.Invoke(this, false, "Отсутствует подключение к базе данных");
                else
                {
                    SqlCommand command;
                    command = new SqlCommand($"select count([Login]) from Client where [Login] = N'{login}'", sqlConnection);

                    int countСoincidences = (int)command.ExecuteScalar();
                    if (countСoincidences == 0)
                    {
                        sqlConnection.Close();
                        AuthorizationAccepted?.Invoke(this, false, "Пользователь не найден");
                        return;
                    }
                    else
                    {
                        command = new SqlCommand($"select [Password] from Client where [Login] = N'{login}'", sqlConnection);
                        string passwordTrue = (string)command.ExecuteScalar();
                        if (BCrypt.Net.BCrypt.Verify(password, passwordTrue))
                        {
                            AuthorizationAccepted?.Invoke(this, true, "Вход выполнен");

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
                                    media.ProductRelese += Media_ProductRelese;
                                    if (media != null) media.HiringEmployee(journalist);
                                }
                            }
                            reader.Close();

                            client = new Client(login, this);
                            client.NewsReceivedBySubscription += Client_NewsReceivedBySubscription;
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
                                    SubscribedToMedia?.Invoke(this, media);
                                }
                            }

                            sqlConnection.Close();

                            MainWindow mainWindow = new MainWindow(this);
                            mainWindow.ShowDialog();
                        }
                        else
                        {
                            AuthorizationAccepted?.Invoke(this, false, "Неверный логин или пароль");
                            sqlConnection.Close();
                            return;
                        }
                    }
                }
            }
            catch (Exception e) { AuthorizationAccepted?.Invoke(this, false, "Ошибка работы с базой данных"); }
        }
        );
        private void MediaUnSubscripted(MassMedia e)
        {
            SubscriptionMassMedia.Remove(e);
        }
        public ICommand ReadMoreCommand => readMoreCommand ??= new RelayCommand(obj =>
       {
           var objOne = obj as Tuple<object>;
           if (!(objOne.Item1 is MassMediaInformationProduct mediaInformationProduct)) return;

           Page page = new Page(this, MediaProduct.FirstOrDefault(e => e.InformationProduct.LinkProduct == mediaInformationProduct.InformationProduct.LinkProduct));
           page.ShowDialog();
       }
        );
        private void Media_ProductRelese(object sender, MassMediaReleaseInformationProductEventArgs e)
        {
            lock (locker)
            {
                if (!MediaProduct.Contains(e.MassMediaInformationProduct))
                {
                    App.Current.Dispatcher.Invoke(() => MediaProduct.Insert(0, e.MassMediaInformationProduct));

                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    try
                    {
                        sqlConnection.Open();
                        if (sqlConnection.State != System.Data.ConnectionState.Open) AuthorizationAccepted?.Invoke(this, false, "Отсутствует подключение к базе данных");
                        else
                        {
                            SqlCommand command;
                            
                            switch(e.MassMediaInformationProduct.InformationProduct.ProductType)
                            {
                                case InformationProductType.Text:
                                    {
                                        command = new SqlCommand($@"update Text set [NameSMI] = N'{e.MassMediaInformationProduct.NameMassMedia}'  where [link] = N'{e.MassMediaInformationProduct.InformationProduct.LinkProduct}'", sqlConnection);
                                        command.ExecuteNonQuery();
                                        sqlConnection.Close();
                                        break;
                                    }
                                case InformationProductType.Video:
                                    {
                                        command = new SqlCommand($@"update Video set [NameSMI] = N'{e.MassMediaInformationProduct.NameMassMedia}'  where [link] = N'{e.MassMediaInformationProduct.InformationProduct.LinkProduct}'", sqlConnection);
                                        command.ExecuteNonQuery();
                                        sqlConnection.Close();
                                        break;
                                    }
                            }
                        }
                    }
                    catch { }

                }
            }
        }
        private void Client_NewsReceivedBySubscription(MassMediaInformationProduct e)
        {
            lock (locker)
            {
                if (!MediaProductBySubscription.Contains(e))
                {
                    App.Current.Dispatcher.Invoke(() => MediaProductBySubscription.Insert(0, e));
                }
            }
        }
        public ICommand RegistrationCommand => registrationCommand ??= new RelayCommand(obj =>
        {
            var (objOne, objTwo) = obj as Tuple<object, object>;
            if (!(objOne is string login) || !(objTwo is string password)) return;
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State != System.Data.ConnectionState.Open) AuthorizationAccepted?.Invoke(this, false, "Отсутствует подключение к базе данных");
                else
                {
                    SqlCommand command;
                    command = new SqlCommand($"select count([Login]) from Client where [Login] = N'{login}'", sqlConnection);

                    int countСoincidences = (int)command.ExecuteScalar();
                    if (countСoincidences == 1)
                    {
                        RegistrationAccepted.Invoke(this, false, "Такой логин уже существует");
                        sqlConnection.Close();
                        return;
                    }
                    else
                    {
                        command = new SqlCommand($"Insert Into Client([Login], [Password]) values(N'{login}', N'{BCrypt.Net.BCrypt.HashPassword(password)}')", sqlConnection);
                        command.ExecuteNonQuery();
                        RegistrationAccepted.Invoke(this, true, "Клиент успешно зарегистрирован");
                        sqlConnection.Close();
                    }
                }
            }
            catch { RegistrationAccepted?.Invoke(this, false, "Ошибка работы с базой данных"); }
        }
        );
        public ICommand PageClosingCommand => pageClosingCommand ??= new RelayCommand(obj =>
        {
            if (!(obj is EmotionCommandParametr parameters)) return; 

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                SqlCommand command;
                    switch (parameters.Type)
                    {
                        case InformationProductType.Text:
                            {
                                command = new SqlCommand($@"update Text set [Like] = {parameters.Like}, Wow = {parameters.Wow}, HaHa = {parameters.HaHa}, Sad = {parameters.Sad}, Angry = {parameters.Angry}, DisLike = {parameters.DisLike}  where [link] = N'{parameters.Link}'", sqlConnection);
                                command.ExecuteNonQuery();
                                sqlConnection.Close();
                                break;
                            }
                        case InformationProductType.Video:
                            {
                            command = new SqlCommand($@"update Video set [Like] = {parameters.Like}, Wow = {parameters.Wow}, HaHa = {parameters.HaHa}, Sad = {parameters.Sad}, Angry = {parameters.Angry}, DisLike = {parameters.DisLike}  where [link] = N'{parameters.Link}'", sqlConnection);
                            command.ExecuteNonQuery();
                            sqlConnection.Close();
                            break;
                        }
                    }
            }
            catch { }
        }
        );
    }
}
