using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerAPI;

namespace ServerAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Passwords")]
    public class PasswordsController : Controller
    {
        private readonly UserContext _context;

        public PasswordsController(UserContext context)
        {
            _context = context;
        }

        // GET: api/Passwords
        [HttpGet]
        public IEnumerable<Password> GetPasswords()
        {
            return _context.Passwords;
        }

        // GET: api/Passwords/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPassword([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var password = await _context.Passwords.SingleOrDefaultAsync(m => m.PasswordId == id);

            if (password == null)
            {
                return NotFound();
            }

            return Ok(password);
        }

        // PUT: api/Passwords/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPassword([FromRoute] int id, [FromBody] Password password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != password.PasswordId)
            {
                return BadRequest();
            }

            _context.Entry(password).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PasswordExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Passwords
        [HttpPost]
        public async Task<IActionResult> PostPassword([FromBody] Password password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Passwords.Add(password);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPassword", new { id = password.PasswordId }, password);
        }

        // DELETE: api/Passwords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePassword([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var password = await _context.Passwords.SingleOrDefaultAsync(m => m.PasswordId == id);
            if (password == null)
            {
                return NotFound();
            }

            _context.Passwords.Remove(password);
            await _context.SaveChangesAsync();

            return Ok(password);
        }

        private bool PasswordExists(int id)
        {
            return _context.Passwords.Any(e => e.PasswordId == id);
        }
    }
}