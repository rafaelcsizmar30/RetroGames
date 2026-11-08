using MySqlConnector;

namespace RetroGames.Repository;

public abstract class RepositoryBase
{
    private readonly RepositoryConfig _config;

    protected RepositoryBase(RepositoryConfig config)
    {
        _config = config;
    }

    protected async Task<MySqlConnection> AbrirConexaoAsync()
    {
        var conexao = new MySqlConnection(_config.ConnectionString);
        await conexao.OpenAsync();
        return conexao;
    }

    protected MySqlCommand CriarComando(MySqlConnection conexao, string sql)
    {
        return new MySqlCommand(sql, conexao);
    }

    protected void AdicionarParametro(MySqlCommand comando, string nome, object? valor)
    {
        comando.Parameters.AddWithValue(nome, valor ?? DBNull.Value);
    }

    protected async Task<long> ExecutarGravacaoAsync(MySqlCommand comando)
    {
        try
        {
            await comando.ExecuteNonQueryAsync();
            return comando.LastInsertedId;
        }
        catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
        {
            throw new RegistroDuplicadoException("Já existe um registro com este valor.", ex);
        }
    }
}