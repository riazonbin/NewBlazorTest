using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewBlazorTest.Data;

namespace NewBlazorTest.Services
{
    public class MongoDBConnection
    {
        public User? currentUser;

        public void AddToDataBase(User user)
        {
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("NewBlazorTestZaripov");
            var collection = database.GetCollection<User>("UserCollection");
            collection.InsertOne(user);
        }

        public User FindByLogin(string login)
        {
            var client = new MongoClient("mongodb://localhost");
            var filter = Builders<User>.Filter.Eq("Login", login);
            var database = client.GetDatabase("NewBlazorTestZaripov");
            var collection = database.GetCollection<User>("UserCollection");
            return collection.Find(filter).FirstOrDefault();
        }


        public IMongoCollection<User> GetCollection()
        {
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("NewBlazorTestZaripov");

            return  database.GetCollection<User>("UserCollection");
        }
    }
}
