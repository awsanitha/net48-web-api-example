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
                return base.Ok(users);
            }
            catch (Exception ex)
            {
                // Should handle in ExceptionFilter
                return base.StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var user = await _userService.GetUserAsync(id);
                if (user == null)
                    return base.NotFound();

                return base.Ok(user);
            }
            catch (Exception ex)
            {
                // Should handle in ExceptionFilter
                return base.StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NewUser newUser)
        {
            try
            {
                if (!base.ModelState.IsValid)
                    return base.BadRequest("Name and age are required");

                var user = await _userService.AddUserAsync(newUser.Name, newUser.Age);

                return base.Created("/api/users", user);
            }
            catch (ArgumentException ex)
            {
                // Log
                return base.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Should handle in ExceptionFilter
                return base.StatusCode(500);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _userService.RemoveAsync(id);
                return base.NoContent();
            }
            catch (Exception ex)
            {
                // Should handle in ExceptionFilter
                return base.StatusCode(500);
            }
        }
    }
}
