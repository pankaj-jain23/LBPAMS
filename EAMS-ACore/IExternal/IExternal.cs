using EAMS_ACore.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.IExternal
{
    public interface IExternal
    {
      
        Task<ServiceResponse> SendSmsAsync(string uname, string password, string senderidstr, string mobileNo, string message, string entityidstr, string templateidstr);
    }
}
