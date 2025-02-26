namespace GrandNodeWebhookAPI.Services;
public class CustomerService : ICustomerService
{
    private readonly ILogger<CustomerService> _logger;
    private readonly IGrandNodeApiClient _grandNodeApiClient;

    public CustomerService(ILogger<CustomerService> logger, IGrandNodeApiClient grandNodeApiClient)
    {
        _logger = logger;
        _grandNodeApiClient = grandNodeApiClient;
    }

    public async Task<Customer> GetCustomerByEmail(string email)
    {
        try
        {
            // GrandNode API'sini kullanarak müşteriyi getir
            var customer = await _grandNodeApiClient.GetCustomerByEmailAsync(email);
            return customer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Müşteri bilgisi alınırken hata oluştu. Email: {email}");
            throw;
        }
    }

    public async Task<bool> InsertCustomer(Customer customer)
    {
        try
        {
            // Validasyon
            if (string.IsNullOrEmpty(customer.Email))
                throw new ArgumentException("Email adresi boş olamaz.");

            // Müşteri zaten var mı kontrol et
            var existingCustomer = await GetCustomerByEmail(customer.Email);
            if (existingCustomer != null)
                return false;

            // Yeni müşteri oluştur
            customer.CreatedOnUtc = DateTime.UtcNow;
            customer.UpdatedOnUtc = DateTime.UtcNow;

            // GrandNode API'sini kullanarak müşteriyi kaydet
            var result = await _grandNodeApiClient.CreateCustomerAsync(customer);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Müşteri oluşturulurken hata oluştu. Email: {customer.Email}");
            throw;
        }
    }

    public async Task<bool> UpdateCustomer(Customer customer)
    {
        try
        {
            // Validasyon
            if (string.IsNullOrEmpty(customer.Id))
                throw new ArgumentException("Müşteri ID'si boş olamaz.");

            customer.UpdatedOnUtc = DateTime.UtcNow;

            // GrandNode API'sini kullanarak müşteriyi güncelle
            var result = await _grandNodeApiClient.UpdateCustomerAsync(customer);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Müşteri güncellenirken hata oluştu. ID: {customer.Id}");
            throw;
        }
    }
}
