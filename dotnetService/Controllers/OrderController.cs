using System.Net;
using dotnetService.Model;
using dotnetService.RabbitMQ;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using static Pipelines.Sockets.Unofficial.SocketConnection;

namespace dotnetService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IMessageProducer _messagePublisher;

        public OrderController(IDistributedCache distributedCache, IMessageProducer messagePublisher)
        {
            _distributedCache = distributedCache;
            _messagePublisher = messagePublisher;
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        public async Task<ShoppingCart> GetBasket(string userName)
        {
            var basket = await _distributedCache.GetStringAsync(userName);
            return new ShoppingCart() { UserName = userName, Items = JsonSerializer.Deserialize<List<ShoppingCartItem>>(basket) ?? new List<ShoppingCartItem>() };
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateBasket([FromBody] ShoppingCart basket)
        {
            // append basket item to basket of basket.UserName
            var items = await _distributedCache.GetStringAsync(basket.UserName);
            if (string.IsNullOrEmpty(items))
            {
                _distributedCache.SetString(basket.UserName, JsonSerializer.Serialize(basket.Items));
                return Ok();
            }
            List<ShoppingCartItem> shoppingCartItems = JsonSerializer.Deserialize<List<ShoppingCartItem>>(items) ?? new List<ShoppingCartItem>();
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
                    shoppingCartItems.Append(item);
                }
            }
            _distributedCache.SetString(basket.UserName, JsonSerializer.Serialize(shoppingCartItems));
            return Ok();
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] ShoppingCart basketCheckout)
        {
            // Publish order-created message
            _messagePublisher.SendMessage(basketCheckout);
            // remove basket
            await _distributedCache.RemoveAsync(basketCheckout.UserName);
            return Ok($"Basket of user {basketCheckout.UserName} checkout successfully!!");
        }
    }
}
