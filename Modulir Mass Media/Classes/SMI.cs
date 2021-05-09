﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Modulir_Mass_Media.Classes
{
    class MassMediaReleaseInformationProductEventArgs
    {
        public MassMediaInformationProduct MassMediaInformationProduct { get; private set; }
        public MassMediaReleaseInformationProductEventArgs(MassMediaInformationProduct massMediaProduct) => MassMediaInformationProduct = massMediaProduct;
    }

    class MassMedia
    {
        public delegate void MassMediaReleaseInformationProductHandler(object sender, MassMediaReleaseInformationProductEventArgs e);
        public event MassMediaReleaseInformationProductHandler ProductRelese;

        private string nameMassMedia;

        public MassMedia(string nameMassMedia)
        {
            this.nameMassMedia = nameMassMedia;
        }
        List<Journalist> listJournalist = new List<Journalist>(); // список журналистов, работающих на СМИ

        // коллекция информационных продуктов СМИ
        ObservableCollection<MassMediaInformationProduct> massMediaInformationProducts = new ObservableCollection<MassMediaInformationProduct>();

        // Устройство на работу журналиста
        public void HiringEmployee(Journalist journalist)
        {
            listJournalist.Add(journalist);
            journalist.Employment();
            journalist.InformationProductCreated += Journalist_InformationProductCreated;
        }

        // Увольнение с работы журналиста 
        public void DismissalEmployee(Journalist journalist)
        {
            listJournalist.Remove(journalist);
            journalist.InformationProductCreated -= Journalist_InformationProductCreated;
            journalist.Dismissal();
        }

        private void Journalist_InformationProductCreated(object sender, ProductCreatedEventArgs e)
        {
            MassMediaInformationProduct massMediaInformationProduct = new MassMediaInformationProduct(nameMassMedia, e.InformationProduct, DateTime.Now);
            massMediaInformationProducts.Add(massMediaInformationProduct);
            ProductRelese?.Invoke(this, new MassMediaReleaseInformationProductEventArgs(massMediaInformationProduct));
        }
    }
}