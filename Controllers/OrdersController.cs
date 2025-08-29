using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using dvcsharp_core_api.Data;
using dvcsharp_core_api.Models;

namespace dvcsharp_core_api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        private readonly GenericDataContext _context;

        public OrdersController(GenericDataContext context)
        {
            _context = context;
        }

        // GET api/orders
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var userId = int.Parse(User.Identity.Name);
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .ToListAsync();
                
            return Ok(orders);
        }

        // GET api/orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var userId = int.Parse(User.Identity.Name);
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // POST api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = int.Parse(User.Identity.Name);
            order.UserId = userId;
            order.CreatedAt = DateTime.UtcNow;
            
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // PUT api/orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = int.Parse(User.Identity.Name);
            var existingOrder = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (existingOrder == null)
            {
                return NotFound();
            }

            existingOrder.OrderNumber = order.OrderNumber;
            existingOrder.CustomerName = order.CustomerName;
            existingOrder.TotalAmount = order.TotalAmount;

            await _context.SaveChangesAsync();
            return Ok(existingOrder);
        }

        // DELETE api/orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var userId = int.Parse(User.Identity.Name);
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        [HttpGet("search")]
        public IActionResult Search(string keyword)
        {
           if (String.IsNullOrEmpty(keyword)) {
              return Ok("Cannot search without a keyword");
           }

           var query = $"SELECT * From Orders WHERE name LIKE '%{keyword}%' OR description LIKE '%{keyword}%'";
           var orders = _context.Orders
              .FromSql(query)
              .ToList();

           return Ok(orders);
        }
    }
}
