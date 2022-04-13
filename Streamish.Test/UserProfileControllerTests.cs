using Streamish.Controllers;
using Streamish.Models;
using Streamish.Tests.Mocks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Streamish.Tests
{
    public class UserProfileControllerTests
    {
        [Fact]
        public void Get_Returns_All_Users()
        {
            // Arrange 
            var userCount = 20;
            var users = CreateTestUsers(userCount);

            var repo = new InMemoryUserProfileRepository(users);
            var controller = new UserProfileController(repo);

            // Act 
            var result = controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualUsers = Assert.IsType<List<UserProfile>>(okResult.Value);

            Assert.Equal(userCount, actualUsers.Count);
            Assert.Equal(users, actualUsers);
        }

        [Fact]
        public void Get_By_Id_Returns_NotFound_When_Given_Unknown_id()
        {
            // Arrange 
            var users = new List<UserProfile>(); // no videos

            var repo = new InMemoryUserProfileRepository(users);
            var controller = new UserProfileController(repo);

            // Act
            var result = controller.Get(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Get_By_Id_Returns_User_With_Given_Id()
        {
            // Arrange
            var testUserId = 99;
            var users = CreateTestUsers(5);
            users[0].Id = testUserId; // Make sure we know the Id of one of the users

            var repo = new InMemoryUserProfileRepository(users);
            var controller = new UserProfileController(repo);

            // Act
            var result = controller.Get(testUserId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualUser = Assert.IsType<UserProfile>(okResult.Value);

            Assert.Equal(testUserId, actualUser.Id);
        }

        [Fact]
        public void Post_Method_Adds_A_New_User()
        {
            // Arrange 
            var userCount = 20;
            var users = CreateTestUsers(userCount);

            var repo = new InMemoryUserProfileRepository(users);
            var controller = new UserProfileController(repo);

            // Act
            var newUser = new UserProfile()
            {
                Name = "Name",
                Email = "Email",
                ImageUrl = "http://user.url/",
                DateCreated = DateTime.Today,
                
            };

            controller.Post(newUser);

            // Assert
            Assert.Equal(userCount + 1, repo.InternalData.Count);
        }

        [Fact]
        public void Put_Method_Returns_BadRequest_When_Ids_Do_Not_Match()
        {
            // Arrange
            var testUserId = 99;
            var users = CreateTestUsers(5);
            users[0].Id = testUserId; // Make sure we know the Id of one of the users

            var repo = new InMemoryUserProfileRepository(users);
            var controller = new UserProfileController(repo);

            var userToUpdate = new UserProfile()
            {
                Id = testUserId,
                Name = "Updated!",
                Email = "Updated!",
                DateCreated = DateTime.Today,
                ImageUrl = "http://some.url",
            };
            var someOtherUserId = testUserId + 1; // make sure they aren't the same

            // Act
            var result = controller.Put(someOtherUserId, userToUpdate);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Put_Method_Updates_A_User()
        {
            // Arrange
            var testUserId = 99;
            var users = CreateTestUsers(5);
            users[0].Id = testUserId; // Make sure we know the Id of one of the users

            var repo = new InMemoryUserProfileRepository(users);
            var controller = new UserProfileController(repo);

            var userToUpdate = new UserProfile()
            {
                Id = testUserId,
                Name = "Updated!",
                Email = "Updated!",
                DateCreated = DateTime.Today,
                ImageUrl = "http://some.url",
            };

            // Act
            controller.Put(testUserId, userToUpdate);

            // Assert
            var userFromDb = repo.InternalData.FirstOrDefault(p => p.Id == testUserId);
            Assert.NotNull(userFromDb);

            Assert.Equal(userToUpdate.Name, userFromDb.Name);
            Assert.Equal(userToUpdate.Email, userFromDb.Email);
            Assert.Equal(userToUpdate.DateCreated, userFromDb.DateCreated);
            Assert.Equal(userToUpdate.ImageUrl, userFromDb.ImageUrl);
        }

        [Fact]
        public void Delete_Method_Removes_A_User()
        {
            // Arrange
            var testUserId = 99;
            var users = CreateTestUsers(5);
            users[0].Id = testUserId; // Make sure we know the Id of one of the users

            var repo = new InMemoryUserProfileRepository(users);
            var controller = new UserProfileController(repo);

            // Act
            controller.Delete(testUserId);

            // Assert
            var userFromDb = repo.InternalData.FirstOrDefault(p => p.Id == testUserId);
            Assert.Null(userFromDb);
        }

        private List<UserProfile> CreateTestUsers(int count)
        {
            var users = new List<UserProfile>();
            for (var i = 1; i <= count; i++)
            {
                users.Add(new UserProfile()
                {
                    Id = i,
                    Name = $"Name {i}",
                    Email = $"Email {i}",
                    ImageUrl = $"http://user.url/",
                    DateCreated = DateTime.Today.AddDays(-i),
                    
                });
            }
            return users;
        }

        private UserProfile CreateTestUserProfile(int id)
        {
            return new UserProfile()
            {
                Id = id,
                Name = $"User {id}",
                Email = $"user{id}@example.com",
                DateCreated = DateTime.Today.AddDays(-id),
                ImageUrl = $"http://user.url/{id}",
            };
        }
    }
}

