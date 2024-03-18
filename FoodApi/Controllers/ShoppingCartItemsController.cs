using FoodApi.Data;
using FoodApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace FoodApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingCartItemsController : ControllerBase
    {
        private readonly FoodDbContext _dbContext;

        public ShoppingCartItemsController(FoodDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{userId}")]
        public IActionResult Get(int userId)
        {
            var user = _dbContext.ShoppingCartItems.Where(s => s.CustomerId == userId);
            if(user == null)
            {
                return NotFound();
            }
            var shoppingCartItems = from s in _dbContext.ShoppingCartItems.Where(s => s.CustomerId == userId)
                                    join p in _dbContext.Products on s.ProductId equals p.Id
                                    select new
                                    {
                                        Id = s.Id,
                                        Price = s.Price,
                                        TotalAmount = s.TotalAmount,
                                        Qty = s.Qty,
                                        ProductName = p.Name,
                                    };
            return Ok(shoppingCartItems);
        }

        [HttpGet("[action]/{userId}")]
        public IActionResult SubTotal(int userId)
        {
            var subTotal = (from cart in _dbContext.ShoppingCartItems
                            where cart.CustomerId == userId
                            select cart.TotalAmount).Sum();
            return Ok(new {SubTotal = subTotal});
        }

        [HttpGet("[action]/{userId}")]
        public IActionResult Totalitems(int userId) 
        {
            var cartItems = (from cart in _dbContext.ShoppingCartItems
                             where cart.CustomerId == userId
                             select cart.Qty).Sum();
            return Ok(new {Totalitems = cartItems});
        }

        [HttpPost]
        public IActionResult Post([FromBody ] ShoppingCartItem shoppingCartItem)
        {
            var shoppingCart = _dbContext.ShoppingCartItems.FirstOrDefault(s => s.ProductId  == shoppingCartItem.ProductId
            && s.CustomerId == shoppingCartItem.CustomerId);
            if(shoppingCart != null)
            {
                shoppingCart.Qty += shoppingCartItem.Qty;
                shoppingCart.TotalAmount = shoppingCart.Price * shoppingCart.Qty;
            }
            else
            {
                var sCart = new ShoppingCartItem()
                {
                    CustomerId = shoppingCartItem.CustomerId,
                    ProductId = shoppingCartItem.ProductId,
                    Price = shoppingCartItem.Price,
                    Qty = shoppingCartItem.Qty,
                    TotalAmount = shoppingCartItem.TotalAmount
                };
                _dbContext.ShoppingCartItems.Add(sCart);
            }
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpDelete("{userId}")]
        public IActionResult Delete(int userId)
        {
            var shoppingCart = _dbContext.ShoppingCartItems.Where(s => s.CustomerId == userId);
            _dbContext.ShoppingCartItems.RemoveRange(shoppingCart);
            _dbContext.SaveChanges();
            return Ok();
        }

    }
}
