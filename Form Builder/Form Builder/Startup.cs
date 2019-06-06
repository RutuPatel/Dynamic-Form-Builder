using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Form_Builder.Startup))]
namespace Form_Builder
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
