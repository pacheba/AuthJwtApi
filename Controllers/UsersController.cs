using AuthJwtApi.Models;
using AuthJwtApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthJwtApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService) => _userService = userService;

        // GET: api/users
        [HttpGet]
        public ActionResult<IEnumerable<UserResponse>> Get()
        {
            var users = _userService.GetAll().Select(u => new UserResponse(u.Id, u.Username ?? "", u.Email ?? ""));
            return Ok(users);
        }

        // GET: api/users/me
        [HttpGet("me")]
        public ActionResult<UserResponse> Me()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var id)) return Unauthorized();
            var user = _userService.GetById(id);
            if (user == null) return NotFound();
            return Ok(new UserResponse(user.Id, user.Username ?? "", user.Email ?? ""));
        }
    }
}
