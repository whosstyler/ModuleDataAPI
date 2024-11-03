using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;

namespace backend.Services
{
    public class TokenService
    {
        private const string SecretKey = "d8f83a9d4b7461a3b5cb99d37f129c2fb6a765ae8a2936b94d182cb0fd27ae18"; // Store this securely
        private readonly IMongoCollection<BsonDocument> _tokensCollection;

        public TokenService(IConfiguration config)
        {
            var client = new MongoClient(config.GetSection("MongoDB:ConnectionString").Value);
            var database = client.GetDatabase(config.GetSection("MongoDB:DatabaseName").Value);
            _tokensCollection = database.GetCollection<BsonDocument>(config.GetSection("MongoDB:TokensCollection").Value);
        }

        // Validate the token (check JWT signature and expiration in MongoDB)
        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(SecretKey);

            try
            {
                // Validate the token signature
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // Remove default 5 mins skew
                }, out SecurityToken validatedToken);

                // Check if token is stored in MongoDB and is not expired
                var filter = Builders<BsonDocument>.Filter.Eq("token", token);
                var tokenDoc = _tokensCollection.Find(filter).FirstOrDefault();
                if (tokenDoc == null) return false;

                // Check if token is expired
                var expiration = tokenDoc["expiration"].ToUniversalTime();
                if (DateTime.UtcNow > expiration)
                {
                    // Token is expired, remove it from the database
                    _tokensCollection.DeleteOne(filter);
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Generate a new token for a user
        public string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("hwid", user.HWID), // Add HWID to claims
                new Claim("role", user.Role.ToString()), // Add Role to claims
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "yourdomain.com",
                audience: "yourdomain.com",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token is valid for 1 hour
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Store token and expiration in MongoDB
            AddToken(tokenString, DateTime.UtcNow.AddHours(1));

            return tokenString;
        }

        // Store the token and its expiration in MongoDB
        public void AddToken(string token, DateTime expiration)
        {
            var tokenDoc = new BsonDocument
            {
                { "token", token },
                { "expiration", expiration } // Store the expiration time of the token
            };

            _tokensCollection.InsertOne(tokenDoc);
        }
    }
}
