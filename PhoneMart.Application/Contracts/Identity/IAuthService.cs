using PhoneMart.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneMart.Application.Contracts.Identity
{
    public interface IAuthService
    {
        Task<(string token, Guid userId, UserRole role)> LoginAsync(
            string email,
            string password,
            CancellationToken cancellationToken = default
        );
    }

}
