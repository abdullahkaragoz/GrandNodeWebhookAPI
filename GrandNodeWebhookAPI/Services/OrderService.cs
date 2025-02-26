namespace GrandNodeWebhookAPI.Services;
public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly ICustomerService _customerService;
    private readonly IProductService _productService;
    private readonly IGrandNodeApiClient _grandNodeApiClient;

    public OrderService(
        ILogger<OrderService> logger,
        ICustomerService customerService,
        IProductService productService,
        IGrandNodeApiClient grandNodeApiClient)
    {
        _logger = logger;
        _customerService = customerService;
        _productService = productService;
        _grandNodeApiClient = grandNodeApiClient;
    }

    public async Task<(bool success, string message)> ProcessOrderAsync(OrderWebhookRequest orderRequest)
    {
        try
        {
            // Sipariş doğrulama
            var isValid = await ValidateOrderAsync(orderRequest);
            if (!isValid)
            {
                return (false, "Sipariş verileri geçersiz.");
            }

            // Müşteriyi kontrol et veya oluştur
            var customer = await _customerService.GetCustomerByEmail(orderRequest.Customer.Email);
            if (customer == null)
            {
                customer = new Customer
                {
                    Email = orderRequest.Customer.Email,
                    FirstName = orderRequest.Customer.FirstName,
                    LastName = orderRequest.Customer.LastName,
                    Phone = orderRequest.Customer.Phone,
                    BillingAddress = new Address
                    {
                        Address1 = orderRequest.Customer.BillingAddress.Address1,
                        Address2 = orderRequest.Customer.BillingAddress.Address2,
                        City = orderRequest.Customer.BillingAddress.City,
                        State = orderRequest.Customer.BillingAddress.State,
                        ZipCode = orderRequest.Customer.BillingAddress.ZipCode,
                        Country = orderRequest.Customer.BillingAddress.Country
                    },
                    ShippingAddress = new Address
                    {
                        Address1 = orderRequest.Customer.ShippingAddress.Address1,
                        Address2 = orderRequest.Customer.ShippingAddress.Address2,
                        City = orderRequest.Customer.ShippingAddress.City,
                        State = orderRequest.Customer.ShippingAddress.State,
                        ZipCode = orderRequest.Customer.ShippingAddress.ZipCode,
                        Country = orderRequest.Customer.ShippingAddress.Country
                    }
                };
                await _customerService.InsertCustomer(customer);
            }

            // Ürünleri kontrol et
            foreach (var item in orderRequest.Items)
            {
                var product = await _productService.GetProductBySku(item.Sku);
                if (product == null)
                {
                    return (false, $"Ürün bulunamadı: {item.Sku}");
                }

                // Stok kontrolü
                if (!await _productService.CheckProductAvailability(item.Sku, item.Quantity))
                {
                    return (false, $"Ürün stokta yok veya yetersiz stok: {item.Sku}");
                }
            }

            // Siparişi oluştur
            var orderNumber = GenerateOrderNumber();
            var order = new Order
            {
                OrderNumber = orderNumber,
                CustomerId = customer.Id,
                OrderStatus = OrderStatus.Pending,
                OrderNote = orderRequest.OrderNote,
                TotalAmount = orderRequest.TotalAmount,
                CreatedOnUtc = DateTime.UtcNow
            };

            // Siparişi kaydet
            var result = await _grandNodeApiClient.CreateOrderAsync(order);
            if (!result)
            {
                return (false, "Sipariş oluşturulurken bir hata oluştu.");
            }

            return (true, orderNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sipariş işlenirken hata oluştu");
            return (false, "Sipariş işlenirken bir hata oluştu.");
        }
    }

    public async Task<bool> ValidateOrderAsync(OrderWebhookRequest orderRequest)
    {
        if (orderRequest == null) return false;
        if (orderRequest.Customer == null) return false;
        if (string.IsNullOrEmpty(orderRequest.Customer.Email)) return false;
        if (orderRequest.Items == null || !orderRequest.Items.Any()) return false;
        
        foreach (var item in orderRequest.Items)
        {
            if (string.IsNullOrEmpty(item.Sku) || item.Quantity <= 0 || item.UnitPrice <= 0)
                return false;
        }

        return true;
    }

    private string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
    }
}
