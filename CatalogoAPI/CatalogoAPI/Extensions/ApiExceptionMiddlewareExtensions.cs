using CatalogoAPI.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;


/* Esse middleware serve para "tratar" erros de forma global
 * nesse caso são erros não esperados, por exemplo erro na conexão,
 * entra no lugar do trycatch que retornava o status code 500.
 */
namespace CatalogoAPI.Extensions
{
    // tem que ser uma classe estática
    public static class ApiExceptionMiddlewareExtensions
    {
        // o método também tem que ser estático
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            // Aqui estamos usando o middleware
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    // Aqui obtemos o status code
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    // Aqui definimos o tipo de retorno que vai ser json
                    context.Response.ContentType = "application/json";

                    // Aqui obtemos informações e detalhes do erro
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            // E aqui é o retorno 
                            // Código de status 
                            StatusCode = context.Response.StatusCode,
                            // Mensagem
                            Message = contextFeature.Error.Message,
                            // Pilha de erros
                            Trace = contextFeature.Error.StackTrace
                        }.ToString());
                    }
                });
            });
        }
    }
}
