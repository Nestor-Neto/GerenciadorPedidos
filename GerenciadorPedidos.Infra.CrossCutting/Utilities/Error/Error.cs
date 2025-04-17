

namespace GerenciadorPedidos.Infra.CrossCutting.Utilities.Error
{
    /// <summary>
    /// Representa a lista de erros.
    /// </summary>
    public class Error
    {
        public Error(List<ErrorItem> errors)
        {
            Errors = errors;
        }

        public Error(string code, string title, string detail)
        {
            var error = new ErrorItem
            {
                Code = code,
                Title = title,
                Detail = detail
            };

            Errors.Add(error);
        }
        /// <summary>
        /// Lista de erros.
        /// </summary>
        public List<ErrorItem> Errors { get; set; } = new List<ErrorItem>();

        
    }

    /// <summary>
    /// Representa um item de erro.
    /// </summary>
    public class ErrorItem
    {
        /// <summary>
        /// Código de erro específico do endpoint.
        /// </summary>
        /// <example>string</example>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Título legível, erro específico.
        /// </summary>
        /// <example>string</example>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Descrição legível, erro específico.
        /// </summary>
        /// <example>string</example>
        public string Detail { get; set; } = string.Empty;
    }
}
