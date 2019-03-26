using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Jwt_WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Добавить параметры валидации токенов
            var tokenParams = new TokenValidationParameters()
            {
                // укзывает, будет ли валидироваться издатель при валидации токена
                ValidateIssuer = true,
                // будет ли валидироваться время существования
                ValidateLifetime = true,
                // будет ли валидироваться потребитель токена
                ValidateAudience = true,
                // строка, представляющая издателя
                ValidIssuer = Configuration["JWT:issuer"],
                // установка потребителя токена
                ValidAudience = Configuration["JWT:audience"],
                // установка ключа безопасности
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:key"])),
                // валидация ключа безопасности
                ValidateIssuerSigningKey = true
            };

            // Add JWT Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
                AddJwtBearer(jwtconfig =>
                {
                    jwtconfig.TokenValidationParameters = tokenParams;
                });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
