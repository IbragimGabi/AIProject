using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServerAPI.Service
{
    public class VideoService
    {
        public VideoService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public int SplitVideoToFrames(string fileName)
        {
            Directory.CreateDirectory($@".\{fileName}");
            var command = $@" .\NN\ffmpeg\bin\ffmpeg.exe -i .\Files\{fileName} -y .\{fileName}\image%d.png -s WxH";

            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };

            process = Process.Start(processInfo);
            process.WaitForExit();
            var files = Directory.GetFiles($@".\{fileName}\");
            return files.Length;
        }

        public void StartProcessing(string fileName)
        {
            string strCmdText = $@"D:&D:\Work\Anaconda3\Scripts\activate.bat&activate tensorflow&cd C:\Users\Ibragim\source\repos\AIProject\ServerAPI\NN\NN\&python C:\Users\Ibragim\source\repos\AIProject\ServerAPI\NN\NN\Use.py C:\Users\Ibragim\source\repos\AIProject\ServerAPI\{fileName}\";

            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + strCmdText)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };

            process = Process.Start(processInfo);
        }

        public void CombineFramesToVideo(string fileName)
        {
            Directory.CreateDirectory($@".\{fileName}");

            var command = $@".\NN\ffmpeg\bin\ffmpeg.exe -f image2 -i .\{fileName}\out_image%d.png -y .\Files\out_{fileName} -s WxH";

            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = false,
                UseShellExecute = false
            };

            process = Process.Start(processInfo);
            process.WaitForExit();
            //Directory.Delete($@".\{fileName}", true);
            //if (System.IO.File.Exists($@".\Files\out_{fileName}") && System.IO.File.Exists($@".\Files\{fileName}"))
            //    System.IO.File.Delete($@".\Files\{fileName}");
        }

        private string GetFileName(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Name;
        }

        public int SplitVideoToFramesAbsolute(string filePath)
        {
            var fileName = GetFileName(filePath);
            Directory.CreateDirectory($@".\{fileName}");
            var command = $@" .\NN\ffmpeg\bin\ffmpeg.exe -i {filePath} -y .\{fileName}\image%d.png -s WxH";

            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };

            process = Process.Start(processInfo);
            process.WaitForExit();
            var files = Directory.GetFiles($@".\{fileName}\");
            return files.Length;
        }

        public void StartProcessingAbsolute(string filePath)
        {
            var fileName = GetFileName(filePath);
            Configuration.GetConnectionString("PathToAnaconda");
            string strCmdText = $"{Configuration.GetConnectionString("PathToAnaconda")}Scripts\\activate.bat&activate tensorflow&cd {Configuration.GetConnectionString("PathToServer")}NN\\NN\\&python {Configuration.GetConnectionString("PathToServer")}NN\\NN\\Use.py {Configuration.GetConnectionString("PathToServer")}{fileName}\\";

            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + strCmdText)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };

            process = Process.Start(processInfo);
        }

        public void CombineFramesToVideoAbsolute(string filePath)
        {
            var fileName = GetFileName(filePath);
            var fileDirectory = new FileInfo(filePath).Directory.FullName;

            var command = $@".\NN\ffmpeg\bin\ffmpeg.exe -f image2 -i .\{fileName}\out_image%d.png -y .Files\out_{fileName} -s WxH";

            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = false,
                UseShellExecute = false
            };

            process = Process.Start(processInfo);
            process.WaitForExit();
            //Directory.Delete($@".\{fileName}", true);
            //if (System.IO.File.Exists($@".\Files\out_{fileName}") && System.IO.File.Exists($@".\Files\{fileName}"))
            //    System.IO.File.Delete($@".\Files\{fileName}");
        }
    }
}
