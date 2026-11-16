using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class PlataformaService : IPlataformaService
{
    private readonly IRepositorySet _repositorio;

    public PlataformaService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<Plataforma>> ListarAsync()
    {
        return await _repositorio.Plataforma.ListarAsync();
    }

    public async Task<Plataforma> BuscarPorIdAsync(int id)
    {
        var plataforma = await _repositorio.Plataforma.BuscarPorIdAsync(id);
        if (plataforma == null)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNaoEncontrada);
        }
        return plataforma;
    }

    public async Task<Plataforma> InserirAsync(Plataforma plataforma)
    {
        plataforma.Nome = plataforma.Nome.Trim();

        if (await _repositorio.Plataforma.ExisteComNomeAsync(plataforma.Nome))
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNomeDuplicado);
        }

        try
        {
            plataforma.Id = await _repositorio.Plataforma.InserirAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNomeDuplicado);
        }

        return plataforma;
    }

    public async Task AlterarAsync(Plataforma plataforma)
    {
        var existente = await _repositorio.Plataforma.BuscarPorIdAsync(plataforma.Id);
        if (existente == null)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNaoEncontrada);
        }

        plataforma.Nome = plataforma.Nome.Trim();

        if (await _repositorio.Plataforma.ExisteComNomeAsync(plataforma.Nome, plataforma.Id))
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNomeDuplicado);
        }

        try
        {
            await _repositorio.Plataforma.AlterarAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNomeDuplicado);
        }
    }

    public async Task ExcluirAsync(int id)
    {
        var existente = await _repositorio.Plataforma.BuscarPorIdAsync(id);
        if (existente == null)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNaoEncontrada);
        }

        if (await _repositorio.Plataforma.PossuiJogosAsync(id))
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaPossuiJogos);
        }

        await _repositorio.Plataforma.ExcluirAsync(id);
    }
}