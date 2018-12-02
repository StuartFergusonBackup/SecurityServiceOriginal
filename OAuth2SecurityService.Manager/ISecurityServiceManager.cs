using OAuth2SecurityService.DataTransferObjects;
using System.Threading;
using System.Threading.Tasks;
using OAuth2SecurityService.Manager.DbContexts;

namespace OAuth2SecurityService.Manager
{
    public interface ISecurityServiceManager
    {
        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request, CancellationToken cancellationToken);
    }
}