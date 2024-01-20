package com.example.demo.rabbitmq;

import org.springframework.amqp.core.Binding;
import org.springframework.amqp.core.Declarables;
import org.springframework.amqp.core.DirectExchange;
import org.springframework.amqp.core.Queue;
import org.springframework.amqp.rabbit.connection.CachingConnectionFactory;
import org.springframework.amqp.rabbit.connection.ConnectionFactory;
import org.springframework.amqp.rabbit.core.RabbitTemplate;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import com.example.Constants;

@Configuration
public class ConfigureRabbitMq {
        @Value("${spring.rabbitmq.host}")
        private String rabbitmqHost;

        @Value("${spring.rabbitmq.port}")
        private int rabbitmqPort;

        @Value("${spring.rabbitmq.username}")
        private String rabbitmqUsername;

        @Value("${spring.rabbitmq.password}")
        private String rabbitmqPassword;

        // Connection configuarion
        @Bean
        public ConnectionFactory connectionFactory() {
                // print rabbitmq config
                System.out.println("rabbitMQ config: " + rabbitmqHost + " " + rabbitmqPort +
                                " " + rabbitmqUsername
                                + " " + rabbitmqPassword);
                CachingConnectionFactory connectionFactory = new CachingConnectionFactory();
                connectionFactory.setHost("rabbitmq");
                connectionFactory.setPort(5672);
                connectionFactory.setUsername("admin");
                connectionFactory.setPassword("password");
                return connectionFactory;
        }

        @Bean
        public RabbitTemplate rabbitTemplate(ConnectionFactory connectionFactory) {
                return new RabbitTemplate(connectionFactory);
        }

        // Config RabbitMq queue and exchange schema
        @Bean
        public Declarables registartionSchema() {
                return new Declarables(
                                new DirectExchange(Constants.DEFAULT_EXCHANGE_NAME),
                                new Queue(Constants.SEND_EMAIL_QUEUE),
                                new Queue(Constants.CHECK_OUT_QUEUE),
                                new Binding(Constants.CHECK_OUT_QUEUE, Binding.DestinationType.QUEUE,
                                                Constants.DEFAULT_EXCHANGE_NAME,
                                                Constants.CHECKOUT_ROUTE,
                                                null),
                                new Binding(Constants.SEND_EMAIL_QUEUE, Binding.DestinationType.QUEUE,
                                                Constants.DEFAULT_EXCHANGE_NAME,
                                                Constants.SEND_EMAIL_ROUTE,
                                                null));
        }
}
