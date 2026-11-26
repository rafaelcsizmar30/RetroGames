using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Extensions;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _service;

    public AutenticacaoController(IAutenticacaoService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestApiModel request)
    {
        var usuario = await _service.AutenticarAsync(request.Email, request.Senha);
        if (usuario == null)
        {
            return Unauthorized(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.CREDENCIAIS_INVALIDAS.Code,
                ErrorMessage = ResponseCodes.CREDENCIAIS_INVALIDAS.DefaultMessage,
            });
        }

        await HttpContext.SignInAsync(AutenticacaoConfig.Esquema, usuario.ParaClaimsPrincipal());

        return Ok(new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = usuario.ParaResponse(),
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(AutenticacaoConfig.Esquema);

        return Ok(new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO,
        });
    }

    [HttpGet("eu")]
    [Authorize]
    public IActionResult Eu()
    {
        return Ok(new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = User.ParaResponse(),
        });
    }
}