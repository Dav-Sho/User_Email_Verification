using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace User_Email_Verification.Service
{
    public interface AuthService
    {
      Task<ServiceResponse<int>> Register(User user);
      Task<ServiceResponse<string>> Login(string email, string password);
      Task<bool> UserExist(User user);
    }
}