namespace CatalogoAPI.Logging
{
    public class CustomLoggerProviderConfiguration
    {
        // Represenda o nivel do log, pré carregada com warning,
        // mas quando o sistema começar salvar ela ja é sobrescrita
        // com o nivel do que estiver sendo logado.
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;
        // Representa o Id do nossos eventos, pré carregado com 0.
        public int EventId { get; set; } = 0;

        // Dependendo do objetivo podemos colocar mais propriedades.
    }
}
