using System;
using System.Collections.Generic;
using System.Linq;
using Streamish.Models;
using Streamish.Repositories;

namespace Streamish.Tests.Mocks
{
    class InMemoryUserProfileRepository : IUserProfileRepository
    {
        private readonly List<UserProfile> _data;

        public List<UserProfile> InternalData
        {
            get
            {
                return _data;
            }
        }

        public InMemoryUserProfileRepository(List<UserProfile> startingData)
        {
            _data = startingData;
        }

        public void Add(UserProfile user)
        {
            var lastUser = _data.Last();
            user.Id = lastUser.Id + 1;
            _data.Add(user);
        }

        public void Delete(int id)
        {
            var userToDelete = _data.FirstOrDefault(p => p.Id == id);
            if (userToDelete == null)
            {
                return;
            }

            _data.Remove(userToDelete);
        }

        public List<UserProfile> GetAll()
        {
            return _data;
        }

        public UserProfile GetById(int id)
        {
            return _data.FirstOrDefault(p => p.Id == id);
        }

        public void Update(UserProfile user)
        {
            var currentUser = _data.FirstOrDefault(p => p.Id == user.Id);
            if (currentUser == null)
            {
                return;
            }

            currentUser.Name = user.Name;
            currentUser.Email = user.Email;
            currentUser.DateCreated = user.DateCreated;
            currentUser.ImageUrl = user.ImageUrl;
           
        }

        public List<UserProfile> Search(string criterion, bool sortDescending)
        {
            throw new NotImplementedException();
        }

        public List<UserProfile> GetAllWithComments()
        {
            throw new NotImplementedException();
        }

        public UserProfile GetVideoByIdWithComments(int id)
        {
            throw new NotImplementedException();
        }
    }
}
