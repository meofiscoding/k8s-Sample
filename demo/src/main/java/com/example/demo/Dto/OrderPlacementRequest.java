package com.example.demo.Dto;

import com.example.demo.model.Product;

import java.util.List; // Import the correct List class

public class OrderPlacementRequest {
    public double TotalAmount;
    
    public List<Product> products; // Add a semicolon at the end of the declaration
}

