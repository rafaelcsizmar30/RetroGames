using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<List<JogoGenero>> ListarTodosAsync()
    {
        const string sql = "SELECT co_jogo, co_genero FROM tb_jogo_genero";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        return await LerVinculosAsync(leitor);
    }

    public async Task<List<JogoGenero>> ListarPorJogosAsync(List<int> idsJogos)
    {
        if (idsJogos.Count == 0)
        {
            return new List<JogoGenero>();
        }

        var nomes = idsJogos.Select((_, i) => $"@id{i}").ToList();
        var sql = $"SELECT co_jogo, co_genero FROM tb_jogo_genero WHERE co_jogo IN ({string.Join(", ", nomes)})";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        for (var i = 0; i < idsJogos.Count; i++)
        {
            AdicionarParametro(comando, nomes[i], idsJogos[i]);
        }
        await using var leitor = await comando.ExecuteReaderAsync();

        return await LerVinculosAsync(leitor);
    }

    public async Task InserirAsync(int idJogo, List<int> idsGeneros)
    {
        const string sql = "INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES (@idJogo, @idGenero)";

        await using var conexao = await AbrirConexaoAsync();
        foreach (var idGenero in idsGeneros)
        {
            await using var comando = CriarComando(conexao, sql);
            AdicionarParametro(comando, "@idJogo", idJogo);
            AdicionarParametro(comando, "@idGenero", idGenero);
            await ExecutarGravacaoAsync(comando);
        }
    }

    public async Task RemoverPorJogoAsync(int idJogo)
    {
        const string sql = "DELETE FROM tb_jogo_genero WHERE co_jogo = @idJogo";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@idJogo", idJogo);

        await comando.ExecuteNonQueryAsync();
    }

    private static async Task<List<JogoGenero>> LerVinculosAsync(MySqlDataReader leitor)
    {
        var lista = new List<JogoGenero>();
        while (await leitor.ReadAsync())
        {
            lista.Add(new JogoGenero
            {
                IdJogo = leitor.GetInt32("co_jogo"),
                IdGenero = leitor.GetInt32("co_genero"),
            });
        }
        return lista;
    }
}