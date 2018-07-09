using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class Seeds
    {
         private readonly DataContext _context;

        public Seeds(DataContext context)
        {
            _context = context;
        }

        public void seedUsers()
        {
            // _context.Users.RemoveRange(_context.Users);
            // _context.SaveChanges();

            // seed users

            var userData = System.IO.File.ReadAllText("Data/male.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);
            foreach(var user in users)
            {
                //create password hash and password salt
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash("password", out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.UserName = user.UserName.ToLower();

                _context.Users.Add(user);
                             
            }

            _context.SaveChanges();

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
           using (var hmac = new System.Security.Cryptography.HMACSHA512())
           {
               passwordSalt = hmac.Key;
               passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
           }


        }


    
    }
}
