namespace SecurityService.Manager.Services
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    public class DummyMessagingService : IMessagingService
    {
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<SendEmailResponse> SendEmail(SendEmailRequest request,
                                                       CancellationToken cancellationToken)
        {
            return new SendEmailResponse
                   {
                       ApiStatusCode = HttpStatusCode.OK,
                       EmailId = Guid.NewGuid().ToString(),
                       Error = String.Empty,
                       ErrorCode = String.Empty,
                       RequestId = Guid.NewGuid().ToString()
                   };
        }
    }
}