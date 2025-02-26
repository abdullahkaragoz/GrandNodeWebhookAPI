namespace GrandNodeWebhookAPI.Services;
public class GrandNodeApiClient : IGrandNodeApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GrandNodeApiClient> _logger;
    private readonly string _baseUrl;
    private readonly string _apiKey;

    public GrandNodeApiClient(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GrandNodeApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration["GrandNode:ApiUrl"];
        _apiKey = configuration["GrandNode:ApiKey"];

        _httpClient.BaseAddress = new Uri(_baseUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<Customer> GetCustomerByEmailAsync(string email)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/customers/email/{email}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Customer>();
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            throw new Exception($"GrandNode API error: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCustomerByEmailAsync error for email: {Email}", email);
            throw;
        }
    }

    public async Task<bool> CreateCustomerAsync(Customer customer)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/customers", customer);
            
            if (response.IsSuccessStatusCode)
            {
                var createdCustomer = await response.Content.ReadFromJsonAsync<Customer>();
                customer.Id = createdCustomer.Id; // ID'yi güncelle
                return true;
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("CreateCustomerAsync failed: {Error}", error);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateCustomerAsync error for customer: {Email}", customer.Email);
            throw;
        }
    }

    public async Task<bool> UpdateCustomerAsync(Customer customer)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/customers/{customer.Id}", customer);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("UpdateCustomerAsync failed: {Error}", error);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateCustomerAsync error for customer ID: {Id}", customer.Id);
            throw;
        }
    }

    public async Task<Product> GetProductBySkuAsync(string sku)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/products/sku/{sku}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Product>();
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            throw new Exception($"GrandNode API error: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetProductBySkuAsync error for SKU: {Sku}", sku);
            throw;
        }
    }

    public async Task<bool> CreateOrderAsync(Order order)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/orders", order);
            
            if (response.IsSuccessStatusCode)
            {
                var createdOrder = await response.Content.ReadFromJsonAsync<Order>();
                order.Id = createdOrder.Id; // ID'yi güncelle
                return true;
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("CreateOrderAsync failed: {Error}", error);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateOrderAsync error for order number: {OrderNumber}", order.OrderNumber);
            throw;
        }
    }
}
