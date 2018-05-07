using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerAPI.Service
{
    public class Server
    {
        public TcpListener _listener;
        private MemoryStream ms;
        private int port;
        private string file;

        public Server(int port)
        {
            this.port = port;
            ms = new MemoryStream();
        }

        public void Run()
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();

            Thread tMain = new Thread(() =>
            {
                while (true)
                {
                    ListenNetworkStream(_listener.AcceptTcpClient());
                }
            });
            tMain.IsBackground = true;
            tMain.Start();
        }

        public void AddText(String s)
        {
            Thread t = new Thread(() =>
            {
                file += s;
            });
            t.IsBackground = true;
            t.Start();
        }

        NetworkStream ns;
        public void ListenNetworkStream(TcpClient c)
        {
            bool loading = false;
            ns = c.GetStream();
            int thisRead = 0;
            int blockSize = 1024;
            Byte[] dataByte = new Byte[blockSize];
            while (true)
            {
                if (ns.CanRead)
                {
                    thisRead = ns.Read(dataByte, 0, blockSize);
                    ms.Write(dataByte, 0, thisRead);
                    if (loading == false && thisRead == blockSize)
                    {
                        Console.WriteLine("File uploading started");
                        loading = true;
                    }

                    //Console.WriteLine(thisRead);
                    if (thisRead < blockSize && thisRead != 0)
                    {
                        System.IO.File.WriteAllBytes(@".\Files\file1.mp4", Convert2.ToByteArray(ms));
                        Console.WriteLine("File is uploaded");
                        loading = false;
                    }

                    ns.Flush();
                }
            }
        }
    }

    public static class Convert2
    {
        public static byte[] ToByteArray(Stream stream)
        {
            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            for (int totalBytesCopied = 0; totalBytesCopied < stream.Length;)
                totalBytesCopied += stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);
            return buffer;
        }
    }
}