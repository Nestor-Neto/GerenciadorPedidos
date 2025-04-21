using System;

namespace GerenciadorPedidos.Domain.Exceptions
{
    /// <summary>
    /// Exceção base para erros de domínio. Representa violações de regras de negócio e validações do domínio.
    /// Deve ser usada quando uma operação viola as regras de negócio estabelecidas.
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
