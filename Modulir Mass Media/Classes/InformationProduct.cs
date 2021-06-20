using System;

namespace Modulir_Mass_Media.Classes
{
    public enum InformationProductType
    {
        Text,
        Video
    }

    public class InformationProduct
    {
        public InformationProduct(string titleProduct, string contentProduct, string linkProduct, string categoryProduct, InformationProductType productType, int like, int haHa, int wow, int sad, int angry, int disLike)
        {
            TitleProduct = titleProduct;
            ContentProduct = contentProduct;
            LinkProduct = linkProduct;
            CategoryProduct = categoryProduct;
            ProductType = productType;
            Like = like;
            HaHa = haHa;
            Wow = wow;
            Sad = sad;
            Angry = angry;
            DisLike = disLike;
        }
        public InformationProductType ProductType { get; private set; }

        public string TitleProduct { get; private set; }
        public string ContentProduct { get; private set; }
        public string LinkProduct { get; private set; }
        public string CategoryProduct { get; private set; }
        public int Like { get; set; }
        public int Wow { get; set; }
        public int HaHa { get; set; }
        public int Sad { get; set; }
        public int Angry { get; set; }
        public int DisLike { get; set; }
    }

    public class MassMediaInformationProduct
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
