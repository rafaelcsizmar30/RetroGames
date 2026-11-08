namespace RetroGames.Repository;

public class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException(string mensagem, Exception? causa = null)
        : base(mensagem, causa)
    {
    }
}