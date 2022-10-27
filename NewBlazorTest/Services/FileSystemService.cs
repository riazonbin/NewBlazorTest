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

        public void DownloadToLocal()
        {
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("ZaripovImages");
            var gridFS = new GridFSBucket(database);
            using (FileStream fs = new FileStream($"{Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Images/")}DeserializedBall.jpg", FileMode.CreateNew))
            {
                gridFS.DownloadToStreamByName("deserializedBall.jpg", fs);
            }
        }
        public List<GridFSFileInfo> FindFileAsync()
        {
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("ZaripovImages");
            IGridFSBucket gridFS = new GridFSBucket(database);
            var fileInfos = gridFS.Find(new BsonDocument()).ToList();
            return fileInfos;
        }
    }
}
