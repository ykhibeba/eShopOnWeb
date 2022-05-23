using System;
using System.Collections.Generic;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace eShopWeb.Functions.Models;
internal class OrderDeliver
{
    public int Id { get; set; }
    public List<OrderItem> OrderItems { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    public Address ShipToAddress { get; set; }
    public decimal Total { get; set; }

    public decimal CalculateTotal()
    {
        var total = 0m;
        foreach (var item in OrderItems)
        {
            total += item.UnitPrice * item.Units;
        }
        return total;
    }
}
