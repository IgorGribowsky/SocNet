using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocNet.Services.UtilityModels
{
    public class RequestPageData
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int SkippedEntities { get { return (PageIndex - 1) * PageSize; } }

    }
}
