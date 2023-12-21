using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolicyEnforcer.Service.POCO
{
    public class LoginInfo
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string UserID { get; set; }
    }
}
