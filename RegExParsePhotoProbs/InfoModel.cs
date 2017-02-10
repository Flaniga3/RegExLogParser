using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegExParsePhotoProbs
{
    class InfoModel
    {
        public Guid Id;
        public DateTime Start;
        public DateTime End;
        public TimeSpan Duration => End - Start;

        public InfoModel(Guid id)
        {
            Id = id;
        }
    }
}
