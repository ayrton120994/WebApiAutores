using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using WebApiAutores.DTOs;

namespace WebApiAutores.Servicios
{
    public class GeneradorEnlaces
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;

        public GeneradorEnlaces(IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor
            )
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
        }

        private IUrlHelper ConstruirURLHelper()
        {
            var factoria = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();

            return factoria.GetUrlHelper(_actionContextAccessor.ActionContext);
        }

        private async Task<bool> EsAdmin()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var resultado = await _authorizationService.AuthorizeAsync(httpContext.User, "esAdmin");

            return resultado.Succeeded;
        }

        public async Task GenerarEnlace(AutorDTO autorDTO)
        {
            var esAdmin = await EsAdmin();
            var Url = ConstruirURLHelper();

            autorDTO.Enlaces.Add(new DatoHATEOAS(Url.Link("obtenerAutor", new { Id = autorDTO.Id }), "self", "GET"));
            if (esAdmin)
            {
                autorDTO.Enlaces.Add(new DatoHATEOAS(Url.Link("actualizarAutor", new { Id = autorDTO.Id }), "autor-actualizar", "PUT"));
                autorDTO.Enlaces.Add(new DatoHATEOAS(Url.Link("borrarAutor", new { Id = autorDTO.Id }), "borrar-autor", "DELETE"));
            }
        }
    }
}
