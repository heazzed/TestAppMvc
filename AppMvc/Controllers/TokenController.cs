using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AppMvc.Models;
using AppMvc.DataBase;
using Newtonsoft.Json;
using AppMvc.Util;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AppMvc.Controllers
{
    public class TokenController : Controller
    {
        private WebContext _context;

        public TokenController(WebContext context)
        {
            _context = context;
        }

        public IActionResult GetToken(string username, string password)
        {
            if (CheckUser(username, password) is ClaimsIdentity claimsIdentity)
            {
                return GenerateToken(username, claimsIdentity);
            }
            else
            {
                throw new InvalidOperationException(HttpStatusCode.Unauthorized.ToString());
            }
            
        }

        public ClaimsIdentity CheckUser(string username, string password)
        {
            User user = _context.Users.FirstOrDefault(x => x.Email == username && x.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email)
                };

                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token");
                return claimsIdentity;
            }
            throw new InvalidOperationException(HttpStatusCode.Unauthorized.ToString());
        }

        private const string Secret = "mysupersecretkeymysupersecretkeymysupersecretkeymysupersecretkey";

        public IActionResult GenerateToken(string username, ClaimsIdentity claimsIdentity, int expireMinutes = 1)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claimsIdentity.Claims,
                    notBefore: now,
                    expires: now.AddMinutes(1),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),SecurityAlgorithms.HmacSha256)
            );

            var encodedJwt = tokenHandler.WriteToken(jwt);


            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(new[]
            //    {
            //        new Claim(ClaimTypes.Name, username)
            //    }),

            //    Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),

            //    SigningCredentials = new SigningCredentials(
            //        new SymmetricSecurityKey(symmetricKey),
            //        SecurityAlgorithms.HmacSha256Signature)
            //};

            //var stoken = tokenHandler.CreateToken(tokenDescriptor);
            //var token = tokenHandler.WriteToken(stoken);






            //var ans = new List<string>();
            //ans.Add(encodedJwt);
            //ans.Add(claimsIdentity.Name);
            //var response = JsonConvert.SerializeObject(ans);

            return Json(encodedJwt);
        }
    }
}
