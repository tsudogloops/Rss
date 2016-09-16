using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VS_NETCORE.Models.Curation
{
    public class IndexViewModel
    {
        public string Header => new AvailabilityDateTime(DateTime.Now).Value.ToString("yyyy年MM月dd日 HH:mm時点の空き情報");
    }
}
