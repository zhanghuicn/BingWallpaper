using Microsoft.Win32;
using System;
using System.Drawing;
using System.Json;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace BingWallpaper
{
    class Wallpaper
    {
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;
        private const string ImageName = "cn.bing.com.wallpaper_zhui.bmp";

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style : int
        {
            Tiled,
            Centered,
            Stretched
        }

        public static String GetWallpaperUrl()
        {
            var url = "http://cn.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1";
            byte[] bResult = (new WebClient()).DownloadData(url);
            String strJson = Encoding.UTF8.GetString(bResult);

            JsonValue jsonValue = JsonValue.Parse(strJson);
            JsonObject jsonObj = jsonValue as JsonObject;
            String strURL = jsonObj["images"][0]["url"].ToString();

            return ("http://cn.bing.com" + strURL).Replace(@"\/", "/").Replace("\"", "");
        }

        public static void Set(String uri, Style style)
        {
            System.Drawing.Image img = Image.FromStream(new System.Net.WebClient().OpenRead(uri.ToString()));

            // 将图片保存到本地目录
            img.Save(Environment.GetEnvironmentVariable("Temp") + "\\" + ImageName, System.Drawing.Imaging.ImageFormat.Bmp);

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Stretched)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            else if (style == Style.Centered)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            else if (style == Style.Tiled)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }

            // 设置桌面壁纸的API
            SystemParametersInfo(SPI_SETDESKWALLPAPER
                                , 0
                                , Environment.GetEnvironmentVariable("Temp") + "\\" + ImageName
                                , SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}
