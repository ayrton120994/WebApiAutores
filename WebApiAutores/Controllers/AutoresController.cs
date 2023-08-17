using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IServicio _servicio;
        private readonly ServicioTransient _servicioTransient;
        private readonly ServicioScoped _servicioScoped;
        private readonly ServicioSingleton _servicioSingleton;
        private readonly ILogger<AutoresController> logger;

        public AutoresController(
            ApplicationDbContext context,
            IServicio servicio,
            ServicioTransient servicioTransient,
            ServicioScoped servicioScoped,
            ServicioSingleton servicioSingleton,
            ILogger<AutoresController> logger)
        {
            _context = context;
            _servicio = servicio;
            this._servicioTransient = servicioTransient;
            this._servicioScoped = servicioScoped;
            this._servicioSingleton = servicioSingleton;
            this.logger = logger;
        }


        [HttpGet("GUID")]
        //[ResponseCache(Duration = 10)]
        [ServiceFilter(typeof(MiFiltroDeAccion))]
        public ActionResult ObtenerGuids()
        {
            return Ok(new
            {
                AutoresControllerTransient = _servicioTransient.Guid,
                ServicioATransient = _servicio.ObtenerTransient(),
                AutoresControllerScoped = _servicioScoped.Guid,
                ServicioAScoped = _servicio.ObtenerScoped(),
                AutoresControllerSingleton = _servicioSingleton.Guid,
                ServicioASingleton = _servicio.ObtenerSingleton(),
            });

        }

        [HttpGet] // api/autores
        [HttpGet("listado")] // api/autores/listado
        [HttpGet("/listado")] // listado
        //[ResponseCache(Duration = 10)]
        //[Authorize]
        [ServiceFilter(typeof(MiFiltroDeAccion))]
        public async Task<List<Autor>> Get()
        {
            throw new NotImplementedException();
            logger.LogInformation("Estamos obteniendo los autores");
            logger.LogCritical("Estamos obteniendo los autores");
            _servicio.RealizarTarea();
            return await _context.Autores.Include(x => x.Libros).ToListAsync();
        }

        [HttpGet("primero")] // api/autores/primero
        public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int miValor, [FromQuery] string nombre)
        {
            return await _context.Autores.FirstOrDefaultAsync();
        }

        [HttpGet("{id:int}/{param2=ayrton}")]
        public async Task<ActionResult<Autor>> Get(int id, string param2)
        {
            var autor = await _context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            return autor;
        }

        [HttpGet("{nombre}")]
        public async Task<ActionResult<Autor>> Get([FromRoute] string nombre)
        {

            var autor = await _context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));

            if (autor == null)
            {
                return NotFound();
            }

            return autor;
        }


        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Autor autor)
        {
            var existeAutorConElMismoNombre = await _context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);

            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {autor.Nombre}");
            }

            _context.Add(autor);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id:int}")] //api/autores/2
        public async Task<ActionResult> Put(Autor autor, int id)
        {
            var existe = await _context.Autores.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            if (autor.Id != id)
            {
                return BadRequest("El id del autor no coincide con el id de la URL");
            }
            _context.Update(autor);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id:int}")] //api/autores/2
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await _context.Autores.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }
            _context.Remove(new Autor() { Id = id });
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
