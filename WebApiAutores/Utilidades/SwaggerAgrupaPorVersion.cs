using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApiAutores.Utilidades
{
    public class SwaggerAgrupaPorVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var namespaceControlador = controller.ControllerType.Namespace; // Controllers.V1
            var versionApi = namespaceControlador.Split('.').Last().ToLower();

            controller.ApiExplorer.GroupName = versionApi;
        }
    }
}
