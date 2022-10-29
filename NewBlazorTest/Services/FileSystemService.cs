using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using MongoDB.Bson;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;

namespace NewBlazorTest.Services
{
    public class FileSystemService
    {
        private readonly ILogger<FileSystemService> _logger;
        public FileSystemService(ILogger<FileSystemService> logger)
        {
            _logger = logger;
        }
        public void UploadImageToDb()
        {
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("ZaripovImages");
            var gridFS = new GridFSBucket(database);

            using (FileStream fs = new FileStream(@"D:\\OneDrive\\Рабочий стол\\photosToUpload\\ball.jpg", FileMode.Open))
            {
                gridFS.UploadFromStream("ball.jpg", fs);
            }
        }
        public void UploadImageToDb(string fileName, string path)
        {
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("ZaripovImages");
            var gridFS = new GridFSBucket(database);

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                gridFS.UploadFromStream(fileName, fs);
            }
        }

        public void DownloadFileToProject(GridFSFileInfo file)
        {
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("ZaripovImages");
            var gridFS = new GridFSBucket(database);
            using (FileStream fs = new FileStream($"{Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/wwwroot/images/")}{file.Filename}", FileMode.CreateNew))
            {
                gridFS.DownloadToStreamByName(file.Filename, fs);
            }
        }

        public void GetImagesToProjectFolder()
        {
            //AddImagesToDb();

            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("ZaripovImages");
            var gridFS = new GridFSBucket(database);
            var images = gridFS.Find(new BsonDocument()).ToList();
            foreach (var image in images)
            {
                try
                {
                    using (FileStream fs = new FileStream($"{Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/wwwroot/images/")}{image.Filename}", FileMode.CreateNew))
                    {
                        gridFS.DownloadToStreamByName(image.Filename, fs);
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
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("ZaripovImages");
            var gridFS = new GridFSBucket(database);
            var fileInfos = gridFS.Find(new BsonDocument()).ToList();
            var imageInfos = new List<string>();
            foreach(var file in fileInfos)
            {
                imageInfos.Add(file.Filename);
            }
            return imageInfos;
        }

        public bool FileExists(string filename)
        {
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("ZaripovImages");
            var gridFS = new GridFSBucket(database);

            return gridFS.Find(FilterDefinition<GridFSFileInfo>.Empty).ToEnumerable().Any(x => x.Filename == filename);
        }

        private void AddImagesToDb()
        {
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("ZaripovImages");
            var gridFS = new GridFSBucket(database);
            foreach (var file in Directory.GetFiles("wwwroot/images"))
            {
                if (!FileExists(Path.GetFileName(file)))
                    UploadImageToDb(Path.GetFileName(file), file);
            }
        }
    }
}
