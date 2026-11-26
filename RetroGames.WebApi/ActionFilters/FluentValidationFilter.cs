using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.ActionFilters;

public class FluentValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _provider;

    public FluentValidationFilter(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argumento in context.ActionArguments.Values)
        {
            if (argumento == null)
            {
                context.Result = new BadRequestObjectResult(new RetroGamesResponseApiModel
                {
                    ResponseCode = ResponseCodes.MODEL_NULL.Code,
                    ErrorMessage = ResponseCodes.MODEL_NULL.DefaultMessage,
                });
                return;
            }

            var tipoValidator = typeof(IValidator<>).MakeGenericType(argumento.GetType());
            if (_provider.GetService(tipoValidator) is not IValidator validator)
            {
                continue;
            }

            var resultado = await validator.ValidateAsync(new ValidationContext<object>(argumento));
            if (resultado.IsValid)
            {
                continue;
            }

            var erros = resultado.Errors
                .GroupBy(e => ParaCamelCase(e.PropertyName))
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            context.Result = new BadRequestObjectResult(new RetroGamesResponseApiModel<Dictionary<string, string[]>>
            {
                ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
                ErrorMessage = ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage,
                Data = erros,
            });
            return;
        }

        await next();
    }

    private static string ParaCamelCase(string nome)
    {
        return string.IsNullOrEmpty(nome) ? nome : char.ToLowerInvariant(nome[0]) + nome.Substring(1);
    }
}