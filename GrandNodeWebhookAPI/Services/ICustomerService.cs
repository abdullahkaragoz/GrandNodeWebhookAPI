public interface ICustomerService
{
    Task<Customer> GetCustomerByEmail(string email);
    Task<bool> InsertCustomer(Customer customer);
    Task<bool> UpdateCustomer(Customer customer);
} 