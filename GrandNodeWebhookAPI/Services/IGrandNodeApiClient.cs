public interface IGrandNodeApiClient
{
    Task<Customer> GetCustomerByEmailAsync(string email);
    Task<bool> CreateCustomerAsync(Customer customer);
    Task<bool> UpdateCustomerAsync(Customer customer);
    Task<Product> GetProductBySkuAsync(string sku);
    Task<bool> CreateOrderAsync(Order order);
} 