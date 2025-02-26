public interface IProductService
{
    Task<Product> GetProductBySku(string sku);
    Task<bool> CheckProductAvailability(string sku, int quantity);
    Task<List<Product>> GetProductsBySkus(List<string> skus);
} 