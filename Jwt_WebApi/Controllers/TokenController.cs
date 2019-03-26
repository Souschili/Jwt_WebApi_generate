using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Jwt_WebApi.Controllers
{
    public class TokenController : Controller
    {
        private IConfiguration configuration;

        public TokenController(IConfiguration con)
        {
            this.configuration = con;
        }
        public IActionResult CreateToken (string username="admin", string password="admin")
        {
            IActionResult responce = Unauthorized();

            // В реальном приложении проще передавать модель-представление ,а не две строки
            if(username.Equals(password))
            {
                //создаем токен и отдаем его в ответ
                var jwttoken = JwtTokenBuilder();


                responce = Ok(new { access_token=jwttoken });
            }


            return responce;
        }

        [NonAction]
        private string JwtTokenBuilder()
        {
            // подготовим ключ и креденшел заранее для удобства
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //создаем токен с временем жизни 15 минут
            var jwttoken = new JwtSecurityToken(
                                                issuer:configuration["JWT:issuer"],
                                                audience: configuration["JWT:audience"],
                                                signingCredentials: credentials,
                                                expires: DateTime.Now.AddMinutes(15)
                                                );

            // вписываем наш токен и посылаем на..куда нам надо
            return new JwtSecurityTokenHandler().WriteToken(jwttoken);

        }
    }
}