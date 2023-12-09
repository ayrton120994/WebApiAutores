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

            var autorDTO = resultado.Value as AutorDTO;
            if (autorDTO == null)
            {
                var autoresDTO = resultado.Value as List<AutorDTO> ??
                    throw new ArgumentException("Se esperaba una instancia de AutorDTO o List<AutorDTO>");

                autoresDTO.ForEach(async autor => await _generadorEnlaces.GenerarEnlace(autor));

                resultado.Value = autoresDTO;
            }
            else
            {
                await _generadorEnlaces.GenerarEnlace(autorDTO);
            }

            await next();
        }
    }
}
