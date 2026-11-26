using RetroGames.WebApi.Middlewares;
using RetroGames.WebApi.Configuration;
using Swashbuckle.AspNetCore.SwaggerUI;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RetroGames.WebApi.ActionFilters;
using RetroGames.WebApi.ApiModels;
using RetroGames.Services;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "A connection string não foi encontrada. Defina ConnectionStrings__Default no arquivo .env, na raiz do repositório.");
}

builder.Services.AddRepositorios(connectionString);
builder.Services.AddServicosDeNegocio();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services
    .AddControllers(opcoes => opcoes.Filters.Add<FluentValidationFilter>())
    .ConfigureApiBehaviorOptions(opcoes =>
    {
        opcoes.InvalidModelStateResponseFactory = _ =>
            new BadRequestObjectResult(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
                ErrorMessage = ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage,
            });
    });
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddAutenticacaoPorCookie();
var app = builder.Build();
var emailAdmin = builder.Configuration["AdminInicial:Email"];
var senhaAdmin = builder.Configuration["AdminInicial:Senha"];
if (string.IsNullOrWhiteSpace(emailAdmin) || string.IsNullOrWhiteSpace(senhaAdmin))
{
    throw new InvalidOperationException(
        "O administrador inicial não foi configurado. Defina AdminInicial__Email e AdminInicial__Senha no arquivo .env.");
}

using (var escopo = app.Services.CreateScope())
{
    var autenticacao = escopo.ServiceProvider.GetRequiredService<IAutenticacaoService>();
    await autenticacao.GarantirAdministradorInicialAsync(emailAdmin, senhaAdmin);
}
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opcoes =>
    {
        opcoes.SwaggerEndpoint("/openapi/v1.json", "RetroGames API v1");
        opcoes.RoutePrefix = "swagger";
    });
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();