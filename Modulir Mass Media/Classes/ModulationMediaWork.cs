using Modulir_Mass_Media.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Linq;

namespace Modulir_Mass_Media.Classes
{
    class ModulationMediaWork : NotifyPropertyChanged
    {
        // public ObservableCollection<MassMediaInformationProduct> massMediaInformationProducts = new ObservableCollection<MassMediaInformationProduct>();


        private ObservableCollection<MassMediaInformationProduct> massMediaInformationProducts = new ObservableCollection<MassMediaInformationProduct>();

        public ObservableCollection<MassMediaInformationProduct> MassMediaInformationProducts
        {
            get { return massMediaInformationProducts; }
        }

        object locker = new object();

        public ModulationMediaWork()
        {
            //ModulationMedia();
        }



        public void ModulationMedia()
        {
            Journalist Danilkin = new JournalistText("Данилкин Лев Александрович");
            Journalist Dzyadko = new JournalistText("Дзядко Тимофей Викторович");
            Journalist Taratuta = new JournalistText("Таратута Юлия Леонидовна");

            Journalist Shmarov = new JournalistVideo("Шмаров Андрей Игоревич");
            Journalist Simonyan = new JournalistVideo("Симоньян Маргарита Симоновна");

            Journalist Venediktov = new JournalistAudio("Венедиктов Алексей Алексеевич");
            Journalist Bershidsky = new JournalistAudio("Бершидский Леонид Давидович");


            MassMedia vedomosti = new MassMedia("Vedomosti");
            MassMedia tass = new MassMedia("TASS");
            MassMedia russiaToday = new MassMedia("RT");
            MassMedia echoMoskau = new MassMedia("Эхо Москвы");

            vedomosti.HiringEmployee(Taratuta);
            vedomosti.HiringEmployee(Dzyadko);

            russiaToday.HiringEmployee(Simonyan);

            echoMoskau.HiringEmployee(Venediktov);

            tass.HiringEmployee(Danilkin);
            tass.HiringEmployee(Shmarov);
            tass.HiringEmployee(Bershidsky);

            vedomosti.ProductRelese += Vedomosti_ProductRelese;

        }

        private void Vedomosti_ProductRelese(object sender, MassMediaReleaseInformationProductEventArgs e)
        {
            lock (locker)
            {
                if (MassMediaInformationProducts.Any(g => g.InformationProduct.TitleProduct == e.MassMediaInformationProduct.InformationProduct.TitleProduct)) return;
                Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MassMediaInformationProducts.Insert(0, e.MassMediaInformationProduct);
            }));
            }
        }
    }
}
