using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerAPI;
using ServerAPI.DataAccess;

namespace ServerAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Files")]
    public class FilesController : Controller
    {
        private readonly UserContext _context;
        private readonly IUserRepository userRepository;

        public FilesController(UserContext context, IUserRepository userRepository)
        {
            _context = context;
            this.userRepository = userRepository;
        }


        // GET: api/Files
        [HttpPut("{userName}/{fileName}/{fileCheckSum}")]
        public void DefineFileByCheckSum([FromRoute] string userName, [FromRoute] string fileName, [FromRoute] string fileCheckSum)
        {
            var user = userRepository.GetUserByUserName(userName);
            var files = Directory.GetFiles(@".\Files\");
            var file = files.First(_ => CalculateMD5(_) == fileCheckSum);
            var newFile = Rename(file, fileName);
            user.Files.Add(
                new ServerAPI.File
                {
                    FileName = fileName,
                    FilePath = newFile,
                    CheckSum = fileCheckSum
                });
            userRepository.UpdateUser(user.UserId, user);

            int fileCount = SplitVideoToFrames(fileName);
            fileCount = fileCount * 2;
            StartProcessing(fileName);
            while (fileCount != Directory.GetFiles($@".\{fileName}\").Length)
            {
            }
            CombineFramesToVideo(fileName);
        }

        [HttpGet("GetFilePath/{userName}/{fileName}")]
        public string GetFilePath([FromRoute] string userName, [FromRoute] string fileName)
        {
            var user = userRepository.GetUserByUserName(userName);
            var file = user.Files.SingleOrDefault(_ => fileName.Contains(_.FileName));
            return file.FilePath.Replace(file.FileName, "out_" + fileName);
        }

        [HttpGet("GetLastUserFile/{userName}")]
        public string GetLastUserFile([FromRoute] string userName)
        {
            var user = userRepository.GetUserByUserName(userName);
            var file = user.Files.Last();
            return file.FilePath.Replace(file.FileName, "out_" + file.FileName);
        }


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


        public static string Rename(string filePath, string newName)
        {
            var fileInfo = new FileInfo(filePath);
            var newFilePath = fileInfo.Directory.FullName + "\\" + newName;
            fileInfo.MoveTo(newFilePath);
            return newFilePath;
        }

        // GET: api/Files/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFile([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var file = await _context.Files.SingleOrDefaultAsync(m => m.FileId == id);

            if (file == null)
            {
                return NotFound();
            }

            return Ok(file);
        }

        // PUT: api/Files/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFile([FromRoute] int id, [FromBody] File file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != file.FileId)
            {
                return BadRequest();
            }

            _context.Entry(file).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Files
        [HttpPost]
        public async Task<IActionResult> PostFile([FromBody] File file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Files.Add(file);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFile", new { id = file.FileId }, file);
        }

        // DELETE: api/Files/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var file = await _context.Files.SingleOrDefaultAsync(m => m.FileId == id);
            if (file == null)
            {
                return NotFound();
            }

            _context.Files.Remove(file);
            await _context.SaveChangesAsync();

            return Ok(file);
        }

        private bool FileExists(int id)
        {
            return _context.Files.Any(e => e.FileId == id);
        }

        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}