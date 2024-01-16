package com.example.demo.rabbitmq;

import org.springframework.amqp.core.Binding;
import org.springframework.amqp.core.Declarables;
import org.springframework.amqp.core.DirectExchange;
import org.springframework.amqp.core.Queue;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class ConfigureRabbitMq {

    private static final String DEFAULT_EXCHANGE_NAME = "x.default-registration";
    private static final String SEND_EMAIL_QUEUE = "q.send-email";
    private static final String SEND_EMAIL_ROUTE = "send-email";

    private static final String CHECK_OUT_QUEUE = "q.checkout";
    private static final String CHECKOUT_ROUTE = "checkout";

    @Bean
    public Declarables createPostRegistartionSchema() {
        return new Declarables(
                new DirectExchange(DEFAULT_EXCHANGE_NAME),
                new Queue(SEND_EMAIL_QUEUE),
                new Queue(CHECK_OUT_QUEUE),
                new Binding(CHECK_OUT_QUEUE, Binding.DestinationType.QUEUE, DEFAULT_EXCHANGE_NAME, CHECKOUT_ROUTE,
                        null),
                new Binding(SEND_EMAIL_QUEUE, Binding.DestinationType.QUEUE, DEFAULT_EXCHANGE_NAME, SEND_EMAIL_ROUTE,
                        null));
    }
}
