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
            //template = template.Replace("{SUBJECT}", "Orderbekräftelse från RockStart");
            template = template.Replace("{CUSTOMER_NAME}", recipientName);
            template = template.Replace("{CUSTOMER_ORDER}", await BuildOrdersTable(orderId));
            template = template.Replace("{CUSTOMER_ORDER}", await BuildOrdersTable(orderId));

            return SendMail(recipientEmail, _config["Mail:Address"], _config["Mail:Name"], recipientName, subject, template, "html");
        }

        private async Task<string> BuildOrdersTable(int orderId)
        {
            // Get order by Id
            var order = await _customerOrderService.CustomerOrderByIdAsync(orderId);

            StringBuilder builder = new StringBuilder();

            decimal orderTotal = 0;
            foreach (var product in order)
            {
                var ProductCost = product.Amount * product.TotalProductCostDiscount;
                orderTotal += ProductCost;

                builder.Append($@"<tr style=""background-color: #eee; height: 3em"">");
                builder.Append($@"<td valign=""top""><a href=""https://localhost:44364/Product/ProductDetail/" + product.ProductId + $@""">{product.ProductName}</a></td>");
                builder.Append($@"<td valign=""top"">

                    {product.Amount} x {product.Price.ToString("C0")}<br />
                    <span style=""color: green; font-size: small"">
                    { ((product.Discount > 0) ? "-" + (product.Discount * 100).ToString("0") + "% Rabatt" : "")}
                    </span>

                </td>");
                builder.Append($@"<td valign=""top"" align=""right"">{(ProductCost).ToString("C0")}</td>");
                builder.Append($"</tr>");
            }

            builder.Append($@"<tr>
                    <td colspan=""3"" align=""right"" style=""padding-top: 30px; font-weight: bold"">TOTALT:
                        <span style=""color: #fff; padding: 10px 20px 10px 20px; border-radius: 4px; background: #f78300;"">{orderTotal.ToString("C0")}</span>
                    </td>
                </tr>");

            return builder.ToString();
        }

        private string Template()
        {


            return @"<!DOCTYPE html>
                <html>
                <head>
                    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                    <title>OrderBekräftelse - RockStart</title>
                </head>
                <body>
                    <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                        <tr>
                            <td align=""center"" style=""padding: 30px; background-color: #262b2e; color: #fff; font-size: 2.0em;"">RockStart</td>
                        </tr>
                        <tr>
                            <td style=""padding: 30px"">
                                <table width=""100%"">
                                    <tr>
                                        <td style=""padding: 10px 0px 10px 0px"">
                                            <p>Hej {CUSTOMER_NAME}!</p>
                                            <p>Detta är en bekräftelse på att vi mottagit din beställning.<br />Följ din order på <a href=""https://localhost:44364/UserOrder"">mina sidor</a>.</p>
                                            <p>Tack för att du handlat hos RockStart!</p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align=""center"">
                                            <table width=""100%"" cellpadding=""2"" cellspacing=""2"">
                                                <tr style=""background-color: #262b2e; color: #fff"">
                                                    <th style=""padding: 10px"">Produkt</th>
                                                    <th style=""padding: 10px"">Antal</th>
                                                    <th style=""padding: 10px"">Ditt Pris</th>
                                                </tr>
                                                {CUSTOMER_ORDER}
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style=""padding: 20px 0px 30px 0px"">
                                            <a href=""https://localhost:44364/"">RockStart</a> | <a href=""https://localhost:44364/Contact"">Kontakta Oss</a>
                                        </td>
                                    </tr>
                                </table>

                            </td>
                        </tr>
                    </table>
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
