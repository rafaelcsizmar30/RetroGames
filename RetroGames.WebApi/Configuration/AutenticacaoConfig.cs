using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Configuration;

public static class AutenticacaoConfig
{
    public const string Esquema = CookieAuthenticationDefaults.AuthenticationScheme;

    public static IServiceCollection AddAutenticacaoPorCookie(this IServiceCollection services)
    {
        services
            .AddAuthentication(Esquema)
            .AddCookie(opcoes =>
            {
                opcoes.Cookie.Name = "retrogames.sessao";
                opcoes.Cookie.HttpOnly = true;
                opcoes.Cookie.SameSite = SameSiteMode.Strict;
                opcoes.ExpireTimeSpan = TimeSpan.FromHours(2);
                opcoes.SlidingExpiration = true;

                opcoes.Events.OnRedirectToLogin = contexto =>
                    EscreverErro(contexto.Response, StatusCodes.Status401Unauthorized,
                        ResponseCodes.NAO_AUTENTICADO);

                opcoes.Events.OnRedirectToAccessDenied = contexto =>
                    EscreverErro(contexto.Response, StatusCodes.Status403Forbidden,
                        ResponseCodes.ACESSO_NEGADO);
            });

        services.AddAuthorization();

        return services;
    }

    private static Task EscreverErro(HttpResponse resposta, int status, (int Code, string DefaultMessage) erro)
    {
        resposta.StatusCode = status;
        return resposta.WriteAsJsonAsync(new RetroGamesResponseApiModel
        {
            ResponseCode = erro.Code,
            ErrorMessage = erro.DefaultMessage,
        });
    }
}