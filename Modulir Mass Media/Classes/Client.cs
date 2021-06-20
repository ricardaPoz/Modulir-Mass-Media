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

        private List<MassMedia> medias = new List<MassMedia>();
         
        public string Login { get; }
        public Client(string login, ViewModel viewModel)
        {
            Login = login;
            viewModel.SubscribedToMedia += SubscribedToMedia;
            viewModel.UnscribedToMedia += UnscribedToMedia;
        }

        private void UnscribedToMedia(object sender, MassMedia e)
        {
            medias.Remove(e);
            MediaUnSubscripted?.Invoke(e);
            e.ProductRelese -= Media_ProductRelese;
        }

        private void SubscribedToMedia(object sender, MassMedia media)
        {
            medias.Add(media);
            media.ProductRelese += Media_ProductRelese;
        }

        private void Media_ProductRelese(object sender, MassMediaReleaseInformationProductEventArgs e)
        {
            NewsReceivedBySubscription?.Invoke(e.MassMediaInformationProduct);
        }
    }
}
