using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using MongoDB.Bson;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using Microsoft.AspNetCore.Components.Forms;

namespace NewBlazorTest.Services
{
    public class FileSystemService
    {
        private readonly ILogger<FileSystemService> _logger;
        private readonly GridFSBucket _gridFS;
        public FileSystemService(ILogger<FileSystemService> logger)
        {
            _logger = logger;
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("ZaripovImages");
            _gridFS = new GridFSBucket(database);
        }

        #region Upload
        public async Task UploadImageToDb(Stream stream, string fileName)
        {
            await _gridFS.UploadFromStreamAsync(fileName, stream);
        }

        public void UploadImageToDb(string fileName, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                _gridFS.UploadFromStream(fileName, fs);
            }
        }
        #endregion

        #region Download
        public void DownloadFileToProject(GridFSFileInfo file)
        {
            using (FileStream fs = new FileStream($"{Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/wwwroot/images/")}{file.Filename}", FileMode.CreateNew))
            {
                _gridFS.DownloadToStreamByName(file.Filename, fs);
            }
        }

        public void DownloadFileToProject(IBrowserFile file)
        {
            try
            {
                using (FileStream fs = new FileStream($"{Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/wwwroot/images/")}{file.Name}", FileMode.CreateNew))
                {
                    _gridFS.DownloadToStreamByName(file.Name, fs);
                }
            }
            catch (Exception)
            {
                _logger.LogError("Image already exists");
            }
        }
        #endregion

        public void GetImagesToProjectFolder()
        {
            var images = _gridFS.Find(new BsonDocument()).ToList();
            foreach (var image in images)
            {
                try
                {
                    using (FileStream fs = new FileStream($"{Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/wwwroot/images/")}{image.Filename}", FileMode.CreateNew))
                    {
                        _gridFS.DownloadToStreamByName(image.Filename, fs);
                    }
                }
                catch (Exception)
                {
                    _logger.LogError("Image already exists");
                }
                
            }
        }

        public List<string> FindFiles()
        {
            var fileInfos = _gridFS.Find(new BsonDocument()).ToList();
            var imageInfos = new List<string>();
            foreach(var file in fileInfos)
            {
                imageInfos.Add(file.Filename);
            }
            return imageInfos;
        }

        public bool FileExists(string filename)
        {
            return _gridFS.Find(FilterDefinition<GridFSFileInfo>.Empty).ToEnumerable().Any(x => x.Filename == filename);
        }

        private void AddImagesToDb()
        {
            foreach (var file in Directory.GetFiles("wwwroot/images"))
            {
                if (!FileExists(Path.GetFileName(file)))
                    UploadImageToDb(Path.GetFileName(file), file);
            }
        }
    }
}
