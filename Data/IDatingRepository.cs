using System.Collections.Generic;
using System.Threading.Tasks;
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

         Task<IEnumerable<User>> GetUsers();

         Task<User> GetUser(int Id);

    }
}