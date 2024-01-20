package com.example.demo;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@SpringBootApplication
@RestController
public class DemoApplication {
	// Root/Default endpoint
	@RequestMapping("/") 
    public String home() { 
        return "Dockerizing Spring Boot Application"; 
    } 

	// Entry point
	public static void main(String[] args) {
		SpringApplication.run(DemoApplication.class, args);
	}

}
