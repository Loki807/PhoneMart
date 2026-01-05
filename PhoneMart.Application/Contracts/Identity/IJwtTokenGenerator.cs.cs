using PhoneMart.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneMart.Application.Contracts.Identity
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid userId, string email, UserRole role);
    }
}
