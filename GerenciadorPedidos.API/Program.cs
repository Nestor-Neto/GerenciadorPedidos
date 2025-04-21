using GerenciadorPedidos.API;
using GerenciadorPedidos.API.Swagger;
using GerenciadorPedidos.Domain.Interfaces.IServices;
using GerenciadorPedidos.Domain.Services;
using GerenciadorPedidos.Domain.Interfaces.IRepositories;
using GerenciadorPedidos.Domain.Interfaces;
using GerenciadorPedidos.Infra.Repositories;
using GerenciadorPedidos.Domain.Mappers;
using GerenciadorPedidos.Infra.Context;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorPedidos.API
{
    /// <summary>
    /// Program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>                
        [STAThread]
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            
            // Configuração personalizada do Swagger
            builder.Services.AddSwaggerConfiguration();
            builder.Services.AddSwaggerGen(c => 
            {
                c.EnableAnnotations();
            });

            // Configuração do AutoMapper
            builder.Services.AddAutoMapper(typeof(AutoMapperSetup));

            // Configuração do DbContext
            builder.Services.AddDbContext<PedidoDbContext>(options =>
                options.UseInMemoryDatabase("PedidosDb"));

            // Registro dos serviços
            builder.Services.AddScoped<IPedidoService, PedidoService>();
            builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
            builder.Services.AddScoped<ICalculadoraImpostoService, CalculadoraImpostoService>();
            builder.Services.AddScoped<ISistemaBService, SistemaBService>();
            builder.Services.AddScoped<ICalculoImpostoFactoryServices, CalculoImpostoFactoryServices>();
            builder.Services.AddScoped<IFeatureFlagService, FeatureFlagService>();

            // Registro das estratégias de cálculo de imposto
            builder.Services.AddScoped<CalculoImpostoVigenteStrategyServices>();
            builder.Services.AddScoped<CalculoImpostoReformaStrategyServices>();

            // Configure Serilog
            builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
                .Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("/healthcheck")))
                .WriteTo.Console()
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerConfiguration();
                
                // Redireciona a rota raiz para o Swagger quando em desenvolvimento
                app.MapGet("/", context =>
                {
                    context.Response.Redirect("/swagger");
                    return Task.CompletedTask;
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}