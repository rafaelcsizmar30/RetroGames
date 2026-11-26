using RetroGames.WebApi.Middlewares;
using RetroGames.WebApi.Configuration;
using Swashbuckle.AspNetCore.SwaggerUI;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RetroGames.WebApi.ActionFilters;
using RetroGames.WebApi.ApiModels;

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
var app = builder.Build();
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

app.MapControllers();

app.Run();