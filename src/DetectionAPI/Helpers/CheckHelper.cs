using DetectionAPI.Database;
using DetectionAPI.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectionAPI.Helpers
{
    public static class CheckHelper
    {
        /// <summary>
        /// Checks last session by userId and update it if expiried already, then creates new session
        /// </summary>
        /// <param name="userId"></param>
        public static void CheckExpirySessionByUserId(long userId)
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
            }
        }

        /// <summary>
        /// Checks last session by userId on detection limits and sets IsLimitReached on current session
        /// in case of limit is reached already
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>true : if at least one of limits is reached</returns>
        //public bool CheckLimitByUserId(long userId)
        //{
        //    using (var dbContext = new ApiDbContext())
        //    {
        //        var sessionByUserId = dbContext.Set<Session>().Where(p => p.UserId == userId).ToList().LastOrDefault();

        //        if (sessionByUserId.IsLimitReached == true)
        //        {
        //            return true;
        //        }

        //        if (sessionByUserId.PlatesCount >= LimitValues.PlatesCountLimit || sessionByUserId.ImageCount >= LimitValues.ImageCountLimit)
        //        {
        //            sessionByUserId.IsLimitReached = true;
        //            dbContext.SaveChanges();
        //            return true;
        //        }

        //        return false;
        //    }
        //}


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
                availableLimits.IsLimitReached = true;

                if (sessionByUserId.IsLimitReached == true)
                {
                    return availableLimits;
                }

                if (sessionByUserId.PlatesCount >= LimitValues.PlatesCountLimit || sessionByUserId.ImageCount >= LimitValues.ImageCountLimit)
                {
                    sessionByUserId.IsLimitReached = true;
                    dbContext.SaveChanges();

                    return availableLimits;
                }

                return availableLimits;
            }
        }
    }
}
