using System.CodeDom.Compiler;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OstaFandy.PL.DTOs;
using System.IdentityModel.Tokens.Jwt;
using OstaFandy.PL.BL.IBL;

namespace OstaFandy.PL.BL
{
    public class JWTService : IJWTService
    {
        readonly IConfiguration _configuration;
        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GeneratedToken(UserDto user)
        {
            #region Claims  
            var userdata = new List<Claim>
           {
               new Claim("NameIdentifier", user.Id.ToString()),
               new Claim("Email", user.Email),
               new Claim("GivenName", user.FirstName),
               new Claim("Surname", user.LastName),
               new Claim("UserType", user.UserTypes.FirstOrDefault()?.TypeName ?? string.Empty),
           };
            #endregion
            #region secretKey + signingCredentials  
            var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            #endregion

            //generate token  
            var tokenobj = new JwtSecurityToken(
                claims: userdata,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: signingCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenobj);

            return token;
        }

    }
}
