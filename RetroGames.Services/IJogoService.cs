using RetroGames.Model.Model;

namespace RetroGames.Services;

public interface IJogoService
{
    Task<(List<JogoDetalheServiceModel> Itens, int Total)> ListarAsync(
        string? busca, int? idPlataforma, int? idGenero, bool incluirInativos, int pagina, int tamanhoPagina);
    Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id);
    Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo, List<int> idsGeneros);
    Task AlterarAsync(Jogo jogo, List<int> idsGeneros);
    Task DesativarAsync(int id);
}