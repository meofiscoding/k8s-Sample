package com.example.demo.controller;

import org.springframework.web.bind.annotation.RestController;
import com.example.demo.Dto.OrderPlacementRequest;
import jakarta.ws.rs.core.MediaType;

import org.springframework.amqp.rabbit.core.RabbitTemplate;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;


@RestController
@RequestMapping(value = "/api", method = RequestMethod.GET, produces = MediaType.APPLICATION_JSON, consumes = MediaType.APPLICATION_JSON)
public class PaymentController {
    private final RabbitTemplate rabbitTemplate;

    public PaymentController(RabbitTemplate rabbitTemplate) {
        this.rabbitTemplate = rabbitTemplate;
    }

    // TODO: Add data to Redis database
   
    // TODO: Retrieve data from Redis database

    @PostMapping("/payment")
    public ResponseEntity<String> processOrder(@RequestBody OrderPlacementRequest order) {
        // TODO: send data to RabbitMQ so .net microservice can read it
        rabbitTemplate.convertAndSend("", "payment-queue", order);
        return ResponseEntity.ok("Order placed successfully");
    }
}
