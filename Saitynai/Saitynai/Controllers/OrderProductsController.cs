using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saitynai.Models;

namespace Saitynai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderProductsController : ControllerBase
    {
        private readonly SaitynaiContext _context;

        public OrderProductsController(SaitynaiContext context)
        {
            _context = context;
        }

        // GET: api/OrderProducts
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IEnumerable<OrderProduct> GetOrderProducts()
        {
            return _context.OrderProducts//.Include(o => o.Order)
                //.Include(p => p.Product)
                ;//.Include(u => u.Order.User);
        }

        [Authorize]
        [HttpGet]
        [Route("MyOrderProducts")]
        public IEnumerable<OrderProduct> GetMyOrderProducts()
        {
            string userId = User.Claims.First(c => c.Type == "UserId").Value;
            var userOrders = _context.Orders.Where(u => u.UserId == userId);
            List< OrderProduct > ats = new List<OrderProduct>();
            foreach (var item in userOrders)
            {
                ats.AddRange(_context.OrderProducts.Where(o => o.OrderId == item.Id));
            }

            return ats;
        }

        // GET: api/OrderProducts/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = User.Claims.First(c => c.Type == "UserId").Value;
            var orderProduct = await _context.OrderProducts//.Include(o => o.Order)
                //.Include(p => p.Product)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (orderProduct == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Admin"))//jei admin, gali visus
            {
                return Ok(orderProduct);
            }
            else // jei ne admin, žiūri pagal id
            {
                var order = await _context.Orders.FirstOrDefaultAsync(i => i.Id == orderProduct.OrderId);
                if (order.UserId.Equals(userId))
                {
                    return Ok(orderProduct);
                }
                else
                {
                    return Unauthorized();
                }
            }
        }

        // PUT: api/OrderProducts/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderProduct([FromRoute] int id, [FromBody] OrderProduct orderProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != orderProduct.Id)
            {
                return BadRequest();
            }

            string userId = User.Claims.First(c => c.Type == "UserId").Value;
            OrderProduct tempOrderProduct = await _context.OrderProducts.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
            orderProduct.OrderId = tempOrderProduct.OrderId;
            orderProduct.ProductId = tempOrderProduct.ProductId;
            var tempOrder = await _context.Orders.FirstOrDefaultAsync(i => i.Id == tempOrderProduct.OrderId);

            if (User.IsInRole("Admin") || tempOrder.UserId.Equals(userId))//jei admin, gali visus
            {
                //order.Date = DateTime.Now.ToLocalTime();
                //order.UserId = tempOrder.UserId;
                _context.Entry(orderProduct).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderProductExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                        //return BadRequest("Bandykite dar kartą, arba trūksta duomenų");
                    }
                }

                return NoContent();
                //return Ok(order);
            }

            return Unauthorized();
        }

        // POST: api/OrderProducts
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostOrderProduct([FromBody] OrderProduct orderProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userId = User.Claims.First(c => c.Type == "UserId").Value;

            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(i => i.Id == orderProduct.OrderId);
                if (order.UserId.Equals(userId))
                {
                    _context.OrderProducts.Add(orderProduct);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetOrderProduct", new { id = orderProduct.Id }, orderProduct);
                }

                return Unauthorized();
            }
            catch
            {
                return BadRequest("Trūksta arba neteisingi duomenys");
            }
            
        }

        // DELETE: api/OrderProducts/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderProduct = await _context.OrderProducts.FindAsync(id);
            if (orderProduct == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Admin"))//jei admin, gali visus
            {
                _context.OrderProducts.Remove(orderProduct);
                await _context.SaveChangesAsync();

                return Ok(orderProduct);
            }
            else // jei ne admin, žiūri pagal id
            {
                string userId = User.Claims.First(c => c.Type == "UserId").Value;
                var order = await _context.Orders.FirstOrDefaultAsync(i => i.Id == orderProduct.OrderId);
                if (order.UserId.Equals(userId))
                {
                    _context.OrderProducts.Remove(orderProduct);
                    await _context.SaveChangesAsync();

                    return Ok(orderProduct);
                }
                else
                {
                    return Unauthorized();
                }
            }
        }

        private bool OrderProductExists(int id)
        {
            return _context.OrderProducts.Any(e => e.Id == id);
        }
    }
}