using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;
using backend.Models;

namespace backend.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UserService(IConfiguration config)
        {
            var client = new MongoClient(config.GetSection("MongoDB:ConnectionString").Value);
            var database = client.GetDatabase(config.GetSection("MongoDB:DatabaseName").Value);
            _usersCollection = database.GetCollection<User>(config.GetSection("MongoDB:UsersCollection").Value);
        }

        public void CreateUser(User user)
        {
            _usersCollection.InsertOne(user);
        }

        public User GetUser(string username)
        {
            return _usersCollection.Find(u => u.Username == username).FirstOrDefault();
        }

        public bool UserExists(string username)
        {
            return _usersCollection.Find(u => u.Username == username).Any();
        }

        public void UpdateUserToken(User user)
        {
            var update = Builders<User>.Update
                .Set(u => u.Token, user.Token)
                .Set(u => u.TokenExpiration, DateTime.UtcNow.AddHours(1)); // Token valid for 1 hour

            _usersCollection.UpdateOne(u => u.Username == user.Username, update);
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            var enteredPasswordHash = HashPassword(enteredPassword);
            return enteredPasswordHash == storedPasswordHash;
        }
    }
}
