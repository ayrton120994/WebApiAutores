using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Test.PruebasUnitarias
{
    [TestClass]
    public class PrimeraLetraMayusculaAttributeTests
    {
        [TestMethod]
        public void PrimeraLetraMinuscula_DevuelveError()
        {
            //Preparación
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "ayrton";
            var valContext = new ValidationContext(new { Nombre = valor });

            //Ejecución
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            //Verificación
            Assert.AreEqual("La primera letra debe ser mayúscula", resultado?.ErrorMessage);
        }

        [TestMethod]
        public void ValorNulo_NoDevuelveError()
        {
            //Preparación
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string valor = null;
            var valContext = new ValidationContext(new { Nombre = valor });

            //Ejecución
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            //Verificación
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void ValorConPrimeraLetraMayuscula_NoDevuelveError()
        {
            //Preparación
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string valor = "Ayrton";
            var valContext = new ValidationContext(new { Nombre = valor });

            //Ejecución
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            //Verificación
            Assert.IsNull(resultado);
        }
    }
}