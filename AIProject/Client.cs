using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AIProject
{
    public class Client
    {
        public Client(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }
        private NetworkStream ns;
        private string path = string.Empty;
        private string ip;
        private int port;

        public void StartUploading(String S)
        {
            path = S;
        }

        public void Run()
        {
            Thread mainThread = new Thread(() =>
            {
                TcpClient _client = new TcpClient("127.0.0.1", 808);
                ns = _client.GetStream();

                while (true)
                {
                    if (path.Length > 0 && path != string.Empty && ns.CanWrite)
                    {
                        byte[] file = System.IO.File.ReadAllBytes(path);
                        byte[] fileBuffer = new byte[file.Length];
                        ns.Write(file.ToArray(), 0, fileBuffer.GetLength(0));

                        path = string.Empty;

                        ns.Flush();
                    }
                }
            });
            mainThread.IsBackground = true;
            mainThread.Start();
        }
    }
}
