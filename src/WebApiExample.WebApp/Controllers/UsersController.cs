using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
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
                _logger.LogError(ex, "Failed to retrieve all users.");
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
                _logger.LogError(ex, "Failed to retrieve user {UserId}.", id);
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
                _logger.LogWarning(ex, "Rejected new user {UserName}.", newUser?.Name);
                return base.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user {UserName}.", newUser?.Name);
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
                _logger.LogError(ex, "Failed to delete user {UserId}.", id);
                return base.StatusCode(500);
            }
        }
    }
}
