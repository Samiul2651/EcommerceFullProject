using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Business.Services;
using Contracts.Interfaces;
using Contracts.Models;


var factory = new ConnectionFactory { HostName = "localhost" };
await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "order", durable: false, exclusive: false, autoDelete: false,
    arguments: null);

Console.WriteLine(" [*] Waiting for messages.");
IEmailService emailService = new EmailService();
var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine("Received");
    int dots = message.Split('.').Length - 1;
    await Task.Delay(dots * 1000);

    var order = JsonSerializer.Deserialize<Order>(message);
    emailService.SendMail(order);
    Console.WriteLine(" [x] Done");
};

await channel.BasicConsumeAsync("order", autoAck: true, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
