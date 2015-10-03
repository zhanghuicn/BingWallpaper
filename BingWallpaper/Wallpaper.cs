using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace BingWallpaper
{
    class Wallpaper
    {
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style : int
        {
            Tiled,
            Centered,
            Stretched
        }

        private static string _saveDir = System.Environment.GetFolderPath(
                                            System.Environment.SpecialFolder.MyPictures)
                                        + "\\bing\\";

        public static string SaveDir
        {
            get { return _saveDir; }
            set { _saveDir = value; }
        }

        public static String GetWallpaperUrl()
        {
            var url = "http://cn.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1";
            byte[] bResult = (new WebClient()).DownloadData(url);
            String strJson = Encoding.UTF8.GetString(bResult);

            JsonValue jsonValue = JsonValue.Parse(strJson);
            JsonObject jsonObj = jsonValue as JsonObject;
            String strURL = jsonObj["images"][0]["url"].ToString();

            return strURL.Replace(@"\/", "/").Replace("\"", "");
        }

        public static void Set(String uri, Style style)
        {
            System.Drawing.Image img = null;
            if (null != _saveDir && !Directory.Exists(_saveDir))
                Directory.CreateDirectory(_saveDir);
            string imgPath = Path.Combine(null == _saveDir ? Path.GetTempPath() : _saveDir
                                    , DateTime.Now.ToString("yyyy-MM-dd") + ".bmp");
            if (File.Exists(imgPath))
            {
                // 如果文件存在，就直接读取
                img = Image.FromFile(imgPath);
            }
            else
            {
                // 如果文件不存在，就从URI中获取
                img = Image.FromStream(new System.Net.WebClient().OpenRead(uri.ToString()));

                // 将图片保存到本地目录
                img.Save(imgPath, System.Drawing.Imaging.ImageFormat.Bmp);
            }

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
                                , imgPath
                                , SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}
