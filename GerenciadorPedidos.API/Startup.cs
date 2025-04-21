using GerenciadorPedidos.Infra.Context;
using GerenciadorPedidos.Infra.CrossCutting.IoC;
using GerenciadorPedidos.Domain.Mappers;
using GerenciadorPedidos.Infra.CrossCutting.Middleware;
using GerenciadorPedidos.Infra.CrossCutting.Filters;
using GerenciadorPedidos.Infra.CrossCutting.Resources;
using GerenciadorPedidos.Domain.Interfaces;
using GerenciadorPedidos.Domain.Services;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GerenciadorPedidos
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// StartUp
        /// </summary>
        /// <param name="env">en</param>
        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(env.ContentRootPath)
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
           .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
           .AddEnvironmentVariables();
            Configuration = builder.Build();

            // Configura a cultura para português do Brasil
            var culture = new CultureInfo("pt-BR");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        /// <summary>
        /// ConfigureServices
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            //AutoMapper
            services.AddAutoMapper(typeof(AutoMapperSetup));

            //Lendo chaves no arquivo de configuracoes
            services.AddOptions();

            //Injecao de dependencia nativa
            NativeInjector.RegisterServices(services, Configuration);

            //Adicionar a compressão ao servico
            services.AddResponseCompression();

            // Configuração de validação e mensagens em português
            services.AddControllers(options =>
            {
                options.OutputFormatters.RemoveType<StringOutputFormatter>();
                options.Filters.Add<ValidationActionFilter>();
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            }).AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(ValidationMessages));
            });

            //Contexto Entity Framework Core
            services.AddDbContext<PedidoDbContext>(o => { o.UseInMemoryDatabase("PedidosDb"); }, ServiceLifetime.Scoped);

            // Configuração padrão do Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddHttpLogging(logging =>
            {
                logging.LoggingFields =
                HttpLoggingFields.RequestPropertiesAndHeaders |
                HttpLoggingFields.RequestBody |
                HttpLoggingFields.RequestQuery |
                HttpLoggingFields.RequestMethod |
                HttpLoggingFields.RequestProtocol |
                HttpLoggingFields.RequestPath |
                HttpLoggingFields.ResponseBody |
                HttpLoggingFields.ResponseHeaders |
                HttpLoggingFields.ResponsePropertiesAndHeaders |
                HttpLoggingFields.ResponseStatusCode;
                logging.RequestHeaders.Add("x-web-key");
            });

            // Registra as estratégias de cálculo de imposto
            services.AddScoped<ICalculoImpostoFactoryServices, CalculoImpostoFactoryServices>();
            services.AddScoped<IFeatureFlagService, FeatureFlagService>();
        }

        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSerilogRequestLogging(options =>
            {
                // Customize the message template
                options.MessageTemplate = "Handled {RequestPath} Status {StatusCode} RequestMethod {RequestMethod} Elapsed {Elapsed}";

                // Attach additional properties to the request completion event
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("ResponseStatus", httpContext.Response.StatusCode);
                    diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
                    diagnosticContext.Set("RequestPath", httpContext.Request.Path);
                };
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            // Adiciona o middleware de validação
            app.UseMiddleware<ValidationErrorMiddleware>();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            //Adicionar a compressão ao servico
            app.UseResponseCompression();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
