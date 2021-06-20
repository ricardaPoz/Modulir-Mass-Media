namespace Modulir_Mass_Media
{
    internal class EmotionCommandParametr<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        private string linkProduct;
        private string nameMassMedia;
        private int v1;
        private int v2;

        public EmotionCommandParametr(string linkProduct, string nameMassMedia, int v1, int v2)
        {
            this.linkProduct = linkProduct;
            this.nameMassMedia = nameMassMedia;
            this.v1 = v1;
            this.v2 = v2;
        }
    }
}