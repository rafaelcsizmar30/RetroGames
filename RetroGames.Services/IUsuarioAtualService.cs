namespace RetroGames.Services;

public interface IUsuarioAtualService
{
    bool EstaLogado { get; }
    int? Id { get; }
    bool EhAdministrador { get; }
}