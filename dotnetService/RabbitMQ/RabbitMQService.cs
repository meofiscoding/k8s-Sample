using System;
using Newtonsoft.Json;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace dotnetService.RabbitMQ
{
    public class RabbitMQService
    {
        public IModel MsgChannel { get; set; }

        private readonly IConnection _connection;

        // RabbitMQ exchange and queue settings
        public const string DEFAULT_EXCHANGE_NAME = "x.default-registration";

        public const string SEND_EMAIL_QUEUE_NAME = "q.send-email";
        public const string SEND_EMAIL_ROUTE = "send-email";

        public const string SEND_SMS_QUEUE = "q.send-sms";
        public const string SEND_SMS_ROUTE = "send-sms";

        public const string CHECK_OUT_QUEUE = "q.checkout";
        public const string CHECKOUT_ROUTE = "checkout";

        public RabbitMQService()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var config = builder.Build();

            var username = config.GetSection("RabbitMQ:Username").Value;
            var password = config.GetSection("RabbitMQ:Password").Value;
            var hostname = config.GetSection("RabbitMQ:Hostname").Value;

            var isPortValid = int.TryParse(config.GetSection("RabbitMQ:Port").Value, out var port);
            if (username == null || password == null ||
                hostname == null || !isPortValid)
            {
                throw new Exception("Cannot read RabbitMQ settings in appsettings");
            }

            var connectConfig = new ConnectionFactory
            {
                //UserName = username,
                //Password = password,
                HostName = hostname,
                //Port = port,
                DispatchConsumersAsync = true
            };

            // Using multiple endpoints if has many nodes (primary, secondary, ...)
            //var endpoints = new List<AmqpTcpEndpoint> {
            //    new AmqpTcpEndpoint("localhost")
            //};

            _connection = connectConfig.CreateConnection();

            // Setup channel
            MsgChannel = SetupChannel();

            // Setup custom exchange
            SetupDefaultExchange();
        }

        // Documents
        // Send The Message
        // https://rabbitmq.github.io/rabbitmq-dotnet-client/api/RabbitMQ.Client.IModel.html
        public void PublishMessage<T>(T message, string queueName)
        {
            string messageStr = JsonConvert.SerializeObject(message);
            byte[] messageBytes = Encoding.UTF8.GetBytes(messageStr);

            var properties = GetDefaultProperties();
            MsgChannel.BasicPublish(
                DEFAULT_EXCHANGE_NAME,
                queueName,
                properties,
                messageBytes
            );
        }

        public string SubscribeToQueue(AsyncEventHandler<BasicDeliverEventArgs> handler, string queueName)
        {
            var consumer = new AsyncEventingBasicConsumer(MsgChannel);
            consumer.Received += handler;

            var cancelToken = MsgChannel.BasicConsume(queueName, false, consumer);
            return cancelToken;
        }

        private void SetupDefaultExchange()
        {
            MsgChannel.ExchangeDeclare(DEFAULT_EXCHANGE_NAME, ExchangeType.Direct, true);
        }

        private IModel? SetupChannel()
        {
            var channel = _connection.CreateModel();
            return channel;
        }

        // Documents
        // https://rabbitmq.github.io/rabbitmq-dotnet-client/api/RabbitMQ.Client.IBasicProperties.html#RabbitMQ_Client_IBasicProperties_ContentType
        private IBasicProperties GetDefaultProperties()
        {
            var properties = MsgChannel.CreateBasicProperties();
            properties.ContentEncoding = "UTF-8";
            properties.ContentType = "application/json";
            properties.DeliveryMode = 2; // Persistant
            return properties;
        }
    }
}

