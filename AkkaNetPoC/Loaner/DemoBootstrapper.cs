

namespace Loaner
{
    using Nancy;
    using Nancy.TinyIoc;

    public class DemoBootstrapper : DefaultNancyBootstrapper
    {
        private readonly IAppConfiguration _appConfig;
         
        public DemoBootstrapper()
        {
        }

        public DemoBootstrapper(IAppConfiguration appConfig)
        {
            this._appConfig = appConfig;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register(_appConfig);
        }
    }
}