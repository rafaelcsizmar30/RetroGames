using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _service;

    public JogoController(IJogoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] ListarJogoRequestApiModel request)
    {
        var pagina = request.Pagina < 1 ? 1 : request.Pagina;
        var tamanho = request.TamanhoPagina < 1 ? 12 : request.TamanhoPagina;

        var (itens, total) = await _service.ListarAsync(
            request.Busca, request.IdPlataforma, request.IdGenero, pagina, tamanho);

        var dados = new PaginaResponseApiModel<ListarJogoResponseApiModel>
        {
            Itens = itens.Select(i => i.ParaResponse()).ToList(),
            Pagina = pagina,
            TamanhoPagina = tamanho,
            TotalItens = total,
            TotalPaginas = (int)Math.Ceiling(total / (double)tamanho),
        };

        return Ok(new RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = dados,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ListarPorId(int id)
    {
        var detalhe = await _service.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = detalhe.ParaResponsePorId(),
        });
    }
}