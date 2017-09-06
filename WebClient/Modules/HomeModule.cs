using Nancy;

namespace WebClient.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get("/", args => "Hello world from nancy module.");
        }
    }
}