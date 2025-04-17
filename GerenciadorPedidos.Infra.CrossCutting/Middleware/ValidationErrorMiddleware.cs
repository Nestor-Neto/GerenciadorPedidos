using System.Text.Json;
using GerenciadorPedidos.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorPedidos.Infra.CrossCutting.Middleware
{
    /// <summary>
    /// Middleware responsável por interceptar e tratar exceções de validação,
    /// convertendo-as em respostas HTTP padronizadas em português.
    /// </summary>
    public class ValidationErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = new ValidationErrorResponse();

            if (exception is ValidationException validationException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Errors = validationException.Errors;
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Title = "Ocorreu um erro interno no servidor.";
                response.Status = 500;
            }

            response.TraceId = context.TraceIdentifier;
            var result = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(result);
        }
    }

    /// <summary>
    /// Exceção personalizada para encapsular erros de validação com suas mensagens em português.
    /// </summary>
    public class ValidationException : Exception
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(Dictionary<string, string[]> errors)
        {
            Errors = errors;
        }
    }
} 