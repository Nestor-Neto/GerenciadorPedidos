using GerenciadorPedidos.Infra.CrossCutting.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GerenciadorPedidos.Infra.CrossCutting.Filters
{
    /// <summary>
    /// Filtro de ação que intercepta requisições para validar o ModelState,
    /// convertendo erros de validação em exceções com mensagens em português.
    /// </summary>
    public class ValidationActionFilter : IActionFilter, IFilterMetadata
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                throw new ValidationException(errors);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Não é necessário implementar nada aqui
        }
    }
} 