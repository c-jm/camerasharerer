using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CSharpImageProcessor
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {


            // @TODO: This will go into a TcpHandler class or something similar.
            Console.WriteLine("Listening...");
            TcpListener tcpListener = new TcpListener(IPAddress.Parse("192.168.0.111"), 500);
            tcpListener.Stop();
            tcpListener.Start();

            while (true)
            {
                ///@TODO: Async brah.
                TcpClient acceptedClient = tcpListener.AcceptTcpClient();
                Console.WriteLine("Client connected man....");
            }


            //ReadLocalImage(args);
        }

        // @NOTE: This is just a simple teststrap for setting image on the 
        // clipboard, basically throw away code.
        private static void ReadLocalImage(string[] args)
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
