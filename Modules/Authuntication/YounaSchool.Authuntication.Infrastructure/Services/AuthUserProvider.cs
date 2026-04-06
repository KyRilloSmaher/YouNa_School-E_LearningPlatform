using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YounaSchool.Authuntication.Infrastructure.Persistence;
using YouNaSchool.Wallet.Application.Abstractions.OtherModules;

namespace YounaSchool.Authuntication.Infrastructure.Services
{
    public sealed class AuthUserProvider : IAuthUserProvider
    {
        private readonly AuthDbContext _context;

        public AuthUserProvider(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<string?> GetUserEmailAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Where(x => x.Id == userId)
                .Select(x => x.Email)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
