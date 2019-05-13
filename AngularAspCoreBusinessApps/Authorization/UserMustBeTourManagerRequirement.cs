using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularAspCoreBusinessApps.Authorization
{
    public class UserMustBeTourManagerRequirement : IAuthorizationRequirement
    {
        public string Role { get; private set; }

        public UserMustBeTourManagerRequirement(string role)
        {
            Role = role;
        }
    }

}
