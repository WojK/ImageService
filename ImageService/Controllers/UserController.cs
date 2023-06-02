using ImagesService.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImagesService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ImageServiceContext _context = new ImageServiceContext();

        [HttpGet("/get-users")]
        [Authorize]
        public IActionResult GetUsers()
        {

            var users = _context.Users.Select(x=> new { x.Id , x.Name, x.Surname, x.Email } );

            return Ok(users);
        }


        [HttpGet("/hello")]
        [AllowAnonymous]
        public IActionResult Hello()
        {

            return Ok("Hello World!");
        }


    }
}
