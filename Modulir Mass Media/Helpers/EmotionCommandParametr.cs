using Modulir_Mass_Media.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulir_Mass_Media.Helpers
{
    public class EmotionCommandParametr
    {
        public InformationProductType Type { get; private set; }
        public string Link { get; private set; }

        public int Like{ get; private set; }
        public int HaHa{ get; private set; }
        public int Wow { get; private set; }
        public int Sad { get; private set; }
        public int Angry { get; private set; }
        public int DisLike { get; private set; }

        public EmotionCommandParametr(InformationProductType type, string link, int like, int haHa, int wow, int sad, int angry, int disLike)
        {
            Type = type;
            Link = link;
            Like = like;
            HaHa = haHa;
            Wow = wow;
            Sad = sad;
            Angry = angry;
            DisLike = disLike;
        }
    }
}
