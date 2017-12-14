using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiHuLogin
{
   public class PostModel
    {
        public string _xsrf { set; get; }
        public string password { set; get; }
        public Captcha captcha { set; get; }
        public string captcha_type { set; get; }
        public string phone_num { set; get; }
    }
}
