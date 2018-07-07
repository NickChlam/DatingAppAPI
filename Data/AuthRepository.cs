using System;
using System.Threading.Tasks;
using DatingApp.API.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        // public async Task<User[]> GetUsers()
        // {
        //     var Users = await _context.Users.ToArrayAsync();
        //     return Users;
        // }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null){
                return null;
            }
            if(!VerfiyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
            //auth succesful 

            return user;
        }

        private bool VerfiyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            //pass in Key   
             using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
           {
             
               var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
               for(int i= 0; i < computedHash.Length; i++)
               {
                   if(computedHash[i] != passwordHash[i]) return false;
               }
           }
           // have matching password! 
           return true;
        }

        public async Task<User> Register(User user, string password)
        {
           byte[] passwordHash, passwordSalt;
           CreatePasswordHash(password, out passwordHash, out passwordSalt);
           user.PasswordHash = passwordHash;
           user.PasswordSalt = passwordSalt;

        // add changes
           await _context.Users.AddAsync(user);
        //save changes to database 
           await _context.SaveChangesAsync();

           return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
           using (var hmac = new System.Security.Cryptography.HMACSHA512())
           {
               passwordSalt = hmac.Key;
               passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
           }


        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == username))
            {
                return true;
            }
            
            return false;
        }

       
    }
}