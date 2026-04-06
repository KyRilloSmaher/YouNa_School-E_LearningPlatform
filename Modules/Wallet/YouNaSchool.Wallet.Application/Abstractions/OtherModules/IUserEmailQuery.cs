using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Wallet.Application.Abstractions.OtherModules
{
    public interface IAuthUserProvider
    {
        Task<string?> GetUserEmailAsync(Guid userId, CancellationToken cancellationToken);
    }
}
