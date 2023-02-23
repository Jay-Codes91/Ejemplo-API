using Empleados.Models;
using Empleados.Models.Response;
using Empleados.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Empleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmpleadosController : ControllerBase
    {
        private readonly EmpleadosContext _context;

        public EmpleadosController(EmpleadosContext context)
        {
            _context = context;
        }

        // GET: api/<EmpleadosController>
        [HttpGet]
        public async Task<IActionResult> ObtenerEmpleados()
        {
            try
            {
                
                var empleados = await _context.Personals.ToListAsync();
               
                return Ok(empleados);

            }
            catch(Exception)
            {
                //return BadRequest(ex.Message);
                return Unauthorized(new { mensaje = "No tienes permiso"});
            }
        }

        // GET api/<EmpleadosController>/5
        [HttpGet("{id}")]
        public IActionResult ObtenerUnEmpleado(int id)
        {
            try
            {
                var empleado = from e in _context.Personals
                               where e.Id == id
                               select new
                               {
                                   e.Nombre,
                                   e.Apellido1,
                                   e.Apellido2,
                                   e.Correo
                               };
                if(empleado == null)
                {
                    return NotFound(new { mensaje = "El empleado no se encuentra" });
                }

                return Ok(empleado.ToList());
            }
            catch (Exception)
            {
                return BadRequest(new { mensaje = "No se pudo consultar el empleado" });
            }
        }

        // POST api/<EmpleadosController>
        [Authorize(Policy = "MiPerson")]
        [HttpPost]
        public IActionResult AgregarEmpleados([FromBody] Personal personal)
        {
            try
            {
                string encrypPassword = Encriptacion.GetSHA256(personal.Pass);
                Respuesta respuesta = new Respuesta();

                _context.Personals.Add(new Personal()
                {
                    
                    Nombre = personal.Nombre,
                    Apellido1 = personal.Apellido1,
                    Apellido2 = personal.Apellido2,
                    Correo = personal.Correo,
                    Pass = encrypPassword


                });
                _context.SaveChanges();
                respuesta.Exito = 1;
                respuesta.Mensaje = "El empleado se ha añadido con éxito";
                return Ok(respuesta);
                /*_context.Add(personal);
                await _context.SaveChangesAsync();
                return Ok(new {message = "Empleado añadido con éxito"});*/
            }
            catch (Exception)
            {
                return BadRequest(new { message = "No se ha podido agregar el empleado" });
            }
            
        }

        // PUT api/<EmpleadosController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> modificarEmpleado(int id, [FromBody] Personal personal)
        {
            try
            {
                if(id != personal.Id)
                {
                    return NotFound(new { mensaje = "El empleado no se pudo modificar porque no existe en el sistema" });
                }
                _context.Update(personal);
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "El empleado se ha modificado con éxito" });
            }
            catch (Exception)
            {
                return BadRequest(new { mensaje = "No se pudo modificar los campos del empleado" });
            }
        }

        //PATCH api


        [HttpPatch("{id}")]
        [Authorize(Policy = "MiPerson")]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<Personal> patchPersonal)
        {
            if(patchPersonal == null)
            {
                return BadRequest();
            }

            var empleado = await _context.Personals.FirstOrDefaultAsync(x => x.Id == id);

            if(empleado == null)
            {
                return NotFound();
            }

            patchPersonal.ApplyTo(empleado);
            await _context.SaveChangesAsync();
            return Ok(empleado);
        }

        // DELETE api/<EmpleadosController>/5
        [Authorize(Policy = "MiPerson")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> eliminarEmpleado(int id)
        {
            try
            {
                var empleado = await _context.Personals.Where(e => e.Id == id).FirstOrDefaultAsync();
                if(empleado == null)
                {
                    return NotFound(new { mensaje = "No se ha encontrado el empleado en el sistema" });
                }
                _context.Personals.Remove(empleado);
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "El empleado se ha eliminado con éxito" });
            }
            catch (Exception)
            {
                return BadRequest(new { mensaje = "No se pudo eliminar el empleado" });
            }
        }
    }
}
