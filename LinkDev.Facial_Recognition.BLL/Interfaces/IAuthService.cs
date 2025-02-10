using LinkDev.Facial_Recognition.BLL.Helper.Errors;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LinkDev.Facial_Recognition.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<string> CreateTokenAsync(string email, string displayName, IList<string> roles);
    }


}
