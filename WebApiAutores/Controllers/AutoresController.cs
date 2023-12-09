﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAuthorizationService _authorizationService;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration, IAuthorizationService authorizationService)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _authorizationService = authorizationService;
        }

        //[HttpGet("configuraciones")]
        //public ActionResult<string> ObtenerConfiguracion()
        //{
        //    return _configuration["apellido"];
        //}

        [HttpGet(Name = "obtenerAutores")] // api/autores
        [AllowAnonymous]
        public async Task<ColeccionDeRecursos<AutorDTO>> Get()
        {
            var autores = await _context.Autores.ToListAsync();
            var dtos = _mapper.Map<List<AutorDTO>>(autores);
            var esAdmin = await _authorizationService.AuthorizeAsync(User, "esAdmin");

            dtos.ForEach(dto => GenerarEnlace(dto, esAdmin.Succeeded));

            var resultado = new ColeccionDeRecursos<AutorDTO> { Valores = dtos };

            resultado.Enlaces.Add(new DatoHATEOAS(Url.Link("obtenerAutores", new { }), "self", "GET"));

            if (esAdmin.Succeeded)
            {
                resultado.Enlaces.Add(new DatoHATEOAS(Url.Link("crearAutor", new { }), "crear-autor", "POST"));
            }

            return resultado;
        }

        [HttpGet("{id:int}", Name = "obtenerAutor")]
        [AllowAnonymous]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var autor = await _context.Autores
                .Include(autorDB => autorDB.AutoresLibros)
                .ThenInclude(autoLibroDB => autoLibroDB.Libro)
                .FirstOrDefaultAsync(autorBD => autorBD.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            var dto = _mapper.Map<AutorDTOConLibros>(autor);
            var esAdmin = await _authorizationService.AuthorizeAsync(User, "esAdmin");
            GenerarEnlace(dto, esAdmin.Succeeded);

            return dto;
        }

        private void GenerarEnlace(AutorDTO autorDTO, bool esAdmin)
        {
            autorDTO.Enlaces.Add(new DatoHATEOAS(Url.Link("obtenerAutor", new { Id = autorDTO.Id }), "self", "GET"));
            if (esAdmin)
            {
                autorDTO.Enlaces.Add(new DatoHATEOAS(Url.Link("actualizarAutor", new { Id = autorDTO.Id }), "autor-actualizar", "PUT"));
                autorDTO.Enlaces.Add(new DatoHATEOAS(Url.Link("borrarAutor", new { Id = autorDTO.Id }), "borrar-autor", "DELETE"));
            }
        }

        [HttpGet("{nombre}", Name = "obtenerAutorePorNombre")]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute] string nombre)
        {

            var autores = await _context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();

            return _mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpPost(Name = "crearAutor")]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO)
        {
            var existeAutorConElMismoNombre = await _context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);

            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");
            }

            var autor = _mapper.Map<Autor>(autorCreacionDTO);

            _context.Add(autor);
            await _context.SaveChangesAsync();

            var autorDTO = _mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("obtenerAutor", new { id = autor.Id }, autorDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarAutor")] //api/autores/2
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
        {
            var existe = await _context.Autores.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            var autor = _mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;

            _context.Update(autor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "borrarAutor")] //api/autores/2
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
