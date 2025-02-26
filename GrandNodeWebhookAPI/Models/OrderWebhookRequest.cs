public class OrderWebhookRequest
{
    public CustomerInfo Customer { get; set; }
    public List<OrderItem> Items { get; set; }
    public string OrderNote { get; set; }
    public decimal TotalAmount { get; set; }
}

public class CustomerInfo
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public AddressInfo BillingAddress { get; set; }
    public AddressInfo ShippingAddress { get; set; }
}

public class AddressInfo
{
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }
}

public class OrderItem
{
    public string Sku { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
} 