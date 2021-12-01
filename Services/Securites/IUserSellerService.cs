using Models.Securites;
using Services.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Securites
{
    public interface IUserSellerService : IGenericRepository<UserSeller>
    {
        bool IsValidUserCredentials(string deviceCode, string password);

        string UserPasswordChange(string deviceCode, string oldPassword, string newPassword);

        string GetUserRole();
    }
}
