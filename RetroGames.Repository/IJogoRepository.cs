using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IJogoRepository
{
    Task<(List<Jogo> Itens, int Total)> ListarAsync(
        string? busca, int? idPlataforma, List<int>? ids,
        bool incluirInativos, int pagina, int tamanhoPagina);
    Task<Jogo?> BuscarPorIdAsync(int id);
    Task<int> InserirAsync(Jogo jogo);
    Task AlterarAsync(Jogo jogo);
    Task DesativarAsync(int id);
}