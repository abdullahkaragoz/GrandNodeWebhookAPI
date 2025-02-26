namespace GrandNodeWebhookAPI.Services;
public class ProductService : IProductService
{
    private readonly ILogger<ProductService> _logger;
    private readonly IGrandNodeApiClient _grandNodeApiClient;

    public ProductService(ILogger<ProductService> logger, IGrandNodeApiClient grandNodeApiClient)
    {
        _logger = logger;
        _grandNodeApiClient = grandNodeApiClient;
    }

    public async Task<Product> GetProductBySku(string sku)
    {
        try
        {
            if (string.IsNullOrEmpty(sku))
                throw new ArgumentException("SKU boş olamaz.");

            // GrandNode API'sini kullanarak ürünü getir
            var product = await _grandNodeApiClient.GetProductBySkuAsync(sku);
            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ürün bilgisi alınırken hata oluştu. SKU: {sku}");
            throw;
        }
    }

    public async Task<bool> CheckProductAvailability(string sku, int quantity)
    {
        try
        {
            var product = await GetProductBySku(sku);
            if (product == null)
                return false;

            return product.Published && product.StockQuantity >= quantity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ürün stok kontrolü yapılırken hata oluştu. SKU: {sku}");
            throw;
        }
    }

    public async Task<List<Product>> GetProductsBySkus(List<string> skus)
    {
        try
        {
            if (skus == null || !skus.Any())
                throw new ArgumentException("SKU listesi boş olamaz.");

            var products = new List<Product>();
            foreach (var sku in skus)
            {
                var product = await GetProductBySku(sku);
                if (product != null)
                    products.Add(product);
            }

            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ürünler getirilirken hata oluştu.");
            throw;
        }
    }
}
