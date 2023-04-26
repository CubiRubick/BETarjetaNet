using BETarjetas.Helpers;
using BETarjetas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BETarjetas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TarjetaController : ControllerBase
    {
        private readonly IConfiguration conf;
        private readonly AplicationDbContext _context;

        public TarjetaController(AplicationDbContext context, IConfiguration config)
        {
            _context = context;
            conf = config;
        }


        // GET: api/<TarjetaController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var listTarjetas = await _context.TarjetaCreditos.ToListAsync();
                return Ok(listTarjetas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        // GET api/<TarjetaController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var tarjeta = await _context.TarjetaCreditos.FindAsync(id);
                
                if(tarjeta == null)
                {
                    return NotFound();
                }

                return Ok(tarjeta);
                


            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<TarjetaController>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] TarjetaCredito tarjeta)
        {
            string secret = this.conf.GetValue<string>("SecretKey");
            var jwtHelper = new TarjetaHelper(secret);
            var token = jwtHelper.CreateToken(tarjeta.Titular);
            try
            {
                _context.Add(tarjeta);
                await _context.SaveChangesAsync();
                return Ok(new { tarjeta, token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // PUT api/<TarjetaController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TarjetaCredito tarjeta)
        {

            try
            {
                if (id != tarjeta.Id)
                {
                    return NotFound();
                }

                _context.Update(tarjeta);
                await _context.SaveChangesAsync();
                return Ok(new { message = "La Tarjeta fue actualizada con exito!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // DELETE api/<TarjetaController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            try
            {
                var tarjeta = await _context.TarjetaCreditos.FindAsync(id);
                if (tarjeta ==  null)
                {
                    return NotFound();
                }

                _context.TarjetaCreditos.Remove(tarjeta);
                await _context.SaveChangesAsync();

                return Ok(new { message = "La Tarjeta fue Eliminada con Exito" });



            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
