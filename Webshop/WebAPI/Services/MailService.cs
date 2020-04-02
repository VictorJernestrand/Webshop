using Microsoft.Extensions.Configuration;
using MimeKit; // Nuget package 'MailKit'
using System;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Services
{

    public class MailService
    {
        private readonly IConfiguration _config;
        private readonly CustomerOrderService _customerOrderService;

        public MailService(IConfiguration config, CustomerOrderService customerOrderService)
        {
            this._config = config;
            this._customerOrderService = customerOrderService;
        }

        public async Task<bool> SendOrderConfirmationMailAsync(string recipientEmail, string recipientName, string subject, int orderId)
        {
            string template = Template();
            template = template.Replace("{SUBJECT}", "Orderbekräftelse från RockStart");
            template = template.Replace("{CUSTOMER_NAME}", recipientName);
            template = template.Replace("{CUSTOMER_ORDER}", await BuildOrdersTable(orderId));

            return SendMail(recipientEmail, _config["Mail:Address"], _config["Mail:Name"], recipientName, subject, template, "html");
        }

        private async Task<string> BuildOrdersTable(int orderId)
        {
            // Get order by Id
            var order = await _customerOrderService.CustomerOrderByIdAsync(orderId);

            StringBuilder builder = new StringBuilder();
            builder.Append($"<table>");

            decimal orderTotal = 0;
            foreach (var product in order)
            {
                orderTotal += product.TotalProductCostDiscount;
                builder.Append($"<tr>");
                builder.Append($"<td><a href=\"#\">{product.ProductName}</a></td>");
                builder.Append($"<td>{product.Amount} x {product.Price.ToString("C0")}</td>");
                builder.Append($"<td>{(product.Price * product.Amount).ToString("C0")}</td>");
                builder.Append($"<td>{(orderTotal).ToString("C0")}</td>");
                builder.Append($"</tr>");
            }

            builder.Append($"</table>");
            builder.Append($"<div><b>Totalt:</b> {orderTotal.ToString("C0")}</div>");
            return builder.ToString();
        }

        private string Template()
        {
            return @"<!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset=""utf-8"" />
                        <title>{SUBJECT}</title>
                    </head>
                    <body>
                        <p>Hej {CUSTOMER_NAME}! Detta är en orderbekräftelse på att vi mottagit din order.</p>
                        <p>Din order:</p>
                        {CUSTOMER_ORDER}

                        <p>Tack för att du handlat hos RockStart!</p>
                    </body>
                    </html>";
        }

        /// <summary>
        /// Sends an email based on the method-parameters
        /// </summary>
        /// <param name="toMail">Recipients emaila</param>
        /// <param name="fromMail">Senders email</param>
        /// <param name="senderName">senders name</param>
        /// <param name="recipientName">recipients name</param>
        /// <param name="subject">Email subject</param>
        /// <param name="bodyMessage">Email message</param>
        /// <param name="type">Enum message type: 1=New customer, 2=Forgot Password, 3=OrderConfirmation, 4=Contact Form</param>
        /// <returns>True if successful, else false</returns>
        private bool SendMail(string toMail, string fromMail, string senderName, string recipientName, string subject, string bodyMessage, string type = "text")
        {
            try
            {
                // Create a MailKit object
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, fromMail));
                message.To.Add(new MailboxAddress(recipientName, toMail));
                message.Subject = subject;
                message.Body = new TextPart(type)
                {
                    Text = bodyMessage
                };

                // Send away...
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(_config["Mail:SMTP"], int.Parse(_config["Mail:Port"]), false);
                    client.Authenticate(_config["Mail:Address"], _config["Mail:Password"]);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception)
            {
                // Handle error here, log...
                return false;
            }

            return true;
        }
    }
}
