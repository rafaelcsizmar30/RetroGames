using System.Net;

namespace RetroGames.Services;

public static class BusinessErrors
{
    public static readonly (int Codigo, string Mensagem, HttpStatusCode Status) PlataformaNaoEncontrada =
        (100, "Plataforma não encontrada.", HttpStatusCode.NotFound);

    public static readonly (int Codigo, string Mensagem, HttpStatusCode Status) PlataformaNomeDuplicado =
        (101, "Já existe uma plataforma com este nome.", HttpStatusCode.Conflict);

    public static readonly (int Codigo, string Mensagem, HttpStatusCode Status) PlataformaPossuiJogos =
        (102, "Não é possível excluir: existem jogos desta plataforma.", HttpStatusCode.Conflict);

    public static readonly (int Codigo, string Mensagem, HttpStatusCode Status) JogoNaoEncontrado =
        (103, "Jogo não encontrado.", HttpStatusCode.NotFound);

    public static readonly (int Codigo, string Mensagem, HttpStatusCode Status) CondicaoNaoEncontrada =
        (104, "A condição informada não existe.", HttpStatusCode.NotFound);

    public static readonly (int Codigo, string Mensagem, HttpStatusCode Status) GeneroNaoEncontrado =
        (105, "Um ou mais gêneros informados não existem.", HttpStatusCode.NotFound);
}