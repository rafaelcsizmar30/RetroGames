namespace RetroGames.Services;

public class BusinessRuleException : Exception
{
    public (int Codigo, string Mensagem, System.Net.HttpStatusCode Status) Erro { get; }

    public BusinessRuleException((int Codigo, string Mensagem, System.Net.HttpStatusCode Status) erro)
        : base(erro.Mensagem)
    {
        Erro = erro;
    }
}