namespace CatalogoAPI.Logging
{
    // Implementa a interface ILogger
    public class CustomerLogger : ILogger
    {
        readonly string loggerName;
        readonly CustomLoggerProviderConfiguration loggerConfig;

        public CustomerLogger(string name, CustomLoggerProviderConfiguration config)
        {
            loggerName = name;
            loggerConfig = config;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == loggerConfig.LogLevel;
        }

        // Método da própria interface que o sistema irá executar com nosso código
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception exception, Func<TState, Exception, string> formatter)
        {
            // Formatando a mensagem do log
            string mensagem = $"{logLevel.ToString()}: {eventId.Id} - {formatter(state, exception)}";

            EscreverTextoNoArquivo(mensagem);
        }

        private void EscreverTextoNoArquivo(string mensagem)
        {
            // caminho onde vai salvar o log
            // obs: curiosamente desta forma pega C:\Users\Daniel\Documents...
            // que é a padrão, MAS, minha  pasta documents está setada para D:\
            string caminhoArquivoLog = Environment
                .ExpandEnvironmentVariables(@"%userprofile%\Documents\TestesLogs\CatalogoAPI_Log.txt");

            // Escopo instanciando o StreamWriter que estamos usando para escrever no arquivo de texto.
            using (StreamWriter streamWriter = new StreamWriter(caminhoArquivoLog, true))
            {
                try
                {
                    // Escreve mensagem no arquivo de texto.
                    streamWriter.WriteLine(mensagem);
                    streamWriter.Close();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
