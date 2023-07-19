namespace WebApiAutores.Servicios
{
    public interface IServicio
    {
        public Guid ObtenerTransient();
        public Guid ObtenerSingleton();
        public Guid ObtenerScoped();
        public void RealizarTarea();
    }

    public class ServicioA : IServicio
    {
        private readonly ILogger<ServicioA> logger;
        private readonly ServicioTransient _servicioTransient;
        private readonly ServicioSingleton _servicioSingleton;
        private readonly ServicioScoped _servicioScoped;

        public ServicioA(ILogger<ServicioA> logger, ServicioTransient servicioTransient,
            ServicioSingleton servicioSingleton, ServicioScoped servicioScoped
            )
        {
            this.logger = logger;
            this._servicioTransient = servicioTransient;
            this._servicioSingleton = servicioSingleton;
            this._servicioScoped = servicioScoped;
        }

        public Guid ObtenerTransient() { return _servicioTransient.Guid; }
        public Guid ObtenerSingleton() { return _servicioSingleton.Guid; }
        public Guid ObtenerScoped() { return _servicioScoped.Guid; }

        public void RealizarTarea()
        {
            //throw new NotImplementedException();
        }
    }

    public class ServicioB : IServicio
    {
        public Guid ObtenerScoped()
        {
            throw new NotImplementedException();
        }

        public Guid ObtenerSingleton()
        {
            throw new NotImplementedException();
        }

        public Guid ObtenerTransient()
        {
            throw new NotImplementedException();
        }

        public void RealizarTarea()
        {
            //throw new NotImplementedException();
        }
    }

    public class ServicioTransient
    {
        public Guid Guid = Guid.NewGuid();
    }
    public class ServicioScoped
    {
        public Guid Guid = Guid.NewGuid();
    }
    public class ServicioSingleton
    {
        public Guid Guid = Guid.NewGuid();
    }

}
