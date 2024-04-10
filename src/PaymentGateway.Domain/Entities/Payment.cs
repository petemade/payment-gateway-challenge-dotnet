﻿namespace PaymentGateway.Domain.Entities;

public class Payment
{
    public string CardNumber { get; set; } = null!;

    public int ExpiryMonth { get; set; }

    public int ExpiryYear { get; set; }

    public string Currency { get; set; } = null!;

    public int Amount { get; set; }

    public string Cvv { get; set; } = null!;
}