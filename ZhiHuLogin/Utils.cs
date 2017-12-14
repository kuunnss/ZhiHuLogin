using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Media.Imaging;
using HtmlAgilityPack;

namespace ZhiHuLogin
{
    public static class Utils
    {
        public static readonly CookieContainer CookieContainer = new CookieContainer();
        public static BitmapImage GetZhiHuImage()
        {
            var str = GetTimeStamp(DateTime.Now);
            var url = $"https://www.zhihu.com/captcha.gif?r=+{str}&type=login&lang=cn";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "get";
            request.ContentType = "image/gif";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36";
            request.CookieContainer = CookieContainer;
            using (var res = request.GetResponse())
            {
                var sr = res.GetResponseStream();
                var img = Image.FromStream(sr);
                Bitmap bmp = new Bitmap(img);

                return BitmapToBitmapImage(bmp);

            }
        }

        private static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            Bitmap bitmapSource = new Bitmap(bitmap.Width, bitmap.Height);
            int i, j;
            for (i = 0; i < bitmap.Width; i++)
                for (j = 0; j < bitmap.Height; j++)
                {
                    Color pixelColor = bitmap.GetPixel(i, j);
                    Color newColor = Color.FromArgb(pixelColor.R, pixelColor.G, pixelColor.B);
                    bitmapSource.SetPixel(i, j, newColor);
                }
            MemoryStream ms = new MemoryStream();
            bitmapSource.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(ms.ToArray());
            bitmapImage.EndInit();

            return bitmapImage;
        }

        private static string GetTimeStamp(System.DateTime time, int length = 13)
        {
            long ts = ConvertDateTimeToInt(time);
            return ts.ToString().Substring(0, length);
        }
        /// <summary>        
        /// 时间戳转为C#格式时间        
        /// </summary>        
        /// <param name=”timeStamp”></param>        
        /// <returns></returns>        
        public static DateTime ConvertStringToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }        /// <summary>  
                 /// 将c# DateTime时间格式转换为Unix时间戳格式  
                 /// </summary>  
                 /// <param name="time">时间</param>  
                 /// <returns>long</returns>  
        private static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }

        private static Captcha _captcha;
        public static Captcha GetSingleCaptcha()
        {
            if (_captcha == null)
            {
                _captcha = new Captcha
                {
                    List = new List<Data>()
                };
            }
            return _captcha;
        }

        public static string GetStr()
        {
            var str2 = string.Empty;
            for (int i = 0; i < _captcha.List.Count; i++)
            {
                var item = _captcha.List[i];
                str2 += $"[{item.X},{item.Y + 0.6094}]";
                if (i < _captcha.List.Count - 1)
                {
                    str2 += ",";
                }
            }
            var str = "{\"img_size\":[200,44],\"input_points\":[" + str2 + "]}";
            return str.Substring(0, str.Length);
        }

        public static string GetHtml()
        {

            var request = (HttpWebRequest)WebRequest.Create("https://www.zhihu.com/");
            request.Method = "get";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36";
            request.CookieContainer = CookieContainer;
            using (var res = request.GetResponse())
            {
                var sr = res.GetResponseStream();
                var s = new StreamReader(sr);
                return s.ReadToEnd();
            }
        }

        public static string GetXs(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var res = doc.DocumentNode.SelectSingleNode("//input[@name='_xsrf']");
            var xsrf = res.Attributes["value"].Value;
            Getudid(xsrf);
            return xsrf;
        }

        public static void Getudid(string xsrf)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://www.zhihu.com/udid");
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36";
            request.Method = "post";
            request.Accept = "*/*";
            request.CookieContainer = CookieContainer;
            using (var req = request.GetRequestStream())
            {
                req.Write(Encoding.UTF8.GetBytes(xsrf), 0, Encoding.UTF8.GetBytes(xsrf).Length);
            }
            request.GetResponse();
        }


        public static string GetUserInfo()
        {
            var request =
                (HttpWebRequest) WebRequest.Create("https://www.zhihu.com/api/v4/me?include=following_question_count");
            request.CookieContainer = CookieContainer;
            request.Accept = "application/json, text/plain, */*";
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36";
            using (var res = request.GetResponse())
            {
                var sr = new StreamReader(res.GetResponseStream());
                return sr.ReadToEnd();
            }

        }
    }
}
