using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlataformaController : ControllerBase
{
    private readonly IPlataformaService _service;

    public PlataformaController(IPlataformaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var plataformas = await _service.ListarAsync();
        var dados = plataformas.Select(p => p.ParaResponse()).ToList();

        return Ok(new RetroGamesResponseApiModel<List<ListarPlataformaResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = dados,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ListarPorId(int id)
    {
        var plataforma = await _service.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = plataforma.ParaResponse(),
        });
    }

    [HttpPost]
    public async Task<IActionResult> Inserir([FromBody] InserirPlataformaRequestApiModel request)
    {
        var criada = await _service.InserirAsync(request.ParaModel());

        var envelope = new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = criada.ParaResponse(),
        };
        return CreatedAtAction(nameof(ListarPorId), new { id = criada.Id }, envelope);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Alterar(int id, [FromBody] AlterarPlataformaRequestApiModel request)
    {
        await _service.AlterarAsync(request.ParaModel(id));
        var atualizada = await _service.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = atualizada.ParaResponse(),
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        await _service.ExcluirAsync(id);

        return Ok(new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO,
        });
    }
}