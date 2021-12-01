using Modulir_Mass_Media.Helpers;
using System;


namespace Modulir_Mass_Media.Classes
{
    public enum InformationProductType
    {
        Text,
        Video
    }

    public class InformationProduct : NotifyPropertyChanged
    {
        private int like;
        private int wow;
        private int haHa;
        private int sad;
        private int angry;
        private int disLike;

        public InformationProduct(string titleProduct, string contentProduct, string linkProduct, string categoryProduct, InformationProductType productType, int like, int haHa, int wow, int sad, int angry, int disLike)
        {
            TitleProduct = titleProduct;
            ContentProduct = contentProduct;
            LinkProduct = linkProduct;
            CategoryProduct = categoryProduct;
            ProductType = productType;
            this.like = like;
            this.haHa = haHa;
            this.wow = wow;
            this.sad = sad;
            this.angry = angry;
            this.disLike = disLike;

            ViewModel.EditLikeCommand += EditLikeCommand;
            ViewModel.EditHaHaCommand += EditHaHaCommand;
            ViewModel.EditWowCommand += EditWowCommand;
            ViewModel.EditSadCommand += EditSadCommand;
            ViewModel.EditAngryCommand += EditAngryCommand;
            ViewModel.EditDisLikeCommand += EditDisLikeCommand;
        }
        
        public InformationProductType ProductType { get; private set; }
        public string TitleProduct { get; private set; }
        public string ContentProduct { get; private set; }
        public string LinkProduct { get; private set; }
        public string CategoryProduct { get; private set; }

        #region Свойства
        public int Like
        {
            get => like;
            private set
            {
                like = value;
                OnPropertyChanged();
            }
        }

        public int Wow
        {
            get => wow;
            private set
            {
                wow = value;
                OnPropertyChanged();
            }
        }
        public int HaHa
        {
            get => haHa;
            private set
            {
                haHa = value;
                OnPropertyChanged();
            }
        }
        public int Sad
        {
            get => sad;
            private set
            {
                sad = value;
                OnPropertyChanged();
            }
        }
        public int Angry
        {
            get => angry;
            private set
            {
                angry = value;
                OnPropertyChanged();
            }
        }
        public int DisLike
        {
            get => disLike;
            private set
            {
                disLike = value;
                OnPropertyChanged();
            }
        }

        #endregion
        private void EditDisLikeCommand(int disLike, string link)
        {
            if (link == LinkProduct) DisLike += disLike;
        }

        private void EditAngryCommand(int angry, string link)
        {
            if (link == LinkProduct) Angry += angry;
        }

        private void EditSadCommand(int sad, string link)
        {
            if (link == LinkProduct) Sad += sad;
        }

        private void EditWowCommand(int wow, string link)
        {
            if (link == LinkProduct) Wow += wow;
        }

        private void EditHaHaCommand(int haHa, string link)
        {
            if (link == LinkProduct) HaHa += haHa;
        }

        private void EditLikeCommand(int like, string link)
        {
            if (link == LinkProduct) Like += like;
        }

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
