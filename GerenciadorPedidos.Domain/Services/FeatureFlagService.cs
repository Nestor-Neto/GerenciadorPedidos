using GerenciadorPedidos.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GerenciadorPedidos.Domain.Services
{
    /// <summary>
    /// Serviço responsável por gerenciar as Feature Flags do sistema.
    /// Implementa o padrão Feature Toggle para controlar funcionalidades em tempo de execução.
    /// </summary>
    /// <remarks>
    /// Este serviço permite:
    /// - Ativar/desativar funcionalidades sem deploy
    /// - Testes A/B
    /// - Lançamento gradual de features
    /// - Controle de funcionalidades por ambiente
    /// </remarks>
    public class FeatureFlagService : IFeatureFlagService
    {
        /// <summary>
        /// Configuração do .NET Core que contém as feature flags.
        /// Permite acesso às configurações definidas no appsettings.json
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Nome da seção no arquivo de configuração que contém as feature flags.
        /// Usado para agrupar todas as flags em uma única seção.
        /// </summary>
        private const string FeatureFlagsSection = "FeatureFlags";

        /// <summary>
        /// Construtor que inicializa o serviço com a configuração necessária.
        /// </summary>
        /// <param name="configuration">
        /// Configuração do .NET Core injetada via DI.
        /// Deve conter a seção "FeatureFlags" configurada.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Lançada se a configuration for nula
        /// </exception>
        public FeatureFlagService(IConfiguration configuration)
        {
            _configuration = configuration ?? 
                throw new ArgumentNullException(
                    nameof(configuration),
                    "A configuração é necessária para o serviço de Feature Flags");
        }

        /// <summary>
        /// Verifica se uma feature flag está habilitada.
        /// </summary>
        /// <param name="featureName">
        /// Nome da feature flag a ser verificada.
        /// Deve corresponder exatamente ao nome na configuração.
        /// </param>
        /// <returns>
        /// true se a feature está habilitada,
        /// false se está desabilitada ou não existe
        /// </returns>
        public bool IsEnabled(string featureName)
        {
            try
            {
                // Constrói o caminho completo da configuração
                // Exemplo: "FeatureFlags:ReformaTributaria"
                string configPath = $"{FeatureFlagsSection}:{featureName}";

                // Obtém o valor da feature flag
                // Se não encontrar, retorna false por padrão
                return _configuration.GetValue<bool>(configPath);
            }
            catch (Exception)
            {
                // Em caso de erro na leitura da configuração,
                // retorna false por segurança
                return false;
            }
        }

        /// <summary>
        /// Obtém todas as feature flags configuradas e seus valores.
        /// Método útil para diagnóstico e logging.
        /// </summary>
        /// <returns>
        /// Dicionário com os nomes das features e seus estados
        /// </returns>
        private Dictionary<string, bool> ObterTodasFeatureFlags()
        {
            var flags = new Dictionary<string, bool>();
            var section = _configuration.GetSection(FeatureFlagsSection);
            
            foreach (var child in section.GetChildren())
            {
                flags[child.Key] = section.GetValue<bool>(child.Key);
            }

            return flags;
        }

        /// <summary>
        /// Verifica se uma feature flag está configurada,
        /// independente de seu valor.
        /// </summary>
        /// <param name="featureName">Nome da feature flag</param>
        /// <returns>
        /// true se a feature está configurada,
        /// false se não existe na configuração
        /// </returns>
        private bool FeatureExiste(string featureName)
        {
            string configPath = $"{FeatureFlagsSection}:{featureName}";
            var section = _configuration.GetSection(configPath);
            return section.Exists();
        }

        /// <summary>
        /// Valida se uma feature flag está corretamente configurada.
        /// Verifica a existência e o tipo do valor.
        /// </summary>
        /// <param name="featureName">Nome da feature flag</param>
        /// <returns>
        /// true se a configuração é válida,
        /// false se há algum problema
        /// </returns>
        private bool ValidarFeatureFlag(string featureName)
        {
            try
            {
                string configPath = $"{FeatureFlagsSection}:{featureName}";
                var value = _configuration.GetValue<bool?>(configPath);
                return value.HasValue;
            }
            catch
            {
                return false;
            }
        }
    }
} 