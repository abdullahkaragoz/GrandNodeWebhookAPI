using Microsoft.AspNetCore.Mvc;

namespace GrandNodeWebhookAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ICustomerService _customerService;
    private readonly IProductService _productService;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(
        IOrderService orderService,
        ICustomerService customerService,
        IProductService productService,
        ILogger<WebhookController> logger)
    {
        _orderService = orderService;
        _customerService = customerService;
        _productService = productService;
        _logger = logger;
    }

    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder([FromBody] OrderWebhookRequest orderRequest)
    {
        try
        {
            var (success, message) = await _orderService.ProcessOrderAsync(orderRequest);

            if (!success)
            {
                return BadRequest(new { error = message });
            }

            return Ok(new { orderNumber = message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sipariş işlenirken beklenmeyen bir hata oluştu");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}


