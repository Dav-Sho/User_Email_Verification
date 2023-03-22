using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User_Email_Verification.Service.Implementation
{
    public class AuthImpl : AuthService
    {
        public Task<ServiceResponse<string>> Login(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<int>> Register(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserExist(User user)
        {
            throw new NotImplementedException();
        }
    }
}