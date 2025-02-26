public interface IOrderService
{
    Task<(bool success, string message)> ProcessOrderAsync(OrderWebhookRequest orderRequest);
    Task<bool> ValidateOrderAsync(OrderWebhookRequest orderRequest);
} 