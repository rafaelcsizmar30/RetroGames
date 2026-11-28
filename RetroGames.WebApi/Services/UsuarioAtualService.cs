using System.Security.Claims;
using RetroGames.Model.Constantes;
using RetroGames.Services;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Services;

public class UsuarioAtualService : IUsuarioAtualService
{
    private readonly IHttpContextAccessor _accessor;

    public UsuarioAtualService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    private ClaimsPrincipal? Principal => _accessor.HttpContext?.User;

    public bool EstaLogado => Principal?.Identity?.IsAuthenticated == true;

    public int? Id => EstaLogado ? Principal!.ObterIdUsuario() : null;

    public bool EhAdministrador => EstaLogado && Principal!.IsInRole(PerfilNomes.Administrador);
}