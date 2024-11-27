using Contracts.Interfaces;
using Contracts.Models;
using DotLiquid;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Dynamic;
using System.Reflection;

namespace Business.Services
{
    public static class CustomFilters
    {
        public static decimal Multiply(decimal a, decimal b)
        {
            return a*b;
        }
    }
    public class EmailService : IEmailService
    {

        public async void SendMail(Order order)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("istiaqsamiul@gmail.com", "MyStore");
            var subject = "Your Order has been Placed";
            var to = new EmailAddress(order.Email, "");
            var plainTextContent = GeneratePlainTextEmail(order);
            var htmlContent = await GetHtml(order);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            Console.WriteLine(response.StatusCode);
        }

        public async Task<string> GetHtml(Order order)
        {
            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "EmailTemplate.liquid");
            string templateContent = await File.ReadAllTextAsync(filePath);
            var orderJson = JsonConvert.SerializeObject(order);
            Template.RegisterFilter(typeof(CustomFilters));
            var actualPayload = ConvertToDictionary(JsonConvert.DeserializeObject<ExpandoObject>(orderJson));
            Template template = Template.Parse(templateContent);
            var renderedText = template.Render(Hash.FromAnonymousObject(new { Order = actualPayload }));
            Console.WriteLine(renderedText);
            return renderedText;
        }

        public static Dictionary<string, object> ConvertToDictionary(ExpandoObject payload)
        {
            if (payload == null)
            {
                return null;
            }
            var dictionary = new Dictionary<string, object>();
            foreach (var item in payload)
            {
                if (item.Value is ExpandoObject o)
                {
                    var newItem = ConvertToDictionary(o);
                    dictionary.Add(item.Key, newItem);
                }
                else if (item.Value is List<object> items)
                {
                    var list = new List<object>();

                    foreach (var listItem in items)
                    {
                        if (listItem is ExpandoObject expandoObject)
                        {
                            var newItem = ConvertToDictionary(expandoObject);
                            list.Add(newItem);
                        }
                        else
                        {
                            list.Add(listItem);
                        }
                    }
                    dictionary.Add(item.Key, list);
                }
                else
                {
                    dictionary.Add(item.Key, item.Value);
                }
            }
            return dictionary;
        }

        public string GeneratePlainTextEmail(Order order)
        {
            var productLines = string.Join("\n", order.Products.Select(p =>
                $"Product: {p.Name}\nPrice: {p.Price} x {p.Quantity} = {p.Price * p.Quantity}\nCategory: {p.Category}\n"));

            string plainText = $@"
Order Confirmation

Order ID: {order.Id}
Customer ID: {order.CustomerId}
Order Time: {order.OrderTime}
Total Price: {order.Price}

Shipping Address:
{order.Address}
Phone: {order.PhoneNumber}

Products:
{productLines}

Thank you for your purchase!
";
            return plainText;
        }
    }

    
}
