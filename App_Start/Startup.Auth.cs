using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;
using Owin;
using WebShop.Models.Entity;

namespace WebShop
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            //app.CreatePerOwinContext(WebShopEntities.Create);
          
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
        }
    }
}