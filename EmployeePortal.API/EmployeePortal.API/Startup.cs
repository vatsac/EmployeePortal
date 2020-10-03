using System;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Owin;

using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using AutoMapper;
using EmployeePortal.API.App_Start;

[assembly: OwinStartup(typeof(EmployeePortal.API.Startup))]

namespace EmployeePortal.API
{
    public class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {

            //app.UseJwtBearerAuthentication(
            //    new JwtBearerAuthenticationOptions
            //    {

            //        TokenValidationParameters = new TokenValidationParameters()
            //        {
            //            ValidateIssuer = true,
            //            ValidateAudience = true,
            //            ValidateIssuerSigningKey = true,
            //            ValidIssuer = "http://localhost:4200", //some string, normally web url,  
            //            ValidAudience = "http://localhost:4200",
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my_secret_key_12345"))
            //        }
            //    });

            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("my_secret_key_12345")),
                     ValidateIssuer = false,
                     ValidateAudience = false
                            }
                });
        

        HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);

        }
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888


            ConfigureAuth(app);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            Mapper.Initialize(c => c.AddProfile<MappingProfile>());

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);

            //GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
        }
    }
}
