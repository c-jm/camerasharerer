using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CSharpImageProcessor
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No Images Specified");
                return;
            }

            var fi = new FileInfo(args[0]);

            var imageData = new byte[fi.Length];

            using (FileStream fs = fi.OpenRead())
            {
                fs.Read(imageData, 0, (int)fi.Length);
            }

            using (var ms = new MemoryStream(imageData))
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = ms;
                img.EndInit();
                Clipboard.SetImage(img);
            }
        }
    }
}
