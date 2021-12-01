using System;

namespace Modulir_Mass_Media.Classes
{
    public class MassMedia
    {
        public event Action<MassMediaInformationProduct> ProductRelese;

        public string NameMedia { get; private set; }
        public MassMedia(string nameMedia)
        {
            NameMedia = nameMedia;
            ViewModel.JournalistHired += JournalistHired;
        }
        private void JournalistHired(MassMedia massMedia, Journalist journalist)
        {
            if(massMedia == this) journalist.InformationProductCreated += InformationProductCreated;
        }
        private void InformationProductCreated(InformationProduct informationProduct)
        {
            MassMediaInformationProduct massMediaInformationProduct = new MassMediaInformationProduct(NameMedia, informationProduct, DateTime.Now);
            ProductRelese?.Invoke(massMediaInformationProduct);
        }
    }
}
