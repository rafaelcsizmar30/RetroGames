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

    [HttpPost]
    public async Task<IActionResult> Inserir([FromBody] InserirJogoRequestApiModel request)
    {
        var criado = await _service.InserirAsync(request.ParaModel(), request.IdsGeneros);

        var envelope = new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = criado.ParaResponsePorId(),
        };
        return CreatedAtAction(nameof(ListarPorId), new { id = criado.Jogo.Id }, envelope);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Alterar(int id, [FromBody] AlterarJogoRequestApiModel request)
    {
        await _service.AlterarAsync(request.ParaModel(id), request.IdsGeneros);
        var atualizado = await _service.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = atualizado.ParaResponsePorId(),
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        await _service.DesativarAsync(id);

        return Ok(new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO,
        });
    }
}