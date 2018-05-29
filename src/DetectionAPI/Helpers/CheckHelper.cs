using DetectionAPI.Database;
using DetectionAPI.Database.Entities;
using System;
using System.Linq;

namespace DetectionAPI.Helpers
{
    public static class CheckHelper
    {
        /// <summary>
        /// Checks last session by userId and update it if expiried already, then creates new session
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Current session id</returns>
        public static long CheckExpirySessionByUserId(long userId)
        {
            using (var dbContext = new ApiDbContext())
            {
                var userByUserId = dbContext.Set<User>().Where(p => p.Id == userId).ToList().LastOrDefault();
                var sessionByUserId = dbContext.Set<Session>().Where(p => p.UserId == userId).ToList().LastOrDefault();

                if (sessionByUserId.ExpiryDate < DateTime.Now)
                {
                    var newSession = new Session
                    {
                        CreationTime = DateTime.Now,
                        ExpiryDate = DateTime.Now.AddMonths(1),
                        ImageCount = 0,
                        IsLimitReached = false,
                        PlatesCount = 0,
                        SessionType = userByUserId.UserType,
                        UserId = userByUserId.Id
                    };

                    dbContext.Sessions.Add(newSession);
                    dbContext.SaveChanges();

                    var createdSession = dbContext.Set<Session>().Where(p => p.UserId == userId).ToList().LastOrDefault();
                    userByUserId.SessionId = createdSession.Id;
                    dbContext.SaveChanges();
                }

                sessionByUserId = dbContext.Set<Session>().Where(p => p.UserId == userId).ToList().LastOrDefault();

                return sessionByUserId.Id;

            }
        }

        /// <summary>
        /// Check session on limits and sets flags if limit is reached
        /// </summary>
        /// <param name="userId"></param>
        /// <returns><see cref="AvailableLimits"/></returns>
        public static AvailableLimits CheckLimitByUserId(long userId)
        {
            var availableLimits = new AvailableLimits
            {
                ImagesLimit = LimitValues.ImageCountLimit,
                PlatesLimit = LimitValues.PlatesCountLimit,
                IsLimitReached = false
            };

            using (var dbContext = new ApiDbContext())
            {
                var sessionByUserId = dbContext.Set<Session>().Where(p => p.UserId == userId).ToList().LastOrDefault();

                availableLimits.CurrentImagesCount = sessionByUserId.ImageCount;
                availableLimits.CurrentPlatesCount = sessionByUserId.PlatesCount;
                availableLimits.CurrentPlan = sessionByUserId.SessionType;

                if (sessionByUserId.IsLimitReached == true)
                {
                    availableLimits.IsLimitReached = true;
                    return availableLimits;
                }

                if (sessionByUserId.PlatesCount >= LimitValues.PlatesCountLimit || sessionByUserId.ImageCount >= LimitValues.ImageCountLimit)
                {
                    sessionByUserId.IsLimitReached = true;
                    dbContext.SaveChanges();

                    availableLimits.IsLimitReached = true;
                    return availableLimits;
                }

                return availableLimits;
            }
        }
    }
}
