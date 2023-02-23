using Empleados.Common;
using Empleados.Models;
using Empleados.Models.Response;
using Empleados.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Empleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly EmpleadosContext EmpleadosContext;
        private readonly AppSettings appSettings;

        public LoginController(EmpleadosContext empleadosContext, IOptions<AppSettings> appSettings)
        {
            EmpleadosContext = empleadosContext;
            this.appSettings = appSettings.Value;
        }



        // POST api/<LoginController>
        [HttpPost("{login}")]
        public IActionResult Autentificar([FromBody] Personal personal)
        {
            string pass = Encriptacion.GetSHA256(personal.Pass);
            UserResponse userResponse = new UserResponse();
            Respuesta respuesta = new Respuesta();
            if (personal == null)
            {
                respuesta.Exito = 0;
                respuesta.Mensaje = "Correo o contraseña incorrecta";
                return BadRequest(respuesta);
            }

            var persona = EmpleadosContext.Personals.Where(d=> d.Nombre == personal.Nombre &&
                          d.Pass == pass).FirstOrDefault();

            if(persona == null)
            {
                respuesta.Exito = 0;
                respuesta.Mensaje = "Usuario o contraseña incorrecta";
                return NotFound(respuesta);
            }

            userResponse.Nombre = persona.Nombre;
            userResponse.Token = GetToken(persona);
            respuesta.Exito = 1;
            respuesta.Data = userResponse;
            return Ok(respuesta);
        }

        private string GetToken(Personal personal)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var llave = Encoding.ASCII.GetBytes(appSettings.Secreto);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, personal.Id.ToString()),
                        new Claim(ClaimTypes.Name, personal.Nombre),
                        new Claim("Nombre", personal.Nombre)
                    }
                    ),
                Expires = DateTime.UtcNow.AddDays(60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(llave), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }



    }
}
