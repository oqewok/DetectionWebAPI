using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                    CurrentSessionId = 0,
                    Password = "api_pass1234q",
                    Username = "api_user1234q",
                    UserType = 2
                },

                new User
                {
                    AccessToken = "test_api_token1234_2",
                    CreationTime = DateTime.Now,
                    CurrentSessionId = 1,
                    Password = "api_pass1234q_2",
                    Username = "api_user1234q_2",
                    UserType = 0
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            base.Seed(context);
        }
    }
}
