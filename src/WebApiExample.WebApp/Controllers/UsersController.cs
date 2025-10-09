using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApiExample.DataStore.Models;
using WebApiExample.WebApp.Models;
using WebApiExample.WebApp.Services;

namespace WebApiExample.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Should handle in ExceptionFilter
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var user = await _userService.GetUserAsync(id);
                if (user == null)
                    return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                // Should handle in ExceptionFilter
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NewUser newUser)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Name and age are required");

                var user = await _userService.AddUserAsync(newUser.Name, newUser.Age);

                return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
            }
            catch (ArgumentException ex)
            {
                // Log
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Should handle in ExceptionFilter
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _userService.RemoveAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Should handle in ExceptionFilter
                return StatusCode(500);
            }
        }
    }
}