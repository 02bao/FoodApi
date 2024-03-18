using FoodApi.Data;
using FoodApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace FoodApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly FoodDbContext _dbContext;

        public OrdersController(FoodDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("[action]")]
        public IActionResult PendingOrders()
        {
            var orders = _dbContext.Orders.Where(order => order.IsOrderCompleted == false);
            return Ok(orders);
        }

        [HttpGet("[action]")]
        public IActionResult CompleteOrders() 
        {
            var orders = _dbContext.Orders.Where(order => order.IsOrderCompleted == true);
            return Ok(orders);
        }

        [HttpGet("[action]/{orderId}")]
        public IActionResult OrderDetails(int orderId)
        {
            var orders = _dbContext.Orders.Where(order => order.Id == orderId)
                .Include(order => order.OrderDetails)
                .ThenInclude(product => product.Product);

            return Ok(orders);
        }

        [HttpGet("[action]")]
        public IActionResult OrdersCount()
        {
            var orders = (from order in _dbContext.Orders
                          where order.IsOrderCompleted == false
                          select order.IsOrderCompleted).Count();
            return Ok(new { PendingOrders = orders});
        }

        [HttpGet("[action]/{userId}")]
        public IActionResult OrdersByUser(int userId )
        {
            var orders = _dbContext.Orders.Where(orders => orders.UserId == userId)
                .OrderByDescending(o => o.OrderPlaced);
            return Ok(orders);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Order order)
        {
            order.IsOrderCompleted = false;
            order.OrderPlaced = DateTime.Now;
            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();

            var shoppingCartItems = _dbContext.ShoppingCartItems.Where(cart => cart.CustomerId == order.UserId);
            foreach(var item in shoppingCartItems)
            {
                var orderDetail = new OrderDetail()
                {
                    Price = item.Price,
                    TotalAmount = item.TotalAmount,
                    Qty = item.Qty,
                    ProductId = item.ProductId,
                    OrderId = order.Id,
                };
                _dbContext.OrderDetails.Add(orderDetail);
            }
            _dbContext.SaveChanges();
            _dbContext.ShoppingCartItems.RemoveRange(shoppingCartItems);
            _dbContext.SaveChanges();

            return Ok(new {OrderId = order.Id});
        }

        [HttpPut("[action]/{orderId}")]
        public IActionResult MArkOrderComplete( int orderId , [FromBody] Order order)
        {
            var entity = _dbContext.Orders.Find(orderId);
            if(entity == null)
            {
                return NotFound("No order found against this id...");
            }
            else
            {
                entity.IsOrderCompleted = order.IsOrderCompleted;
                _dbContext.SaveChanges();
                return Ok("Order Completed");
            }
        }
    }
}
