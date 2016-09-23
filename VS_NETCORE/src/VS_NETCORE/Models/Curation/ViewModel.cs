using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VS_NETCORE.Models.Curation
{
    public class ViewModel
    {
        public CurationViewModel[] CurationViewModel { get; set; }
    }

    /// <summary></summary>
    public class CurationViewModel
    {
        /// <summary></summary>
        public string HPTitle { get; set; }

        /// <summary></summary>
        public string RssURL { get; set; }


        public CurationTitleViewModel[] CurationTitleViewModel { get; set; }


    }
    public class CurationTitleViewModel
    {
        /// <summary></summary>
        public string Title { get; set; }

        /// <summary></summary>
        public string Link { get; set; }

    }
}
