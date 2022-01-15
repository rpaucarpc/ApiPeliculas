using ApiPeliculas.Data;
using ApiPeliculas.Helpers;
using ApiPeliculas.PeliculasMapper;
using ApiPeliculas.Repository;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApiPeliculas
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
            services.AddDbContext<ApplicationDbContext>(Options => Options.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection")));
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IPeliculaRepository, PeliculaRepositorio>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            // Agregar dependencia de token
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddAutoMapper(typeof(PeliculasMappers));

            // De aqui en adelante configuracion de documentacion de nuestra API
            services.AddSwaggerGen( options => {
                options.SwaggerDoc("ApiPeliculasCategorias", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "API Categorias Peliculas",
                    Version = "1",
                    Description = "Backend peliculas",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "admin@gmail.com",
                        Name = "AdminAdmin",
                        Url = new Uri("https://youtube.com")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_license")
                    }
                });
                options.SwaggerDoc("ApiPeliculas", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "API Peliculas",
                    Version = "1",
                    Description = "Backend peliculas",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "admin@gmail.com",
                        Name = "AdminAdmin",
                        Url = new Uri("https://youtube.com")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_license")
                    }
                });
                options.SwaggerDoc("ApiPeliculasUsuarios", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "API Usuarios Peliculas",
                    Version = "1",
                    Description = "Backend peliculas",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "admin@gmail.com",
                        Name = "AdminAdmin",
                        Url = new Uri("https://youtube.com")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_license")
                    }
                });
                var archivoXmlComentarios = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var rutaApiComentarios = Path.Combine(AppContext.BaseDirectory, archivoXmlComentarios);
                options.IncludeXmlComments(rutaApiComentarios);

                // Definir esquema de seguridad
                options.AddSecurityDefinition("Bearer",
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                { 
                  Description = "Autenticacion JWT (Bearer)",
                  Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                  Scheme = "bearer"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement 
                {
                    { 
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        }, new List<string>()
                    }
                });
            });

            services.AddControllers();
            // Damos soporte para CORS
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler( builder => {
                    builder.Run(async context => {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();

                        if ( error != null )
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }
            // Linea para documentacion api
            app.UseSwagger();
            app.UseSwaggerUI( options => {
                options.SwaggerEndpoint("/swagger/ApiPeliculasCategorias/swagger.json","API Categorias Peliculas");
                options.SwaggerEndpoint("/swagger/ApiPeliculas/swagger.json", "API Peliculas");
                options.SwaggerEndpoint("/swagger/ApiPeliculasUsuarios/swagger.json", "API Usuarios Peliculas");
                options.RoutePrefix = "";
            });

            app.UseRouting();

            // Estos son para la autenticacion y autorizacion
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Damos soporte para CORS
            app.UseCors( x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader() );
        }
    }
}
