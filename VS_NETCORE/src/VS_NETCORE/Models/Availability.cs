using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VS_NETCORE.Models.Curation;

namespace VS_NETCORE.Models
{
    public class Availability
    {
        public string Title { get; set; }
        public Uri Url { get; set; }
        public IEnumerable<AvailabilityDateTime> Dates { get; set; }
    }
}
