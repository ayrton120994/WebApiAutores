using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class ComentarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ComentarioController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            var existeLibro = await _context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var comentarios = await _context.Comentarios
                .Where(comentarioDB => comentarioDB.LibroId == libroId).ToListAsync();

            return _mapper.Map<List<ComentarioDTO>>(comentarios);
        }


        [HttpGet("{id:int}", Name = "obtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> GetPorId(int id, int libroId)
        {
            var comentario = await _context.Comentarios.Where(comentarioDB => comentarioDB.LibroId == libroId).FirstOrDefaultAsync(comentarioDB => comentarioDB.Id == id);

            if (comentario == null)
            {
                return NotFound();
            }

            return _mapper.Map<ComentarioDTO>(comentario);
        }

        [HttpPost]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var existeLibro = await _context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var comentario = _mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = libroId;

            _context.Add(comentario);
            await _context.SaveChangesAsync();

            var comentarioDTO = _mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("obtenerComentario", new { id = comentario.Id, libroId = libroId }, comentarioDTO);
        }
    }
}
