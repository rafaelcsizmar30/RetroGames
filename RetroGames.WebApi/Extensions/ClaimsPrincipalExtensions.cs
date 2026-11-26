using System.Security.Claims;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.WebApi.Configuration;

namespace RetroGames.WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static ClaimsPrincipal ParaClaimsPrincipal(this Usuario usuario)
    {
        var perfil = usuario.IdPerfil == PerfilIds.Administrador
            ? PerfilNomes.Administrador
            : PerfilNomes.Cliente;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, perfil),
        };

        var identidade = new ClaimsIdentity(claims, AutenticacaoConfig.Esquema);
        return new ClaimsPrincipal(identidade);
    }

    public static int? ObterIdUsuario(this ClaimsPrincipal principal)
    {
        var valor = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(valor, out var id) ? id : null;
    }
}