using System.Collections.Generic;

namespace ZhiHuLogin
{
    public class Captcha
    {
        public string img_size { set; get; }
        public List<Data> List { set; get; }
    }

    public class Data
    {
        public double X { set; get; }
        public double Y { set; get; }
    }
}
