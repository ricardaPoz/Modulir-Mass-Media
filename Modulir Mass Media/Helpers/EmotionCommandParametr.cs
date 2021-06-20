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
        public InformationProductType Type { get; set; }
        public string Link { get; set; }

        public int Like{ get; set; }
        public int HaHa{ get; set; }
        public int Wow { get; set; }
        public int Sad { get; set; }
        public int Angry { get; set; }
        public int DisLike { get; set; }

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
