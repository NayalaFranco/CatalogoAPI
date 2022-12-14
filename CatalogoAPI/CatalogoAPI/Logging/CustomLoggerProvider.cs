using System.Collections.Concurrent;

namespace CatalogoAPI.Logging
{
    // Implementa a interface ILoggerProvider
    public class CustomLoggerProvider : ILoggerProvider
    {
        readonly CustomLoggerProviderConfiguration loggerConfig;

        readonly ConcurrentDictionary<string, CustomerLogger> loggers =
                 new ConcurrentDictionary<string, CustomerLogger>();

        // O construtor recebe e define a nossa configuração do logger
        public CustomLoggerProvider(CustomLoggerProviderConfiguration config)
        {
            loggerConfig = config;
        }

        // CreateLogger cria uma instancia do meu log customizado
        // por nome de categoria e vai armazenar isso em um dicionario
        // para isso a gente define a instancia acima de CurrentDictionary
        public ILogger CreateLogger(string categoryName)
        {
            // e aqui a gente define o nosso logger a ser instanciado.
            return loggers.GetOrAdd(categoryName, name => new CustomerLogger(name, loggerConfig));
        }

        // Libera os recursos depois que não precisa mais usar.
        public void Dispose()
        {
            loggers.Clear();
        }
    }
}
