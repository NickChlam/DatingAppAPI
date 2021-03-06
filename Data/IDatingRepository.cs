using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.models;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
        //instead of having multiple add methods ( generics) 
        //take amy class and add entries to repository 
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         Task<bool> SaveAll();

         Task<PagedList<User>> GetUsers(UserParams userParams);

         Task<User> GetUser(int Id);

         Task<Photo> GetPhoto(int Id);

         Task<Photo> GetMainPhotoForUser(int userId);

    }
}