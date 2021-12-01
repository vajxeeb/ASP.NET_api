using Models.Securites;
using Services.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface IUserService : IGenericRepository<User>
    {
        bool IsValidUserCredentials(string userName, string password);

        string GetUserRole(string userName);
    }
}
