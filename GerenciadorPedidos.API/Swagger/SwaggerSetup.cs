using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GerenciadorPedidos.API.Swagger
{
    /// <summary>
    /// SwaggerSetup
    /// </summary>
    public static class SwaggerSetup
    {
        /// <summary>
        /// AddSwaggerConfiguration
        /// </summary>
        /// <param name="services">services</param>
        /// <returns></returns>
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            return services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Gerenciador de Pedidos API",
                    Version = "v1",
                    Description = "API para gerenciamento de pedidos",
                    Contact = new OpenApiContact
                    {
                        Name = "Equipe de Desenvolvimento",
                        Email = "dev@empresa.com"
                    }
                });

                // Configura o caminho do arquivo XML
                var xmlFiles = new[] 
                { 
                    "GerenciadorPedidos.API.xml",
                    "GerenciadorPedidos.Domain.xml"
                };

                foreach (var xmlFile in xmlFiles)
                {
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }
                }
                
                // Configurações adicionais
                c.UseInlineDefinitionsForEnums();
                c.EnableAnnotations();
                c.OrderActionsBy(apiDesc => apiDesc.RelativePath);
            });
        }

        /// <summary>
        /// UseSwaggerConfiguration
        /// </summary>
        /// <param name="app">app</param>
        /// <returns></returns>
        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gerenciador de Pedidos API v1");
                c.RoutePrefix = "swagger";
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                c.DefaultModelsExpandDepth(1);
                c.DisplayRequestDuration();
            });

            return app;
        }
    }
} 