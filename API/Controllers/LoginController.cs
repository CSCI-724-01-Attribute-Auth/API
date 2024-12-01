using API.Authorization;
using API.Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace API.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginController"/> class.
        /// </summary>
        /// <param name="loginService">Service to handle login operations.</param>
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Login([FromQuery] string userId)
        {
            try
            {
                return StatusCode(StatusCodes.Status200OK, new { access_token = _loginService.Login(userId), token_type = "Bearer", expires_in = 3600 });
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);

            }
        }
    }
}
