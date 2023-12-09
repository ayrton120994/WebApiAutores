using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get()
        {
            var datosHateoas = new List<DatoHATEOAS>();

            datosHateoas.Add(new DatoHATEOAS(Url.Link("ObtenerRoot", new { }), "self", "GET"));
            datosHateoas.Add(new DatoHATEOAS(Url.Link("obtenerAutores", new { }), "autores", "GET"));

            var esAdmin = await _authorizationService.AuthorizeAsync(User, "esAdmin");

            if (esAdmin.Succeeded)
            {
                datosHateoas.Add(new DatoHATEOAS(Url.Link("crearAutor", new { }), "autor-crear", "POST"));
                datosHateoas.Add(new DatoHATEOAS(Url.Link("crearLibro", new { }), "libro-crear", "POST"));
            }
            return datosHateoas;
        }
    }
}
