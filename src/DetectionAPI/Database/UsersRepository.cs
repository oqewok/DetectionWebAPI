using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DetectionAPI.Database;
using DetectionAPI.Database.Entities;

namespace DbAndLogging.Database
{
    public class UsersRepository
    {
        #region .ctor

        public UsersRepository(ApiDbContext context)
        {
            Context = context;
        }

        #endregion

        #region Properties

        ApiDbContext Context { get; set; }

        #endregion

        #region Methods

        public bool AddRecord(User user)
        {
            var usernameFilter = new UsersRepositoryUsernamePasswordFilter
            {
                Username = user.Username,
                Password = user.Password
            };

            if (FetchRecords(usernameFilter).Count != 0)
            {
                return false;
            }

            Context
                .Set<User>()
                .Add(user);

            Context.SaveChanges();

            return true;
        }

        public IReadOnlyList<UserRecord> FetchRecords()
        {
            var records = new List<UserRecord>();

            var users = Context
                .Set<User>()
                .ToList();

            foreach (var u in users)
            {
                records.Add(
                    new UserRecord
                    {
                        AccessToken = u.AccessToken,
                        CreationTime = u.CreationTime,
                        CurrentSessionId = u.SessionId,
                        Password = u.Password,
                        UserId = u.Id,
                        Username = u.Username,
                        UserType = u.UserType
                    });
            }

            return records;

        }

        public IReadOnlyList<UserRecord> FetchRecords(UsersRepositoryIdFiler filter)
        {
            var records = new List<UserRecord>();

            var users = Context
                .Set<User>()
                .Where(e => e.Id == filter.UserId)
                .ToList();

            foreach(var u in users)
            {
                records.Add(
                    new UserRecord
                    {
                        AccessToken = u.AccessToken,
                        CreationTime = u.CreationTime,
                        CurrentSessionId = u.SessionId,
                        Password = u.Password,
                        UserId = u.Id,
                        Username = u.Username,
                        UserType = u.UserType
                    });
            }

            return records;

        }

        public IReadOnlyList<UserRecord> FetchRecords(UsersRepositoryUsernamePasswordFilter filter)
        {
            var records = new List<UserRecord>();

            var users = Context
                .Set<User>()
                .Where(e => e.Username == filter.Username)
                .Where(e => e.Password == filter.Password)
                .ToList();

            foreach (var u in users)
            {
                records.Add(
                    new UserRecord
                    {
                        AccessToken = u.AccessToken,
                        CreationTime = u.CreationTime,
                        CurrentSessionId = u.SessionId,
                        Password = u.Password,
                        UserId = u.Id,
                        Username = u.Username,
                        UserType = u.UserType
                    });
            }

            return records;

        }

        public IReadOnlyList<UserRecord> FetchRecords(UsersRepositoryTokenFilter filter)
        {
            var records = new List<UserRecord>();

            var users = Context
                .Set<User>()
                .Where(e => e.AccessToken == filter.Token)
                .ToList();

            foreach (var u in users)
            {
                records.Add(
                    new UserRecord
                    {
                        AccessToken = u.AccessToken,
                        CreationTime = u.CreationTime,
                        CurrentSessionId = u.SessionId,
                        Password = u.Password,
                        UserId = u.Id,
                        Username = u.Username,
                        UserType = u.UserType
                    });
            }

            return records;

        }



        #endregion

        public class UserRecord
        {
            #region Properties

            public long UserId { get; set; }

            public long UserType { get; set; }

            public DateTime CreationTime { get; set; }

            public string Username { get; set; }

            public string Password { get; set; }

            public string AccessToken { get; set; }

            public long CurrentSessionId { get; set; }

            #endregion

            public override string ToString() => $@"Id :{UserId}, User type: {UserType}, Created: {CreationTime}, Username: {Username}, Password: {Password}, AccessToken: {AccessToken}, SeesionId: {CurrentSessionId}";
        }


        public class UsersRepositoryIdFiler
        {
            public long UserId { get; set; }
        }

        public class UsersRepositoryUsernamePasswordFilter
        {
            public string Username { get; set; }

            public string Password { get; set; }
        }

        public class UsersRepositoryTokenFilter
        {
            public string Token { get; set; }
        }
    }
}
