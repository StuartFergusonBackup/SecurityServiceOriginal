namespace SecurityService.Manager.Services
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Shared.General;

    public class MessagingService : ClientProxyBase, IMessagingService
    {
        #region Public Methods

        #region public async Task<SendEmailResponse> SendEmail(SendEmailRequest request, CancellationToken cancellationToken)        
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<SendEmailResponse> SendEmail(SendEmailRequest request, CancellationToken cancellationToken)
        {
            SendEmailResponse response = null;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationReader.GetValue("ServiceAddresses","MessagingService"));

                Logger.LogInformation(client.BaseAddress.ToString());

                String requestSerialised = JsonConvert.SerializeObject(request);
                StringContent httpContent = new StringContent(requestSerialised, Encoding.UTF8, "application/json");
                
                try
                {  
                    var httpResponse  = await client.PostAsync("/api/Email", httpContent, CancellationToken.None);
                                  
                    String content = await this.HandleResponse(httpResponse, cancellationToken);

                    response = JsonConvert.DeserializeObject<SendEmailResponse>(content);
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }

            return response;
        }
        #endregion

        #endregion
    }
}