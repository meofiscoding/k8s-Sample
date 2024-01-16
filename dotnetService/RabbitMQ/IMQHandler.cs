using System;
using RabbitMQ.Client.Events;

namespace dotnetService.RabbitMQ
{
    public interface IMQHandler
    {
        Task MQEventHandler(object? sender, BasicDeliverEventArgs args);
    }
}

