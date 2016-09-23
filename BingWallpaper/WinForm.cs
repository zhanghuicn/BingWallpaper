using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BingWallpaper
{
    public partial class WinForm : Form
    {
        public WinForm()
        {
            InitializeComponent();

            Wallpaper.Set(Wallpaper.GetWallpaperUrl(), Wallpaper.Style.Stretched);

            System.Environment.Exit(0);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.notifyIcon1.Visible = false;
        }
    }
}
