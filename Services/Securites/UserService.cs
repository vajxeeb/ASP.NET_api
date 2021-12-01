using Models.DataContext;
using Models.Securites;
using Services.Repository.Implementation;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class UserService : GenericRepository<User>, IUserService
    {
        private readonly EfDbContext _context;
        public UserService(EfDbContext context) : base(context)
        {
            _context = context;
        }

        public bool IsValidUserCredentials(string userName, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userName)) return false;

                if (string.IsNullOrWhiteSpace(password)) return false;
               
                return _context.User.Any(t => t.UserName == userName && t.Password == password && t.IsActive);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        //public bool IsAnExistingUser(string userName)
        //{
        //    return _users.ContainsKey(userName);
        //}

        public string GetUserRole(string userName)
        {
            //if (!IsAnExistingUser(userName)) return string.Empty;
            //if (userName == "admin") return UserRoles.Admin;
            return UserRoles.BasicUser;
        }
    }

    public static class UserRoles
    {
        public const string Admin = nameof(Admin);
        public const string BasicUser = nameof(BasicUser);
    }
}
