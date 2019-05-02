using System.Threading;
using System.Threading.Tasks;

namespace OAuth2SecurityService.Manager.Services
{
    using System.Reflection.Metadata;

    public interface IMessagingService
    {
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<SendEmailResponse> SendEmail(SendEmailRequest request, CancellationToken cancellationToken);
    }
}
