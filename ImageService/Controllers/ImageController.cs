using ImagesService.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ImagesService.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImagesService.Data;
using System.Text.Json;
using System.Linq;
using ImagesService.Services;
using ImagesService.Services.Interfaces;

namespace ImagesService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {

        private readonly ImageServiceContext _context = new ImageServiceContext();

        private readonly IImgService _imageService;

        public ImageController(IImgService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("/add-image")]
        [Authorize]
        public IActionResult AddImage([FromForm] IFormFile image, [FromQuery] string option)
        {

            string? jwt = Request.Headers["Authorization"];
            User? user = ImageService.Utils.JwtUtils.GetUserFromJWT(jwt, _context);
            if (user == null)
            {
                return BadRequest(JsonSerializer.Serialize(new { Message = "Our service does not allow your image" }));
            }

            bool isSave =  _imageService.IsImageSave(image).Result;

            if (!isSave)
            {
                return BadRequest(JsonSerializer.Serialize(new { Message = "Our service does not allow your image" }));
            }


            Image image2 = new Image
            {
                ImageName = System.IO.Path.GetFileName(image.FileName),
                ContentType = image.ContentType,
            };


            if (option == "Public")
            {
                image2.Public = true;
                image2.Private = false;
                image2.SelectedUsers = false;
            }
            if (option == "Private")
            {
                image2.Public = false;
                image2.Private = true;
                image2.SelectedUsers = false;
            }

            if (option == "SelectedUsers")
            {
                image2.Public = false;
                image2.Private = false;
                image2.SelectedUsers = true;
            }

            var reader = new System.IO.BinaryReader(image.OpenReadStream());
            image2.Content = reader.ReadBytes((int)image.Length);

            user.Images = new List<Image> { image2 };
            _context.SaveChanges();

            return Ok(JsonSerializer.Serialize(new { id = image2.Id }));
        }

        [HttpPost("/add-users")]
        [Authorize]
        public IActionResult AddUsersToImage([FromQuery] int idImage, [FromBody] List<int> usersId)
        {

            string? jwt = Request.Headers["Authorization"];
            User? user = ImageService.Utils.JwtUtils.GetUserFromJWT(jwt, _context);
            if (user == null)
            {
                return BadRequest(JsonSerializer.Serialize(new { Message = "User is null" }));
            }

            var img = _context.Images.FirstOrDefault(x => x.Id == idImage);
            if(img.User.Id != user.Id)
            {
                return BadRequest();

            }

            foreach (int userId in usersId)
            {
                _context.SelectedUsers.Add(new SelectedUsers { idImage = idImage, idUser = userId });
            }

            _context.SaveChanges();

            return Ok();
        }

        [HttpGet("/get-myprivateimages")]
        [Authorize]
        public IActionResult GetUserImages()
        {

            string? jwt = Request.Headers["Authorization"];
            User? user = ImageService.Utils.JwtUtils.GetUserFromJWT(jwt, _context);
            if (user == null)
            {
                return BadRequest(JsonSerializer.Serialize(new { Message = "User is null" }));
            }

            var images =  _context.Images.Where(x => x.User.Id == user.Id && x.Private == true)
                .Select(x=> new { Id = x.Id ,Name = x.ImageName,
                    Content = "data:" + x.ContentType + ";base64, " + Convert.ToBase64String(x.Content) });

            return Ok(images);
        }

        [HttpGet("/get-selectedusersimage")]
        [Authorize]
        public IActionResult GetSelectionImages()
        {

            string? jwt = Request.Headers["Authorization"];
            User? user = ImageService.Utils.JwtUtils.GetUserFromJWT(jwt, _context);
            if (user == null)
            {
                return BadRequest(JsonSerializer.Serialize(new { Message = "User is null" }));
            }

            var selected = _context.SelectedUsers.Where(x => x.idUser == user.Id).Select(x => x.idImage ).ToArray();

            var images = _context.Images.Where(x => selected.Contains(x.Id) && x.SelectedUsers == true)
                .Select(x => new {
                    Id = x.Id,
                    UserName = x.User.Name,
                    UserSurname = x.User.Surname,
                    Name = x.ImageName,
                    Content = "data:" + x.ContentType + ";base64, " + Convert.ToBase64String(x.Content)
                });


            return Ok(images);
        }



        [HttpGet("/get-publicimages")]
        [Authorize]
        public IActionResult GetPublicImages()
        {

            string? jwt = Request.Headers["Authorization"];
            User? user = ImageService.Utils.JwtUtils.GetUserFromJWT(jwt, _context);
            if (user == null)
            {
                return BadRequest(JsonSerializer.Serialize(new { Message = "User is null" }));
            }


            var images = _context.Images.Where(x => x.Public == true)
                .Select(x => new {
                    Id = x.Id,
                    UserName = x.User.Name,
                    UserSurname = x.User.Surname,
                    Name = x.ImageName,
                    Content = "data:" + x.ContentType + ";base64, " + Convert.ToBase64String(x.Content)
                });

            return Ok(images);
        }

    }
}
