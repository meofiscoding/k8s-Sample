using System.Net;
using dotnetService.Model;
using dotnetService.RabbitMQ;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace dotnetService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RabbitMQService _rabbitMQService;

        public SampleController(IDistributedCache distributedCache, RabbitMQService rabbitMQService)
        {
            _distributedCache = distributedCache;
            _rabbitMQService = rabbitMQService;
        }

        /// <summary>
        /// Get basket item from Redis
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>

        [HttpGet("{userName}", Name = "GetBasket")]
        public async Task<SampleShoppingCart> GetBasket(string userName)
        {
            var basket = await _distributedCache.GetStringAsync(userName);
            return new SampleShoppingCart() { UserName = userName, Items = JsonSerializer.Deserialize<List<SampleShoppingCartItem>>(basket) ?? new List<SampleShoppingCartItem>() };
        }

        /// <summary>
        /// Update basket item to Redis
        /// </summary>
        /// <param name="basket"></param>
        /// <returns></returns>

        [HttpPost]
        [ProducesResponseType(typeof(SampleShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateBasket([FromBody] SampleShoppingCart basket)
        {
            // append basket item to basket of basket.UserName
            var items = await _distributedCache.GetStringAsync(basket.UserName);
            if (string.IsNullOrEmpty(items))
            {
                _distributedCache.SetString(basket.UserName, JsonSerializer.Serialize(basket.Items));
                return Ok();
            }
            List<SampleShoppingCartItem> shoppingCartItems = JsonSerializer.Deserialize<List<SampleShoppingCartItem>>(items) ?? new List<SampleShoppingCartItem>();
            foreach (var item in shoppingCartItems)
            {
                // check if items contain item.ProductId
                var existingItem = shoppingCartItems.FirstOrDefault(i => i.ProductId == item.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity += item.Quantity;
                }
                else
                {
                    _ = shoppingCartItems.Append(item);
                }
            }
            _distributedCache.SetString(basket.UserName, JsonSerializer.Serialize(shoppingCartItems));
            return Ok();
        }

        /// <summary>
        /// Publish a message to RabbitMQ and remove basket item from Redis
        /// </summary>
        /// <param name="basketCheckout"></param>
        /// <returns></returns>

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] SampleShoppingCart basketCheckout)
        {
            // Publish order-created message
            _rabbitMQService.PublishMessage(basketCheckout, RabbitMQService.CHECKOUT_ROUTE);
            // remove basket
            await _distributedCache.RemoveAsync(basketCheckout.UserName);
            return Ok($"Basket of user {basketCheckout.UserName} checkout successfully!!");
        }
    }
}
