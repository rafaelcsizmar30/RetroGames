using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    private const string Colunas = @"co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
                                     qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo";

    public JogoRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<(List<Jogo> Itens, int Total)> ListarAsync(
        string? busca, int? idPlataforma, List<int>? ids,
        bool incluirInativos, int pagina, int tamanhoPagina)
    {
        // Lista de ids vazia (não nula) significa "nenhum jogo": resultado vazio.
        if (ids is { Count: 0 })
        {
            return (new List<Jogo>(), 0);
        }

        var where = new List<string>();
        if (!incluirInativos) where.Add("st_ativo = TRUE");
        if (!string.IsNullOrWhiteSpace(busca)) where.Add("no_jogo LIKE @busca");
        if (idPlataforma.HasValue) where.Add("co_plataforma = @idPlataforma");

        var parametrosIds = new List<string>();
        if (ids != null)
        {
            for (var i = 0; i < ids.Count; i++) parametrosIds.Add($"@id{i}");
            where.Add($"co_seq_jogo IN ({string.Join(", ", parametrosIds)})");
        }

        var filtro = where.Count > 0 ? "WHERE " + string.Join(" AND ", where) : "";

        await using var conexao = await AbrirConexaoAsync();

        void AdicionarFiltros(MySqlCommand cmd)
        {
            if (!string.IsNullOrWhiteSpace(busca)) AdicionarParametro(cmd, "@busca", $"%{busca}%");
            if (idPlataforma.HasValue) AdicionarParametro(cmd, "@idPlataforma", idPlataforma.Value);
            if (ids != null)
            {
                for (var i = 0; i < ids.Count; i++) AdicionarParametro(cmd, $"@id{i}", ids[i]);
            }
        }

        int total;
        await using (var cmdTotal = CriarComando(conexao, $"SELECT COUNT(*) FROM tb_jogo {filtro}"))
        {
            AdicionarFiltros(cmdTotal);
            total = Convert.ToInt32(await cmdTotal.ExecuteScalarAsync());
        }

        var itens = new List<Jogo>();
        var sql = $@"SELECT {Colunas} FROM tb_jogo {filtro}
                     ORDER BY no_jogo
                     LIMIT @limite OFFSET @deslocamento";
        await using (var cmd = CriarComando(conexao, sql))
        {
            AdicionarFiltros(cmd);
            AdicionarParametro(cmd, "@limite", tamanhoPagina);
            AdicionarParametro(cmd, "@deslocamento", (pagina - 1) * tamanhoPagina);
            await using var leitor = await cmd.ExecuteReaderAsync();
            while (await leitor.ReadAsync()) itens.Add(Mapear(leitor));
        }

        return (itens, total);
    }

    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        var sql = $"SELECT {Colunas} FROM tb_jogo WHERE co_seq_jogo = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync();

        return await leitor.ReadAsync() ? Mapear(leitor) : null;
    }

    public async Task<int> InserirAsync(Jogo jogo)
    {
        const string sql = @"INSERT INTO tb_jogo
            (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
             dt_cadastro, co_plataforma, co_condicao_jogo)
            VALUES (@nome, @descricao, @ano, @preco, @estoque, @ativo,
                    @dataCadastro, @idPlataforma, @idCondicao)";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@ano", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@dataCadastro", jogo.DataCadastro);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicao", jogo.IdCondicaoJogo);

        return (int)await ExecutarGravacaoAsync(comando);
    }

    public async Task AlterarAsync(Jogo jogo)
    {
        const string sql = @"UPDATE tb_jogo
            SET no_jogo = @nome, ds_jogo = @descricao, nu_ano_lancamento = @ano,
                vl_preco = @preco, qt_estoque = @estoque,
                co_plataforma = @idPlataforma, co_condicao_jogo = @idCondicao
            WHERE co_seq_jogo = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", jogo.Id);
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@ano", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicao", jogo.IdCondicaoJogo);

        await comando.ExecuteNonQueryAsync();
    }

    public async Task DesativarAsync(int id)
    {
        const string sql = "UPDATE tb_jogo SET st_ativo = FALSE WHERE co_seq_jogo = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);

        await comando.ExecuteNonQueryAsync();
    }

    private static Jogo Mapear(MySqlDataReader leitor)
    {
        var colunaDescricao = leitor.GetOrdinal("ds_jogo");
        return new Jogo
        {
            Id = leitor.GetInt32("co_seq_jogo"),
            Nome = leitor.GetString("no_jogo"),
            Descricao = leitor.IsDBNull(colunaDescricao) ? null : leitor.GetString(colunaDescricao),
            AnoLancamento = leitor.GetInt32("nu_ano_lancamento"),
            Preco = leitor.GetDecimal("vl_preco"),
            Estoque = leitor.GetInt32("qt_estoque"),
            Ativo = leitor.GetBoolean("st_ativo"),
            DataCadastro = leitor.GetDateTime("dt_cadastro"),
            IdPlataforma = leitor.GetInt32("co_plataforma"),
            IdCondicaoJogo = leitor.GetInt32("co_condicao_jogo"),
        };
    }
}