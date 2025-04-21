namespace GerenciadorPedidos.Domain.Interfaces.IServices
{
    /// <summary>
    /// Interface para serviço de cache
    /// Utilizada para otimizar consultas frequentes
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Obtém um item do cache
        /// </summary>
        /// <typeparam name="T">Tipo do item</typeparam>
        /// <param name="key">Chave do item no cache</param>
        /// <returns>Item do cache ou null se não encontrado</returns>
        Task<T?> ObterAsync<T>(string key) where T : class;

        /// <summary>
        /// Armazena um item no cache
        /// </summary>
        /// <typeparam name="T">Tipo do item</typeparam>
        /// <param name="key">Chave para armazenamento</param>
        /// <param name="value">Valor a ser armazenado</param>
        /// <param name="expiracao">Tempo de expiração do item</param>
        Task ArmazenarAsync<T>(string key, T value, TimeSpan expiracao) where T : class;

        /// <summary>
        /// Remove um item do cache
        /// </summary>
        /// <param name="key">Chave do item a ser removido</param>
        Task RemoverAsync(string key);
    }
} 