using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YounaSchool.Authentication.Domain.Enums;

public enum SessionStatus
{
    Active = 1,
    Revoked = 2,
    Expired = 3
}