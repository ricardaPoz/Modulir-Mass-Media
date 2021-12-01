using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulir_Mass_Media.Classes
{
    public class Client
    {
        public event Action<MassMediaInformationProduct> NewsReceivedBySubscription;
        public event Action<MassMedia> MediaUnSubscripted;

        public string Login { get; private set; }
        public Client(string login)
        {
            Login = login;
            ViewModel.SubscribedToMedia += SubscribedToMedia;
            ViewModel.UnscribedToMedia += UnscribedToMedia;
        }

        private void UnscribedToMedia(MassMedia media)
        {
            MediaUnSubscripted?.Invoke(media);
            media.ProductRelese -= Media_ProductRelese;
        }

        private void SubscribedToMedia(MassMedia media)
        {
            media.ProductRelese += Media_ProductRelese;
        }

        private void Media_ProductRelese(MassMediaInformationProduct mediaInformationProduct)
        {
            NewsReceivedBySubscription?.Invoke(mediaInformationProduct);
        }
    }
}
