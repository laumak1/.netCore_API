using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saitynai.Models;
using Saitynai.ViewModels;

namespace Saitynai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SaitynaiContext _context;
        private readonly UserManager<User> _userManager;

        public UsersController(SaitynaiContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Users
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            return _context.Users;//.Include(o => o.Orders).ThenInclude(x => x.OrderProduct);
        }

        // GET: api/Users/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users//.Include(o => o.Orders)
                .FirstOrDefaultAsync(i => i.Id == id); ;

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // GET: api/Users/5
        [Authorize]
        [HttpGet]
        [Route("MyProfile")]
        public async Task<IActionResult> GetMyProfile()
        {
            string userId = User.Claims.First(c => c.Type == "UserId").Value;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users//.Include(o => o.Orders)
                    .FirstOrDefaultAsync(i => i.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }



        // PUT: api/Users/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] string id, [FromBody] User user)
        {
            //string userId = User.Claims.First(c => c.Type == "UserID").Value;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;
            //var result = await _userManager.UpdateAsync(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest("Bandykite dar kartą, arba trūksta duomenų");
                }
            }

            return NoContent();
        }


        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                Email = model.Email,
                UserName = model.Email,
                Sex = model.Sex,
                BirthDate = model.BirthDate,
                SecurityStamp = Guid.NewGuid().ToString()
            };


            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                }
                return Ok(result);
            }
            catch
            {
                return BadRequest("Bandykite dar kartą, arba neteisingi duomenys");
            }
            
            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }


        // DELETE: api/Users/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}