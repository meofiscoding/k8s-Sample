package com.example.demo.controller;

import org.springframework.web.bind.annotation.RestController;
import jakarta.ws.rs.core.MediaType;

import org.springframework.amqp.rabbit.core.RabbitTemplate;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;


@RestController
@RequestMapping(value = "/api", method = RequestMethod.GET, produces = MediaType.APPLICATION_JSON, consumes = MediaType.APPLICATION_JSON)
public class SampleController {
    private final RabbitTemplate rabbitTemplate;

    public SampleController(RabbitTemplate rabbitTemplate) {
        this.rabbitTemplate = rabbitTemplate;
    }

    // Sending message to queue
    @PostMapping("/notify")
    public ResponseEntity<String> notify(@RequestBody String message) {
        rabbitTemplate.convertAndSend("x.default-registration", "send-email", message);
        return ResponseEntity.ok("Message %s sent to queue sms and email successfully".formatted(message));
    }
}
