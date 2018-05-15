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
using ServerAPI.Service;

namespace ServerAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Files")]
    public class FilesController : Controller
    {
        private readonly UserContext _context;
        private readonly IUserRepository userRepository;
        private readonly VideoService videoService;

        public FilesController(UserContext context, IUserRepository userRepository, VideoService videoService)
        {
            _context = context;
            this.userRepository = userRepository;
            this.videoService = videoService;
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

            int fileCount = videoService.SplitVideoToFrames(fileName);
            fileCount = fileCount * 2;
            videoService.StartProcessing(fileName);
            while (fileCount != Directory.GetFiles($@".\{fileName}\").Length)
            {
            }
            videoService.CombineFramesToVideo(fileName);
        }

        // GET: api/Files
        [HttpGet("ProcessVideo/{filePath}")]
        public void ProcessVideo([FromRoute] string filePath)
        {
            int fileCount = videoService.SplitVideoToFramesAbsolute(filePath);
            fileCount = fileCount * 2;
            videoService.StartProcessingAbsolute(filePath);
            while (fileCount != Directory.GetFiles($@".\{filePath}\").Length)
            {
            }
            videoService.CombineFramesToVideoAbsolute(filePath);
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

        [HttpGet("GetUserFiles/{userName}")]
        public File[] GetUserFiles([FromRoute] string userName)
        {
            var user = userRepository.GetUserByUserName(userName);
            var files = user.Files.ToList().FindAll(_ => _.UserId == user.UserId);
            files.ForEach(_ => _.FilePath.Replace(_.FileName, "out_" + _.FileName));
            return files.ToArray();
        }

        [HttpGet("GetFilesStatus/{userName}/{fileName}")]
        public bool GetFilesStatus([FromRoute] string userName, [FromRoute] string fileName)
        {
            var user = userRepository.GetUserByUserName(userName);
            var file = user.Files.ToList().FindAll(_ => _.UserId == user.UserId).FirstOrDefault(_ => fileName.Contains(_.FileName));
            if (file != null)
            {
                if (System.IO.File.Exists(file.FilePath.Replace(file.FileName, "out_" + file.FileName)))
                    return true;
                else
                    return false;
            }
            else
                return false;
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