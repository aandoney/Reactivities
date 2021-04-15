using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;

namespace Infrastructure.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string userEmail, string emailSubject, string message)
        {
            Configuration.Default.AddApiKey("api-key", _config["Sendinblue:Key"]);
            var client = new TransactionalEmailsApi();
            var msg = new SendSmtpEmail(
                            new SendSmtpEmailSender(_config["Sendinblue:User"], _config["Sendinblue:User"]),
                            new List<SendSmtpEmailTo> { new SendSmtpEmailTo(userEmail) },
                            htmlContent: message,
                            textContent: message,
                            subject: emailSubject
                        );

            await client.SendTransacEmailAsync(msg);
        }
    }
}