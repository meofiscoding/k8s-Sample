using System;
using RabbitMQ.Client.Events;
using System.Text;
using RabbitMQ.Client;

namespace dotnetService.RabbitMQ
{
    public abstract class BasicMQHandler : IMQHandler
    {
        private readonly IModel _channel;

        public BasicMQHandler(IModel channel)
        {
            _channel = channel;
        }

        // Template method
        public async Task MQEventHandler(object? sender, BasicDeliverEventArgs args)
        {
            var body = args.Body.ToArray();
            if (body == null)
            {
                await Console.Error.WriteLineAsync(
                    $"Message body is empty. {args.Exchange}/{args.RoutingKey}"
                );
                return;
            }

            var message = Encoding.UTF8.GetString(body);
            if (message == null)
            {
                await Console.Error.WriteLineAsync(
                    $"Message body cannot be parsed into string. {args.Exchange}/{args.RoutingKey}"
                );
                return;
            }

            var success = await HandleMessage(message);

            // Acknowledge the message to safely delete it
            if (success)
            {
                _channel.BasicAck(args.DeliveryTag, false);
            }
            await Task.Yield();
        }

        public abstract Task<bool> HandleMessage(string message);
    }
}

