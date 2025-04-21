using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using GerenciadorPedidos.Domain.Interfaces.IServices;

namespace GerenciadorPedidos.Infra.Services
{
    /// <summary>
    /// Implementação do serviço de cache usando IMemoryCache
    /// Adequado para ambiente de desenvolvimento e testes
    /// Para produção, considerar usar Redis ou similar
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheService> _logger;

        public MemoryCacheService(
            IMemoryCache cache,
            ILogger<MemoryCacheService> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<T?> ObterAsync<T>(string key) where T : class
        {
            try
            {
                var value = _cache.Get<T>(key);
                _logger.LogDebug(
                    "Cache {Result} para chave: {Key}", 
                    value != null ? "HIT" : "MISS", 
                    key);
                
                return Task.FromResult(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter do cache: {Key}", key);
                return Task.FromResult<T?>(null);
            }
        }

        public Task ArmazenarAsync<T>(string key, T value, TimeSpan expiracao) where T : class
        {
            try
            {
                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(expiracao)
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                _cache.Set(key, value, options);
                _logger.LogDebug("Valor armazenado em cache: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao armazenar em cache: {Key}", key);
            }

            return Task.CompletedTask;
        }

        public Task RemoverAsync(string key)
        {
            try
            {
                _cache.Remove(key);
                _logger.LogDebug("Valor removido do cache: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover do cache: {Key}", key);
            }

            return Task.CompletedTask;
        }
    }
} 