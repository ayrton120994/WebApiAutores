using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using WebApiAutores.DTOs;
using WebApiAutores.Servicios;

namespace WebApiAutores.Utilidades
{
    public class HATEOASAutorFilterAttribute : HATEOASFiltroAttribute
    {
        private readonly GeneradorEnlaces _generadorEnlaces;

        public HATEOASAutorFilterAttribute(GeneradorEnlaces generadorEnlaces)
        {
            _generadorEnlaces = generadorEnlaces;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            bool debeIncluir = DebeIncluirHATEOAS(context);

            if (!debeIncluir)
            {
                await next();
                return;
            }

            var resultado = context.Result as ObjectResult;
            var modelo = resultado.Value as AutorDTO ?? throw new ArgumentNullException("Se esperaba una instancia de AutorDTO");

            await _generadorEnlaces.GenerarEnlace(modelo);

            await next();
        }
    }
}
