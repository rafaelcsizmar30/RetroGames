using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private readonly IRepositorySet _repositorio;

    public JogoService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<(List<JogoDetalheServiceModel> Itens, int Total)> ListarAsync(
        string? busca, int? idPlataforma, int? idGenero, int pagina, int tamanhoPagina)
    {
        List<int>? ids = null;
        if (idGenero.HasValue)
        {
            var vinculos = await _repositorio.JogoGenero.ListarTodosAsync();
            ids = vinculos
                .Where(v => v.IdGenero == idGenero.Value)
                .Select(v => v.IdJogo)
                .Distinct()
                .ToList();
        }

        var (jogos, total) = await _repositorio.Jogo.ListarAsync(
            busca, idPlataforma, ids, false, pagina, tamanhoPagina);

        var detalhes = await MontarDetalhesAsync(jogos);
        return (detalhes, total);
    }

    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);
        if (jogo == null)
        {
            throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
        }

        var detalhes = await MontarDetalhesAsync(new List<Jogo> { jogo });
        return detalhes[0];
    }

    public async Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo, List<int> idsGeneros)
    {
        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;
        jogo.DataCadastro = DateTime.UtcNow;
        var idsDistintos = idsGeneros.Distinct().ToList();

        await ConferirReferenciasAsync(jogo, idsDistintos);

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            jogo.Id = await _repositorio.Jogo.InserirAsync(jogo);
            await _repositorio.JogoGenero.InserirAsync(jogo.Id, idsDistintos);
            transactionScope.Complete();
        }

        return await BuscarPorIdAsync(jogo.Id);
    }

    public async Task AlterarAsync(Jogo jogo, List<int> idsGeneros)
    {
        var existente = await _repositorio.Jogo.BuscarPorIdAsync(jogo.Id);
        if (existente == null)
        {
            throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
        }

        jogo.Nome = jogo.Nome.Trim();
        var idsDistintos = idsGeneros.Distinct().ToList();

        await ConferirReferenciasAsync(jogo, idsDistintos);

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            await _repositorio.Jogo.AlterarAsync(jogo);
            await _repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);
            await _repositorio.JogoGenero.InserirAsync(jogo.Id, idsDistintos);
            transactionScope.Complete();
        }
    }

    public async Task DesativarAsync(int id)
    {
        var existente = await _repositorio.Jogo.BuscarPorIdAsync(id);
        if (existente == null)
        {
            throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
        }

        await _repositorio.Jogo.DesativarAsync(id);
    }

    private async Task ConferirReferenciasAsync(Jogo jogo, List<int> idsGeneros)
    {
        var plataforma = await _repositorio.Plataforma.BuscarPorIdAsync(jogo.IdPlataforma);
        if (plataforma == null)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNaoEncontrada);
        }

        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();
        if (!condicoes.Any(c => c.Id == jogo.IdCondicaoJogo))
        {
            throw new BusinessRuleException(BusinessErrors.CondicaoNaoEncontrada);
        }

        var generos = await _repositorio.Genero.ListarAsync();
        if (idsGeneros.Count == 0 || idsGeneros.Any(id => !generos.Any(g => g.Id == id)))
        {
            throw new BusinessRuleException(BusinessErrors.GeneroNaoEncontrado);
        }
    }

    // Sempre 4 consultas, qualquer que seja o tamanho da lista: plataformas, condições, gêneros e vínculos.
    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(List<Jogo> jogos)
    {
        if (jogos.Count == 0)
        {
            return new List<JogoDetalheServiceModel>();
        }

        var plataformas = await _repositorio.Plataforma.ListarAsync();
        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();
        var generos = await _repositorio.Genero.ListarAsync();
        var vinculos = await _repositorio.JogoGenero.ListarPorJogosAsync(jogos.Select(j => j.Id).ToList());

        return jogos.Select(jogo => new JogoDetalheServiceModel
        {
            Jogo = jogo,
            NomePlataforma = plataformas.FirstOrDefault(p => p.Id == jogo.IdPlataforma)?.Nome ?? string.Empty,
            DescricaoCondicao = condicoes.FirstOrDefault(c => c.Id == jogo.IdCondicaoJogo)?.Descricao ?? string.Empty,
            Generos = generos
                .Where(g => vinculos.Any(v => v.IdJogo == jogo.Id && v.IdGenero == g.Id))
                .ToList(),
        }).ToList();
    }
}