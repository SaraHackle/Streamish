using Streamish.Models;
using System.Collections.Generic;

namespace Streamish.Repositories
{
    public interface IUserProfileRepository
    {
        List<UserProfile> GetAll();
        UserProfile GetById(int id);
        UserProfile GetUserByIdWithVideos(int id);
        void Add(UserProfile user);
        void Update(UserProfile user);
        void Delete(int id);
    }
}