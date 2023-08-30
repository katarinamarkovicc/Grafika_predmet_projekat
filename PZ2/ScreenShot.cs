using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace PZ2
{
    public class ScreenShot
    {
        public static void takeScreenShoot()
        {
            string CurrentDirectory = Directory.GetCurrentDirectory();
            string time = DateTime.Now.ToLongTimeString().Replace(":", "_");
            time = time.Replace(":", "_").Trim();
            CurrentDirectory += "\\" + time;

            // If directory does not exist, create it
            if (!Directory.Exists(CurrentDirectory))
            {
                Directory.CreateDirectory(CurrentDirectory);
            }
            Bitmap bm = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(0, 0, 0, 0, bm.Size);
            string fileName = DateTime.Now.ToLongDateString().Replace("/", "_") + ".png";

            bm.Save(Path.Combine(CurrentDirectory, fileName), ImageFormat.Png);
        }
    }
}
