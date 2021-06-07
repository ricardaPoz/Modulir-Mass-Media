using Modulir_Mass_Media.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Threading;
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
        Thread thread;

        public ModulationMediaWork()
        {
            
            thread = new Thread(() =>
            {
                DateTime.Now.ToString("D", CultureInfo.CreateSpecificCulture("ru-RU"));
                MassMediaInformationProducts.Add(new MassMediaInformationProduct(namesmi, new InformationProduct("ЕС запретил полеты белорусских авиакомпаний в своем воздушном пространстве", "БРЮССЕЛЬ, 4 июня. /ТАСС/. Решение Евросоюза запретить полеты авиакомпаний Белоруссии в воздушном пространстве сообщества и посадку их самолетов в европейских аэропортах вступает в силу с 5 июня. Об этом говорится в Официальном журнале ЕС, в котором в пятницу опубликованы соответствующие нормативные акты.", "Link", "category"), dt));
                MassMediaInformationProducts.Add(new MassMediaInformationProduct(namesmi, new InformationProduct(title, content, "Link", "category"), dt));
                MassMediaInformationProducts.Add(new MassMediaInformationProduct(namesmi, new InformationProduct(title, content, "Link", "category"), dt));
                MassMediaInformationProducts.Insert(0, new MassMediaInformationProduct(namesmi + "das", new InformationProduct(title, content, "Link", "category"), dt));
            }
            );
            thread.Start();
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
            massMediaInformationProducts.Add(e.MassMediaInformationProduct);
        }

    }
}
