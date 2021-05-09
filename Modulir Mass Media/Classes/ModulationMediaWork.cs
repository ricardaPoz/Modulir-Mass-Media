using Modulir_Mass_Media.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Timers;

namespace Modulir_Mass_Media.Classes
{
    class ModulationMediaWork : NotifyPropertyChanged
    {
        public ObservableCollection<MassMediaInformationProduct> massMediaInformationProducts = new ObservableCollection<MassMediaInformationProduct>();


        public ObservableCollection<MassMediaInformationProduct> MassMediaInformationProducts { get; set; } = new ObservableCollection<MassMediaInformationProduct>();

        string namesmi = "Ведомости";
        string title = "Блинкен признал, что ряд действий США подрывали миропорядок";
        string content = "Некоторые действия властей США за последние годы подрывали «основанный на правилах порядок», заявил...   ";

        DateTime dt = DateTime.Now;
        

        public ModulationMediaWork()
        {
            DateTime.Now.ToString("D", CultureInfo.CreateSpecificCulture("ru-RU"));
            MassMediaInformationProducts.Add(new MassMediaInformationProduct(namesmi, new InformationProduct(title, content, "Link", "category"), dt));
            MassMediaInformationProducts.Add(new MassMediaInformationProduct(namesmi, new InformationProduct(title, content, "Link", "category"), dt));
            MassMediaInformationProducts.Add(new MassMediaInformationProduct(namesmi, new InformationProduct(title, content, "Link", "category"), dt));
            MassMediaInformationProducts.Add(new MassMediaInformationProduct(namesmi, new InformationProduct(title, content, "Link", "category"), dt));
          
        }

  

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

        public void ModulationMedia()
        {
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
            massMediaInformationProducts.Add(e.MassMediaInformationProduct);
        }

    }
}
