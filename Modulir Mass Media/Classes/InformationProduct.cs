using System;
using System.Collections.Generic;
using System.Text;

namespace Modulir_Mass_Media.Classes
{
    class InformationProduct
    {
        public InformationProduct(string titleProduct, string contentProduct, string linkProduct, string categoryProduct)
        {
            TitleProduct = titleProduct;
            ContentProduct = contentProduct;
            LinkProduct = linkProduct;
            CategoryProduct = categoryProduct;
        }

        public string TitleProduct { get; private set; }
        public string ContentProduct { get; private set; }
        public string LinkProduct { get; private set; }
        public string CategoryProduct { get; private set; }
        public int CountLikeInformationProduct { get; set; }
    }

    class MassMediaInformationProduct
    {
        public InformationProduct InformationProduct { get; private set; }
        public DateTime DatePublication { get; private set; }
        public string NameMassMedia { get; private set; }

        public MassMediaInformationProduct(string nameMassMedia, InformationProduct informationProduct, DateTime datePublication)
        {
            NameMassMedia = nameMassMedia;
            InformationProduct = informationProduct;
            DatePublication = datePublication;
        }
    }
}
