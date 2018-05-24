using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using DetectionAPI.Database.Entities;

namespace DetectionAPI.Database.Entities
{
    public class ApiDbContextInitializer : CreateDatabaseIfNotExists<ApiDbContext>
    {
        protected override void Seed(ApiDbContext context)
        {
            var users = new[]
            {
                new User
                {
                    AccessToken = "test_api_token1234",
                    CreationTime = DateTime.Now,
                    SessionId = 1,
                    Password = "api_pass1234q",
                    Username = "api_user1234q",
                    UserType = 2,
                },

                new User
                {
                    AccessToken = "test_api_token1234_2",
                    CreationTime = DateTime.Now,
                    SessionId = 2,
                    Password = "api_pass1234q_2",
                    Username = "api_user1234q_2",
                    UserType = 0
                }
            };

            var sessions = new[]
            {
                new Session{
                    CreationTime = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddMonths(1),
                    ImageCount = 0,
                    IsLimitReached = false,
                    PlatesCount = 0,
                    SessionType = 2,
                    UserId = 1,
                    User = users[0]
                },

                new Session{
                    CreationTime = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddMonths(1),
                    ImageCount = 0,
                    IsLimitReached = false,
                    PlatesCount = 0,
                    SessionType = 0,
                    UserId = 2,
                    User = users[1]
                },

            };

            var images = new[]
            {
                new ImageInfo
                {
                    ImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Images"),
                    MarkupPath = Path.Combine(Directory.GetCurrentDirectory(), "Markup"),
                    PlatesCount = 1,
                    Session = sessions[0],
                    UploadDate = DateTime.Now,
                    SessionId = sessions[0].Id,
                    UserId = 1
                },

                new ImageInfo
                {
                    ImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Images"),
                    MarkupPath = Path.Combine(Directory.GetCurrentDirectory(), "Markup"),
                    PlatesCount = 2,
                    Session = sessions[0],
                    UploadDate = DateTime.Now,
                    SessionId = sessions[0].Id,
                    UserId = 1
                },
            };



            context.Users.AddRange(users);
            context.Sessions.AddRange(sessions);
            context.Images.AddRange(images);
            context.SaveChanges();

            base.Seed(context);
        }
    }
}
