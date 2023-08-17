using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAutores.Filtros
{
    public class MiFiltroDeAccion : IActionFilter
    {
        private readonly ILogger<MiFiltroDeAccion> logger;

        public MiFiltroDeAccion(ILogger<MiFiltroDeAccion> _logger)
        {
            this.logger = _logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            logger.LogInformation("Después de ejecutar la acción");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation("Antes de ejecutar la acción");
        }
    }
}
