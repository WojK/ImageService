using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using ImagesService.Data;
using ImagesService.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageService.Utils
{
    public class JwtUtils
    {
        public static User? GetUserFromJWT(string? jwt, ImageServiceContext _context)
        {
            if (jwt == null) return null;

            jwt = jwt.Replace("Bearer ", string.Empty);
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            var email = token.Claims.First(claim => claim.Type == ClaimTypes.Email).Value;
            User? user = _context.Users.FirstOrDefault(u => u.Email == email);
            return user;
        }
    }
}
