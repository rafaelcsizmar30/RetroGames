using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        RetroGamesResponseApiModel resposta;

        if (exception is BusinessRuleException regra)
        {
            httpContext.Response.StatusCode = (int)regra.Erro.Status;
            resposta = new RetroGamesResponseApiModel
            {
                ResponseCode = regra.Erro.Codigo,
                ErrorMessage = regra.Erro.Mensagem,
            };
        }
        else
        {
            _logger.LogError(exception, "Erro inesperado ao processar a requisição.");

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            resposta = new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.ERRO,
                ErrorMessage = "Ocorreu um erro inesperado. Tente novamente mais tarde.",
            };
        }

        await httpContext.Response.WriteAsJsonAsync(resposta, cancellationToken);
        return true;
    }
}