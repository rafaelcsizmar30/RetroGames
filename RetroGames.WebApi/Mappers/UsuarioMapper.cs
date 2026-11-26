using System.Security.Claims;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Mappers;

public static class UsuarioMapper
{
    public static UsuarioLogadoResponseApiModel ParaResponse(this Usuario usuario)
    {
        return new UsuarioLogadoResponseApiModel
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.IdPerfil == PerfilIds.Administrador
                ? PerfilNomes.Administrador
                : PerfilNomes.Cliente,
        };
    }

    public static UsuarioLogadoResponseApiModel ParaResponse(this ClaimsPrincipal principal)
    {
        return new UsuarioLogadoResponseApiModel
        {
            Id = principal.ObterIdUsuario() ?? 0,
            Nome = principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
            Email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            Perfil = principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty,
        };
    }
}