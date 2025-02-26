public class Order
{
    public string Id { get; set; }
    public string OrderNumber { get; set; }
    public string CustomerId { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public string OrderNote { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public enum OrderStatus
{
    Pending,
    Processing,
    Complete,
    Cancelled
} 