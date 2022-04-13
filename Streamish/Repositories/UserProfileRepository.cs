using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Streamish.Models;
using Streamish.Utils;

namespace Streamish.Repositories
{

    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration configuration) : base(configuration) { }

        public List<Video> Videos { get; private set; }

        public List<UserProfile> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT up.Id AS UserProfileId, up.Name, up.Email, up.DateCreated AS UserProfileDateCreated,
                               up.ImageUrl AS UserProfileImageUrl                       
                        FROM UserProfile up                                                
                        ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        var users = new List<UserProfile>();
                        while (reader.Read())
                        {
                            users.Add(new UserProfile()
                            {
                                Id = DbUtils.GetInt(reader, "UserProfileId"),
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl")
                                
                                
                            });
                        }

                        return users;
                    }
                }
            }
        }

        public UserProfile GetById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                          SELECT up.Id , up.Name, up.Email, 
                                 up.DateCreated AS UserProfileDateCreated,
                                 up.ImageUrl AS UserProfileImageUrl                       
                         FROM UserProfile up                       
                         WHERE Id = @Id
                         ";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        UserProfile user = null;
                        if (reader.Read())
                        {
                            user = new UserProfile()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl")
                                
                            };
                        }

                        return user;
                    }
                }
            }
        }

       
        public UserProfile GetUserByIdWithVideos(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT up.Id, up.Name, up.Email, up.ImageUrl, up.DateCreated,
                                                v.Id as VidId, v.Title, v.Description, v.Url, v.DateCreated as VideoDate, v.UserProfileId,
                                                c.Id as CommentId, c.Message, c.UserProfileId AS CommentUserProfileId, c.VideoId
                                         FROM UserProfile up
                                         LEFT JOIN Video v ON up.Id = v.UserProfileId
                                         lEFT JOIN Comment c ON v.Id = c.VideoId
                                         WHERE up.Id = 1
                                         ";
                    DbUtils.AddParameter(cmd, "@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        UserProfile user = null;
                        while (reader.Read())
                        {
                            var userId = DbUtils.GetInt(reader, "Id");
                            if (user == null)
                            {
                                user = new UserProfile()
                                {
                                    Id = userId,
                                    Name = DbUtils.GetString(reader, "Name"),
                                    Email = DbUtils.GetString(reader, "Email"),
                                    ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                                    DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                                    Videos = new List<Video>()
                                };

                            }
                            if (DbUtils.IsNotDbNull(reader, "VidId"))
                            {
                                var videoId = DbUtils.GetInt(reader, "VidId");
                                var existingVideo = user.Videos.FirstOrDefault(x => x.Id == videoId);

                                if (existingVideo == null)
                                    user.Videos.Add(new Video()
                                    {
                                        Id = videoId,
                                        Title = DbUtils.GetString(reader, "Title"),
                                        Description = DbUtils.GetString(reader, "Description"),
                                        Url = DbUtils.GetString(reader, "Url"),
                                        DateCreated = DbUtils.GetDateTime(reader, "VideoDate"),
                                        UserProfileId = userId,
                                        Comments = new List<Comment>()



                                    });



                            }
                            foreach (var video in user.Videos)
                            {
                                if (DbUtils.IsNotDbNull(reader, "CommentId"))
                                    video.Comments.Add(new Comment()
                                    {
                                        Id = DbUtils.GetInt(reader, "CommentId"),
                                        Message = DbUtils.GetString(reader, "Message"),
                                        VideoId = video.Id,
                                        UserProfileId = DbUtils.GetInt(reader, "CommentUserprofileId")

                                    });
                            }

                        }
                        return user;

                    }
                }
            }
        }

        public void Add(UserProfile user)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO UserProfile (Name, Email, DateCreated, ImageUrl)
                        OUTPUT INSERTED.ID
                        VALUES (@Name, @Email, @DateCreated, @ImageUrl)";

                    DbUtils.AddParameter(cmd, "@Name", user.Name);
                    DbUtils.AddParameter(cmd, "@Email", user.Email);
                    DbUtils.AddParameter(cmd, "@DateCreated", user.DateCreated);
                    DbUtils.AddParameter(cmd, "@ImageUrl", user.ImageUrl);
                    

                    user.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(UserProfile user)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE UserProfile
                           SET [Name] = @Name,
                               Email = @Email,
                               ImageUrl = @ImageUrl
                               
                         WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@Name", user.Name);
                    DbUtils.AddParameter(cmd, "@Email", user.Email);
                    DbUtils.AddParameter(cmd, "@ImageUrl", user.ImageUrl);
                    DbUtils.AddParameter(cmd, "@Id", user.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM UserProfile WHERE Id = @Id";
                    DbUtils.AddParameter(cmd, "@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

