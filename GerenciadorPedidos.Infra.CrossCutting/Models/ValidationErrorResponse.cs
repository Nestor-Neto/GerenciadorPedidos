namespace GerenciadorPedidos.Infra.CrossCutting.Models
{
    /// <summary>
    /// Classe responsável por padronizar o formato de resposta de erros de validação em português,
    /// seguindo o padrão solicitado com título, status, erros e traceId.
    /// </summary>
    public class ValidationErrorResponse
    {
        public string Title { get; set; } = "Ocorreram um ou mais erros de validação.";
        public int Status { get; set; } = 400;
        public Dictionary<string, string[]> Errors { get; set; } = new();
        public string TraceId { get; set; } = string.Empty;
    }
} 