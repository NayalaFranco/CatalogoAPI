namespace CatalogoAPI.Pagination
{
    public class QueryStringParameters
    {
        const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                // Aqui vai verificar se o valor informador é maior que os 50 da const,
                // se for ele atribui 50, senão ele atribui o valor que foi passado.
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}
