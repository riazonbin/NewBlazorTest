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
    public static class MongoDBConnection
    {
        public static User? currentUser;

        public static void AddToDataBase(User user)
        {
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("NewBlazorTestZaripov");
            var collection = database.GetCollection<User>("UserCollection");
            collection.InsertOne(user);
        }

        public static User FindByLogin(string login)
        {
            var client = new MongoClient("mongodb://localhost");
            var filter = Builders<User>.Filter.Eq("Login", login);
            var database = client.GetDatabase("NewBlazorTestZaripov");
            var collection = database.GetCollection<User>("UserCollection");
            return collection.Find(filter).FirstOrDefault();
        }


        public static IMongoCollection<User> GetCollection()
        {
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("NewBlazorTestZaripov");

            return  database.GetCollection<User>("UserCollection");
        }
    }
}
