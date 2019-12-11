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
    public class OrdersController : ControllerBase
    {
        private readonly SaitynaiContext _context;

        public OrdersController(SaitynaiContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IEnumerable<Order> GetOrders()
        {
            return _context.Orders; //.Include(o => o.OrderProduct);//.Include(u => u.User);
        }

        // GET: api/Orders
        [Authorize]
        [HttpGet]
        [Route("MyOrders")]
        public IEnumerable<Order> GetMyOrders()
        {
            string userId = User.Claims.First(c => c.Type == "UserId").Value;
            return _context.Orders.Where(o => o.UserId == userId); //.Include(o => o.OrderProduct);//.Include(u => u.User);
        }


        // GET: api/Orders/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = User.Claims.First(c => c.Type == "UserId").Value;
            var order = await _context.Orders//.Include(o => o.OrderProduct)//.Include(u => u.User)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Admin"))//jei admin, gali visus
            {
                return Ok(order);
            }
            else // jei ne admin, žiūri pagal id
            {
                if (order.UserId.Equals(userId))
                {
                    return Ok(order);
                }
                else
                {
                    return Unauthorized();
                }
            }
            
        }

        // PUT: api/Orders/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder([FromRoute] int id, [FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }
            string userId = User.Claims.First(c => c.Type == "UserId").Value;
            Order tempOrder = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
            if(tempOrder.UserId != null)
            if (User.IsInRole("Admin") || tempOrder.UserId.Equals(userId))//jei admin, gali visus
            {
                order.Date = DateTime.Now.ToLocalTime();
                order.UserId = tempOrder.UserId;
                _context.Entry(order).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(id))
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

        // POST: api/Orders
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //string userId = User.Claims.First(c => c.Type == "UserId").Value;
            order.UserId = User.Claims.First(c => c.Type == "UserId").Value;
            order.Date = DateTime.Now.ToLocalTime();
            
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return BadRequest("Bandykite dar kartą, arba neteisingi duomenys");
            }

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Admin"))//jei admin, gali visus
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return Ok(order);
            }
            else // jei ne admin, žiūri pagal id
            {
                string userId = User.Claims.First(c => c.Type == "UserId").Value;
                if (order.UserId.Equals(userId))
                {
                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync();

                    return Ok(order);
                }
                else
                {
                    return Unauthorized();
                }
            }
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}