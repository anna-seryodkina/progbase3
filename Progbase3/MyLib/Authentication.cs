using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace MyLib
{
    public class Authentication
    {
        public SqliteConnection connection;
        public Authentication(SqliteConnection connection)
        {
            this.connection = connection;
        }
        public bool Registration(string login, string password, string fullname)
        {
            UserRepository userRepo = new UserRepository(this.connection);
            if(userRepo.UserExists(login))
            {
                // already exists
                return false;
            }

            User user = new User();
            user.login = login;
            user.fullname = fullname;
            user.id = userRepo.InsertUser(user);

            SHA256 sha256Hash = SHA256.Create();
            string hash = GetPasswordHash(sha256Hash, password);
            userRepo.SetPassword(hash, user.id);
            return true;
        }

        private static string GetPasswordHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public User Login(string login, string password)
        {
            SHA256 sha256Hash = SHA256.Create();
            string hash = GetPasswordHash(sha256Hash, password);

            UserRepository userRepo = new UserRepository(this.connection);
            User user = userRepo.GetUserByLoginPassword(login, hash);

            return user; // null or normal user
        }
    }
}