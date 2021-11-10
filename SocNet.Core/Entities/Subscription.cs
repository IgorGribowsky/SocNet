using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocNet.Core.Entities
{
    public class Subscription
    {
        public int SubscriberUserId { get; set; }

        public int TargetUserId { get; set; }
    }
}
