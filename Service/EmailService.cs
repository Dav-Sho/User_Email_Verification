using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User_Email_Verification.Service
{
    public interface EmailService
    {
        void SendEmail(Message message);
        
        
    }
}