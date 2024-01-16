using RabbitMQ.Client;
using Serilog;

namespace dotnetService.RabbitMQ
{
    public class EmailHandler : BasicMQHandler
    {
        public EmailHandler(IModel channel) : base(channel)
        {
        }

        public override Task<bool> HandleMessage(string message)
        {
            Console.WriteLine($"Email message received: {message}");
            return Task.FromResult(true);
        }
    }
}

