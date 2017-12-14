using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ZhiHuLogin
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Pic.Source = Utils.GetZhiHuImage();

        }

        private void Pic_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var res = e.Source as FrameworkElement;
            Point p = Mouse.GetPosition(res);
            var data = Utils.GetSingleCaptcha();
            data.List.Add(new Data()
            {
                X = p.X,
                Y = p.Y
            });
            MessageBox.Show(p.X + "," + p.Y);

        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var str = Utils.GetStr();
            var postparam =
                $"_xsrf={Utils.GetXs(Utils.GetHtml())}&password={P1.Password}&captcha={str}&captcha_type=cn&phone_num={U1.Text.Trim()}";

            var request = (HttpWebRequest)WebRequest.Create("https://www.zhihu.com/login/phone_num");
            request.Method = "post";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.CookieContainer = Utils.CookieContainer;
            using (var req = request.GetRequestStream())
            {
                var bytes = Encoding.UTF8.GetBytes(postparam);
                req.Write(bytes, 0, bytes.Length);
            }
            using (var res = request.GetResponse())
            {
                var r = res.GetResponseStream();
                var strr = new StreamReader(r);
                var mes = strr.ReadToEnd();
                MessageBox.Show(mes);
                MessageBox.Show(Utils.GetUserInfo());
            }
        }

    }
}
