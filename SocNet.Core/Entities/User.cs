using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocNet.Core.Entities
{
    class User
    {
        public int Id { get; private set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }
    }
}
